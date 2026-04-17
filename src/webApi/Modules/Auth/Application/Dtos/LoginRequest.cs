namespace webApi.Application.Dtos.Auth.Request;

public record LoginRequest(
    string Email,
    string Password
);