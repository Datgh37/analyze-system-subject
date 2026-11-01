namespace ExpenseTracker.Maui.Models.SmartUtilities;

public class SubscriptionModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int BillingCycle { get; set; } // 1: Weekly, 2: Monthly, 3: Quarterly, 4: Yearly
    public string BillingCycleName { get; set; } = string.Empty;
    public DateTime NextBillingDate { get; set; }
    public int DaysUntilNextBilling { get; set; }
    public Guid? WalletId { get; set; }
    public string? WalletName { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int Status { get; set; } // 1: Active, 2: Cancelled, 3: Paused, 4: FreeTrial
    public string StatusName { get; set; } = string.Empty;
    public DateTime? TrialEndDate { get; set; }
    public bool AutoRenew { get; set; }
    public int RemindDaysBefore { get; set; }
    public string? Notes { get; set; }
    public decimal MonthlyCost { get; set; }
    public decimal AnnualCost { get; set; }
    public bool IsExpiringSoon { get; set; }
}

public class CreateSubscriptionRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int BillingCycle { get; set; } = 2; // Monthly
    public DateTime NextBillingDate { get; set; } = DateTime.UtcNow.AddMonths(1);
    public Guid CategoryId { get; set; }
    public Guid? WalletId { get; set; }
    public int Status { get; set; } = 1; // Active
    public DateTime? TrialEndDate { get; set; }
    public bool AutoRenew { get; set; } = true;
    public int RemindDaysBefore { get; set; } = 3;
    public string? Notes { get; set; }
}

public class UpdateSubscriptionRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int BillingCycle { get; set; }
    public DateTime NextBillingDate { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? WalletId { get; set; }
    public int Status { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public bool AutoRenew { get; set; }
    public int RemindDaysBefore { get; set; }
    public string? Notes { get; set; }
}

public class SubscriptionCandidateModel
{
    public string ServiceName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal SuggestedAmount { get; set; }
    public int SuggestedBillingCycle { get; set; }
    public int OccurrencesCount { get; set; }
    public DateTime LastChargedDate { get; set; }
    public string DetectionSource { get; set; } = "CatalogMatch";
    public string AdviceMessage { get; set; } = string.Empty;
}

public class SubscriptionAuditReportModel
{
    public int ActiveSubscriptionsCount { get; set; }
    public decimal TotalMonthlyDrain { get; set; }
    public decimal TotalAnnualDrain { get; set; }
    public int ZombieSubscriptionsCount { get; set; }
    public int FreeTrialAlertsCount { get; set; }
    public List<SubscriptionModel> ManagedSubscriptions { get; set; } = new();
    public List<SubscriptionCandidateModel> DetectedCandidates { get; set; } = new();
    public string FinancialHealthAlert { get; set; } = string.Empty;
}

public class UpcomingRenewalModel
{
    public Guid SubscriptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime NextBillingDate { get; set; }
    public int DaysLeft { get; set; }
    public string? WalletName { get; set; }
    public bool IsFreeTrialEnding { get; set; }
}

