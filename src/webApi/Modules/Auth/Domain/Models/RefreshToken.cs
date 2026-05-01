
using webApi.Modules.Users.Domain.Models;

namespace webApi.Modules.Auth.Domain.Models;
public class RefreshToken
{
    public Guid Id { get; set; }

    public string Token { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsRevoked { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    private RefreshToken(){}

    public static RefreshToken Create(string token, string userId, int daysValid = 7)
    {
        return new RefreshToken
        {
            Token = token,
            UserId = Guid.Parse(userId),
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(daysValid)
        };
    }

    public void Revoke()
    {
        IsRevoked = true;
    }

    public bool IsTokenValid()
    {
        return 
            !IsRevoked &&
                ExpiryDate >= DateTime.UtcNow;
    }
}
