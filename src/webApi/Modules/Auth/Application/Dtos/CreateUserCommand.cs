namespace webApi.Modules.Users.Application.Dtos;

public record CreateUserCommand(
    string Email,
    string PasswordHash
);
