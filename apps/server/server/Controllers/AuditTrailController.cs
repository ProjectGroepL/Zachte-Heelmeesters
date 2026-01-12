using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using System.Security.Claims;

namespace ZhmApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditTrailController : ControllerBase
    {
        private readonly ApiContext _context;

        public AuditTrailController(ApiContext context)
        {
            _context = context;
        }

        // what would the http be here?
        [HttpGet]
        public async Task<IActionResult> GetAuditTrails([FromQuery] DateTime date)
        {
            // 🔐 Enforce Role = (admin / auditor)
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            if (roleClaim?.Value != "Systeembeheerder")
            {
                return Forbid();
            }

            var start = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var end = start.AddDays(1);


            var logs = await _context.AuditTrails
                .AsNoTracking()
                .Where(a =>
                    a.Timestamp >= start &&
                    a.Timestamp < end
                )
                .OrderBy(a => a.Timestamp)
                .Select(a => new AuditTrailDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    IpAddress = a.IpAddress!,
                    Details = a.Details,
                    Timestamp = a.Timestamp,
                    Method = a.Method,
                    Path = a.Path,
                    StatusCode = a.StatusCode,
                    UserAgent = a.UserAgent
                })
                .ToListAsync();

            return Ok(logs);
        }
    }
}
