using webApi.Application.Dtos;

namespace webApi.Modules.Auth.Application.Dtos;

public record VerifyEmailResponse(
    string Email,
    List<string> Roles,
    Token Jwt
);
