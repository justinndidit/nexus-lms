namespace webApi.Modules.Auth.Application.Dtos;

public record JwtPayload(
    Guid UserId,
    string Email,
    List<string> Roles 
);