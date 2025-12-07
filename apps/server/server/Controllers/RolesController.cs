using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Models;

namespace ZhmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: api/roles
        [HttpGet]
        public IActionResult GetRoles()
        {
            return Ok(_roleManager.Roles.ToList());
        }

        // POST: api/roles/create?roleName=Doctor
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return Conflict("Role already exists.");

            var role = new Role { Name = roleName };
            var result = await _roleManager.CreateAsync(role);

            return result.Succeeded ? Ok(role) : BadRequest(result.Errors);
        }

        // POST: api/roles/assign?userId=3&roleName=Doctor
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            if (!await _roleManager.RoleExistsAsync(roleName))
                return NotFound("Role not found.");

            var result = await _userManager.AddToRoleAsync(user, roleName);

            return result.Succeeded ? Ok($"Assigned {roleName} to {user.UserName}")
                                   : BadRequest(result.Errors);
        }

        // GET: api/roles/user/3
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
    }
}
