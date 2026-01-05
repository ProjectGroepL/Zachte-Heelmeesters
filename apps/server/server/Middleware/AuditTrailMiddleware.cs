using System.Security.Claims;
using ZhmApi.Data;
using ZhmApi.Models;
namespace ZhmApi.Middleware
{
    public class AuditTrailMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditTrailMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private string GetClientIp(HttpContext context)
        {
            // Respect X-Forwarded-For header (proxies)
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "Unknown";
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        public async Task InvokeAsync(HttpContext context, ApiContext db)
        {
            bool isGet = context.Request.Method == HttpMethods.Get;

            int? statusCode = null;

            try
            {
                await _next(context);
            }
            catch
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }
                throw;
            }
            finally
            {
                statusCode = context.Response.StatusCode;
            }

            if (isGet)
                return; // skip logging for GET requests

            try
            {
                var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
                int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : (int?)null;

                var audit = new AuditTrail
                {
                    UserId = userId,
                    IpAddress = GetClientIp(context),
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    StatusCode = statusCode ?? 0,
                    Timestamp = DateTimeOffset.UtcNow
                };

                db.AuditTrails.Add(audit);
                await db.SaveChangesAsync();
            }
            catch
            {
                // NEVER let audit logging fail the request
            }
        }


    }
}