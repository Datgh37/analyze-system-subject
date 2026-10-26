using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Stores one-time verification tokens for email confirmation, phone validation, and password resets
public class UserOtpCode : BaseEntity
{
    public Guid? UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string OtpCode { get; private set; } = string.Empty;
    public OtpPurpose Purpose { get; private set; }
    public DateTime ExpiredAt { get; private set; }
    public bool IsUsed { get; private set; } = false;
    public DateTime? UsedAt { get; private set; }

    public User? User { get; private set; }

    // Parameterless constructor for EF Core materialization
    private UserOtpCode() { }

    public UserOtpCode(
        string email,
        string otpCode,
        OtpPurpose purpose,
        TimeSpan validityDuration,
        Guid? userId = null,
        string? phoneNumber = null)
    {
        Email = email.Trim().ToLowerInvariant();
        OtpCode = otpCode.Trim();
        Purpose = purpose;
        UserId = userId;
        PhoneNumber = phoneNumber?.Trim();
        CreatedAt = DateTime.UtcNow;
        ExpiredAt = DateTime.UtcNow.Add(validityDuration);
        IsUsed = false;
    }

    // Validates whether the provided OTP code matches, has not expired, and has not yet been consumed
    public bool IsValid(string inputCode)
    {
        if (IsUsed) return false;
        if (DateTime.UtcNow > ExpiredAt) return false;
        return string.Equals(OtpCode, inputCode?.Trim(), StringComparison.Ordinal);
    }

    // Consumes the OTP token so it cannot be replayed
    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
