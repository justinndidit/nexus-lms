namespace webApi.Application.Dtos;

public record Token(
    string JwtToken,
    string RefreshToken
);
