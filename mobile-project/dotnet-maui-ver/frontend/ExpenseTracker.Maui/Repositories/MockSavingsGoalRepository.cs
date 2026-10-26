using ExpenseTracker.Maui.Models.SavingsGoals;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock implementation of ISavingsGoalRepository for offline testing and demo
public class MockSavingsGoalRepository : ISavingsGoalRepository
{
    private readonly List<SavingsGoalModel> _goals = new();
    private readonly List<SavingsGoalContributionModel> _contributions = new();

    public MockSavingsGoalRepository()
    {
        var userId = Guid.NewGuid();
        var walletId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var g1Id = Guid.Parse("c1111111-1111-1111-1111-111111111111");
        _goals.Add(new SavingsGoalModel
        {
            Id = g1Id,
            UserId = userId,
            Name = "Quỹ Khẩn Cấp 6 Tháng",
            Description = "Dự phòng chi tiêu thiết yếu trong trường hợp khẩn cấp",
            TargetAmount = 60000000m,
            CurrentAmount = 35000000m,
            RemainingAmount = 25000000m,
            ProgressPercent = 58.3m,
            StartDate = DateTime.UtcNow.AddMonths(-6),
            TargetDate = DateTime.UtcNow.AddMonths(6),
            Type = SavingsGoalType.EmergencyFund,
            Status = SavingsGoalStatus.Active,
            Priority = SavingsGoalPriority.High,
            IconKey = "shield",
            ColorHex = "#10B981",
            IsEmergencyFund = true,
            IsCompleted = false,
            MonthlyDepositRequired = 4166667m,
            IsAutoDepositEnabled = true,
            AutoDepositAmount = 4000000m,
            AutoDepositDayOfMonth = 5,
            AutoDepositWalletId = walletId
        });

        var g2Id = Guid.Parse("c2222222-2222-2222-2222-222222222222");
        _goals.Add(new SavingsGoalModel
        {
            Id = g2Id,
            UserId = userId,
            Name = "Mua MacBook Pro M4",
            Description = "Nâng cấp thiết bị làm việc công nghệ cao",
            TargetAmount = 45000000m,
            CurrentAmount = 22500000m,
            RemainingAmount = 22500000m,
            ProgressPercent = 50.0m,
            StartDate = DateTime.UtcNow.AddMonths(-3),
            TargetDate = DateTime.UtcNow.AddMonths(3),
            Type = SavingsGoalType.MajorPurchase,
            Status = SavingsGoalStatus.Active,
            Priority = SavingsGoalPriority.Medium,
            IconKey = "laptop",
            ColorHex = "#3B82F6",
            IsEmergencyFund = false,
            IsCompleted = false,
            MonthlyDepositRequired = 7500000m
        });

        var g3Id = Guid.Parse("c3333333-3333-3333-3333-333333333333");
        _goals.Add(new SavingsGoalModel
        {
            Id = g3Id,
            UserId = userId,
            Name = "Du Lịch Nhật Bản Mùa Thu",
            Description = "Trải nghiệm văn hóa & ngắm lá đỏ Kyoto",
            TargetAmount = 30000000m,
            CurrentAmount = 18000000m,
            RemainingAmount = 12000000m,
            ProgressPercent = 60.0m,
            StartDate = DateTime.UtcNow.AddMonths(-4),
            TargetDate = DateTime.UtcNow.AddMonths(2),
            Type = SavingsGoalType.Travel,
            Status = SavingsGoalStatus.Active,
            Priority = SavingsGoalPriority.Medium,
            IconKey = "plane",
            ColorHex = "#EC4899",
            IsEmergencyFund = false,
            IsCompleted = false,
            MonthlyDepositRequired = 6000000m
        });

        _contributions.Add(new SavingsGoalContributionModel
        {
            Id = Guid.NewGuid(),
            SavingsGoalId = g1Id,
            WalletId = walletId,
            WalletName = "Ví Tiền Mặt",
            Amount = 5000000m,
            Type = ContributionType.Deposit,
            ContributionDate = DateTime.UtcNow.AddDays(-5),
            Note = "Trích lương tháng"
        });
    }

    public Task<List<SavingsGoalSummaryModel>> GetSavingsGoalsAsync(SavingsGoalStatus? status = null)
    {
        var query = status.HasValue ? _goals.Where(g => g.Status == status.Value) : _goals;
        var summaries = query.Select(g => new SavingsGoalSummaryModel
        {
            Id = g.Id,
            Name = g.Name,
            TargetAmount = g.TargetAmount,
            CurrentAmount = g.CurrentAmount,
            ProgressPercent = g.ProgressPercent,
            TargetDate = g.TargetDate,
            Type = g.Type,
            Status = g.Status,
            IconKey = g.IconKey,
            ColorHex = g.ColorHex,
            IsEmergencyFund = g.IsEmergencyFund,
            MonthlyDepositRequired = g.MonthlyDepositRequired
        }).ToList();

        return Task.FromResult(summaries);
    }

