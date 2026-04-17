using System.ComponentModel.DataAnnotations;

namespace webApi.Modules.Auth.Application.Dtos;

public record VerifyEmailRequest(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string VerificationToken
);