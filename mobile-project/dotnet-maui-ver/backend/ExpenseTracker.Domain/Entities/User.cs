using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// User aggregate root representing an authenticated individual account
public class User : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string BaseCurrency { get; private set; } = "VND";
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public bool IsEmailVerified { get; private set; } = false;

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public ICollection<Wallet> Wallets { get; private set; } = new List<Wallet>();
    public ICollection<Category> Categories { get; private set; } = new List<Category>();
    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
    public ICollection<RecurringIncome> RecurringIncomes { get; private set; } = new List<RecurringIncome>();
    public ICollection<SalarySlipDetail> SalarySlipDetails { get; private set; } = new List<SalarySlipDetail>();
    public ICollection<AutoCategorizationRule> AutoCategorizationRules { get; private set; } = new List<AutoCategorizationRule>();
    public ICollection<UserOtpCode> OtpCodes { get; private set; } = new List<UserOtpCode>();

    // Parameterless constructor for EF Core materialization
    private User() { }

    public User(string fullName, string email, string passwordHash, string baseCurrency = "VND", bool isEmailVerified = false)
    {
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        BaseCurrency = string.IsNullOrWhiteSpace(baseCurrency) ? "VND" : baseCurrency.Trim().ToUpperInvariant();
        Status = UserStatus.Active;
        IsEmailVerified = isEmailVerified;
        CreatedAt = DateTime.UtcNow;
    }

    // Updates profile details and stamps audit modification time
    public void UpdateProfile(string fullName, string baseCurrency)
    {
        FullName = fullName.Trim();
        BaseCurrency = string.IsNullOrWhiteSpace(baseCurrency) ? "VND" : baseCurrency.Trim().ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    // Updates password hash securely
    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    // Marks user's email address as verified after valid OTP confirmation
    public void VerifyEmail()
    {
        IsEmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // Locks user account from performing operations
    public void LockAccount()
    {
        Status = UserStatus.Locked;
        UpdatedAt = DateTime.UtcNow;
    }
}
