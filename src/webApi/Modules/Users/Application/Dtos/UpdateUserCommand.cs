namespace webApi.Modules.Users.Application.Dtos;

public record UpdateUserActiveStatusCommand(
    string Email,
    bool Activate
);