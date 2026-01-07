using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Models;

namespace ZhmApi.Controllers
{
    [Authorize] // patient mag dit zien
    [ApiController]
    [Route("api/specialists")]
    public class SpecialistsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public SpecialistsController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecialists()
        {
            // ✅ Identity manier om users met rol Specialist op te halen
            var specialists = await _userManager.GetUsersInRoleAsync("Specialist");

            var result = specialists.Select(u => new
            {
                id = u.Id,
                fullName = $"{u.FirstName} {u.LastName}".Trim()
            });

            return Ok(result);
        }
    }
}
