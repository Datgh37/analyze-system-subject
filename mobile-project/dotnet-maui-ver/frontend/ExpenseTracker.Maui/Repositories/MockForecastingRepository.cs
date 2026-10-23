using ExpenseTracker.Maui.Models.Forecasting;

namespace ExpenseTracker.Maui.Repositories;

// Mock implementation supplying rich simulated data for UI Designer offline testing (B3 Protocol)
public class MockForecastingRepository : IForecastingRepository
{
    public Task<ExpenseForecastModel> GetExpenditureForecastAsync()
    {
        var today = DateTime.UtcNow.Date;
        var startDate = new DateTime(today.Year, today.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var totalDays = (endDate - startDate).Days + 1;
        var daysPassed = Math.Clamp((today - startDate).Days + 1, 1, totalDays);
        var remainingDays = Math.Max(0, totalDays - daysPassed);

        var trajectory = new List<DailyForecastPointModel>();
        decimal runningSpent = 0m;
        decimal dailyBudgetLimit = Math.Round(15000000m / totalDays, 0);

        for (int i = 0; i < totalDays; i++)
        {
            var d = startDate.AddDays(i);
            bool isWeekend = d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday;
            decimal daySpent = 0m;

            if (d <= today)
            {
                daySpent = isWeekend ? 650000m : 420000m;
                runningSpent += daySpent;
            }
            else
            {
                // Projected future spend
                daySpent = isWeekend ? 650000m : 420000m;
                runningSpent += daySpent;
            }

            decimal cumLimit = dailyBudgetLimit * (i + 1);

            trajectory.Add(new DailyForecastPointModel
            {
                Date = d,
                DayLabel = d.ToString("dd/MM"),
                IsWeekend = isWeekend,
                DailySpent = daySpent,
                CumulativeSpent = runningSpent,
                DailyBudgetLimit = dailyBudgetLimit,
                CumulativeBudgetLimit = cumLimit,
                IsOverBudget = runningSpent > 15000000m
            });
        }

        var model = new ExpenseForecastModel
        {
            BudgetId = Guid.NewGuid(),
            BudgetName = "Ngân Sách Sinh Hoạt Tháng " + today.Month,
            TotalBudget = 15000000m,
            CurrentSpent = 9450000m,
            DaysPassed = daysPassed,
            TotalDaysInCycle = totalDays,
            RemainingDays = remainingDays,
            WeekdayDailyVelocity = 420000m,
            WeekendDailyVelocity = 650000m,
            EstimatedFutureSpent = 7400000m,
            EstimatedTotalSpent = 16850000m,
            ProjectedOverspendAmount = 1850000m,
            ExhaustionDate = today.AddDays(8),
            DaysUntilExhaustion = 8,
            RiskLevel = "HighRisk",
            AdvisoryMessage = "⚠️ Cảnh báo: Tốc độ chi tiêu cuối tuần (650k/ngày) cao hơn 55% so với ngày thường. Dự kiến ngân sách sẽ bị vượt trần 1,850,000 đ vào ngày 27/" + today.Month.ToString("D2") + ".",
            InsufficientData = false,
            Trajectory = trajectory
        };

        return Task.FromResult(model);
    }

    public Task<LiquidityDepletionModel> GetLiquidityDepletionAsync()
    {
        var today = DateTime.UtcNow.Date;
        var nextSalary = new DateTime(today.Year, today.Month, 5);
        if (today.Day >= 5)
        {
            nextSalary = nextSalary.AddMonths(1);
        }

        int daysToSalary = (nextSalary - today).Days;
        int daysUntilDepletion = 6;
        int deficitDays = Math.Max(0, daysToSalary - daysUntilDepletion);

        var model = new LiquidityDepletionModel
        {
            TotalLiquidFunds = 3200000m,
            Recent7DaysDailyVelocity = 485000m,
            DaysUntilDepletion = daysUntilDepletion,
            OutOfMoneyDate = today.AddDays(daysUntilDepletion),
            NextSalaryDate = nextSalary,
            DaysUntilNextSalary = daysToSalary,
            DeficitDaysBeforeSalary = deficitDays,
            IsLiquidityCrisis = deficitDays > 0,
            LiquidityHealthStatus = deficitDays > 0 ? "Crisis" : "Safe",
            AdvisoryMessage = deficitDays > 0
                ? $"🚨 BÁO ĐỘNG ĐỎ THANH KHOẢN: Với tốc độ tiêu 485,000 đ/ngày, tiền lỏng khả dụng chỉ còn đủ sống trong 6 ngày (cạn tiền vào ngày {today.AddDays(daysUntilDepletion):dd/MM/yyyy}). Bạn sẽ bị thiếu hụt tiền mặt {deficitDays} ngày trước khi kỳ lương mới về vào ngày {nextSalary:dd/MM/yyyy}!"
                : "Thanh khoản khả dụng an toàn đảm bảo chi tiêu đến kỳ nhận lương kế tiếp."
        };

        return Task.FromResult(model);
    }

    public Task<List<CategoryDeficitRiskModel>> GetCategoryRisksAsync()
    {
        var today = DateTime.UtcNow.Date;

        var list = new List<CategoryDeficitRiskModel>
        {
            new()
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Ăn uống & Thực phẩm",
                IsEssential = true,
                AllocatedAmount = 6000000m,
                CurrentSpent = 5200000m,
                DailyVelocity = 288000m,
                ProjectedSpentEnd = 7800000m,
                RiskRatio = 130.0m,
                EstimatedExhaustionDate = today.AddDays(3),
                DaysUntilExhaustion = 3,
                RiskSeverity = "HighRisk",
                Advice = "⚠️ Danh mục thiết yếu dự kiến vượt hạn mức 130%. Nguy cơ cạn hạn mức trong 3 ngày tới!"
            },
            new()
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Xăng xe & Đi lại",
                IsEssential = true,
                AllocatedAmount = 1500000m,
                CurrentSpent = 1400000m,
                DailyVelocity = 77000m,
                ProjectedSpentEnd = 1650000m,
                RiskRatio = 110.0m,
                EstimatedExhaustionDate = today.AddDays(2),
                DaysUntilExhaustion = 2,
                RiskSeverity = "Caution",
                Advice = "⚡ Dự kiến chạm trần ngân sách (110%). Cần hạn chế di chuyển không cần thiết."
            },
            new()
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Cà phê & Hẹn hò",
                IsEssential = false,
                AllocatedAmount = 2000000m,
                CurrentSpent = 1200000m,
                DailyVelocity = 66000m,
                ProjectedSpentEnd = 1850000m,
                RiskRatio = 92.5m,
                EstimatedExhaustionDate = today.AddDays(12),
                DaysUntilExhaustion = 12,
                RiskSeverity = "Safe",
                Advice = "Chi tiêu trong giới hạn an toàn."
            },
            new()
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Hóa đơn & Tiện ích",
                IsEssential = true,
                AllocatedAmount = 2500000m,
                CurrentSpent = 2100000m,
                DailyVelocity = 116000m,
                ProjectedSpentEnd = 2450000m,
                RiskRatio = 98.0m,
                EstimatedExhaustionDate = today.AddDays(15),
                DaysUntilExhaustion = 15,
                RiskSeverity = "Safe",
                Advice = "Chi tiêu trong giới hạn an toàn."
            }
        };

        return Task.FromResult(list);
    }

    public Task<WhatIfSimulationResultModel> SimulateWhatIfAsync(WhatIfSimulationRequestModel request)
    {
        var today = DateTime.UtcNow.Date;
        decimal dsaBefore = 450000m;
        decimal netWorthBefore = 45000000m;
        decimal dtiBefore = 12.5m;
        var outOfMoneyBefore = today.AddDays(22);

        decimal dsaAfter = dsaBefore;
        decimal netWorthAfter = netWorthBefore;
        decimal dtiAfter = dtiBefore;
        var outOfMoneyAfter = outOfMoneyBefore;
        int daysShifted = 0;
        string desc = string.Empty;
        string impact = string.Empty;
        bool isHighRisk = false;

        switch (request.ScenarioType)
        {
            case WhatIfScenarioType.LumpSumExpense:
                var lump = Math.Max(0m, request.LumpSumAmount);
                desc = $"Mua sắm lớn trả một lần: {lump:N0} đ";
                dsaAfter = Math.Max(0m, dsaBefore - Math.Round(lump / 15m, 0));
                netWorthAfter = netWorthBefore - lump;
                daysShifted = Math.Min(15, (int)(lump / 300000m));
                outOfMoneyAfter = today.AddDays(Math.Max(1, 22 - daysShifted));
                isHighRisk = lump > 10000000m || dsaAfter < dsaBefore * 0.6m;
                impact = $"Nếu chi {lump:N0} đ ngay lúc này: Hạn mức tiêu mỗi ngày (DSA) sẽ bị siết giảm từ {dsaBefore:N0} đ xuống còn {dsaAfter:N0} đ. Ngày cạn sạch tiền bị kéo sớm {daysShifted} ngày (rơi vào ngày {outOfMoneyAfter:dd/MM/yyyy}).";
                break;

            case WhatIfScenarioType.InstallmentLoan:
                var installment = Math.Max(0m, request.MonthlyInstallmentAmount);
                desc = $"Vay mua trả góp: {installment:N0} đ/tháng trong {request.InstallmentMonths} tháng";
                dsaAfter = Math.Max(0m, dsaBefore - Math.Round(installment / 30m, 0));
                dtiAfter = Math.Round(dtiBefore + (installment / 20000000m * 100m), 1);
                isHighRisk = dtiAfter > 40m;
                impact = $"Nếu vay trả góp {installment:N0} đ/tháng: Tỷ số nợ trên thu nhập (DTI) sẽ tăng từ {dtiBefore}% lên {dtiAfter}%. {(dtiAfter > 40m ? "⚠️ Cảnh báo: Tỷ số nợ vượt ngưỡng an toàn 40%!" : "DTI vẫn nằm trong ngưỡng an toàn.")}";
                break;

            case WhatIfScenarioType.IncomeReduction:
                var redPct = Math.Clamp(request.IncomeReductionPercentage, 0m, 100m);
                desc = $"Giả định thu nhập bị sụt giảm: {redPct}%";
                dsaAfter = Math.Max(0m, Math.Round(dsaBefore * (1m - redPct / 100m), 0));
                dtiAfter = Math.Round(dtiBefore / Math.Max(0.1m, (1m - redPct / 100m)), 1);
                isHighRisk = redPct >= 20m;
                impact = $"Nếu thu nhập bị sụt giảm {redPct}%: Hạn mức tiêu mỗi ngày của bạn phải giảm còn {dsaAfter:N0} đ để không bị thâm hụt tài chính.";
                break;
        }

        decimal changePct = dsaBefore > 0 ? Math.Round(((dsaAfter - dsaBefore) / dsaBefore) * 100m, 1) : 0m;

        var result = new WhatIfSimulationResultModel
        {
            ScenarioType = request.ScenarioType,
            ScenarioDescription = desc,
            DsaBefore = dsaBefore,
            DsaAfter = dsaAfter,
            DsaChangePercentage = changePct,
            OutOfMoneyDateBefore = outOfMoneyBefore,
            OutOfMoneyDateAfter = outOfMoneyAfter,
            DaysShiftedEarlier = daysShifted,
            NetWorthBefore = netWorthBefore,
            NetWorthAfter = netWorthAfter,
            NetWorthChangeAmount = netWorthAfter - netWorthBefore,
            DtiBefore = dtiBefore,
            DtiAfter = dtiAfter,
            IsHighRiskDecision = isHighRisk,
            ImpactSummaryMessage = impact
        };

        return Task.FromResult(result);
    }
}
