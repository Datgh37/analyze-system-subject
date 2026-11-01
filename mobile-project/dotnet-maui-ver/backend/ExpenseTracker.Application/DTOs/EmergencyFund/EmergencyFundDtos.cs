namespace ExpenseTracker.Application.DTOs.EmergencyFund;

// DTO providing personal emergency fund recommendation based on average essential needs (UC-06.04)
public class EmergencyFundRecommendationDto
{
    public decimal MonthlyEssentialExpenses { get; set; }
    public decimal ThreeMonthsAmount { get; set; }
    public decimal SixMonthsAmount { get; set; }
    public decimal TwelveMonthsAmount { get; set; }
    public decimal CurrentEmergencyFundAmount { get; set; }
    public decimal CurrentMonthsCovered { get; set; }
    public string RecommendationNote { get; set; } = string.Empty;
}

// DTO for setting up or updating emergency fund goal (UC-06.04)
public class SetupEmergencyFundDto
{
    public int TargetMonths { get; set; } = 6; // 3, 6, or 12
    public decimal? TargetAmount { get; set; }
    public Guid? InitialDepositWalletId { get; set; }
    public decimal? InitialDepositAmount { get; set; }
}

// DTO reflecting the current emergency fund status and anti-impulse discipline (UC-06.04)
public class EmergencyFundStatusDto
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

// DTO for depositing money into emergency fund
public class DepositEmergencyFundDto
{
    public decimal Amount { get; set; }
    public Guid SourceWalletId { get; set; }
    public string? Note { get; set; }
}

// DTO for withdrawing money from emergency fund with cognitive friction nudge (UC-06.04, UC-06.03)
public class WithdrawEmergencyFundDto
{
    public decimal Amount { get; set; }
    public Guid TargetWalletId { get; set; }
    public string EmergencyReason { get; set; } = string.Empty; // "Medical", "JobLoss", "UrgentRepair", "OtherCritical"
    public string DetailedExplanation { get; set; } = string.Empty; // Richard Thaler friction
}

