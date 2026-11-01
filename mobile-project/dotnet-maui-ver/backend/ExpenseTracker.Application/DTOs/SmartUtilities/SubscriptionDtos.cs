using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.SmartUtilities;

// DTO chi tiết thực thể Thuê bao định kỳ (UC-03.09)
public class SubscriptionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public SubscriptionBillingCycle BillingCycle { get; set; }
    public string BillingCycleName { get; set; } = string.Empty;
    public DateTime NextBillingDate { get; set; }
    public int DaysUntilNextBilling { get; set; }
    public Guid? WalletId { get; set; }
    public string? WalletName { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime? TrialEndDate { get; set; }
    public bool AutoRenew { get; set; }
    public int RemindDaysBefore { get; set; }
    public string? Notes { get; set; }
    public decimal MonthlyCost { get; set; }
    public decimal AnnualCost { get; set; }
    public bool IsExpiringSoon { get; set; }
}

// DTO tạo mới thuê bao thủ công
public class CreateSubscriptionDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public SubscriptionBillingCycle BillingCycle { get; set; } = SubscriptionBillingCycle.Monthly;
    public DateTime NextBillingDate { get; set; } = DateTime.UtcNow.AddMonths(1);
    public Guid CategoryId { get; set; }
    public Guid? WalletId { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime? TrialEndDate { get; set; }
    public bool AutoRenew { get; set; } = true;
    public int RemindDaysBefore { get; set; } = 3;
    public string? Notes { get; set; }
}

// DTO cập nhật thuê bao
public class UpdateSubscriptionDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public SubscriptionBillingCycle BillingCycle { get; set; }
    public DateTime NextBillingDate { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? WalletId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public bool AutoRenew { get; set; }
    public int RemindDaysBefore { get; set; }
    public string? Notes { get; set; }
}

// Ứng viên thuê bao phát hiện từ thuật toán quét (Catalog hoặc Cluster)
public class SubscriptionCandidateDto
{
    public string ServiceName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal SuggestedAmount { get; set; }
    public SubscriptionBillingCycle SuggestedBillingCycle { get; set; }
    public int OccurrencesCount { get; set; }
    public DateTime LastChargedDate { get; set; }
    public string DetectionSource { get; set; } = "CatalogMatch"; // "CatalogMatch", "RecurringCluster", "FreeTrialTrap"
    public string AdviceMessage { get; set; } = string.Empty;
}

// Báo cáo kiểm toán bẫy thuê bao toàn diện (UC-03.09)
public class SubscriptionAuditReportDto
{
    public int ActiveSubscriptionsCount { get; set; }
    public decimal TotalMonthlyDrain { get; set; }
    public decimal TotalAnnualDrain { get; set; }
    public int ZombieSubscriptionsCount { get; set; }
    public int FreeTrialAlertsCount { get; set; }
    public List<SubscriptionDto> ManagedSubscriptions { get; set; } = new();
    public List<SubscriptionCandidateDto> DetectedCandidates { get; set; } = new();
    public string FinancialHealthAlert { get; set; } = string.Empty;
}

// Khoản thuê bao sắp đến hạn thanh toán
public class UpcomingRenewalDto
{
    public Guid SubscriptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime NextBillingDate { get; set; }
    public int DaysLeft { get; set; }
    public string? WalletName { get; set; }
    public bool IsFreeTrialEnding { get; set; }
}

// Legacy DTOs for backward-compatibility with older tests
public record DetectedSubscriptionDto(
    string ServiceName,
    Guid CategoryId,
    string CategoryName,
    decimal RecurringAmount,
    decimal EstimatedAnnualCost,
    int OccurrencesCount,
    DateTime LastChargedDate,
    string AdviceMessage
);

public record SubscriptionAnalysisResultDto(
    List<DetectedSubscriptionDto> Subscriptions,
    decimal TotalMonthlyDrain,
    decimal TotalAnnualDrain,
    string FinancialHealthAlert
);