    public Task<SavingsGoalModel> GetSavingsGoalByIdAsync(Guid id)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id)
            ?? throw new KeyNotFoundException($"Mục tiêu với ID {id} không tồn tại.");
        return Task.FromResult(goal);
    }

    public Task<SavingsGoalModel> CreateSavingsGoalAsync(CreateSavingsGoalModel request)
    {
        var newGoal = new SavingsGoalModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            TargetAmount = request.TargetAmount,
            CurrentAmount = request.InitialAmount,
            RemainingAmount = Math.Max(0m, request.TargetAmount - request.InitialAmount),
            ProgressPercent = request.TargetAmount > 0 ? (request.InitialAmount / request.TargetAmount) * 100m : 0m,
            StartDate = request.StartDate,
            TargetDate = request.TargetDate,
            Type = request.Type,
            Status = SavingsGoalStatus.Active,
            Priority = request.Priority,
            IconKey = request.IconKey ?? "piggy-bank",
            ColorHex = request.ColorHex ?? "#2563EB",
            IsEmergencyFund = request.Type == SavingsGoalType.EmergencyFund,
            IsCompleted = false
        };

        _goals.Add(newGoal);
        return Task.FromResult(newGoal);
    }

    public Task<SavingsGoalModel> UpdateSavingsGoalAsync(Guid id, UpdateSavingsGoalModel request)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id)
            ?? throw new KeyNotFoundException($"Mục tiêu với ID {id} không tồn tại.");

        goal.Name = request.Name;
        goal.Description = request.Description;
        goal.TargetAmount = request.TargetAmount;
        goal.TargetDate = request.TargetDate;
        goal.Priority = request.Priority;
        goal.IconKey = request.IconKey ?? goal.IconKey;
        goal.ColorHex = request.ColorHex ?? goal.ColorHex;

        return Task.FromResult(goal);
    }

    public Task<SavingsGoalModel> PatchSavingsGoalAsync(Guid id, PatchSavingsGoalModel request)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id)
            ?? throw new KeyNotFoundException($"Mục tiêu với ID {id} không tồn tại.");

        if (request.Name != null) goal.Name = request.Name;
        if (request.Description != null) goal.Description = request.Description;
        if (request.TargetAmount.HasValue) goal.TargetAmount = request.TargetAmount.Value;
        if (request.TargetDate.HasValue) goal.TargetDate = request.TargetDate.Value;
        if (request.Priority.HasValue) goal.Priority = request.Priority.Value;
        if (request.Status.HasValue) goal.Status = request.Status.Value;

        return Task.FromResult(goal);
    }

    public Task<SavingsGoalModel> PatchGoalStatusAsync(Guid id, PatchSavingsGoalStatusModel request)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id)
            ?? throw new KeyNotFoundException($"Mục tiêu với ID {id} không tồn tại.");

        goal.Status = request.Status;
        return Task.FromResult(goal);
    }

    public Task DeleteSavingsGoalAsync(Guid id)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id);
        if (goal != null) _goals.Remove(goal);
        return Task.CompletedTask;
    }

    public Task<SavingsGoalContributionModel> DepositAsync(Guid id, DepositSavingsGoalModel request)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id)
            ?? throw new KeyNotFoundException($"Mục tiêu với ID {id} không tồn tại.");

        goal.CurrentAmount += request.Amount;
        goal.RemainingAmount = Math.Max(0m, goal.TargetAmount - goal.CurrentAmount);
        goal.ProgressPercent = goal.TargetAmount > 0 ? (goal.CurrentAmount / goal.TargetAmount) * 100m : 0m;
        if (goal.CurrentAmount >= goal.TargetAmount)
        {
            goal.IsCompleted = true;
            goal.Status = SavingsGoalStatus.Completed;
            goal.CompletedAt = DateTime.UtcNow;
        }

        var contrib = new SavingsGoalContributionModel
        {
            Id = Guid.NewGuid(),
            SavingsGoalId = id,
            WalletId = request.WalletId,
            WalletName = "Ví Chính",
            Amount = request.Amount,
            Type = ContributionType.Deposit,
            ContributionDate = DateTime.UtcNow,
            Note = request.Note ?? "Nạp tiền tích lũy"
        };
        _contributions.Add(contrib);

        return Task.FromResult(contrib);
    }

    public Task<SavingsGoalContributionModel> WithdrawAsync(Guid id, WithdrawSavingsGoalModel request)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id)
            ?? throw new KeyNotFoundException($"Mục tiêu với ID {id} không tồn tại.");

        goal.CurrentAmount = Math.Max(0m, goal.CurrentAmount - request.Amount);
        goal.RemainingAmount = Math.Max(0m, goal.TargetAmount - goal.CurrentAmount);
        goal.ProgressPercent = goal.TargetAmount > 0 ? (goal.CurrentAmount / goal.TargetAmount) * 100m : 0m;

        var contrib = new SavingsGoalContributionModel
        {
            Id = Guid.NewGuid(),
            SavingsGoalId = id,
            WalletId = request.TargetWalletId,
            WalletName = "Ví Chính",
            Amount = -request.Amount,
            Type = ContributionType.Withdrawal,
            ContributionDate = DateTime.UtcNow,
            Note = request.Reason ?? "Rút tiền mục tiêu"
        };
        _contributions.Add(contrib);

        return Task.FromResult(contrib);
    }

    public Task<List<SavingsGoalContributionModel>> GetGoalContributionsAsync(Guid id)
    {
        var list = _contributions.Where(c => c.SavingsGoalId == id).OrderByDescending(c => c.ContributionDate).ToList();
        return Task.FromResult(list);
    }

    public Task<SavingsGoalModel> ConfigureAutoDepositAsync(Guid id, ConfigureAutoDepositModel request)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id)
            ?? throw new KeyNotFoundException($"Mục tiêu với ID {id} không tồn tại.");

        goal.IsAutoDepositEnabled = request.IsEnabled;
        goal.AutoDepositAmount = request.Amount;
        goal.AutoDepositDayOfMonth = request.DayOfMonth;
        goal.AutoDepositWalletId = request.WalletId;

        return Task.FromResult(goal);
    }

    public Task<SavingsFeasibilityModel> GetGoalFeasibilityAsync(Guid id)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id)
            ?? throw new KeyNotFoundException($"Mục tiêu với ID {id} không tồn tại.");

        var model = new SavingsFeasibilityModel
        {
            GoalId = id,
            GoalName = goal.Name,
            MonthlyRequired = goal.MonthlyDepositRequired,
            ActiveSavingsBudgetPool = 5_000_000m,
            IsFeasible = true,
            FeasibilityScorePercent = 85m,
            AdviceMessage = "Mục tiêu khả thi cao với nguồn thu nhập và ngân sách tiết kiệm hiện tại. Hãy duy trì kỷ luật trích tiền đầu tháng!"
        };

        return Task.FromResult(model);
    }

    public Task<JarsSummaryModel> GetJarsSummaryAsync()
    {
        decimal total = _goals.Sum(g => g.CurrentAmount);

        var model = new JarsSummaryModel
        {
            TotalSavingsBalance = total,
            TotalGoalsCount = _goals.Count,
            CompletedGoalsCount = _goals.Count(g => g.IsCompleted),
            Jars = new List<JarItemModel>
            {
                new()
                {
                    JarCode = "SURVIVAL",
                    Name = "Hũ Thiết Yếu (Survival)",
                    Description = "Ăn uống, thuê nhà, sinh hoạt cơ bản",
                    TotalAmount = 35000000m,
                    TargetPercentage = 50m,
                    ActualPercentage = 46.4m,
                    ColorHex = "#EF4444",
                    IconKey = "home"
                },
                new()
                {
                    JarCode = "OPTIONAL",
                    Name = "Hũ Mong Muốn (Optional)",
                    Description = "Mua sắm, cà phê, thiết bị cá nhân",
                    TotalAmount = 22500000m,
                    TargetPercentage = 30m,
                    ActualPercentage = 29.8m,
                    ColorHex = "#F59E0B",
                    IconKey = "shopping-bag"
                },
                new()
                {
                    JarCode = "CULTURE",
                    Name = "Hũ Văn Hóa & Học Tập (Culture)",
                    Description = "Du lịch, sách, khóa học trau dồi",
                    TotalAmount = 18000000m,
                    TargetPercentage = 10m,
                    ActualPercentage = 23.8m,
                    ColorHex = "#8B5CF6",
                    IconKey = "book"
                },
                new()
                {
                    JarCode = "EXTRA",
                    Name = "Hũ Bất Thường (Extra)",
                    Description = "Hiếu hỉ, ma chay, phát sinh bất ngờ",
                    TotalAmount = 0m,
                    TargetPercentage = 10m,
                    ActualPercentage = 0m,
                    ColorHex = "#10B981",
                    IconKey = "gift"
                }
            }
        };

        return Task.FromResult(model);
    }
}
