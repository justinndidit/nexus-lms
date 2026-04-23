
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

    public static User CreateUserRequestToModel(CreateUserRequest req, string passwordHas)
    {
        return new User(req.Email, passwordHas);
    }


    public static List<string> ModelToRoleNames(List<UserRole> roles)
    {
        List<string> roleNames = [];

        foreach (var role in roles)
        {
            roleNames.Add(role.Role.RoleName);
        }

        return roleNames;
    }
}
