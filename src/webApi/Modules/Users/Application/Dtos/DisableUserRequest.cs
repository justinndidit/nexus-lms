namespace webApi.Modules.Auth.Application.Dtos;

public record DisableUserRequest(
    Guid UserId,
    string Reason
);
