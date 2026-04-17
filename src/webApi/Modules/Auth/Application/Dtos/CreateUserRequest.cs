using System.ComponentModel.DataAnnotations;
using webApi.Application.Dtos.Auth;
using webApi.Domain.Models;

namespace webApi.Domain.Dtos.Auth;

public record CreateUserRequest (
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [Length(6, 15)]
    string Password
);
