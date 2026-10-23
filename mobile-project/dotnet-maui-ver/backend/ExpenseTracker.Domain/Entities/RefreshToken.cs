using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

// Refresh token entity managing user session persistence and token rotation
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Parameterless constructor for EF Core materialization
    private RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    // Revokes current refresh token and optionally links replacement token
    public void Revoke(string? replacedByToken = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
        UpdatedAt = DateTime.UtcNow;
    }
}
