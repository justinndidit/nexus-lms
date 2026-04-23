namespace webApi.Modules.Auth.Application.Dtos;

public record ExchangeTokenRequest(
    string RefreshToken
);
