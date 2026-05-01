namespace webApi.Modules.Rbac.Application.DTOs;

public record CreateUserResult(
    Guid UserId,
    string Email,
    List<string> RoleNames
);
