
using webApi.Modules.Auth.Application.Dtos;

namespace webApi.Modules.Auth.Application.Common.Mappers;

public static class AuthMapper
{
    public static LoginResponse ToLoginResponse(string email, List<string>roles, Token tokenData)
    {
        return new LoginResponse(
            email,
            roles,
            tokenData
        );  
    }

    public static Token ToTokenData(string jwtToken, string refreshToken)
    {
        return new Token(jwtToken, refreshToken);
    }

}
