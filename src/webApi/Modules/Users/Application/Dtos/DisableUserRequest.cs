namespace webApi.Modules.Users.Application.Dtos;

public record DisableUserRequest(
    Guid UserId,
    string Reason
);
