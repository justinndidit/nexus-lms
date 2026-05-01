using System;
using Microsoft.EntityFrameworkCore;
using webApi.Data;
using webApi.Modules.Auth.Domain.Interfaces;
using webApi.Modules.Rbac.Domain.Interfaces;
using webApi.Modules.Rbac.Domain.Models;

namespace webApi.Modules.Rbac.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{

    private readonly LMSApiApplicationContext _dbContext;

    public PermissionRepository(LMSApiApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Permission> AddAsync(Permission permission, CancellationToken ct = default)
    {
        await _dbContext.Permissions.AddAsync(permission);
        await _dbContext.SaveChangesAsync();
        return permission;
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Name == name);
        return permission != null;
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.Permissions.Select(
            p => p
        ).ToListAsync();
    }

    public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Permissions.FirstOrDefaultAsync(
            p => p.Id == id
        );
    }

    public async Task<IReadOnlyList<Permission>> GetByIdsAsync(
        IEnumerable<Guid> ids, 
        CancellationToken ct = default)
    {
        return await _dbContext.Permissions
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(ct);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _dbContext.Permissions.FirstOrDefaultAsync(
            p => p.Name == name
        );
    }

    public async void Remove(Permission permission)
    {
        _dbContext.Permissions.Remove(permission);
        await _dbContext.SaveChangesAsync();
    }

    public async void Update(Permission permission)
    {
        _dbContext.Update(permission);
        await _dbContext.SaveChangesAsync();
    }
}
