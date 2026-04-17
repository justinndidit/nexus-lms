namespace webApi.Application.Dtos.Auth.Request;

public record LogoutRequest(
    string RrfreshToken
);