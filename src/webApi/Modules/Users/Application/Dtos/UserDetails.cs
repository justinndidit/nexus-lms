using System.ComponentModel.DataAnnotations;

namespace webApi.Application.Dtos.Auth;

public record UserDetails(
    [Required]
    [Length(1, 50)]
    string FirstName,

    [Required]
    [Length(1, 50)]
    string LastName,

    [Required]
    [EmailAddress]
    string Email,
    
    [Required]
    string Title
);