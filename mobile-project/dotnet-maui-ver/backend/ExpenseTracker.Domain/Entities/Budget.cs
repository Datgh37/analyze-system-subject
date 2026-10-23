using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Aggregate Root managing budget cycle, methodologies (50/30/20, ZBB, Kakeibo), and allocations (UC-05.01 - 05.06)
public class Budget
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public BudgetPeriodType PeriodType { get; private set; } = BudgetPeriodType.Monthly;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public BudgetMethod Method { get; private set; } = BudgetMethod.Rule503020;
    public BudgetStatus Status { get; private set; } = BudgetStatus.Active;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation collections
    private readonly List<BudgetAllocation> _allocations = new();
    public IReadOnlyCollection<BudgetAllocation> Allocations => _allocations.AsReadOnly();

    private readonly List<DailySpendingAllowance> _dailyAllowances = new();
    public IReadOnlyCollection<DailySpendingAllowance> DailyAllowances => _dailyAllowances.AsReadOnly();

    private Budget() { } // EF Core constructor

    public Budget(
        Guid userId,
        string name,
        decimal totalAmount,
        DateTime startDate,
        DateTime endDate,
        BudgetPeriodType periodType = BudgetPeriodType.Monthly,
        BudgetMethod method = BudgetMethod.Rule503020)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên kế hoạch ngân sách không được để trống.", nameof(name));

        if (totalAmount <= 0)
            throw new ArgumentException("Tổng ngân sách cấp phát phải lớn hơn 0 đ.", nameof(totalAmount));

        if (endDate.Date < startDate.Date)
            throw new ArgumentException("Ngày kết thúc ngân sách phải sau hoặc bằng ngày bắt đầu.", nameof(endDate));

        Id = Guid.NewGuid();
        UserId = userId;
        Name = name.Trim();
        TotalAmount = totalAmount;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        PeriodType = periodType;
        Method = method;
        Status = BudgetStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddAllocation(Guid categoryId, decimal allocatedAmount, decimal alertThresholdPercent = 80m)
    {
        if (_allocations.Any(a => a.CategoryId == categoryId))
            throw new ArgumentException("Danh mục này đã được phân bổ trong kế hoạch ngân sách.", nameof(categoryId));

        var allocation = new BudgetAllocation(Id, categoryId, allocatedAmount, alertThresholdPercent);
        _allocations.Add(allocation);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordExpense(Guid categoryId, decimal amount)
    {
        var allocation = _allocations.FirstOrDefault(a => a.CategoryId == categoryId);
        if (allocation != null)
        {
            allocation.RecordExpense(amount);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RevertExpense(Guid categoryId, decimal amount)
    {
        var allocation = _allocations.FirstOrDefault(a => a.CategoryId == categoryId);
        if (allocation != null)
        {
            allocation.RevertExpense(amount);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RebalanceAllocation(Guid categoryId, decimal newAllocatedAmount)
    {
        var allocation = _allocations.FirstOrDefault(a => a.CategoryId == categoryId);
        if (allocation == null)
            throw new ArgumentException("Không tìm thấy phân bổ của danh mục trong ngân sách.", nameof(categoryId));

        allocation.Rebalance(newAllocatedAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AdjustTotalBudget(decimal newTotalAmount)
    {
        if (newTotalAmount <= 0)
            throw new ArgumentException("Tổng ngân sách điều chỉnh phải lớn hơn 0 đ.", nameof(newTotalAmount));

        TotalAmount = newTotalAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        Status = BudgetStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }

    // Computed properties
    public decimal TotalAllocated => _allocations.Sum(a => a.AllocatedAmount);
    public decimal TotalSpent => _allocations.Sum(a => a.CurrentSpent);
    public decimal TotalRemaining => TotalAmount - TotalSpent;

    public decimal OverallSpentPercent => TotalAmount > 0 
        ? Math.Round((TotalSpent / TotalAmount) * 100m, 2) 
        : 0m;

    public int TotalDays => (EndDate.Date - StartDate.Date).Days + 1;
}
