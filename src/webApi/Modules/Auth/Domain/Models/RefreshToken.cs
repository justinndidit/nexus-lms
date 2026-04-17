
namespace webApi.Domain.Models;
public class RefreshToken
{
    public Guid Id { get; set; }

    public string Token { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsRevoked { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;

    private RefreshToken(){}

    public static RefreshToken Create(string token, Guid userId, int daysValid = 7)
    {
        return new RefreshToken
        {
            Token = token,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(daysValid)
        };
    }
}
