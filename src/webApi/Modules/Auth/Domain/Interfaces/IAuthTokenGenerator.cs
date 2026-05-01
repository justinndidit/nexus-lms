using System;

namespace webApi.Modules.Auth.Domain.Interfaces;

public interface IAuthTokenGenerator
{
    string GenerateCode(int length);
}
