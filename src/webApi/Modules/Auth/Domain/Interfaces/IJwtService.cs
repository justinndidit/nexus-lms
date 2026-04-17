using System.Security.Claims;
using webApi.Modules.Auth.Application.Dtos;

namespace webApi.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(JwtPayload payload);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}