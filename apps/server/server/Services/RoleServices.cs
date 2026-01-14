using Microsoft.AspNetCore.Identity;
using ZhmApi.Dtos;
using ZhmApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace ZhmApi.Services
{
    public class RoleService : IRoleService 
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleService(
            RoleManager<Role> roleManager,
            UserManager<User> userManager
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IEnumerable<RoleDto>> GetRoleAsync()
        {
            return _roleManager.Roles.Select(r => new RoleDto
            {
            Id = r.Id,
            Name = r.Name!,
            Description = r.Description 
            });
        }

        public async Task<RoleDto> CreateRoleAsync(RoleCreateDto dto)
        {
            if (await _roleManager.RoleExistsAsync(dto.Name))
                throw new InvalidOperationException("Role Already Exists");

            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ",
                result.Errors.Select(e => e.Description)));

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                Description = role.Description 
            };

        }
        
        public async Task AssignRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new KeyNotFoundException("User not found");

            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new KeyNotFoundException("Role not found");
            
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
                throw new InvalidOperationException("Faild to assign role"); 
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new KeyNotFoundException("User not found");
            
            return await _userManager.GetRolesAsync(user);
        }
        public async Task<bool> UserHasRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new KeyNotFoundException("User not found");
            return await _userManager.IsInRoleAsync(user, roleName);
        }
    }
}