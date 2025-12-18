using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZhmApi.Data;
using ZhmApi.Models;

namespace ZhmApi.Controllers
{
    [ApiController]
    public class AuditTrailController : ControllerBase
    {
        private readonly ApiContext _context;

        public AuditTrailController(ApiContext context)
        {
            _context = context;
        }

        protected async Task LogAudit(string action, string details, string result)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return; // user not logged in → don't log

            var audit = new AuditTrail
            {
                UserId = userId,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Action = action,
                Details = details,
                Result = result
            };

            _context.AuditTrails.Add(audit);
            await _context.SaveChangesAsync();
        }
    }
}
