using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace webApi.Application.Dtos.Auth.Response;

public record CreateUserResponse(
    Guid UserId,
    string Email,
    List<string> Roles
);
