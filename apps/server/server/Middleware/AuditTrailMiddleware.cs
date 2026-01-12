using System.Security.Claims;
using ZhmApi.Data;
using ZhmApi.Models;
using ZhmApi.Extensions;
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

        private static readonly HashSet<string> ExcludedPaths = new()
        {
            "/api/auth/login",
            "/api/auth/logout",
            "/api/auth/refresh"
        };


        public async Task InvokeAsync(HttpContext context, ApiContext db)
        {
            bool isGet = context.Request.Method == HttpMethods.Get;
            string path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

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

            if (isGet || ExcludedPaths.Contains(path))
                return; // skip logging for GET requests or the login/logout and refresh to prevent duplicate loggin and useless information.

            try
            {
                var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
                int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : (int?)null;

                var endpoint = context.GetEndpoint();

                var audit = new AuditTrail
                {
                    UserId = userId,
                    IpAddress = GetClientIp(context),
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    StatusCode = statusCode ?? 0,
                    Timestamp = DateTimeOffset.UtcNow,

                    UserAgent = context.Request.Headers["User-Agent"]
                        .ToString()
                        .Truncate(512),

                    Details = endpoint?.DisplayName
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