
using webApi.Modules.Rbac.Domain.Models;

namespace webApi.Modules.Rbac.Domain.Interfaces;
public interface IRbacService
{
 public Task AssignUserToRole(Guid userId, string roleName);
 public Task<bool> RoleWithNameExists(string roleName);
 public Task<Role> GetRoleByRoleName(string roleName);   

 public Task<IReadOnlyList<string>> GetAllRolesByUserId(Guid userId);
}
