using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Models;

namespace ZhmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Specialist")]
    public class SpecialistIcalController : ControllerBase
    {
        private readonly ApiContext _context;

        public SpecialistIcalController(ApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SaveIcal([FromBody] SpecialistIcalDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user ID from claims (same pattern as in AuthController.Me)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                              User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token claims" });
            }

            // Upsert SpecialistIcal
            var existing = await _context.SpecialistIcals
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existing == null)
            {
                existing = new SpecialistIcal
                {
                    UserId = userId,
                    Url = dto.Url
                };
                _context.SpecialistIcals.Add(existing);
            }
            else
            {
                existing.Url = dto.Url;
                _context.SpecialistIcals.Update(existing);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "iCal link saved", url = existing.Url });
        }
    }
}
