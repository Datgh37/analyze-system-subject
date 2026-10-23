using ExpenseTracker.Maui.Models.Budgets;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock implementation of IBudgetRepository for standalone testing and demo
public class MockBudgetRepository : IBudgetRepository
{
    private BudgetModel? _activeBudget;
    private readonly List<BudgetSummaryModel> _budgets = new();

    public MockBudgetRepository()
    {
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var allocations = new List<BudgetAllocationModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                CategoryName = "Ăn uống & Thực phẩm",
                CategoryIcon = "food",
                CategoryColor = "#EF4444",
                BudgetRuleMapping = 1, // Needs 50%
                AllocatedAmount = 4500000m,
                CurrentSpent = 2100000m,
                RemainingAmount = 2400000m,
                SpentPercent = 46.7m,
                AlertThresholdPercent = 80m,
                IsAlertTriggered = false,
                IsExceeded = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                CategoryName = "Nhà ở & Tiện ích",
                CategoryIcon = "home",
                CategoryColor = "#3B82F6",
                BudgetRuleMapping = 1, // Needs 50%
                AllocatedAmount = 3000000m,
                CurrentSpent = 2800000m,
                RemainingAmount = 200000m,
                SpentPercent = 93.3m,
                AlertThresholdPercent = 80m,
                IsAlertTriggered = true,
                IsExceeded = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                CategoryName = "Cà phê & Giải trí",
                CategoryIcon = "coffee",
                CategoryColor = "#F59E0B",
                BudgetRuleMapping = 2, // Wants 30%
                AllocatedAmount = 2000000m,
                CurrentSpent = 850000m,
                RemainingAmount = 1150000m,
                SpentPercent = 42.5m,
                AlertThresholdPercent = 80m,
                IsAlertTriggered = false,
                IsExceeded = false
            }
        };

        _activeBudget = new BudgetModel(
            Id: budgetId,
            UserId: userId,
            Name: "Ngân Sách Tháng Hiện Tại (50/30/20)",
            PeriodType: 1,
            PeriodTypeName: "Hàng tháng",
            StartDate: new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            EndDate: new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)),
            TotalAmount: 15000000m,
            Method: 1,
            MethodName: "Quy tắc 50/30/20",
            Status: 1,
            StatusName: "Đang hoạt động",
            TotalAllocated: 9500000m,
            TotalSpent: 5750000m,
            TotalRemaining: 3750000m,
            OverallSpentPercent: 60.5m,
            TotalDays: DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month),
            Allocations: allocations
        );

        _budgets.Add(new BudgetSummaryModel(
            Id: budgetId,
            Name: _activeBudget.Name,
            TotalAmount: _activeBudget.TotalAmount,
            TotalSpent: _activeBudget.TotalSpent,
            TotalRemaining: _activeBudget.TotalRemaining,
            OverallSpentPercent: _activeBudget.OverallSpentPercent,
            StatusName: _activeBudget.StatusName
        ));
    }

    public Task<BudgetModel?> GetActiveBudgetAsync() => Task.FromResult(_activeBudget);

    public Task<List<BudgetSummaryModel>> GetBudgetsAsync() => Task.FromResult(_budgets.ToList());

    public Task<BudgetModel> GetBudgetByIdAsync(Guid id)
    {
        if (_activeBudget != null && _activeBudget.Id == id) return Task.FromResult(_activeBudget);
        throw new KeyNotFoundException($"Không tìm thấy ngân sách {id}");
    }

    public Task<BudgetModel> CreateBudgetAsync(CreateBudgetModel request)
    {
        var model = new BudgetModel(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Name: request.Name,
            PeriodType: request.PeriodType,
            PeriodTypeName: "Tháng",
            StartDate: request.StartDate,
            EndDate: request.EndDate,
            TotalAmount: request.TotalAmount,
            Method: request.Method,
            MethodName: "Tùy chỉnh",
            Status: 1,
            StatusName: "Đang hoạt động",
            TotalAllocated: request.TotalAmount,
            TotalSpent: 0,
            TotalRemaining: request.TotalAmount,
            OverallSpentPercent: 0,
            TotalDays: (request.EndDate - request.StartDate).Days + 1,
            Allocations: new List<BudgetAllocationModel>()
        );
        _activeBudget = model;
        return Task.FromResult(model);
    }

    public Task CloseBudgetAsync(Guid id)
    {
        _activeBudget = null;
        return Task.CompletedTask;
    }

    public Task<DsaStatusModel> GetCurrentDsaStatusAsync()
    {
        var dsa = new DsaStatusModel(
            Date: DateTime.Today,
            DailyAllowanceAmount: 250000m,
            ActualSpentToday: 105000m,
            RemainingAllowanceToday: 145000m,
            RemainingFlexibleBudget: 3750000m,
            RemainingDays: 15,
            IsExceeded: false,
            AdviceMessage: "Hạn mức ngày an toàn DSA: Bạn còn 145.000 đ cho hôm nay."
        );
        return Task.FromResult(dsa);
    }

    public Task<VelocityStatusModel> GetSpendingVelocityAsync()
    {
        var velocity = new VelocityStatusModel(
            VelocityIndex: 0.95m,
            LevelName: "Bình thường",
            LevelColorHex: "#10B981",
            ActualVelocityPerDay: 191666m,
            TargetVelocityPerDay: 200000m,
            OutOfMoneyForecastDate: null,
            DaysUntilDepletion: null,
            ForecastMessage: "Tốc độ tiêu tiền bình thường (95% mục tiêu)."
        );
        return Task.FromResult(velocity);
    }

    public Task<BudgetProgressModel> GetBudgetProgressOverviewAsync()
    {
        var allocations = _activeBudget?.Allocations ?? new List<BudgetAllocationModel>();
        var dsa = new DsaStatusModel(
            Date: DateTime.Today,
            DailyAllowanceAmount: 250000m,
            ActualSpentToday: 105000m,
            RemainingAllowanceToday: 145000m,
            RemainingFlexibleBudget: 3750000m,
            RemainingDays: 15,
            IsExceeded: false,
            AdviceMessage: "Hôm nay an toàn."
        );
        var velocity = new VelocityStatusModel(
            VelocityIndex: 0.95m,
            LevelName: "Ổn định",
            LevelColorHex: "#10B981",
            ActualVelocityPerDay: 191666m,
            TargetVelocityPerDay: 200000m,
            OutOfMoneyForecastDate: null,
            DaysUntilDepletion: null,
            ForecastMessage: "Tốc độ tiêu tiền ổn định."
        );

        var overview = new BudgetProgressModel(
            BudgetId: _activeBudget?.Id ?? Guid.NewGuid(),
            BudgetName: _activeBudget?.Name ?? "Ngân Sách Mẫu",
            TotalAmount: _activeBudget?.TotalAmount ?? 15000000m,
            TotalSpent: _activeBudget?.TotalSpent ?? 5750000m,
            TotalRemaining: _activeBudget?.TotalRemaining ?? 9250000m,
            OverallSpentPercent: _activeBudget?.OverallSpentPercent ?? 38.3m,
            DaysPassed: DateTime.Today.Day,
            TotalDays: DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month),
            Dsa: dsa,
            Velocity: velocity,
            Allocations: allocations
        );
        return Task.FromResult(overview);
    }
}
