namespace webApi.Modules.Auth.Application.Dtos;

public record Token(
    string JwtToken,
    string RefreshToken
);
