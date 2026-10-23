using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Domain CQRS analytics projection service implementing quantitative personal finance models (UC-07.01 -> UC-07.05)
public class AnalyticsReportService : IAnalyticsReportService
{
    private readonly IApplicationDbContext _context;

    public AnalyticsReportService(IApplicationDbContext context)
    {
        _context = context;
    }

    // UC-07.01: Income vs Expense Comparison over time
    public async Task<Result<IncomeExpenseComparisonDto>> GetIncomeExpenseComparisonAsync(
        Guid userId,
        DateTime fromDate,
        DateTime toDate,
        string groupBy = "month",
        CancellationToken cancellationToken = default)
    {
        if (fromDate > toDate)
            (fromDate, toDate) = (toDate, fromDate);

        var fromUtc = DateTime.SpecifyKind(fromDate.Date, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(toDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && !t.ExcludeFromReport && t.TransactionDate >= fromUtc && t.TransactionDate <= toUtc)
            .ToListAsync(cancellationToken);

        var totalIncome = transactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = transactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);
        var netSurplus = totalIncome - totalExpense;

        // Calculate previous matching period for MoM
        var duration = toUtc - fromUtc;
        var prevFromUtc = fromUtc.Subtract(duration);
        var prevToUtc = fromUtc.AddTicks(-1);

        var prevTransactions = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && !t.ExcludeFromReport && t.TransactionDate >= prevFromUtc && t.TransactionDate <= prevToUtc)
            .ToListAsync(cancellationToken);

