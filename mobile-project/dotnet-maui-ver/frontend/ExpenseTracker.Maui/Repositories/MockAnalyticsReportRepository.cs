using ExpenseTracker.Maui.Models.Reports;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock implementation of IAnalyticsReportRepository providing rich mock data for offline testing & UI/UX design
public class MockAnalyticsReportRepository : IAnalyticsReportRepository
{
    private decimal _currentAssets = 125_000_000m;
    private decimal _currentLiabilities = 15_000_000m;
    private readonly List<NetWorthSnapshotModel> _snapshots = new();

    public MockAnalyticsReportRepository()
    {
        // Seed 6 months of historical snapshots for trend charts
        var now = DateTime.UtcNow.Date;
        _snapshots.Add(new NetWorthSnapshotModel { Id = Guid.NewGuid(), SnapshotDate = now.AddMonths(-5), TotalAssets = 80_000_000m, TotalLiabilities = 25_000_000m, NetWorth = 55_000_000m, Notes = "Tháng 5/2026" });
        _snapshots.Add(new NetWorthSnapshotModel { Id = Guid.NewGuid(), SnapshotDate = now.AddMonths(-4), TotalAssets = 90_000_000m, TotalLiabilities = 22_000_000m, NetWorth = 68_000_000m, Notes = "Tháng 6/2026" });
        _snapshots.Add(new NetWorthSnapshotModel { Id = Guid.NewGuid(), SnapshotDate = now.AddMonths(-3), TotalAssets = 100_000_000m, TotalLiabilities = 20_000_000m, NetWorth = 80_000_000m, Notes = "Tháng 7/2026" });
        _snapshots.Add(new NetWorthSnapshotModel { Id = Guid.NewGuid(), SnapshotDate = now.AddMonths(-2), TotalAssets = 110_000_000m, TotalLiabilities = 18_000_000m, NetWorth = 92_000_000m, Notes = "Tháng 8/2026" });
        _snapshots.Add(new NetWorthSnapshotModel { Id = Guid.NewGuid(), SnapshotDate = now.AddMonths(-1), TotalAssets = 118_000_000m, TotalLiabilities = 16_000_000m, NetWorth = 102_000_000m, Notes = "Tháng 9/2026" });
        _snapshots.Add(new NetWorthSnapshotModel { Id = Guid.NewGuid(), SnapshotDate = now, TotalAssets = _currentAssets, TotalLiabilities = _currentLiabilities, NetWorth = _currentAssets - _currentLiabilities, Notes = "Tháng hiện tại" });
    }

    public Task<IncomeExpenseComparisonModel> GetIncomeExpenseAsync(DateTime from, DateTime to, string groupBy = "month")
    {
        var model = new IncomeExpenseComparisonModel
        {
            TotalIncome = 35_000_000m,
            TotalExpense = 21_500_000m,
            NetSurplus = 13_500_000m,
            GrowthMoM = 4.5m,
            GrowthYoY = 12.0m,
            Breakdown = new List<PeriodDataPointModel>
            {
                new() { PeriodLabel = "Tuần 1", StartDate = from, EndDate = from.AddDays(7), Income = 25_000_000m, Expense = 6_500_000m, NetSurplus = 18_500_000m },
                new() { PeriodLabel = "Tuần 2", StartDate = from.AddDays(8), EndDate = from.AddDays(14), Income = 0m, Expense = 5_000_000m, NetSurplus = -5_000_000m },
                new() { PeriodLabel = "Tuần 3", StartDate = from.AddDays(15), EndDate = from.AddDays(21), Income = 10_000_000m, Expense = 4_800_000m, NetSurplus = 5_200_000m },
                new() { PeriodLabel = "Tuần 4", StartDate = from.AddDays(22), EndDate = to, Income = 0m, Expense = 5_200_000m, NetSurplus = -5_200_000m }
            }
        };

        return Task.FromResult(model);
    }

    public Task<NetWorthReportModel> GetNetWorthAsync(DateTime? from = null, DateTime? to = null)
    {
        var netWorth = _currentAssets - _currentLiabilities;
        var model = new NetWorthReportModel
        {
            TotalAssets = _currentAssets,
            TotalLiabilities = _currentLiabilities,
            CurrentNetWorth = netWorth,
            IsNegativeNetWorthWarning = netWorth < 0,
            HealthAdvice = netWorth >= 0
                ? "Tài sản ròng tăng trưởng vững chắc. Hãy tiếp tục duy trì tỷ lệ tích lũy và kiểm soát nợ tín dụng."
                : "Cảnh báo nợ vượt tài sản. Cần cơ cấu nợ khẩn cấp.",
            HistoricalSnapshots = new List<NetWorthSnapshotModel>(_snapshots)
        };

        return Task.FromResult(model);
    }

    public Task<NetWorthSnapshotModel> RecordNetWorthSnapshotAsync(string? notes = null)
    {
        var today = DateTime.UtcNow.Date;
        var existing = _snapshots.FirstOrDefault(s => s.SnapshotDate == today);
        if (existing != null)
        {
            existing.TotalAssets = _currentAssets;
            existing.TotalLiabilities = _currentLiabilities;
            existing.NetWorth = _currentAssets - _currentLiabilities;
            existing.Notes = notes ?? existing.Notes;
            return Task.FromResult(existing);
        }

        var newSnapshot = new NetWorthSnapshotModel
        {
            Id = Guid.NewGuid(),
            SnapshotDate = today,
            TotalAssets = _currentAssets,
            TotalLiabilities = _currentLiabilities,
            NetWorth = _currentAssets - _currentLiabilities,
            Notes = notes ?? "Ảnh chụp số dư cập nhật"
        };
        _snapshots.Add(newSnapshot);
        return Task.FromResult(newSnapshot);
    }

