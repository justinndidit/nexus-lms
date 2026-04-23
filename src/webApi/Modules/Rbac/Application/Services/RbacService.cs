using webApi.Modules.Rbac.Domain.Interfaces;

namespace webApi.Modules.Rbac.Application.Services;
public class RbacService : IRbacService
{
    private readonly ILogger<RbacService> _logger;
    private readonly IRoleRepository _roleRepo;
    public RbacService(ILogger<RbacService> logger,
                        IRoleRepository roleRepo)
    {
        _logger = logger;
        _roleRepo = roleRepo;
    }
    public async Task AssignUserToRole(Guid userId, string roleName)
    {
        Role role = await _roleRepo.GetByNameAsync(roleName) ?? throw new BadHttpRequestException("Role does not exist");
        await _roleRepo.AddUserToRole(userId, role.Id);
    }

    public async Task<Role> GetRoleByRoleName(string roleName)
    {
        return 
            await _roleRepo.GetByNameAsync(roleName)
            ?? throw new KeyNotFoundException("Role not found");
    }

    public async Task<bool> RoleWithNameExists(string roleName)
    {
        return
            await _roleRepo.GetByNameAsync(roleName) is not null;
    }
}