        var prevExpense = prevTransactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);
        decimal growthMoM = 0;
        if (prevExpense > 0)
        {
            growthMoM = Math.Round(((totalExpense - prevExpense) / prevExpense) * 100m, 2);
        }
        else if (totalExpense > 0)
        {
            growthMoM = 100m;
        }

        // Calculate same period last year for YoY
        var yoyFromUtc = fromUtc.AddYears(-1);
        var yoyToUtc = toUtc.AddYears(-1);

        var yoyTransactions = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && !t.ExcludeFromReport && t.TransactionDate >= yoyFromUtc && t.TransactionDate <= yoyToUtc)
            .ToListAsync(cancellationToken);

        var yoyExpense = yoyTransactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);
        decimal growthYoY = 0;
        if (yoyExpense > 0)
        {
            growthYoY = Math.Round(((totalExpense - yoyExpense) / yoyExpense) * 100m, 2);
        }
        else if (totalExpense > 0)
        {
            growthYoY = 100m;
        }

        // Generate period breakdown
        var breakdown = new List<PeriodDataPointDto>();
        if (groupBy.Equals("day", StringComparison.OrdinalIgnoreCase))
        {
            var groupedByDay = transactions
                .GroupBy(t => t.TransactionDate.Date)
                .OrderBy(g => g.Key);

            foreach (var g in groupedByDay)
            {
                var inc = g.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount);
                var exp = g.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);
                breakdown.Add(new PeriodDataPointDto
                {
                    PeriodLabel = g.Key.ToString("yyyy-MM-dd"),
                    StartDate = g.Key,
                    EndDate = g.Key.AddDays(1).AddTicks(-1),
                    Income = inc,
                    Expense = exp,
                    NetSurplus = inc - exp
                });
            }
        }
        else // default to month
        {
            var groupedByMonth = transactions
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

            foreach (var g in groupedByMonth)
            {
                var inc = g.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount);
                var exp = g.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);
                var start = new DateTime(g.Key.Year, g.Key.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var end = start.AddMonths(1).AddTicks(-1);

                breakdown.Add(new PeriodDataPointDto
                {
                    PeriodLabel = $"{g.Key.Year}-{g.Key.Month:D2}",
                    StartDate = start,
                    EndDate = end,
                    Income = inc,
                    Expense = exp,
                    NetSurplus = inc - exp
                });
            }
        }

        return Result<IncomeExpenseComparisonDto>.Success(new IncomeExpenseComparisonDto
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            NetSurplus = netSurplus,
            GrowthMoM = growthMoM,
            GrowthYoY = growthYoY,
            Breakdown = breakdown
        });
    }

    // UC-07.02: Net Worth Snapshot and Evolution
    public async Task<Result<NetWorthReportDto>> GetNetWorthReportAsync(
        Guid userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        // 1. Calculate live point-in-time assets & liabilities
        var (totalAssets, totalLiabilities) = await CalculateLiveAssetsAndLiabilitiesAsync(userId, cancellationToken);
        var currentNetWorth = totalAssets - totalLiabilities;
        var isNegative = currentNetWorth < 0;

        string? advice = isNegative
            ? "Cảnh báo rủi ro: Tổng dư nợ thẻ/vay đang vượt quá tổng tài sản tích lũy. Khuyến nghị tập trung trả nợ theo phương pháp Debt Avalanche/Snowball."
            : "Sức khỏe tài sản ròng tích cực. Hãy duy trì quỹ dự phòng khẩn cấp và tái đầu tư thặng dư.";

        // 2. Fetch historical snapshots
        var query = _context.NetWorthSnapshots
            .AsNoTracking()
            .Where(s => s.UserId == userId);

        if (fromDate.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(fromDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(s => s.SnapshotDate >= fromUtc);
        }

        if (toDate.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(toDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(s => s.SnapshotDate <= toUtc);
        }

        var snapshots = await query
            .OrderBy(s => s.SnapshotDate)
            .Select(s => new NetWorthSnapshotDto
            {
                Id = s.Id,
                SnapshotDate = s.SnapshotDate,
                TotalAssets = s.TotalAssets,
                TotalLiabilities = s.TotalLiabilities,
                NetWorth = s.NetWorth,
                Notes = s.Notes
            })
            .ToListAsync(cancellationToken);

        return Result<NetWorthReportDto>.Success(new NetWorthReportDto
        {
            CurrentNetWorth = currentNetWorth,
            TotalAssets = totalAssets,
            TotalLiabilities = totalLiabilities,
            IsNegativeNetWorthWarning = isNegative,
            HealthAdvice = advice,
            HistoricalSnapshots = snapshots
        });
    }

    // UC-07.02: Trigger Net Worth Snapshot
    public async Task<Result<NetWorthSnapshotDto>> RecordNetWorthSnapshotAsync(
        Guid userId,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        var (totalAssets, totalLiabilities) = await CalculateLiveAssetsAndLiabilitiesAsync(userId, cancellationToken);
        var todayUtc = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);

        var existingSnapshot = await _context.NetWorthSnapshots
            .FirstOrDefaultAsync(s => s.UserId == userId && s.SnapshotDate == todayUtc, cancellationToken);

        if (existingSnapshot != null)
        {
            existingSnapshot.Recalculate(totalAssets, totalLiabilities, notes);
        }
        else
        {
            var snapshot = new Domain.Entities.NetWorthSnapshot(userId, todayUtc, totalAssets, totalLiabilities, notes);
            _context.NetWorthSnapshots.Add(snapshot);
            existingSnapshot = snapshot;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<NetWorthSnapshotDto>.Success(new NetWorthSnapshotDto
        {
            Id = existingSnapshot.Id,
            SnapshotDate = existingSnapshot.SnapshotDate,
            TotalAssets = existingSnapshot.TotalAssets,
            TotalLiabilities = existingSnapshot.TotalLiabilities,
            NetWorth = existingSnapshot.NetWorth,
            Notes = existingSnapshot.Notes
        });
    }

    // UC-07.03: Net Cashflow and Waterfall Analysis
    public async Task<Result<NetCashflowReportDto>> GetNetCashflowReportAsync(
        Guid userId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        if (fromDate > toDate)
            (fromDate, toDate) = (toDate, fromDate);

        var fromUtc = DateTime.SpecifyKind(fromDate.Date, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(toDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && !t.ExcludeFromReport && t.TransactionDate >= fromUtc && t.TransactionDate <= toUtc)
            .ToListAsync(cancellationToken);

        var totalIncome = transactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = transactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);
        var netCashflow = totalIncome - totalExpense;

        decimal savingsRate = 0;
        if (totalIncome > 0)
        {
            savingsRate = Math.Round((netCashflow / totalIncome) * 100m, 2);
        }
        else if (netCashflow > 0)
        {
            savingsRate = 100m;
        }
        else if (netCashflow < 0)
        {
            savingsRate = -100m;
        }

        string healthStatus;
        string recommendation;
        if (savingsRate >= 20m)
        {
            healthStatus = "Excellent";
            recommendation = "Dòng tiền xuất sắc (Tỷ lệ tích lũy >= 20%). Tối ưu đầu tư dài hạn và gia tăng tài sản.";
        }
        else if (savingsRate >= 10m)
        {
            healthStatus = "Good";
            recommendation = "Dòng tiền tốt (10-20%). Tiếp tục duy trì kế hoạch tích lũy vào các mục tiêu tài chính.";
        }
        else if (savingsRate >= 0m)
        {
            healthStatus = "Fragile";
            recommendation = "Dòng tiền bấp bênh (0-10%). Cân nhắc cắt giảm các khoản chi tiêu tùy chọn (Wants).";
        }
        else
        {
            healthStatus = "Dangerous";
            recommendation = "Cảnh báo thâm hụt: Chi tiêu đang vượt thu nhập. Cần rà soát và thắt chặt ngân sách khẩn cấp.";
        }

        // Build Waterfall steps
        var expenses = transactions.Where(t => t.TransactionType == TransactionType.Expense).ToList();
        var needsExpense = expenses.Where(t => t.IsEssential || t.Category?.BudgetRuleMapping == BudgetRuleMapping.Needs).Sum(t => t.Amount);
        var savingsExpense = expenses.Where(t => t.Category?.BudgetRuleMapping == BudgetRuleMapping.SavingsInvestments).Sum(t => t.Amount);
        var wantsExpense = totalExpense - needsExpense - savingsExpense;
        if (wantsExpense < 0) wantsExpense = 0;

        var waterfallSteps = new List<WaterfallStepDto>
        {
            new() { StepName = "Tổng Thu Nhập", Amount = totalIncome, RunningBalance = totalIncome, StepType = "Inflow" },
            new() { StepName = "Chi Tiêu Thiết Yếu", Amount = needsExpense, RunningBalance = totalIncome - needsExpense, StepType = "Outflow" },
            new() { StepName = "Chi Tiêu Tùy Chọn", Amount = wantsExpense, RunningBalance = totalIncome - needsExpense - wantsExpense, StepType = "Outflow" },
            new() { StepName = "Tích Lũy / Tiết Kiệm", Amount = savingsExpense, RunningBalance = netCashflow, StepType = "Outflow" },
            new() { StepName = "Dòng Tiền Ròng", Amount = netCashflow, RunningBalance = netCashflow, StepType = "Balance" }
        };

        return Result<NetCashflowReportDto>.Success(new NetCashflowReportDto
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            NetCashflow = netCashflow,
            SavingsRate = savingsRate,
            HealthStatus = healthStatus,
            HealthRecommendation = recommendation,
            WaterfallSteps = waterfallSteps
        });
    }

    // UC-07.04: Expense Structure Analysis (Sub-categories, 50/30/20, Kakeibo)
    public async Task<Result<ExpenseStructureReportDto>> GetExpenseStructureReportAsync(
        Guid userId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        if (fromDate > toDate)
            (fromDate, toDate) = (toDate, fromDate);

        var fromUtc = DateTime.SpecifyKind(fromDate.Date, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(toDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var expenses = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && !t.ExcludeFromReport && t.TransactionType == TransactionType.Expense && t.TransactionDate >= fromUtc && t.TransactionDate <= toUtc)
            .ToListAsync(cancellationToken);

        var totalExpense = expenses.Sum(t => t.Amount);
        var bySubCategory = new List<CategorySpendingShareDto>();
        var dominantAlerts = new List<string>();

        // 1. By SubCategory
        var groupedByCategory = expenses
            .GroupBy(t => new { t.CategoryId, Name = t.Category?.Name ?? "Khác", Icon = t.Category?.IconKey ?? "folder", Color = t.Category?.ColorHex ?? "#10B981" })
            .OrderByDescending(g => g.Sum(t => t.Amount));

        foreach (var group in groupedByCategory)
        {
            var catAmount = group.Sum(t => t.Amount);
            var percentage = totalExpense > 0 ? Math.Round((catAmount / totalExpense) * 100m, 2) : 0m;
            var isDominant = percentage > 30m;

            if (isDominant)
            {
                dominantAlerts.Add($"Danh mục '{group.Key.Name}' chiếm {percentage:F1}% tổng chi tiêu (>30%), cần rà soát lại hạn mức ngân sách.");
            }

            bySubCategory.Add(new CategorySpendingShareDto
            {
                CategoryId = group.Key.CategoryId,
                CategoryName = group.Key.Name,
                IconKey = group.Key.Icon,
                ColorHex = group.Key.Color,
                Amount = catAmount,
                Percentage = percentage,
                IsDominantAlert = isDominant
            });
        }

        // 2. By 50/30/20
        decimal needsAmount = 0;
        decimal wantsAmount = 0;
        decimal savingsAmount = 0;

        foreach (var t in expenses)
        {
            if (t.Category?.BudgetRuleMapping == BudgetRuleMapping.Needs || t.IsEssential)
            {
                needsAmount += t.Amount;
            }
            else if (t.Category?.BudgetRuleMapping == BudgetRuleMapping.SavingsInvestments)
            {
                savingsAmount += t.Amount;
            }
            else
            {
                wantsAmount += t.Amount;
            }
        }

        var by503020 = new Expense503020SummaryDto
        {
            NeedsAmount = needsAmount,
            NeedsPercentage = totalExpense > 0 ? Math.Round((needsAmount / totalExpense) * 100m, 2) : 0m,
            WantsAmount = wantsAmount,
            WantsPercentage = totalExpense > 0 ? Math.Round((wantsAmount / totalExpense) * 100m, 2) : 0m,
            SavingsDebtAmount = savingsAmount,
            SavingsDebtPercentage = totalExpense > 0 ? Math.Round((savingsAmount / totalExpense) * 100m, 2) : 0m
        };

        // 3. By Kakeibo (Survival, Optional, Culture, Extra)
        decimal survivalAmount = 0;
        decimal optionalAmount = 0;
        decimal cultureAmount = 0;
        decimal extraAmount = 0;

        foreach (var t in expenses)
        {
            var catName = t.Category?.Name?.ToLowerInvariant() ?? string.Empty;
            if (catName.Contains("sách") || catName.Contains("học") || catName.Contains("giáo dục") || catName.Contains("culture"))
            {
                cultureAmount += t.Amount;
            }
            else if (catName.Contains("bất thường") || catName.Contains("sự cố") || catName.Contains("sửa") || catName.Contains("phạt") || catName.Contains("extra"))
            {
                extraAmount += t.Amount;
            }
            else if (t.IsEssential || t.Category?.BudgetRuleMapping == BudgetRuleMapping.Needs)
            {
                survivalAmount += t.Amount;
            }
            else
            {
                optionalAmount += t.Amount;
            }
        }

        var byKakeibo = new ExpenseKakeiboSummaryDto
        {
            SurvivalAmount = survivalAmount,
            SurvivalPercentage = totalExpense > 0 ? Math.Round((survivalAmount / totalExpense) * 100m, 2) : 0m,
            OptionalAmount = optionalAmount,
            OptionalPercentage = totalExpense > 0 ? Math.Round((optionalAmount / totalExpense) * 100m, 2) : 0m,
            CultureAmount = cultureAmount,
            CulturePercentage = totalExpense > 0 ? Math.Round((cultureAmount / totalExpense) * 100m, 2) : 0m,
            ExtraAmount = extraAmount,
            ExtraPercentage = totalExpense > 0 ? Math.Round((extraAmount / totalExpense) * 100m, 2) : 0m
        };

        return Result<ExpenseStructureReportDto>.Success(new ExpenseStructureReportDto
        {
            TotalExpense = totalExpense,
            BySubCategory = bySubCategory,
            By503020 = by503020,
            ByKakeibo = byKakeibo,
            DominantCategoryAlerts = dominantAlerts
        });
    }

    // UC-07.05: Spending Trend & Z-Score Outlier Detection
    public async Task<Result<OutlierDetectionReportDto>> DetectSpendingOutliersAsync(
        Guid userId,
        int days = 30,
        double threshold = 2.0,
        CancellationToken cancellationToken = default)
    {
        if (days <= 0) days = 30;
        if (threshold <= 0) threshold = 2.0;

        var fromUtc = DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(-days), DateTimeKind.Utc);

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && !t.ExcludeFromReport && t.TransactionType == TransactionType.Expense && t.TransactionDate >= fromUtc)
            .ToListAsync(cancellationToken);

        var outliers = new List<OutlierTransactionDto>();
        var surges = new List<CategorySurgeDto>();

        // Group by category to compute statistical properties
        var groupedByCategory = transactions.GroupBy(t => t.CategoryId);

        foreach (var group in groupedByCategory)
        {
            var list = group.ToList();
            var n = list.Count;
            if (n < 2) continue; // Sample size too small for standard deviation

            // Compute sample mean
            var mean = list.Average(t => (double)t.Amount);

            // Compute sample standard deviation: sqrt( sum((x - mean)^2) / (n - 1) )
            var sumSquaredDiff = list.Sum(t => Math.Pow((double)t.Amount - mean, 2));
            var variance = sumSquaredDiff / (n - 1);
            var stdDev = Math.Sqrt(variance);

            // Guard against division by zero when standard deviation is zero
            if (stdDev <= 0.000001) continue;

            foreach (var t in list)
            {
                var zScore = ((double)t.Amount - mean) / stdDev;
                if (zScore >= threshold)
                {
                    outliers.Add(new OutlierTransactionDto
                    {
                        TransactionId = t.Id,
                        TransactionDate = t.TransactionDate,
                        Amount = t.Amount,
                        CategoryId = t.CategoryId,
                        CategoryName = t.Category?.Name ?? "Khác",
                        Note = t.Note,
                        CategoryMean = Math.Round((decimal)mean, 2),
                        CategoryStdDev = Math.Round((decimal)stdDev, 2),
                        ZScore = Math.Round(zScore, 2),
                        AlertSeverity = zScore >= 3.0 ? "High" : "Moderate"
                    });
                }
            }

            // Category surge check: If current total expense is unusually high
            var catTotal = list.Sum(t => t.Amount);
            var baselineAvg = (decimal)mean * (days / 30m);
            if (baselineAvg > 0 && catTotal > baselineAvg * 1.5m)
            {
                var surgePercent = Math.Round(((catTotal - baselineAvg) / baselineAvg) * 100m, 1);
                surges.Add(new CategorySurgeDto
                {
                    CategoryId = group.Key,
                    CategoryName = list.FirstOrDefault()?.Category?.Name ?? "Khác",
                    PreviousAverage = Math.Round(baselineAvg, 2),
                    CurrentPeriodTotal = catTotal,
                    SurgePercentage = surgePercent,
                    WarningMessage = $"Chi tiêu danh mục tăng vọt +{surgePercent}% so với mức trung bình dự kiến."
                });
            }
        }

        return Result<OutlierDetectionReportDto>.Success(new OutlierDetectionReportDto
        {
            TotalAnalyzedTransactions = transactions.Count,
            OutlierCount = outliers.Count,
            Outliers = outliers.OrderByDescending(o => o.ZScore).ToList(),
            CategorySurges = surges
        });
    }

    private async Task<(decimal totalAssets, decimal totalLiabilities)> CalculateLiveAssetsAndLiabilitiesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var wallets = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId && w.Status == WalletStatus.Active)
            .ToListAsync(cancellationToken);

        decimal assets = 0;
        decimal liabilities = 0;

        foreach (var wallet in wallets)
        {
            if (wallet.WalletType == WalletType.CreditCard)
            {
                if (wallet.Balance < 0)
                {
                    liabilities += Math.Abs(wallet.Balance);
                }
                else
                {
                    assets += wallet.Balance;
                }
            }
            else
            {
                if (!wallet.IsExcludedFromTotal)
                {
                    if (wallet.Balance >= 0)
                    {
                        assets += wallet.Balance;
                    }
                    else
                    {
                        liabilities += Math.Abs(wallet.Balance);
                    }
                }
            }
        }

        var activeSavings = await _context.SavingsGoals
            .AsNoTracking()
            .Where(g => g.UserId == userId && g.Status == SavingsGoalStatus.Active)
            .SumAsync(g => g.CurrentAmount, cancellationToken);

        assets += activeSavings;

        return (assets, liabilities);
    }
}
