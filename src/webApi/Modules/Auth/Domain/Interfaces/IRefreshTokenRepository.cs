using System;

namespace webApi.Modules.Auth.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByToken(string token);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
}