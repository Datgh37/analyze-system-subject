using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpenseTracker.Application.DTOs.Forecasting;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Domain application service implementing predictive analytics, cashflow exhaustion radars and What-If sandbox (UC-08.01 -> UC-08.04)
public class ForecastingService : IForecastingService
{
    private readonly IApplicationDbContext _context;

    public ForecastingService(IApplicationDbContext context)
    {
        _context = context;
    }

    // UC-08.01: Time-series expenditure projection with weekday/weekend decomposition
    public async Task<Result<ExpenseForecastDto>> GetExpenditureForecastAsync(Guid userId, CancellationToken ct = default)
    {
        var activeBudget = await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        // Fallback: If no Active budget, look for the most recent budget
        activeBudget ??= await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (activeBudget == null)
        {
            return Result<ExpenseForecastDto>.Success(new ExpenseForecastDto
            {
                BudgetId = Guid.Empty,
                BudgetName = "Chưa thiết lập ngân sách",
                TotalBudget = 0m,
                CurrentSpent = 0m,
                DaysPassed = 0,
                TotalDaysInCycle = 30,
                RemainingDays = 0,
                InsufficientData = true,
                RiskLevel = "Safe",
                AdvisoryMessage = "Bạn chưa có kế hoạch ngân sách nào. Vui lòng thiết lập ngân sách tại trang Quản lý Ngân sách để kích hoạt mô hình dự báo chi tiêu."
            });
        }

        var today = DateTime.UtcNow.Date;
        var startDate = activeBudget.StartDate.Date;
        var endDate = activeBudget.EndDate.Date;

        var totalDays = Math.Max(1, (endDate - startDate).Days + 1);
        var daysPassed = Math.Max(1, (today - startDate).Days + 1);
        var remainingDays = Math.Max(0, (endDate - today).Days);

        // Exception Flow E1: Insufficient data if cycle has been active for less than 3 days
        if (daysPassed < 3)
        {
            return Result<ExpenseForecastDto>.Success(new ExpenseForecastDto
            {
                BudgetId = activeBudget.Id,
                BudgetName = activeBudget.Name,
                TotalBudget = activeBudget.TotalAmount,
                CurrentSpent = activeBudget.TotalSpent,
                DaysPassed = daysPassed,
                TotalDaysInCycle = totalDays,
                RemainingDays = remainingDays,
                InsufficientData = true,
                RiskLevel = "Safe",
                AdvisoryMessage = "Hệ thống đang thu thập dữ liệu chi tiêu ban đầu (Cần tối thiểu 3 ngày để kích hoạt mô hình dự báo chuỗi thời gian)."
            });
        }

        // Query all past expense transactions in the active budget cycle
        var startUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        var todayEndUtc = DateTime.SpecifyKind(today.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var pastExpenses = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        !t.IsDeleted &&
                        !t.ExcludeFromReport &&
                        t.TransactionType == TransactionType.Expense &&
                        t.TransactionDate >= startUtc &&
                        t.TransactionDate <= todayEndUtc)
            .ToListAsync(ct);

        var currentSpent = pastExpenses.Sum(t => t.Amount);

        // Outlier detection: Filter out spike transactions (Z-Score > 2.0 or > 3x median for small sample) to avoid skewing baseline daily velocity
        var normalExpenses = new List<Transaction>();
        if (pastExpenses.Count >= 2)
        {
            var amounts = pastExpenses.Select(t => (double)t.Amount).OrderBy(a => a).ToList();
            var median = amounts[amounts.Count / 2];
            var mean = amounts.Average();
            var variance = amounts.Sum(a => Math.Pow(a - mean, 2)) / (amounts.Count - 1);
            var stdDev = Math.Sqrt(variance);

            if (stdDev > 0.0001)
            {
                foreach (var exp in pastExpenses)
                {
                    var z = ((double)exp.Amount - mean) / stdDev;
                    // Standard Z-Score threshold (Z > 2.0).
                    // For small sample size (N <= 10), max possible sample Z is bounded by (N-1)/sqrt(N) (Shiffler 1988),
                    // so we also flag as outlier if amount >= 3.0 * median when median > 0.
                    bool isOutlier = z > 2.0 || (pastExpenses.Count <= 10 && median > 0 && (double)exp.Amount >= 3.0 * median);
                    if (!isOutlier)
                    {
                        normalExpenses.Add(exp);
                    }
                }
            }
            else
            {
                normalExpenses.AddRange(pastExpenses);
            }
        }
        else
        {
            normalExpenses.AddRange(pastExpenses);
        }

        // Count past weekdays and weekends in the cycle up to today
        int pastWeekdays = 0;
        int pastWeekends = 0;
        for (var d = startDate; d <= today; d = d.AddDays(1))
        {
            if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                pastWeekends++;
            else
                pastWeekdays++;
        }

        decimal weekdaySpent = normalExpenses
            .Where(t => t.TransactionDate.DayOfWeek != DayOfWeek.Saturday && t.TransactionDate.DayOfWeek != DayOfWeek.Sunday)
            .Sum(t => t.Amount);

        decimal weekendSpent = normalExpenses
            .Where(t => t.TransactionDate.DayOfWeek == DayOfWeek.Saturday || t.TransactionDate.DayOfWeek == DayOfWeek.Sunday)
            .Sum(t => t.Amount);

        decimal vWeekday = pastWeekdays > 0 ? Math.Round(weekdaySpent / pastWeekdays, 2) : 0m;
        decimal vWeekend = pastWeekends > 0 ? Math.Round(weekendSpent / pastWeekends, 2) : 0m;

        // If either velocity is 0, fallback to general daily average
        if (vWeekday == 0m && vWeekend > 0m) vWeekday = vWeekend;
        if (vWeekend == 0m && vWeekday > 0m) vWeekend = vWeekday;
        if (vWeekday == 0m && vWeekend == 0m && daysPassed > 0)
        {
            var avg = Math.Round(currentSpent / daysPassed, 2);
            vWeekday = avg;
            vWeekend = avg;
        }

        // Count remaining weekdays and weekends from tomorrow to endDate
        int remainingWeekdays = 0;
        int remainingWeekends = 0;
        for (var d = today.AddDays(1); d <= endDate; d = d.AddDays(1))
        {
            if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                remainingWeekends++;
            else
                remainingWeekdays++;
        }

        decimal estimatedFutureSpent = Math.Round((remainingWeekdays * vWeekday) + (remainingWeekends * vWeekend), 2);
        decimal estimatedTotalSpent = currentSpent + estimatedFutureSpent;
        decimal projectedDeficit = estimatedTotalSpent - activeBudget.TotalAmount;
        bool isOverspend = projectedDeficit > 0m;

        string riskLevel;
        string advisory;
        if (isOverspend)
        {
            if (projectedDeficit > activeBudget.TotalAmount * 0.15m)
            {
                riskLevel = "HighRisk";
                advisory = $"⚠️ Nguy cơ bội chi nghiêm trọng! Dự kiến bạn sẽ chi tiêu vượt ngân sách {projectedDeficit:N0} đ ({Math.Round((projectedDeficit / activeBudget.TotalAmount) * 100m, 1)}%). Cần tái cân đối hạn mức ngay.";
            }
            else
            {
                riskLevel = "Caution";
                advisory = $"⚡ Cảnh báo: Tốc độ chi tiêu dự kiến sẽ làm vượt ngân sách {projectedDeficit:N0} đ. Hãy thắt chặt các khoản chi tùy chọn.";
            }
        }
        else
        {
            riskLevel = "Safe";
            var surplus = Math.Abs(projectedDeficit);
            advisory = $"✅ Kế hoạch chi tiêu nằm trong tầm kiểm soát an toàn. Bạn dự kiến tiết kiệm thặng dư được {surplus:N0} đ khi kết thúc chu kỳ.";
        }

        // Build trajectory points for side-by-side visualization
        var trajectory = new List<ForecastDailyPointDto>();
        decimal runningSpent = 0m;
        var dailySpentMap = pastExpenses
            .GroupBy(t => t.TransactionDate.Date)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        for (var d = startDate; d <= today; d = d.AddDays(1))
        {
            if (dailySpentMap.TryGetValue(d, out var spentOnDay))
                runningSpent += spentOnDay;

            trajectory.Add(new ForecastDailyPointDto
            {
                Date = d,
                ActualCumulativeSpent = runningSpent,
                ProjectedCumulativeSpent = runningSpent,
                IsWeekend = d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday,
                IsFuture = false
            });
        }

        decimal projectedRunning = runningSpent;
        for (var d = today.AddDays(1); d <= endDate; d = d.AddDays(1))
        {
            var isWeekend = d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday;
            projectedRunning += isWeekend ? vWeekend : vWeekday;

            trajectory.Add(new ForecastDailyPointDto
            {
                Date = d,
                ActualCumulativeSpent = null,
                ProjectedCumulativeSpent = Math.Round(projectedRunning, 2),
                IsWeekend = isWeekend,
                IsFuture = true
            });
        }

        return Result<ExpenseForecastDto>.Success(new ExpenseForecastDto
        {
            BudgetId = activeBudget.Id,
            BudgetName = activeBudget.Name,
            TotalBudget = activeBudget.TotalAmount,
            CurrentSpent = currentSpent,
            DaysPassed = daysPassed,
            TotalDaysInCycle = totalDays,
            RemainingDays = remainingDays,
            RemainingWeekdays = remainingWeekdays,
            RemainingWeekends = remainingWeekends,
            WeekdayDailyVelocity = vWeekday,
            WeekendDailyVelocity = vWeekend,
            EstimatedFutureSpent = estimatedFutureSpent,
            EstimatedTotalSpent = estimatedTotalSpent,
            ProjectedDeficitOrSurplus = projectedDeficit,
            IsOverspendProjected = isOverspend,
            RiskLevel = riskLevel,
            InsufficientData = false,
            AdvisoryMessage = advisory,
            Trajectory = trajectory
        });
    }

