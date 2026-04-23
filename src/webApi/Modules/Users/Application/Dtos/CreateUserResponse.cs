using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace webApi.Modules.Users.Application.Dtos;

public record CreateUserResponse(
    string UserId,
    string Email,
    List<string> Roles
);
