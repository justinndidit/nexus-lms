namespace webApi.Modules.Auth.Application.Dtos;

public record LogoutRequest(
    string RefreshToken
);