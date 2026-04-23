namespace webApi.Modules.Auth.Application.Dtos;

public record JwtPayload(
    string UserId,
    string Email,
    List<string> Roles 
);