using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Aggregate Root quản lý cam kết đăng ký dịch vụ định kỳ và bẫy gia hạn ngầm (UC-03.09)
public class Subscription : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public SubscriptionBillingCycle BillingCycle { get; private set; } = SubscriptionBillingCycle.Monthly;
    public DateTime NextBillingDate { get; private set; }

    public Guid? WalletId { get; private set; }
    public Wallet? Wallet { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;
    public DateTime? TrialEndDate { get; private set; }
    public bool AutoRenew { get; private set; } = true;
    public int RemindDaysBefore { get; private set; } = 3;
    public string? Notes { get; private set; }

    // Computed properties
    public decimal AnnualCost => BillingCycle switch
    {
        SubscriptionBillingCycle.Weekly => Amount * 52m,
        SubscriptionBillingCycle.Monthly => Amount * 12m,
        SubscriptionBillingCycle.Quarterly => Amount * 4m,
        SubscriptionBillingCycle.Yearly => Amount,
        _ => Amount * 12m
    };

    public decimal MonthlyCost => BillingCycle switch
    {
        SubscriptionBillingCycle.Weekly => Math.Round(Amount * 52m / 12m, 0),
        SubscriptionBillingCycle.Monthly => Amount,
        SubscriptionBillingCycle.Quarterly => Math.Round(Amount / 3m, 0),
        SubscriptionBillingCycle.Yearly => Math.Round(Amount / 12m, 0),
        _ => Amount
    };

    // Parameterless constructor for EF Core materialization
    private Subscription() { }

    public Subscription(
        Guid userId,
        string name,
        decimal amount,
        SubscriptionBillingCycle billingCycle,
        DateTime nextBillingDate,
        Guid categoryId,
        Guid? walletId = null,
        SubscriptionStatus status = SubscriptionStatus.Active,
        DateTime? trialEndDate = null,
        bool autoRenew = true,
        int remindDaysBefore = 3,
        string? notes = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Mã người dùng không hợp lệ.", nameof(userId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên dịch vụ thuê bao không được để trống.", nameof(name));

        if (amount <= 0 && status != SubscriptionStatus.FreeTrial)
            throw new ArgumentException("Số tiền thuê bao phải lớn hơn 0 đ (trừ trường hợp dùng thử miễn phí).", nameof(amount));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("Danh mục chi tiêu không hợp lệ.", nameof(categoryId));

        if (remindDaysBefore < 0 || remindDaysBefore > 30)
            throw new ArgumentOutOfRangeException(nameof(remindDaysBefore), "Số ngày nhắc trước phải từ 0 đến 30 ngày.");

        UserId = userId;
        Name = name.Trim();
        Amount = amount;
        BillingCycle = billingCycle;
        NextBillingDate = nextBillingDate.Kind == DateTimeKind.Utc 
            ? nextBillingDate 
            : DateTime.SpecifyKind(nextBillingDate, DateTimeKind.Utc);
        CategoryId = categoryId;
        WalletId = walletId;
        Status = status;
        TrialEndDate = trialEndDate.HasValue 
            ? (trialEndDate.Value.Kind == DateTimeKind.Utc ? trialEndDate.Value : DateTime.SpecifyKind(trialEndDate.Value, DateTimeKind.Utc))
            : null;
        AutoRenew = autoRenew;
        RemindDaysBefore = remindDaysBefore;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        decimal amount,
        SubscriptionBillingCycle billingCycle,
        DateTime nextBillingDate,
        Guid categoryId,
        Guid? walletId,
        SubscriptionStatus status,
        DateTime? trialEndDate,
        bool autoRenew,
        int remindDaysBefore,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên dịch vụ thuê bao không được để trống.", nameof(name));

        if (amount <= 0 && status != SubscriptionStatus.FreeTrial)
            throw new ArgumentException("Số tiền thuê bao phải lớn hơn 0 đ.", nameof(amount));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("Danh mục chi tiêu không hợp lệ.", nameof(categoryId));

        if (remindDaysBefore < 0 || remindDaysBefore > 30)
            throw new ArgumentOutOfRangeException(nameof(remindDaysBefore), "Số ngày nhắc trước phải từ 0 đến 30 ngày.");

        Name = name.Trim();
        Amount = amount;
        BillingCycle = billingCycle;
        NextBillingDate = nextBillingDate.Kind == DateTimeKind.Utc 
            ? nextBillingDate 
            : DateTime.SpecifyKind(nextBillingDate, DateTimeKind.Utc);
        CategoryId = categoryId;
        WalletId = walletId;
        Status = status;
        TrialEndDate = trialEndDate.HasValue 
            ? (trialEndDate.Value.Kind == DateTimeKind.Utc ? trialEndDate.Value : DateTime.SpecifyKind(trialEndDate.Value, DateTimeKind.Utc))
            : null;
        AutoRenew = autoRenew;
        RemindDaysBefore = remindDaysBefore;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    // Tự động nhảy kỳ thanh toán tiếp theo sau khi đã trừ tiền
    public void RenewToNextCycle()
    {
        NextBillingDate = BillingCycle switch
        {
            SubscriptionBillingCycle.Weekly => NextBillingDate.AddDays(7),
            SubscriptionBillingCycle.Monthly => NextBillingDate.AddMonths(1),
            SubscriptionBillingCycle.Quarterly => NextBillingDate.AddMonths(3),
            SubscriptionBillingCycle.Yearly => NextBillingDate.AddYears(1),
            _ => NextBillingDate.AddMonths(1)
        };

        if (Status == SubscriptionStatus.FreeTrial)
        {
            Status = SubscriptionStatus.Active;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Pause()
    {
        Status = SubscriptionStatus.Paused;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = SubscriptionStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }
}

