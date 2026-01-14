using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Models;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService; 
        }

        // "api/roles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
         => Ok(await _roleService.GetRoleAsync());

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto dto)
        {
            var role = await _roleService.CreateRoleAsync(dto);
            return Ok(role);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(int userId, string roleName)
        {
            await _roleService.AssignRoleAsync(userId, roleName);
            return Ok();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(int userId)
            => Ok(await _roleService.GetUserRolesAsync(userId));

    }
}
