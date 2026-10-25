namespace ExpenseTracker.Application.DTOs.SavingsGoals;

// Response representation of a personal savings goal (UC-06.01)
public record SavingsGoalDto(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description,
    decimal TargetAmount,
    decimal CurrentAmount,
    DateTime StartDate,
    DateTime TargetDate,
    int Type,
    string TypeName,
    int Status,
    string StatusName,
    int Priority,
    string PriorityName,
    string? IconKey,
    string? ColorHex,
    DateTime? CompletedAt,
    decimal ProgressPercent,
    decimal RemainingAmount,
    bool IsCompleted,
    bool IsEmergencyFund,
    bool IsAutoDepositEnabled,
    decimal? AutoDepositAmount,
    int? AutoDepositDayOfMonth,
    Guid? AutoDepositWalletId
);

// Payload to create a new savings goal (UC-06.01)
public record CreateSavingsGoalDto(
    string Name,
    decimal TargetAmount,
    DateTime StartDate,
    DateTime TargetDate,
    int Type = 6, // Default: Other
    int Priority = 2, // Default: Medium
    decimal InitialAmount = 0m,
    string? Description = null,
    string? IconKey = null,
    string? ColorHex = null
);

// Payload to update savings goal details
public record UpdateSavingsGoalDto(
    string Name,
    string? Description,
    decimal TargetAmount,
    DateTime TargetDate,
    int Priority,
    string? IconKey = null,
    string? ColorHex = null
);

// Payload to partially update savings goal metadata via HTTP PATCH (RFC 7396)
public record PatchSavingsGoalDto(
    string? Name = null,
    string? Description = null,
    decimal? TargetAmount = null,
    DateTime? TargetDate = null,
    int? Priority = null,
    string? IconKey = null,
    string? ColorHex = null,
    int? Status = null
);

// Payload to patch savings goal status (Active = 1, Completed = 2, Paused = 3, Cancelled = 4)
public record PatchSavingsGoalStatusDto(
    int Status
);


// Payload to deposit money into a savings goal (UC-06.02)
public record DepositSavingsGoalDto(
    Guid WalletId,
    decimal Amount,
    string? Note = null
);

// Payload to withdraw money from a savings goal with Thaler Nudge (UC-06.03)
public record WithdrawSavingsGoalDto(
    Guid TargetWalletId,
    decimal Amount,
    string Reason
);

// Payload to configure auto-deposit schedule (UC-06.04)
public record ConfigureAutoDepositDto(
    bool IsEnabled,
    decimal? Amount,
    int? DayOfMonth,
    Guid? WalletId
);

// Details of a single contribution/withdrawal transaction
public record SavingsGoalContributionDto(
    Guid Id,
    Guid SavingsGoalId,
    Guid WalletId,
    string WalletName,
    decimal Amount,
    int Type,
    string TypeName,
    DateTime ContributionDate,
    string? Note,
    Guid? TransactionId
);

// Financial feasibility evaluation comparing required savings with budget limit (UC-06.01)
public record GoalFeasibilityDto(
    Guid GoalId,
    string GoalName,
    decimal TargetAmount,
    decimal CurrentAmount,
    decimal RemainingAmount,
    int MonthsRemaining,
    decimal MonthlyDepositRequired,
    decimal MonthlyAvailableSavings,
    decimal FeasibilityPercent,
    bool IsFeasible,
    string AdviceMessage
);

// Breakdown item of savings across Kakeibo/6 Jars categories (UC-06.05)
public record JarCategorySummaryDto(
    int Type,
    string JarName,
    string IconKey,
    string ColorHex,
    decimal TotalCurrentAmount,
    decimal TotalTargetAmount,
    decimal PercentageOfTotalSavings,
    int GoalsCount
);

// Overall Jars summary report (UC-06.05)
public record JarsSummaryDto(
    decimal TotalSavedAmount,
    decimal TotalTargetAmount,
    decimal OverallProgressPercent,
    int ActiveGoalsCount,
    int CompletedGoalsCount,
    List<JarCategorySummaryDto> JarsBreakdown
);
