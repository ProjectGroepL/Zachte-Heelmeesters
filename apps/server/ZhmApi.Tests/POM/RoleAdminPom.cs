using ZhmApi.Services;

public class RoleAdminPom
{
    private readonly IRoleService _service;

    public RoleAdminPom(IRoleService service)
    {
        _service = service;
    }

    public Task AssignRole(int userId, string role)
        => _service.AssignRoleAsync(userId, role);

    public Task<bool> GetUserRoles(int userId, string role)
        => _service.UserHasRoleAsync(userId, role);
}
