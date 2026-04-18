namespace webApi.Application.Dtos.Auth.Request;

public record ExchangeTokenRequest(
    string RefreshToken
);
