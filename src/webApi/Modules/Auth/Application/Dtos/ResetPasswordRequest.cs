using System.ComponentModel.DataAnnotations;

namespace webApi.Application.Dtos.Auth.Request;

public record ResetPasswordRequest(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Password,
    [Required]
    string RePassword
);
