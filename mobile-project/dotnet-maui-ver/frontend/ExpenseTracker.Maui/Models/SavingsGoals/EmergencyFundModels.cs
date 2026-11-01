namespace ExpenseTracker.Maui.Models.SavingsGoals;

public class EmergencyFundRecommendationModel
{
    public decimal MonthlyEssentialExpenses { get; set; }
    public decimal ThreeMonthsAmount { get; set; }
    public decimal SixMonthsAmount { get; set; }
    public decimal TwelveMonthsAmount { get; set; }
    public decimal CurrentEmergencyFundAmount { get; set; }
    public decimal CurrentMonthsCovered { get; set; }
    public string RecommendationNote { get; set; } = string.Empty;
}

public class SetupEmergencyFundRequest
{
    public int TargetMonths { get; set; } = 6;
    public decimal? TargetAmount { get; set; }
    public Guid? InitialDepositWalletId { get; set; }
    public decimal? InitialDepositAmount { get; set; }
}

public class EmergencyFundStatusModel
{
    public Guid? SavingsGoalId { get; set; }
    public bool HasFundConfigured { get; set; }
    public int TargetMonths { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal MonthlyEssentialExpenses { get; set; }
    public decimal MonthsCovered { get; set; }
    public decimal ProgressPercent { get; set; }
    public bool IsLocked { get; set; } = true;
    public string Status { get; set; } = "NotConfigured";
    public DateTime? LastDepositAt { get; set; }
}

public class DepositEmergencyFundRequest
{
    public decimal Amount { get; set; }
    public Guid SourceWalletId { get; set; }
    public string? Note { get; set; }
}

public class WithdrawEmergencyFundRequest
{
    public decimal Amount { get; set; }
    public Guid TargetWalletId { get; set; }
    public string EmergencyReason { get; set; } = string.Empty;
    public string DetailedExplanation { get; set; } = string.Empty;
}