    // UC-08.02: Liquidity depletion & out-of-money date forecast vs next salary date
    public async Task<Result<LiquidityDepletionDto>> GetLiquidityDepletionForecastAsync(Guid userId, CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;

        // Query liquid funds: Cash and Bank wallets only (excluding investment or locked goals)
        var wallets = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId && w.Status == WalletStatus.Active)
            .ToListAsync(ct);

        var liquidFunds = wallets
            .Where(w => w.WalletType == WalletType.Cash || w.WalletType == WalletType.BankAccount || w.WalletType == WalletType.EWallet)
            .Sum(w => w.Balance);

        // Exception Flow E1: Immediate exhaustion if liquid funds <= 0
        if (liquidFunds <= 0m)
        {
            return Result<LiquidityDepletionDto>.Success(new LiquidityDepletionDto
            {
                TotalLiquidFunds = liquidFunds,
                AverageDailyVelocity7Days = 0m,
                DaysUntilDepletion = 0,
                OutOfMoneyDate = today,
                NextSalaryDate = null,
                DeficitDaysBeforeSalary = 0,
                IsLiquidityCrisis = true,
                LiquidityHealthStatus = "Crisis",
                AdvisoryMessage = "🚨 BÁO ĐỘNG ĐỎ KHẨN CẤP! Bạn đã cạn sạch tiền thanh khoản khả dụng trong các ví. Ngừng ngay mọi chi tiêu không bắt buộc!"
            });
        }

