namespace webApi.Modules.Auth.Application.Dtos;

public record ExchangeTokenResponse(
    string JwtToken,
    string RefreshToken
);
