using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Services;
using BCrypt.Net;


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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized("invalid_credentials");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("invalid_credentials");

            // If no 2FA → login success
            if (!user.TwoFactorEnabled)
                return Ok(new { token = "demo-token-no-2fa" });

            // Create and send 2FA
            var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, user.Email);

            return Ok(new { requires2FA = true, tempSessionId = sessionId });
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> Verify([FromBody] TwoFaVerifyDto dto)
        {
            var result = await _twoFa.VerifyCodeAsync(dto.TempSessionId, dto.Code);

            if (!result.success)
                return BadRequest(new { error = result.reason });

            return Ok(new { token = "demo-jwt-token" });
        }

        [HttpPost("resend-code")]
        public async Task<IActionResult> Resend([FromBody] ResendDto dto)
        {
            var result = await _twoFa.ResendAsync(dto.TempSessionId);

            if (!result.success)
                return BadRequest(new { error = result.reason });

            return Ok(new { message = "resent" });
        }
    }
}