        // Compute 7-day average daily velocity
        var sevenDaysAgoUtc = DateTime.SpecifyKind(today.AddDays(-7), DateTimeKind.Utc);
        var todayEndUtc = DateTime.SpecifyKind(today.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var recentExpensesSum = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        !t.IsDeleted &&
                        !t.ExcludeFromReport &&
                        t.TransactionType == TransactionType.Expense &&
                        t.TransactionDate >= sevenDaysAgoUtc &&
                        t.TransactionDate <= todayEndUtc)
            .SumAsync(t => (decimal?)t.Amount, ct) ?? 0m;

        decimal velocity7Days = Math.Round(recentExpensesSum / 7m, 2);

        // Determine Next Salary Date (from recurring income or default cycle)
        DateTime? nextSalaryDate = null;
        var activeRecurring = await _context.RecurringIncomes
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.IsActive)
            .OrderBy(r => r.NextDueDate)
            .FirstOrDefaultAsync(ct);

        if (activeRecurring != null && activeRecurring.NextDueDate.Date >= today)
        {
            nextSalaryDate = activeRecurring.NextDueDate.Date;
        }
        else
        {
            // Default: 25th of current month or next month
            nextSalaryDate = today.Day <= 25
                ? new DateTime(today.Year, today.Month, 25)
                : new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(24);
        }

        // Guard against zero velocity (safe infinite liquidity)
        if (velocity7Days <= 0m)
        {
            return Result<LiquidityDepletionDto>.Success(new LiquidityDepletionDto
            {
                TotalLiquidFunds = liquidFunds,
                AverageDailyVelocity7Days = 0m,
                DaysUntilDepletion = 9999,
                OutOfMoneyDate = null,
                NextSalaryDate = nextSalaryDate,
                DeficitDaysBeforeSalary = 0,
                IsLiquidityCrisis = false,
                LiquidityHealthStatus = "Safe",
                AdvisoryMessage = "✅ Không ghi nhận chi tiêu trong 7 ngày qua. Thanh khoản an toàn tuyệt đối."
            });
        }

        int daysUntilDepletion = (int)Math.Floor(liquidFunds / velocity7Days);
        var outOfMoneyDate = today.AddDays(daysUntilDepletion);

        int deficitDays = 0;
        bool isCrisis = false;
        string status;
        string advisory;

        if (nextSalaryDate.HasValue && outOfMoneyDate < nextSalaryDate.Value)
        {
            deficitDays = (nextSalaryDate.Value - outOfMoneyDate).Days;
            isCrisis = true;
            status = "Crisis";
            advisory = $"🚨 BÁO ĐỘNG ĐỎ THANH KHOẢN! Với tốc độ chi tiêu {velocity7Days:N0} đ/ngày, bạn dự kiến sẽ hết sạch tiền vào ngày {outOfMoneyDate:dd/MM/yyyy}. Bạn sẽ bị thiếu tiền trong {deficitDays} ngày trước khi nhận kỳ lương tiếp theo ({nextSalaryDate.Value:dd/MM/yyyy})!";
        }
        else
        {
            isCrisis = false;
            status = daysUntilDepletion >= 60 ? "Safe" : "Stable";
            advisory = $"✅ Thanh khoản an toàn: Số dư khả dụng đủ duy trì trong {daysUntilDepletion} ngày tới (vượt qua kỳ lương tiếp theo ngày {nextSalaryDate?.ToString("dd/MM/yyyy")}).";
        }

        return Result<LiquidityDepletionDto>.Success(new LiquidityDepletionDto
        {
            TotalLiquidFunds = liquidFunds,
            AverageDailyVelocity7Days = velocity7Days,
            DaysUntilDepletion = daysUntilDepletion,
            OutOfMoneyDate = outOfMoneyDate,
            NextSalaryDate = nextSalaryDate,
            DeficitDaysBeforeSalary = deficitDays,
            IsLiquidityCrisis = isCrisis,
            LiquidityHealthStatus = status,
            AdvisoryMessage = advisory
        });
    }

    // UC-08.03: Category deficit risk radar ranking high-risk essential categories
    public async Task<Result<List<CategoryDeficitRiskDto>>> GetCategoryDeficitRisksAsync(Guid userId, CancellationToken ct = default)
    {
        var activeBudget = await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        // Fallback: If no Active budget, look for the most recent budget
        activeBudget ??= await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (activeBudget == null || activeBudget.Allocations.Count == 0)
            return Result<List<CategoryDeficitRiskDto>>.Success(new List<CategoryDeficitRiskDto>());

        var today = DateTime.UtcNow.Date;
        var startDate = activeBudget.StartDate.Date;
        var endDate = activeBudget.EndDate.Date;

        var daysPassed = Math.Max(1, (today - startDate).Days + 1);
        var remainingDays = Math.Max(0, (endDate - today).Days);

        var radarList = new List<CategoryDeficitRiskDto>();

        foreach (var alloc in activeBudget.Allocations)
        {
            var isEssential = alloc.Category?.BudgetRuleMapping == BudgetRuleMapping.Needs;
            var allocated = alloc.AllocatedAmount;
            var currentSpent = alloc.CurrentSpent;

            // Daily velocity of this specific category
            decimal velocity = daysPassed > 0 ? Math.Round(currentSpent / daysPassed, 2) : 0m;
            decimal projectedSpent = currentSpent + (velocity * remainingDays);
            decimal riskRatio = allocated > 0m ? Math.Round((projectedSpent / allocated) * 100m, 1) : 0m;

            DateTime? exhaustionDate = null;
            int? daysUntilExhaustion = null;

            if (velocity > 0m && currentSpent < allocated)
            {
                decimal daysLeft = (allocated - currentSpent) / velocity;
                daysUntilExhaustion = (int)Math.Ceiling(daysLeft);
                exhaustionDate = today.AddDays(daysUntilExhaustion.Value);
            }
            else if (currentSpent >= allocated)
            {
                daysUntilExhaustion = 0;
                exhaustionDate = today;
            }

            string severity;
            string advice;

            if (isEssential && riskRatio > 115m)
            {
                severity = "HighRisk";
                advice = $"⚠️ Danh mục thiết yếu dự kiến vượt hạn mức {riskRatio}%. Nguy cơ cạn hạn mức vào ngày {exhaustionDate?.ToString("dd/MM/yyyy")}.";
            }
            else if (riskRatio >= 100m)
            {
                severity = "Caution";
                advice = $"⚡ Dự kiến chạm trần ngân sách ({riskRatio}%). Cần theo dõi chặt chẽ.";
            }
            else
            {
                severity = "Safe";
                advice = "Chi tiêu trong giới hạn an toàn.";
            }

            radarList.Add(new CategoryDeficitRiskDto
            {
                CategoryId = alloc.CategoryId,
                CategoryName = alloc.Category?.Name ?? "Chưa phân loại",
                IsEssential = isEssential,
                AllocatedAmount = allocated,
                CurrentSpent = currentSpent,
                DailyVelocity = velocity,
                ProjectedSpentEnd = projectedSpent,
                RiskRatio = riskRatio,
                EstimatedExhaustionDate = exhaustionDate,
                DaysUntilExhaustion = daysUntilExhaustion,
                RiskSeverity = severity,
                Advice = advice
            });
        }

        // Sort: HighRisk first, then Caution, then Safe; within severity by RiskRatio descending
        var sortedRadar = radarList
            .OrderByDescending(r => r.RiskSeverity == "HighRisk")
            .ThenByDescending(r => r.RiskSeverity == "Caution")
            .ThenByDescending(r => r.RiskRatio)
            .ToList();

        return Result<List<CategoryDeficitRiskDto>>.Success(sortedRadar);
    }

    // UC-08.04: Interactive What-If simulation sandbox (In-memory, zero DB mutation)
    public async Task<Result<WhatIfSimulationResultDto>> SimulateWhatIfScenarioAsync(
        Guid userId,
        WhatIfSimulationRequestDto request,
        CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;

        // 1. Current State Baseline
        var activeBudget = await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        var totalDays = activeBudget != null ? Math.Max(1, (activeBudget.EndDate.Date - activeBudget.StartDate.Date).Days + 1) : 30;
        var daysPassed = activeBudget != null ? Math.Max(1, (today - activeBudget.StartDate.Date).Days + 1) : 1;
        var remainingDays = Math.Max(1, totalDays - daysPassed + 1);

        decimal currentFlexible = activeBudget != null ? activeBudget.TotalRemaining : 10000000m;
        decimal dsaBefore = Math.Round(currentFlexible / remainingDays, 0);

        var wallets = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId && w.Status == WalletStatus.Active)
            .ToListAsync(ct);

        decimal liquidFunds = wallets
            .Where(w => w.WalletType == WalletType.Cash || w.WalletType == WalletType.BankAccount || w.WalletType == WalletType.EWallet)
            .Sum(w => w.Balance);

        var goals = await _context.SavingsGoals
            .AsNoTracking()
            .Where(g => g.UserId == userId && (g.Status == SavingsGoalStatus.Active || g.Status == SavingsGoalStatus.Completed))
            .ToListAsync(ct);

        // Actual Emergency Fund Baseline (UC-08.04 & Phase 05)
        var emergencyGoals = goals.Where(g => g.IsEmergencyFund).ToList();
        decimal emergencyFundBefore = emergencyGoals.Sum(g => g.CurrentAmount);
        decimal emergencyFundAfter = emergencyFundBefore;

        // Actual Debts Baseline (Phase 03 & 04)
        var activeDebts = await _context.Debts
            .AsNoTracking()
            .Where(d => d.UserId == userId && d.Status == DebtStatus.Active)
            .ToListAsync(ct);

        decimal totalDebtBalance = activeDebts.Sum(d => d.CurrentBalance);
        decimal actualMonthlyDebt = activeDebts.Sum(d => d.MinimumPayment);

        decimal totalAssets = wallets.Sum(w => w.Balance) + goals.Sum(g => g.CurrentAmount);
        decimal totalLiabilities = wallets.Where(w => w.WalletType == WalletType.CreditCard).Sum(w => Math.Abs(w.Balance)) + totalDebtBalance;
        decimal netWorthBefore = totalAssets - totalLiabilities;

        // Fallback for debt payment: If no specific Debts but credit card balance exists, assume 5% minimum
        if (actualMonthlyDebt <= 0m && totalLiabilities > 0m)
        {
            actualMonthlyDebt = totalLiabilities * 0.05m;
        }

        // Baseline velocity & runway
        var sevenDaysAgoUtc = DateTime.SpecifyKind(today.AddDays(-7), DateTimeKind.Utc);
        var recentExpensesSum = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && !t.IsDeleted && t.TransactionType == TransactionType.Expense && t.TransactionDate >= sevenDaysAgoUtc)
            .SumAsync(t => (decimal?)t.Amount, ct) ?? 0m;

        decimal velocity = recentExpensesSum > 0 ? Math.Round(recentExpensesSum / 7m, 2) : 200000m;
        int daysUntilDepletionBefore = velocity > 0 ? (int)Math.Floor(liquidFunds / velocity) : 999;
        DateTime outOfMoneyBefore = today.AddDays(daysUntilDepletionBefore);

        decimal essentialMonthlySpending = velocity > 0m ? velocity * 30m : 10_000_000m;
        decimal runwayMonthsBefore = essentialMonthlySpending > 0m
            ? Math.Round(emergencyFundBefore / essentialMonthlySpending, 1)
            : 0m;
        decimal runwayMonthsAfter = runwayMonthsBefore;

        // Baseline monthly income & DTI
        var incomeSum = await _context.RecurringIncomes
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.IsActive)
            .SumAsync(r => (decimal?)r.ExpectedAmount, ct) ?? 0m;

        decimal monthlyIncome = incomeSum > 0m ? incomeSum : 15_000_000m;
        decimal dtiBefore = monthlyIncome > 0 ? Math.Round((actualMonthlyDebt / monthlyIncome) * 100m, 1) : 0m;

        // 2. Compute Simulation Impact (Pure in-memory)
        decimal dsaAfter = dsaBefore;
        decimal netWorthAfter = netWorthBefore;
        decimal dtiAfter = dtiBefore;
        DateTime outOfMoneyAfter = outOfMoneyBefore;
        int daysShifted = 0;
        string scenarioDesc = string.Empty;
        string impactMessage = string.Empty;
        bool isHighRisk = false;
        bool isLiquidDeficitRisk = false;

        switch (request.ScenarioType)
        {
            case WhatIfScenarioType.LumpSumExpense:
                var lumpSum = Math.Max(0m, request.LumpSumAmount);
                scenarioDesc = $"Mua sắm lớn trả một lần: {lumpSum:N0} đ";

                var newFlexible = Math.Max(0m, currentFlexible - lumpSum);
                dsaAfter = Math.Round(newFlexible / remainingDays, 0);
                netWorthAfter = netWorthBefore - lumpSum;

                var newLiquid = Math.Max(0m, liquidFunds - lumpSum);
                int daysAfter = velocity > 0 ? (int)Math.Floor(newLiquid / velocity) : 0;
                outOfMoneyAfter = today.AddDays(daysAfter);
                daysShifted = Math.Max(0, daysUntilDepletionBefore - daysAfter);

                isLiquidDeficitRisk = lumpSum > liquidFunds;
                isHighRisk = isLiquidDeficitRisk || (dsaBefore > 0m && dsaAfter < dsaBefore * 0.5m);

                impactMessage = isLiquidDeficitRisk
                    ? $"🚨 CẢNH BÁO NGUY HIỂM: Khoản chi {lumpSum:N0} đ vượt quá tổng tiền mặt khả dụng ({liquidFunds:N0} đ) trong ví! Ví của bạn sẽ bị âm ngay lập tức nếu thực hiện giao dịch này."
                    : $"Nếu chi {lumpSum:N0} đ ngay lúc này: Hạn mức ngày DSA sẽ bị siết giảm từ {dsaBefore:N0} đ xuống còn {dsaAfter:N0} đ ({Math.Round(((dsaBefore - dsaAfter) / (dsaBefore > 0 ? dsaBefore : 1)) * 100m, 1)}%). Ngày cạn sạch tiền bị kéo sớm {daysShifted} ngày (rơi vào ngày {outOfMoneyAfter:dd/MM/yyyy}).";
                break;

            case WhatIfScenarioType.InstallmentLoan:
                var monthlyPayment = Math.Max(0m, request.MonthlyInstallmentAmount);
                var months = Math.Max(1, request.InstallmentMonths);
                scenarioDesc = $"Vay mua trả góp: {monthlyPayment:N0} đ/tháng trong {months} tháng";

                dsaAfter = Math.Max(0m, Math.Round((currentFlexible - monthlyPayment) / remainingDays, 0));
                decimal newMonthlyDebt = actualMonthlyDebt + monthlyPayment;
                dtiAfter = monthlyIncome > 0 ? Math.Round((newMonthlyDebt / monthlyIncome) * 100m, 1) : 0m;
                netWorthAfter = netWorthBefore;

                isHighRisk = dtiAfter > 40m; // DTI danger threshold
                impactMessage = $"Nếu vay trả góp {monthlyPayment:N0} đ/tháng: Tỷ số nợ trên thu nhập (DTI) sẽ tăng từ {dtiBefore}% lên {dtiAfter}%. {(dtiAfter > 40m ? "⚠️ Cảnh báo: Tỷ số nợ vượt ngưỡng an toàn 40%!" : "DTI vẫn nằm trong ngưỡng an toàn.")}";
                break;

            case WhatIfScenarioType.IncomeReduction:
                var reductionPct = Math.Clamp(request.IncomeReductionPercentage, 0m, 100m);
                scenarioDesc = $"Giả định thu nhập bị sụt giảm: {reductionPct}%";

                decimal reducedIncome = monthlyIncome * (1m - (reductionPct / 100m));
                dtiAfter = reducedIncome > 0 ? Math.Round((actualMonthlyDebt / reducedIncome) * 100m, 1) : 100m;
                decimal reducedFlexible = Math.Max(0m, currentFlexible * (1m - (reductionPct / 100m)));
                dsaAfter = Math.Round(reducedFlexible / remainingDays, 0);

                isHighRisk = reductionPct >= 20m || dtiAfter > 40m;
                impactMessage = $"Nếu thu nhập giảm {reductionPct}%: Hạn mức tiêu mỗi ngày DSA của bạn phải siết giảm còn {dsaAfter:N0} đ để không bị thâm hụt. Tỷ số nợ DTI tăng lên {dtiAfter}%.";
                break;

            case WhatIfScenarioType.EmergencyShock:
                var emergencyExpense = Math.Max(0m, request.EmergencyExpenseAmount);
                var incomeDropPct = Math.Clamp(request.EmergencyIncomeDropPercentage, 0m, 100m);
                scenarioDesc = $"Cú sốc biến cố khẩn cấp: Chi phí đột xuất {emergencyExpense:N0} đ, thu nhập giảm {incomeDropPct}%";

                emergencyFundAfter = Math.Max(0m, emergencyFundBefore - emergencyExpense);
                runwayMonthsAfter = essentialMonthlySpending > 0m
                    ? Math.Round(emergencyFundAfter / essentialMonthlySpending, 1)
                    : 0m;

                decimal shockedIncome = monthlyIncome * (1m - (incomeDropPct / 100m));
                dtiAfter = shockedIncome > 0 ? Math.Round((actualMonthlyDebt / shockedIncome) * 100m, 1) : 100m;
                netWorthAfter = netWorthBefore - emergencyExpense;

                var totalLiquidAvailable = liquidFunds + emergencyFundBefore;
                isLiquidDeficitRisk = emergencyExpense > totalLiquidAvailable;
                isHighRisk = runwayMonthsAfter < 3.0m || isLiquidDeficitRisk || dtiAfter > 40m;

                impactMessage = isLiquidDeficitRisk
                    ? $"🚨 BÁO ĐỘNG ĐỎ BIẾN CỐ: Chi phí khẩn cấp {emergencyExpense:N0} đ vượt quá toàn bộ Quỹ dự phòng và tiền mặt ({totalLiquidAvailable:N0} đ)! Nguy cơ vỡ nợ hoặc buộc phải vay nóng."
                    : $"Sau biến cố khẩn cấp: Quỹ dự phòng còn lại {emergencyFundAfter:N0} đ. Số tháng sinh tồn (Runway) giảm từ {runwayMonthsBefore} tháng xuống còn {runwayMonthsAfter} tháng. {(runwayMonthsAfter < 3.0m ? "⚠️ Cảnh báo: Quỹ dự phòng dưới ngưỡng an toàn tối thiểu (3 tháng)!" : "Quỹ dự phòng vẫn ở mức an toàn.")}";
                break;
        }

        decimal dsaChangePct = dsaBefore > 0 ? Math.Round(((dsaAfter - dsaBefore) / dsaBefore) * 100m, 1) : 0m;

        return Result<WhatIfSimulationResultDto>.Success(new WhatIfSimulationResultDto
        {
            ScenarioType = request.ScenarioType,
            ScenarioDescription = scenarioDesc,
            DsaBefore = dsaBefore,
            DsaAfter = dsaAfter,
            DsaChangePercentage = dsaChangePct,
            OutOfMoneyDateBefore = outOfMoneyBefore,
            OutOfMoneyDateAfter = outOfMoneyAfter,
            DaysShiftedEarlier = daysShifted,
            NetWorthBefore = netWorthBefore,
            NetWorthAfter = netWorthAfter,
            NetWorthChangeAmount = netWorthAfter - netWorthBefore,
            DtiBefore = dtiBefore,
            DtiAfter = dtiAfter,
            EmergencyFundRemainingBefore = emergencyFundBefore,
            EmergencyFundRemainingAfter = emergencyFundAfter,
            RunwayMonthsBefore = runwayMonthsBefore,
            RunwayMonthsAfter = runwayMonthsAfter,
            IsHighRiskDecision = isHighRisk,
            IsLiquidDeficitRisk = isLiquidDeficitRisk,
            ImpactSummaryMessage = impactMessage
        });
    }

    // UC-08.04: Alternate Flow A1 - Convert simulated purchase into a savings goal
    public async Task<Result<ConvertToSavingsGoalResultDto>> ConvertWhatIfToSavingsGoalAsync(
        Guid userId,
        ConvertToSavingsGoalRequestDto request,
        CancellationToken ct = default)
    {
        if (request == null)
            return Result<ConvertToSavingsGoalResultDto>.Failure("Yêu cầu không hợp lệ.");

        if (request.TargetAmount <= 0m)
            return Result<ConvertToSavingsGoalResultDto>.Failure("Số tiền mục tiêu phải lớn hơn 0.");

        var today = DateTime.UtcNow.Date;
        var targetDate = request.TargetDate.Date;
        if (targetDate <= today)
        {
            targetDate = today.AddMonths(6); // Default 6 months in future if invalid
        }

        int daysRemaining = Math.Max(30, (targetDate - today).Days);
        decimal monthsRemaining = Math.Max(1.0m, Math.Round(daysRemaining / 30.0m, 1));
        decimal monthlyCommitment = Math.Round(request.TargetAmount / monthsRemaining, 0);

        var goal = new SavingsGoal(
            userId: userId,
            name: string.IsNullOrWhiteSpace(request.GoalName) ? "Mục tiêu mua sắm What-If" : request.GoalName.Trim(),
            targetAmount: request.TargetAmount,
            startDate: DateTime.SpecifyKind(today, DateTimeKind.Utc),
            targetDate: DateTime.SpecifyKind(targetDate, DateTimeKind.Utc),
            type: SavingsGoalType.MajorPurchase,
            priority: SavingsGoalPriority.Medium,
            initialAmount: 0m,
            description: request.Notes ?? "Được khởi tạo từ Kịch bản mô phỏng tài chính What-If (Luồng A1)"
        );

        _context.SavingsGoals.Add(goal);
        await _context.SaveChangesAsync(ct);

        return Result<ConvertToSavingsGoalResultDto>.Success(new ConvertToSavingsGoalResultDto
        {
            GoalId = goal.Id,
            GoalName = goal.Name,
            TargetAmount = goal.TargetAmount,
            TargetDate = targetDate,
            MonthlyCommitment = monthlyCommitment,
            Message = $"Đã thiết lập thành công mục tiêu tích lũy '{goal.Name}'. Bạn cần tiết kiệm khoảng {monthlyCommitment:N0} đ/tháng trong {monthsRemaining} tháng để hoàn thành mục tiêu này thay vì mua bốc đồng!"
        });
    }
}
