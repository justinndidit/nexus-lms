using System.ComponentModel.DataAnnotations;
using webApi.Domain.Models;

namespace webApi.Application.Dtos.Auth.Response;

public record LoginResponse(
    // [Required]
    // [Length(1, 50)]
    // string FirstName,

    // [Required]
    // [Length(1, 50)]
    // string LastName,

    [Required]
    [EmailAddress]
    string Email,
    
    // [Required]
    // [MaxLength(50)]
    // string Title,
    
    List<string> Roles,
    Token TokenData
);
