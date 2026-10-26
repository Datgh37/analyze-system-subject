namespace ExpenseTracker.Maui.Models.Budgets;

// Individual category budget allocation model (UC-05.01/05.02)
public class BudgetAllocationModel
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryIcon { get; set; }
    public string? CategoryColor { get; set; }
    public int? BudgetRuleMapping { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal CurrentSpent { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal SpentPercent { get; set; }
    public decimal AlertThresholdPercent { get; set; } = 80m;
    public bool IsAlertTriggered { get; set; }
    public bool IsExceeded { get; set; }

    // UI Presentation helpers
    public string FormattedAllocated => $"{AllocatedAmount:N0} đ";
    public string FormattedSpent => $"{CurrentSpent:N0} đ";
    public string FormattedRemaining => $"{RemainingAmount:N0} đ";
    public string ProgressPercentLabel => $"{SpentPercent:F0}%";
    public double ProgressFraction => Math.Clamp((double)SpentPercent / 100.0, 0.0, 1.0);

    // Ergonomic 3-band color scale (UC-05.02 Step 5)
    public string ProgressBarColor => SpentPercent switch
    {
        < 70m => "#10B981",  // Safe Green
        < 90m => "#F59E0B",  // Caution Amber
        _ => "#EF4444"       // Danger Red
    };

    public string RuleName => BudgetRuleMapping switch
    {
        1 => "Thiết yếu (Needs 50%)",
        2 => "Mong muốn (Wants 30%)",
        3 => "Tích lũy (Savings 20%)",
        _ => "Phân bổ khác"
    };

    public string DisplayCategoryIcon => (CategoryName?.ToLowerInvariant()) switch
    {
        var n when n != null && (n.Contains("ăn") || n.Contains("uống") || n.Contains("thực phẩm") || n.Contains("food")) => "🍜",
        var n when n != null && (n.Contains("nhà") || n.Contains("tiện ích") || n.Contains("điện") || n.Contains("nước") || n.Contains("home")) => "🏠",
        var n when n != null && (n.Contains("cà phê") || n.Contains("cafe") || n.Contains("coffee") || n.Contains("giải trí")) => "☕",
        var n when n != null && (n.Contains("mua") || n.Contains("sắm") || n.Contains("shop")) => "🛍️",
        var n when n != null && (n.Contains("xe") || n.Contains("xăng") || n.Contains("di chuyển") || n.Contains("grab")) => "🚗",
        var n when n != null && (n.Contains("học") || n.Contains("sách")) => "📚",
        var n when n != null && (n.Contains("y tế") || n.Contains("thuốc") || n.Contains("khám")) => "💊",
        var n when n != null && (n.Contains("tích lũy") || n.Contains("tiết kiệm")) => "💰",
        _ => !string.IsNullOrWhiteSpace(CategoryIcon) && CategoryIcon.Length <= 2 ? CategoryIcon : "🏷️"
    };
}

public record CreateBudgetAllocationModel(
    Guid CategoryId,
    decimal AllocatedAmount,
    decimal AlertThresholdPercent = 80m
);

public record CreateBudgetModel(
    string Name,
    int PeriodType,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalAmount,
    int Method,
    List<CreateBudgetAllocationModel>? Allocations = null
);

public record UpdateBudgetModel(
    string Name,
    decimal TotalAmount
);

public record RebalanceAllocationModel(
    Guid CategoryId,
    decimal NewAllocatedAmount
);

public record RebalanceBudgetModel(
    List<RebalanceAllocationModel> Rebalances
);

public record BudgetModel(
    Guid Id,
    Guid UserId,
    string Name,
    int PeriodType,
    string PeriodTypeName,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalAmount,
    int Method,
    string MethodName,
    int Status,
    string StatusName,
    decimal TotalAllocated,
    decimal TotalSpent,
    decimal TotalRemaining,
    decimal OverallSpentPercent,
    int TotalDays,
    List<BudgetAllocationModel> Allocations
)
{
    public string FormattedTotal => $"{TotalAmount:N0} đ";
    public string FormattedSpent => $"{TotalSpent:N0} đ";
    public string FormattedRemaining => $"{TotalRemaining:N0} đ";
    public double OverallProgressFraction => Math.Clamp((double)OverallSpentPercent / 100.0, 0.0, 1.0);
    public double ProgressFraction => OverallProgressFraction;
    public decimal TotalAllocatedAmount => TotalAllocated;
    public decimal TotalSpentAmount => TotalSpent;
    public decimal TotalRemainingAmount => TotalRemaining;
    public string PeriodSummary => $"{StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";
}

public record BudgetSummaryModel(
    Guid Id,
    string Name,
    decimal TotalAmount,
    decimal TotalSpent,
    decimal TotalRemaining,
    decimal OverallSpentPercent,
    string StatusName
);

// Adaptive Daily Spending Allowance (DSA) status model (UC-05.05)
public record DsaStatusModel(
    DateTime Date,
    decimal DailyAllowanceAmount,
    decimal ActualSpentToday,
    decimal RemainingAllowanceToday,
    decimal RemainingFlexibleBudget,
    int RemainingDays,
    bool IsExceeded,
    string AdviceMessage
)
{
    public string FormattedDailyAllowance => $"{DailyAllowanceAmount:N0} đ";
    public string FormattedSpentToday => $"{ActualSpentToday:N0} đ";
    public string FormattedRemainingToday => $"{RemainingAllowanceToday:N0} đ";
    public string StatusBadgeColor => IsExceeded ? "#EF4444" : "#10B981";
    public decimal DailyAllowance => DailyAllowanceAmount;
    public decimal TodaySpent => ActualSpentToday;
    public decimal RemainingToday => RemainingAllowanceToday;
    public string DsaColor => StatusBadgeColor;
    public string Note => AdviceMessage;
}

// Spending Velocity tracker status model (UC-05.06)
public record VelocityStatusModel(
    decimal VelocityIndex,
    string LevelName,
    string LevelColorHex,
    decimal ActualVelocityPerDay,
    decimal TargetVelocityPerDay,
    DateTime? OutOfMoneyForecastDate,
    int? DaysUntilDepletion,
    string ForecastMessage
)
{
    public string VelocityIndexLabel => $"{VelocityIndex:F2}x";
    public string FormattedActualVelocity => $"{ActualVelocityPerDay:N0} đ/ngày";
    public string FormattedTargetVelocity => $"{TargetVelocityPerDay:N0} đ/ngày";
    public bool HasDepletionForecast => OutOfMoneyForecastDate.HasValue;
    public string VelocityColor => LevelColorHex;
    public string StatusText => LevelName;
    public string BurnRateDescription => HasDepletionForecast ? "Vượt tốc độ cho phép!" : "Tốc độ an toàn";
    public decimal ActualDailyBurnRate => ActualVelocityPerDay;
    public decimal TargetDailyBurnRate => TargetVelocityPerDay;
    public bool IsCritical => HasDepletionForecast;
}

// Unified overview of budget progress, DSA widget, and velocity tracker (UC-05.02)
public record BudgetProgressModel(
    Guid BudgetId,
    string BudgetName,
    decimal TotalAmount,
    decimal TotalSpent,
    decimal TotalRemaining,
    decimal OverallSpentPercent,
    int DaysPassed,
    int TotalDays,
    DsaStatusModel Dsa,
    VelocityStatusModel Velocity,
    List<BudgetAllocationModel> Allocations
);
