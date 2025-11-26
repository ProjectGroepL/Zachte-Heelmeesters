using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Services;
using BCrypt.Net;
using ZhmApi.Models;


namespace ZhmApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApiContext _db;
        private readonly TwoFactorService _twoFa;

        public AuthController(ApiContext db, TwoFactorService twoFa)
        {
            _db = db;
            _twoFa = twoFa;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return Conflict(new { error = "email_exists" });

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                ZipCode = dto.ZipCode,
                Street = dto.Street,
                HouseNumber = dto.HouseNumber,
                PostalCode = dto.PostalCode,
                City = dto.City,
                Province = dto.Province,
                Country = dto.Country,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                TwoFactorEnabled = true // <-- heel belangrijk
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "registered" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized("invalid_credentials");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("invalid_credentials");

            // Als 2FA uit → direct ingelogd
            if (!user.TwoFactorEnabled)
                return Ok(new { token = "jwt-token-zonder-2fa" });

            // 2FA actief → stuur code
            var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, user.Email);

            return Ok(new { requires2FA = true, tempSessionId = sessionId });
        }

        // 2️⃣ VERIFY CODE
        [HttpPost("verify-2fa")]
        public async Task<IActionResult> Verify([FromBody] TwoFaVerifyDto dto)
        {
            var (success, reason) = await _twoFa.VerifyCodeAsync(dto.TempSessionId, dto.Code);

            if (!success)
                return BadRequest(new { error = reason });

            return Ok(new { token = "jwt-token-echte" });
        }

        // 3️⃣ RESEND CODE
        [HttpPost("resend-code")]
        public async Task<IActionResult> Resend([FromBody] ResendDto dto)
        {
            var (success, reason) = await _twoFa.ResendAsync(dto.TempSessionId);

            if (!success)
                return BadRequest(new { error = reason });

            return Ok(new { message = "resent" });
        }
    }
}
