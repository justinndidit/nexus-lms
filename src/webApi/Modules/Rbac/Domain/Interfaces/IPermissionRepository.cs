using System;

namespace webApi.Modules.Rbac.Domain.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Permission?> GetByNameAsync(string name, CancellationToken ct = default);

    Task<IReadOnlyList<Permission>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken ct = default
    );

    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken ct = default);

    Task<Permission> AddAsync(Permission permission, CancellationToken ct = default);

    void Update(Permission permission);

    void Remove(Permission permission);

    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}