using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Models;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
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
                RoleId = 1, // Default to Patient role
                EmailConfirmed = false // Will be set to true after email verification
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Registration failed", errors = result.Errors });
            }

            // Add user to default role
            await _userManager.AddToRoleAsync(user, "Patient");

            // Generate tokens
            var token = _jwtService.GenerateToken(user.Id);
            var refreshToken = _jwtService.GenerateRefreshToken();

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
                RefreshToken = refreshToken,
                User = userDto
            };

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

            // Generate tokens
            var token = _jwtService.GenerateToken(user.Id);
            var refreshToken = _jwtService.GenerateRefreshToken();

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
                RefreshToken = refreshToken,
                User = userDto
            };

            return Ok(response);
        }
    }
}
