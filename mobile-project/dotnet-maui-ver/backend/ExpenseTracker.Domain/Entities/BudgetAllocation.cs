namespace ExpenseTracker.Domain.Entities;

// Represents allocation of budget limit to a specific category (UC-05.01)
public class BudgetAllocation
{
    public Guid Id { get; private set; }
    public Guid BudgetId { get; private set; }
    public Guid CategoryId { get; private set; }
    public decimal AllocatedAmount { get; private set; }
    public decimal CurrentSpent { get; private set; }
    public decimal AlertThresholdPercent { get; private set; } = 80m; // Default 80% early warning (UC-05.04)

    // Navigation properties
    public Budget? Budget { get; private set; }
    public Category? Category { get; private set; }

    private BudgetAllocation() { } // EF Core constructor

    public BudgetAllocation(
        Guid budgetId,
        Guid categoryId,
        decimal allocatedAmount,
        decimal alertThresholdPercent = 80m)
    {
        if (allocatedAmount < 0)
            throw new ArgumentException("Hạn mức phân bổ danh mục không được âm.", nameof(allocatedAmount));

        if (alertThresholdPercent <= 0 || alertThresholdPercent > 100)
            throw new ArgumentException("Ngưỡng cảnh báo phải nằm trong khoảng từ 1% đến 100%.", nameof(alertThresholdPercent));

        Id = Guid.NewGuid();
        BudgetId = budgetId;
        CategoryId = categoryId;
        AllocatedAmount = allocatedAmount;
        CurrentSpent = 0m;
        AlertThresholdPercent = alertThresholdPercent;
    }

    public void RecordExpense(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Số tiền chi tiêu cập nhật không được âm.", nameof(amount));

        CurrentSpent += amount;
    }

    public void RevertExpense(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Số tiền hoàn tác chi tiêu không được âm.", nameof(amount));

        CurrentSpent = Math.Max(0m, CurrentSpent - amount);
    }

    public void Rebalance(decimal newAllocatedAmount)
    {
        if (newAllocatedAmount < 0)
            throw new ArgumentException("Hạn mức tái cân đối không được âm.", nameof(newAllocatedAmount));

        AllocatedAmount = newAllocatedAmount;
    }

    public void SetAlertThreshold(decimal thresholdPercent)
    {
        if (thresholdPercent <= 0 || thresholdPercent > 100)
            throw new ArgumentException("Ngưỡng cảnh báo phải nằm trong khoảng từ 1% đến 100%.", nameof(thresholdPercent));

        AlertThresholdPercent = thresholdPercent;
    }

    // Computed properties
    public decimal RemainingAmount => AllocatedAmount - CurrentSpent;

    public decimal SpentPercent => AllocatedAmount > 0 
        ? Math.Round((CurrentSpent / AllocatedAmount) * 100m, 2) 
        : (CurrentSpent > 0 ? 100m : 0m);

    public bool IsAlertTriggered => SpentPercent >= AlertThresholdPercent;
    public bool IsExceeded => CurrentSpent > AllocatedAmount;
}
