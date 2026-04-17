using System;
using webApi.Modules.Auth.Domain.Models;

namespace webApi.Modules.Auth.Domain.Interfaces;

public interface IVerificationTokenRepository
{
    public Task<VerificationToken?> GetTokenByEmail(string email);
    public Task<VerificationToken?> GetTokenByToken(string token);

}
