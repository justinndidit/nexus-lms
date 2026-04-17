using System;
using webApi.Domain.Models;

namespace webApi.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByToken(string token);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
}