using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Models;
using ZhmApi.Services;
using ZhmApi.Extensions;
namespace ZhmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ITokenService _tokenService;
        private readonly TwoFactorService _twoFactorService;
        private readonly ApiContext _context;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService,
            ITokenService tokenService,
            TwoFactorService twoFactorService,
            ApiContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _tokenService = tokenService;
            _twoFactorService = twoFactorService;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }

            // Check if General Practitioner exists and is of correct role
            var gp = await _userManager.FindByIdAsync(registerDto.GeneralPractitionerId.ToString());
            if (gp == null)
            {
                return BadRequest(new { message = "General Practitioner does not exist or is invalid" });
            }

            var gpRoles = await _userManager.GetRolesAsync(gp);
            if (!gpRoles.Contains("Huisarts"))
            {
                return BadRequest(new { message = "General Practitioner does not exist or is invalid" });
            }

            // Create new user
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                MiddleName = registerDto.MiddleName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                Street = registerDto.Street,
                HouseNumber = registerDto.HouseNumber,
                HouseNumberAddition = registerDto.HouseNumberAddition,
                ZipCode = registerDto.ZipCode,
                City = registerDto.City,
                Country = registerDto.Country,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Registration failed", errors = result.Errors });
            }

            // Add user to default role
            await _userManager.AddToRoleAsync(user, "Patient");

            // Assign general practitioner
            var gpLink = new DoctorPatients
            {
                PatientId = user.Id,
                DoctorId = registerDto.GeneralPractitionerId
            };

            _context.DoctorPatients.Add(gpLink);

            // Generate tokens
            var token = _jwtService.GenerateToken(user.Id);
            var refreshTokenEntity = await _tokenService.CreateTokenAsync(
                user.Id,
                TokenType.RefreshToken,
                TimeSpan.FromDays(7));

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Street = user.Street,
                HouseNumber = user.HouseNumber,
                HouseNumberAddition = user.HouseNumberAddition,
                ZipCode = user.ZipCode,
                City = user.City,
                Country = user.Country,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Role = "User"
            };

            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshTokenEntity.Value,
                User = userDto
            };

            _context.SaveChanges();

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Check if 2FA is enabled for this user
            if (user.TwoFactorEnabled)
            {
                // Create and send 2FA code
                var sessionId = await _twoFactorService.CreateAndSendCodeAsync(user.Id, user.Email!);

                return Ok(new AuthResponseDto
                {
                    RequiresTwoFactor = true,
                    TempSessionId = sessionId,
                    Token = string.Empty,
                    RefreshToken = string.Empty,
                    User = null!
                });
            }

            // Generate tokens for non-2FA users
            var token = _jwtService.GenerateToken(user.Id);
            var refreshTokenExpirationDays = int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_DAYS") ?? "7");
            var refreshTokenEntity = await _tokenService.CreateTokenAsync(
                user.Id,
                TokenType.RefreshToken,
                TimeSpan.FromDays(refreshTokenExpirationDays));

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Street = user.Street,
                HouseNumber = user.HouseNumber,
                HouseNumberAddition = user.HouseNumberAddition,
                ZipCode = user.ZipCode,
                City = user.City,
                Country = user.Country,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Role = role
            };

            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshTokenEntity.Value,
                User = userDto
            };

            // credentials are valid here, so the userID can be logged in the audit trail.
            await LogLoginSuccess(user);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            _context.AuditTrails.Add(new AuditTrail
            {
                UserId = userId,
                Method = "LOGOUT",
                Path = "/api/auth/logout",
                StatusCode = StatusCodes.Status200OK,
                Timestamp = DateTimeOffset.UtcNow,

                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),

                UserAgent = HttpContext.Request.Headers["User-Agent"]
                    .ToString()
                    .Truncate(512),

                Details = "Gebruiker heeft uitgelogd"
            });

            await _context.SaveChangesAsync();

            // optional: invalidate refresh token here

            return Ok();
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate refresh token
            var tokenEntity = await _tokenService.GetValidTokenAsync(refreshTokenDto.RefreshToken, TokenType.RefreshToken);
            if (tokenEntity == null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var user = tokenEntity.User;

            // Revoke the old refresh token
            await _tokenService.RevokeTokenAsync(refreshTokenDto.RefreshToken, TokenType.RefreshToken);

            // Generate new tokens
            var newToken = _jwtService.GenerateToken(user.Id);
            var refreshTokenExpirationDays = int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_DAYS") ?? "7");
            var newRefreshTokenEntity = await _tokenService.CreateTokenAsync(
                user.Id,
                TokenType.RefreshToken,
                TimeSpan.FromDays(refreshTokenExpirationDays));

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Street = user.Street,
                HouseNumber = user.HouseNumber,
                HouseNumberAddition = user.HouseNumberAddition,
                ZipCode = user.ZipCode,
                City = user.City,
                Country = user.Country,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Role = role
            };

            var response = new AuthResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshTokenEntity.Value,
                User = userDto
            };

            return Ok(response);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            // Get user ID from claims (automatically parsed by JWT middleware)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                             User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token claims" });
            }

            // Get user from database
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Street = user.Street,
                HouseNumber = user.HouseNumber,
                HouseNumberAddition = user.HouseNumberAddition,
                ZipCode = user.ZipCode,
                City = user.City,
                Country = user.Country,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Role = role
            };

            return Ok(userDto);
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] TwoFaVerifyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, reason, userId) = await _twoFactorService.VerifyCodeAsync(dto.TempSessionId, dto.Code);

            if (!success)
            {
                return BadRequest(new { message = GetErrorMessage(reason) });
            }

            // Haal user vanuit userId (dit werkt ALTIJD)
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            // Code pas hier ongeldig maken:
            await _twoFactorService.MarkAsUsedAsync(dto.TempSessionId);

            // Generate tokens after successful 2FA verification
            var token = _jwtService.GenerateToken(user.Id);
            var refreshTokenExpirationDays = int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_DAYS") ?? "7");
            var refreshTokenEntity = await _tokenService.CreateTokenAsync(
                user.Id,
                TokenType.RefreshToken,
                TimeSpan.FromDays(refreshTokenExpirationDays));

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Street = user.Street,
                HouseNumber = user.HouseNumber,
                HouseNumberAddition = user.HouseNumberAddition,
                ZipCode = user.ZipCode,
                City = user.City,
                Country = user.Country,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Role = role
            };

            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshTokenEntity.Value,
                User = userDto
            };

            return Ok(response);
        }



        [HttpPost("resend-2fa")]
        public async Task<IActionResult> ResendTwoFactor([FromBody] ResendDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, reason) = await _twoFactorService.ResendAsync(dto.TempSessionId);

            if (!success)
            {
                return BadRequest(new { message = GetErrorMessage(reason) });
            }

            return Ok(new { message = "Code has been resent to your email" });
        }

        private string GetErrorMessage(string reason)
        {
            return reason switch
            {
                "no_session" => "Invalid or expired session",
                "already_used" => "Code has already been used",
                "expired" => "Code has expired",
                "invalid_code" => "Invalid verification code",
                "too_many_resends" => "Too many resend attempts. Please try logging in again.",
                _ => "An error occurred during verification"
            };
        }

        // helper function to log userID on a succesfull login.
        private async Task LogLoginSuccess(User user)
        {
            var audit = new AuditTrail
            {
                UserId = user.Id,
                Method = "LOGIN",
                Path = "/api/auth/login",
                StatusCode = StatusCodes.Status200OK,
                Timestamp = DateTimeOffset.UtcNow,

                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),

                UserAgent = HttpContext.Request.Headers["User-Agent"]
                        .ToString()
                        .Truncate(512),

                Details = "Gebruiker heeft succesvol ingelogd"
            };

            _context.AuditTrails.Add(audit);
            await _context.SaveChangesAsync();
        }

    }
}
