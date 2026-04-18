using System;
using Microsoft.EntityFrameworkCore;
using webApi.Application.Dtos;
using webApi.Data;
using webApi.Modules.Auth.Domain.Interfaces;
using webApi.Modules.Auth.Domain.Models;

namespace webApi.Modules.Auth.Infrastructure.Repositories;

public class VerificationTokenRepository : IVerificationTokenRepository
{
    private readonly LMSApiApplicationContext _dbContext;

    public VerificationTokenRepository(LMSApiApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VerificationToken> Create(VerificationToken token)
    {
        await _dbContext.VerificationTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();
        return token;
    }
    public async Task<VerificationToken?> GetTokenByEmail(string email)
    {
        return 
            await _dbContext.VerificationTokens
                    .FirstOrDefaultAsync(v => v.Email == email);
    }

    public async Task<VerificationToken?> GetTokenByToken(string token)
    {
        return 
            await _dbContext.VerificationTokens
                .FirstOrDefaultAsync(v => v.Token == token);
    }

    public async Task<VerificationToken> UpdateAsync(VerificationToken token)
    {
        _dbContext.VerificationTokens.Update(token);
        await _dbContext.SaveChangesAsync();
        return token;
    }
}
