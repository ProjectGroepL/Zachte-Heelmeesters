using System.Security.Claims;

namespace ZhmApi.Extensions
{
    public static class ClaimsPrinciplesExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(value!);
        }
    }
}