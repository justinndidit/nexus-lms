namespace webApi.Modules.Users.Application.Dtos;

public record UpdateUserPasswordCommandResponse(
    string UserId,
    string Message
);