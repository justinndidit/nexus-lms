using System.ComponentModel.DataAnnotations;

namespace webApi.Domain.Models;

public class User
{
    [Key]
    public Guid Id { get; private set; }   
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<UserRole> UserRoles { get; private set; } = [];
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private User() {}

    public User(string email,string passwordHash)
    {
        Email = email.ToLower().Trim();
        PasswordHash = passwordHash;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}