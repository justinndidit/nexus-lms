namespace webApi.Modules.Auth.Application.Dtos;

public record LoginRequest(
    string Email,
    string Password
);