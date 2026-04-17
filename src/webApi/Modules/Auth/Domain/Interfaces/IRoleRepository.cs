using webApi.Domain.Models;

namespace webApi.Application.Interfaces;

public interface IRoleRepository
{    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string roleName);
    Task<IEnumerable<Role>> GetAllAsync();
    Task CreateRole(Role role);
    Task UpdateRole(Role role);
    Task DeleteRole(Role role);
    Task AddPermissionToRole(Guid roleId, Guid permissionId);
    Task CreateRoleTx(Role role);
    void UpdateRoleTx(Role role);
    void DeleteRoleTx(Role role);
    Task AddPermissionToRoleTx(Guid roleId, Guid permissionId);
}