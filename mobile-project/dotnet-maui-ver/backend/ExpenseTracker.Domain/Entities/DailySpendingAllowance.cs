namespace ExpenseTracker.Domain.Entities;

// Record tracking Adaptive Daily Spending Allowance (DSA) and Spending Velocity (UC-05.05/05.06)
public class DailySpendingAllowance
{
    public Guid Id { get; private set; }
    public Guid BudgetId { get; private set; }
    public DateTime Date { get; private set; }
    public decimal DailyAllowanceAmount { get; private set; }
    public decimal ActualSpentToday { get; private set; }
    public decimal RemainingAllowanceToday { get; private set; }
    public decimal VelocityIndex { get; private set; } = 1.0m;
    public DateTime? OutOfMoneyForecastDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation property
    public Budget? Budget { get; private set; }

    private DailySpendingAllowance() { } // EF Core constructor

    public DailySpendingAllowance(
        Guid budgetId,
        DateTime date,
        decimal dailyAllowanceAmount,
        decimal actualSpentToday = 0m,
        decimal velocityIndex = 1.0m,
        DateTime? outOfMoneyForecastDate = null)
    {
        Id = Guid.NewGuid();
        BudgetId = budgetId;
        Date = date.Date;
        DailyAllowanceAmount = Math.Max(0m, dailyAllowanceAmount);
        ActualSpentToday = Math.Max(0m, actualSpentToday);
        RemainingAllowanceToday = DailyAllowanceAmount - ActualSpentToday;
        VelocityIndex = Math.Max(0m, velocityIndex);
        OutOfMoneyForecastDate = outOfMoneyForecastDate;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateAllowance(
        decimal newDailyAllowance,
        decimal actualSpentToday,
        decimal velocityIndex,
        DateTime? outOfMoneyForecastDate)
    {
        DailyAllowanceAmount = Math.Max(0m, newDailyAllowance);
        ActualSpentToday = Math.Max(0m, actualSpentToday);
        RemainingAllowanceToday = DailyAllowanceAmount - ActualSpentToday;
        VelocityIndex = Math.Max(0m, velocityIndex);
        OutOfMoneyForecastDate = outOfMoneyForecastDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordExpenseToday(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Số tiền chi tiêu trong ngày không được âm.", nameof(amount));

        ActualSpentToday += amount;
        RemainingAllowanceToday = DailyAllowanceAmount - ActualSpentToday;
        UpdatedAt = DateTime.UtcNow;
    }
}
