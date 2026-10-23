namespace ExpenseTracker.Application.DTOs.Budgets;

// Current state of Adaptive Daily Spending Allowance (UC-05.05)
public record DsaStatusDto(
    DateTime Date,
    decimal DailyAllowanceAmount,
    decimal ActualSpentToday,
    decimal RemainingAllowanceToday,
    decimal RemainingFlexibleBudget,
    int RemainingDays,
    bool IsExceeded,
    string AdviceMessage
);

// Spending Velocity assessment and out-of-money forecast (UC-05.06)
public record VelocityStatusDto(
    decimal VelocityIndex,
    string LevelName, // Safe, Caution, Danger
    string LevelColorHex, // #10B981 (Green), #F59E0B (Amber), #EF4444 (Red)
    decimal ActualVelocityPerDay,
    decimal TargetVelocityPerDay,
    DateTime? OutOfMoneyForecastDate,
    int? DaysUntilDepletion,
    string ForecastMessage
);

// Unified overview of budget progress, DSA widget, and velocity tracker (UC-05.02)
public record BudgetProgressDto(
    Guid BudgetId,
    string BudgetName,
    decimal TotalAmount,
    decimal TotalSpent,
    decimal TotalRemaining,
    decimal OverallSpentPercent,
    int DaysPassed,
    int TotalDays,
    DsaStatusDto Dsa,
    VelocityStatusDto Velocity,
    List<BudgetAllocationDto> Allocations
);
