namespace webApi.Modules.Auth.Domain.Interfaces;

public record UpdateUserPasswordCommandResponse(
    string UserId,
    string Message
);