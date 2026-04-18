namespace webApi.Modules.Auth.Application.Dtos;

public record UpdateUserActiveStatusCommand(
    string Email,
    bool Activate
);