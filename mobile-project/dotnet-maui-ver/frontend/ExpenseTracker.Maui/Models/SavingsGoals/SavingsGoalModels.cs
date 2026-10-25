namespace ExpenseTracker.Maui.Models.SavingsGoals;

public enum SavingsGoalType
{
    EmergencyFund = 1,
    MajorPurchase = 2,
    Investment = 3,
    Education = 4,
    Travel = 5,
    Other = 6
}

public enum SavingsGoalStatus
{
    Active = 1,
    Completed = 2,
    Paused = 3,
    Cancelled = 4
}

public enum SavingsGoalPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}

public enum ContributionType
{
    Deposit = 1,
    Withdrawal = 2,
    InterestOrBonus = 3
}

public class SavingsGoalModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal ProgressPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public SavingsGoalType Type { get; set; }
    public SavingsGoalStatus Status { get; set; }
    public SavingsGoalPriority Priority { get; set; }
    public string IconKey { get; set; } = "piggy-bank";
    public string ColorHex { get; set; } = "#2563EB";
    public bool IsEmergencyFund { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal MonthlyDepositRequired { get; set; }

    public bool IsAutoDepositEnabled { get; set; }
    public decimal? AutoDepositAmount { get; set; }
    public int? AutoDepositDayOfMonth { get; set; }
    public Guid? AutoDepositWalletId { get; set; }

    public List<SavingsGoalContributionModel> RecentContributions { get; set; } = new();

    // Presentation helpers
    public string FormattedCurrentAmount => $"{CurrentAmount:N0} đ";
    public string FormattedTargetAmount => $"{TargetAmount:N0} đ";
    public string FormattedRemainingAmount => $"{RemainingAmount:N0} đ";
    public string FormattedMonthlyRequired => $"{MonthlyDepositRequired:N0} đ/tháng";
    public string FormattedProgressPercent => $"{ProgressPercent:F1}%";
    public double NormalizedProgress => Math.Min(1.0, (double)ProgressPercent / 100.0);
    public string TypeDisplayName => Type switch
    {
        SavingsGoalType.EmergencyFund => "Quỹ Khẩn Cấp",
        SavingsGoalType.MajorPurchase => "Mua Sắm Lớn",
        SavingsGoalType.Investment => "Đầu Tư Sinh Lời",
        SavingsGoalType.Education => "Học Tập / Phát Triển",
        SavingsGoalType.Travel => "Du Lịch / Trải Nghiệm",
        _ => "Mục Tiêu Khác"
    };
}

public class SavingsGoalSummaryModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal ProgressPercent { get; set; }
    public DateTime TargetDate { get; set; }
    public SavingsGoalType Type { get; set; }
    public SavingsGoalStatus Status { get; set; }
    public string IconKey { get; set; } = "piggy-bank";
    public string ColorHex { get; set; } = "#2563EB";
    public bool IsEmergencyFund { get; set; }
    public decimal MonthlyDepositRequired { get; set; }

    public string FormattedCurrentAmount => $"{CurrentAmount:N0} đ";
    public string FormattedTargetAmount => $"{TargetAmount:N0} đ";
    public string FormattedProgressPercent => $"{ProgressPercent:F1}%";
    public double NormalizedProgress => Math.Min(1.0, (double)ProgressPercent / 100.0);
}

public class SavingsGoalContributionModel
{
    public Guid Id { get; set; }
    public Guid SavingsGoalId { get; set; }
    public Guid WalletId { get; set; }
    public string WalletName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public ContributionType Type { get; set; }
    public DateTime ContributionDate { get; set; }
    public string? Note { get; set; }

    public string FormattedAmount => Amount >= 0 ? $"+{Amount:N0} đ" : $"{Amount:N0} đ";
    public string FormattedDate => ContributionDate.ToString("dd/MM/yyyy HH:mm");
    public string AmountColorHex => Amount >= 0 ? "#10B981" : "#EF4444";
}

public class CreateSavingsGoalModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal InitialAmount { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime TargetDate { get; set; } = DateTime.Today.AddMonths(6);
    public SavingsGoalType Type { get; set; } = SavingsGoalType.Other;
    public SavingsGoalPriority Priority { get; set; } = SavingsGoalPriority.Medium;
    public string? IconKey { get; set; }
    public string? ColorHex { get; set; }
}

public class UpdateSavingsGoalModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TargetAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public SavingsGoalPriority Priority { get; set; }
    public string? IconKey { get; set; }
    public string? ColorHex { get; set; }
}

public class DepositSavingsGoalModel
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}

public class WithdrawSavingsGoalModel
{
    public Guid TargetWalletId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ConfigureAutoDepositModel
{
    public bool IsEnabled { get; set; }
    public decimal? Amount { get; set; }
    public int? DayOfMonth { get; set; }
    public Guid? WalletId { get; set; }
}

public class SavingsFeasibilityModel
{
    public Guid GoalId { get; set; }
    public string GoalName { get; set; } = string.Empty;
    public decimal MonthlyRequired { get; set; }
    public decimal ActiveSavingsBudgetPool { get; set; }
    public bool IsFeasible { get; set; }
    public decimal FeasibilityScorePercent { get; set; }
    public string AdviceMessage { get; set; } = string.Empty;

    public string FormattedMonthlyRequired => $"{MonthlyRequired:N0} đ/tháng";
    public string FormattedSavingsBudgetPool => $"{ActiveSavingsBudgetPool:N0} đ/tháng";
    public string ScoreColorHex => IsFeasible ? "#10B981" : (FeasibilityScorePercent >= 70 ? "#F59E0B" : "#EF4444");
}

public class JarsSummaryModel
{
    public decimal TotalSavingsBalance { get; set; }
    public int TotalGoalsCount { get; set; }
    public int CompletedGoalsCount { get; set; }
    public List<JarItemModel> Jars { get; set; } = new();

    public string FormattedTotalBalance => $"{TotalSavingsBalance:N0} đ";
}

public class JarItemModel
{
    public string JarCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal TargetPercentage { get; set; }
    public decimal ActualPercentage { get; set; }
    public string ColorHex { get; set; } = "#2563EB";
    public string IconKey { get; set; } = "archive";

    public string FormattedTotalAmount => $"{TotalAmount:N0} đ";
    public string FormattedPercentages => $"{ActualPercentage:F1}% (Mục tiêu: {TargetPercentage:F0}%)";
}

public class PatchSavingsGoalModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? TargetAmount { get; set; }
    public DateTime? TargetDate { get; set; }
    public SavingsGoalPriority? Priority { get; set; }
    public string? IconKey { get; set; }
    public string? ColorHex { get; set; }
    public SavingsGoalStatus? Status { get; set; }
}

public class PatchSavingsGoalStatusModel
{
    public SavingsGoalStatus Status { get; set; }
}

