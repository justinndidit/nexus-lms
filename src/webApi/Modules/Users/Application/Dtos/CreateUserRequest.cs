using System.ComponentModel.DataAnnotations;

namespace webApi.Modules.Users.Application.Dtos;

public record CreateUserRequest (
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [Length(6, 15)]
    string Password
);
