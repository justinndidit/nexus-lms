using System.ComponentModel.DataAnnotations;

namespace webApi.Modules.Users.Application.Dtos;

public record UpdateUserPasswordCommand(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string PasswordHash
);
