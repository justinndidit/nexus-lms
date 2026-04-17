using Microsoft.EntityFrameworkCore;
using webApi.Application.Interfaces;
using webApi.Data;
using webApi.Domain.Models;

namespace webApi.Application.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly LMSApiApplicationContext _context;

    public RoleRepository(LMSApiApplicationContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id) =>
        await _context.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Role?> GetByNameAsync(string roleName) =>
        await _context.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.RoleName == roleName);

    public async Task<IEnumerable<Role>> GetAllAsync() =>
        await _context.Roles.AsNoTracking().ToListAsync();
    public async Task CreateRole(Role role) {
        await CreateRoleTx(role);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateRole(Role role) {
        UpdateRoleTx(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRole(Role role) {
        DeleteRoleTx(role);
        await _context.SaveChangesAsync();
    }

    public async Task AddPermissionToRole(Guid roleId, Guid permissionId) {
        await AddPermissionToRoleTx(roleId, permissionId);
        await _context.SaveChangesAsync();
    }    
    public async Task CreateRoleTx(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public void UpdateRoleTx(Role role)
    {
        _context.Roles.Update(role);
    }

    public void DeleteRoleTx(Role role)
    {
        _context.Roles.Remove(role);
    }

    public async Task AddPermissionToRoleTx(Guid roleId, Guid permissionId)
    {
        var rolePermission = new RolePermission(roleId, permissionId);
        await _context.RolePermissions.AddAsync(rolePermission);
    }
}