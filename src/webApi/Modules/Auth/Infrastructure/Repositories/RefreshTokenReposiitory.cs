using System;
using Microsoft.EntityFrameworkCore;
using webApi.Application.Interfaces;
using webApi.Data;
using webApi.Domain.Models;

namespace webApi.Application.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly LMSApiApplicationContext _dbContext;

    public RefreshTokenRepository(LMSApiApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetByToken(string token)
    {
        return await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _dbContext.RefreshTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _dbContext.RefreshTokens.Update(token);
        await _dbContext.SaveChangesAsync();
    }
}