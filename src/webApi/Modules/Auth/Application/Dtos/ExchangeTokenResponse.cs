namespace webApi.Application.Dtos.Auth.Response;

public record ExchangeTokenResponse(
    string JwtToken,
    string RefreshToken
);
