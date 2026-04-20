using System.ComponentModel.DataAnnotations;

namespace webApi.Modules.Auth.Domain.Interfaces;

public record UpdateUserPasswordCommand(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string PasswordHash
);
