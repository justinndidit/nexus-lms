using System;
using webApi.Modules.Auth.Domain.Models;

namespace webApi.Modules.Auth.Domain.Interfaces;

public interface IVerificationTokenRepository
{
    public Task<VerificationToken> Create(VerificationToken token);
    public Task<VerificationToken?> GetTokenByEmail(string email);
    public Task<VerificationToken?> GetTokenByToken(string token);
    public Task<VerificationToken> UpdateAsync(VerificationToken token);

}
