using Newtonsoft.Json.Linq;
using ZhmApi.Dtos;

namespace ZhmApi.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetRoleAsync();
        Task<RoleDto> CreateRoleAsync(RoleCreateDto dto);
        Task AssignRoleAsync(int userId, string RoleName);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
        Task<bool> UserHasRoleAsync(int userId, string roleName);
    }
}