    public Task<NetCashflowReportModel> GetCashflowAsync(DateTime from, DateTime to)
    {
        var totalIncome = 35_000_000m;
        var totalExpense = 21_500_000m;
        var netCashflow = totalIncome - totalExpense;
        var savingsRate = Math.Round((netCashflow / totalIncome) * 100m, 1);

        var model = new NetCashflowReportModel
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            NetCashflow = netCashflow,
            SavingsRate = savingsRate,
            HealthStatus = "Excellent",
            HealthRecommendation = "Dòng tiền xuất sắc (Tỷ lệ tiết kiệm > 35%). Khuyến nghị tái đầu tư để tạo nguồn thu nhập thụ động.",
            WaterfallSteps = new List<WaterfallStepModel>
            {
                new() { StepName = "1. Tổng Thu Nhập", Amount = totalIncome, RunningBalance = totalIncome, StepType = "Inflow" },
                new() { StepName = "2. Chi Thiết Yếu (Needs)", Amount = 12_000_000m, RunningBalance = 23_000_000m, StepType = "Outflow" },
                new() { StepName = "3. Chi Hưởng Thụ (Wants)", Amount = 6_500_000m, RunningBalance = 16_500_000m, StepType = "Outflow" },
                new() { StepName = "4. Trả Nợ / Tích Lũy", Amount = 3_000_000m, RunningBalance = 13_500_000m, StepType = "Outflow" },
                new() { StepName = "5. Thặng Dư Dòng Tiền", Amount = netCashflow, RunningBalance = netCashflow, StepType = "Balance" }
            }
        };

        return Task.FromResult(model);
    }

    public Task<ExpenseStructureReportModel> GetExpenseStructureAsync(DateTime from, DateTime to)
    {
        var totalExpense = 21_500_000m;
        var model = new ExpenseStructureReportModel
        {
            TotalExpense = totalExpense,
            BySubCategory = new List<CategorySpendingShareModel>
            {
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Tiền thuê nhà", IconKey = "home", ColorHex = "#3B82F6", Amount = 8_000_000m, Percentage = 37.2m, IsDominantAlert = true },
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Ăn uống & Thực phẩm", IconKey = "utensils", ColorHex = "#10B981", Amount = 5_000_000m, Percentage = 23.3m, IsDominantAlert = false },
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Cà phê & Gặp gỡ", IconKey = "coffee", ColorHex = "#F59E0B", Amount = 2_500_000m, Percentage = 11.6m, IsDominantAlert = false },
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Sách & Khóa học", IconKey = "book", ColorHex = "#8B5CF6", Amount = 2_000_000m, Percentage = 9.3m, IsDominantAlert = false },
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Sửa xe & Xăng cộ", IconKey = "car", ColorHex = "#EF4444", Amount = 2_000_000m, Percentage = 9.3m, IsDominantAlert = false },
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Mua sắm cá nhân", IconKey = "shopping-bag", ColorHex = "#EC4899", Amount = 2_000_000m, Percentage = 9.3m, IsDominantAlert = false }
            },
            By503020 = new Expense503020SummaryModel
            {
                NeedsAmount = 13_000_000m,
                NeedsPercentage = 60.5m,
                WantsAmount = 6_500_000m,
                WantsPercentage = 30.2m,
                SavingsDebtAmount = 2_000_000m,
                SavingsDebtPercentage = 9.3m
            },
            ByKakeibo = new ExpenseKakeiboSummaryModel
            {
                SurvivalAmount = 13_000_000m,
                SurvivalPercentage = 60.5m,
                OptionalAmount = 4_500_000m,
                OptionalPercentage = 20.9m,
                CultureAmount = 2_000_000m,
                CulturePercentage = 9.3m,
                ExtraAmount = 2_000_000m,
                ExtraPercentage = 9.3m
            },
            DominantCategoryAlerts = new List<string>
            {
                "Danh mục 'Tiền thuê nhà' chiếm 37.2% tổng chi tiêu (>30%), cần lưu ý cân đối ngân sách."
            }
        };

        return Task.FromResult(model);
    }

    public Task<OutlierDetectionReportModel> GetOutliersAsync(int days = 30, double threshold = 2.0)
    {
        var model = new OutlierDetectionReportModel
        {
            TotalAnalyzedTransactions = 45,
            OutlierCount = 2,
            Outliers = new List<OutlierTransactionModel>
            {
                new()
                {
                    TransactionId = Guid.NewGuid(),
                    TransactionDate = DateTime.UtcNow.AddDays(-2),
                    Amount = 3_800_000m,
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "Ăn uống & Thực phẩm",
                    Note = "Tiệc liên hoan sinh nhật",
                    CategoryMean = 350_000m,
                    CategoryStdDev = 120_000m,
                    ZScore = 4.2,
                    AlertSeverity = "High"
                },
                new()
                {
                    TransactionId = Guid.NewGuid(),
                    TransactionDate = DateTime.UtcNow.AddDays(-10),
                    Amount = 1_500_000m,
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "Cà phê & Gặp gỡ",
                    Note = "Họp nhóm quán trà cao cấp",
                    CategoryMean = 150_000m,
                    CategoryStdDev = 50_000m,
                    ZScore = 2.8,
                    AlertSeverity = "Moderate"
                }
            },
            CategorySurges = new List<CategorySurgeModel>
            {
                new()
                {
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "Sửa xe & Xăng cộ",
                    PreviousAverage = 800_000m,
                    CurrentPeriodTotal = 2_000_000m,
                    SurgePercentage = 150.0m,
                    WarningMessage = "Chi phí sửa xe tăng vọt +150% do sửa động cơ đột xuất."
                }
            }
        };

        return Task.FromResult(model);
    }
}
