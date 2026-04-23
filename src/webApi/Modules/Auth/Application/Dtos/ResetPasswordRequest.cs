using System.ComponentModel.DataAnnotations;

namespace webApi.Modules.Auth.Application.Dtos;

public record ResetPasswordRequest(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Password,
    [Required]
    string RePassword
);
