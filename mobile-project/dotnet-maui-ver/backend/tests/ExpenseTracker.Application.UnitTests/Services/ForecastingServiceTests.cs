using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Application.DTOs.Forecasting;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class ForecastingServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ForecastingService _service;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _cashWallet;
    private readonly Wallet _bankWallet;
    private readonly Category _foodCategory;
    private readonly Category _rentCategory;
    private readonly Category _entertainmentCategory;

    public ForecastingServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _service = new ForecastingService(_dbContext);

        _cashWallet = new Wallet(_userId, "Tiền mặt", WalletType.Cash, 5_000_000m);
        _bankWallet = new Wallet(_userId, "Vietcombank", WalletType.BankAccount, 15_000_000m);
        _dbContext.Wallets.AddRange(_cashWallet, _bankWallet);

        _foodCategory = new Category("Ăn uống", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Needs);
        _rentCategory = new Category("Thuê nhà", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Needs);
        _entertainmentCategory = new Category("Giải trí", TransactionType.Expense, _userId, budgetRuleMapping: BudgetRuleMapping.Wants);

        _dbContext.Categories.AddRange(_foodCategory, _rentCategory, _entertainmentCategory);
        _dbContext.SaveChanges();
    }

    // UC-08.01 Test 1: Active budget forecasting decomposes weekdays and weekends
    [Fact]
    public async Task GetExpenditureForecast_WithActiveBudget_ShouldForecastAccurately()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-10);
        var endDate = today.AddDays(20); // 31 days total

        var budget = new Budget(_userId, "Ngân sách tháng", 15_000_000m, startDate, endDate, BudgetPeriodType.Monthly, BudgetMethod.Rule503020);
        _dbContext.Budgets.Add(budget);

        // Add 5 weekday transactions (each 200k) and 2 weekend transactions (each 500k)
        for (int i = 1; i <= 7; i++)
        {
            var date = startDate.AddDays(i);
            var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            var amount = isWeekend ? 500_000m : 200_000m;
            var tx = new Transaction(_userId, _cashWallet.Id, _foodCategory.Id, amount, TransactionType.Expense, date);
            _dbContext.Transactions.Add(tx);
        }
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetExpenditureForecastAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.BudgetId.Should().Be(budget.Id);
        result.Value.InsufficientData.Should().BeFalse();
        result.Value.EstimatedTotalSpent.Should().BeGreaterThan(0);
        result.Value.Trajectory.Should().NotBeEmpty();
    }

    // UC-08.01 Test 2: Cycle under 3 days triggers insufficient data flow (E1)
    [Fact]
    public async Task GetExpenditureForecast_WhenCycleUnder3Days_ShouldReturnInsufficientDataState()
    {
        // Arrange: Budget started yesterday (only 2 days passed)
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-1);
        var endDate = today.AddDays(28);

        var budget = new Budget(_userId, "Ngân sách mới", 10_000_000m, startDate, endDate, BudgetPeriodType.Monthly, BudgetMethod.Rule503020);
        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetExpenditureForecastAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.InsufficientData.Should().BeTrue();
        result.Value.AdvisoryMessage.Should().Contain("tối thiểu 3 ngày");
    }

    // UC-08.01 Test 3: Large outlier transaction is excluded from velocity projection (E2)
    [Fact]
    public async Task GetExpenditureForecast_ShouldExcludeOutliersFromVelocityCalculation()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-5);
        var endDate = today.AddDays(25);

        var budget = new Budget(_userId, "Ngân sách có outlier", 20_000_000m, startDate, endDate, BudgetPeriodType.Monthly, BudgetMethod.Rule503020);
        _dbContext.Budgets.Add(budget);

        // Normal daily expenses: 100k
        for (int i = 1; i <= 4; i++)
        {
            var tx = new Transaction(_userId, _cashWallet.Id, _foodCategory.Id, 100_000m, TransactionType.Expense, startDate.AddDays(i));
            _dbContext.Transactions.Add(tx);
        }
        // One huge spike outlier: 50,000,000 (e.g. emergency hospital)
        var outlierTx = new Transaction(_userId, _bankWallet.Id, _foodCategory.Id, 50_000_000m, TransactionType.Expense, startDate.AddDays(2));
        _dbContext.Transactions.Add(outlierTx);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetExpenditureForecastAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // The daily velocity should NOT be influenced by the 50,000,000 outlier
        result.Value!.WeekdayDailyVelocity.Should().BeLessThan(1_000_000m);
    }

    // UC-08.02 Test 4: Liquidity depletion before salary flags crisis
    [Fact]
    public async Task GetLiquidityDepletion_WhenRunOutOfMoneyBeforeSalary_ShouldFlagCrisisAndDeficitDays()
    {
        // Arrange: Liquid funds = 1,000,000; high velocity = 500,000/day -> will deplete in 2 days
        var emptyCash = new Wallet(_userId, "Ví Rỗng", WalletType.Cash, 1_000_000m);
        _dbContext.Wallets.Add(emptyCash);

        // Remove previous wallets to isolate
        _cashWallet.SetReconciledBalance(0m);
        _bankWallet.SetReconciledBalance(0m);

        var today = DateTime.UtcNow.Date;
        // 7 days of expenses: 500k each day -> velocity = 500,000/day
        for (int i = 1; i <= 7; i++)
        {
            var tx = new Transaction(_userId, emptyCash.Id, _foodCategory.Id, 500_000m, TransactionType.Expense, today.AddDays(-i));
            _dbContext.Transactions.Add(tx);
        }

        // Salary arrives in 15 days
        var recurring = new RecurringIncome(_userId, "Lương", 20_000_000m, emptyCash.Id, _foodCategory.Id, RecurringFrequency.Monthly, today.AddDays(15).Day, startDate: today.AddDays(15));
        _dbContext.RecurringIncomes.Add(recurring);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetLiquidityDepletionForecastAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.IsLiquidityCrisis.Should().BeTrue();
        result.Value.DaysUntilDepletion.Should().BeLessThan(5);
        result.Value.DeficitDaysBeforeSalary.Should().BeGreaterThan(0);
        result.Value.LiquidityHealthStatus.Should().Be("Crisis");
    }

    // UC-08.02 Test 5: Zero velocity handled gracefully without divide-by-zero
    [Fact]
    public async Task GetLiquidityDepletion_WhenVelocityZero_ShouldHandleZeroDivisionGracefully()
    {
        // Arrange: No expenses in the last 7 days
        // Act
        var result = await _service.GetLiquidityDepletionForecastAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.IsLiquidityCrisis.Should().BeFalse();
        result.Value.DaysUntilDepletion.Should().Be(9999);
        result.Value.LiquidityHealthStatus.Should().Be("Safe");
    }

    // UC-08.02 Test 6: Zero or negative liquid funds returns immediate crisis (E1)
    [Fact]
    public async Task GetLiquidityDepletion_WhenLiquidFundsZeroOrNegative_ShouldReturnImmediateCrisis()
    {
        // Arrange: Reset all wallets to 0
        _cashWallet.SetReconciledBalance(0m);
        _bankWallet.SetReconciledBalance(0m);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetLiquidityDepletionForecastAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.IsLiquidityCrisis.Should().BeTrue();
        result.Value.DaysUntilDepletion.Should().Be(0);
        result.Value.LiquidityHealthStatus.Should().Be("Crisis");
    }

    // UC-08.03 Test 7: Category with high spending speed ranked as HighRisk
    [Fact]
    public async Task GetCategoryDeficitRisks_WithSpikeCategory_ShouldCalculateExhaustionDateAndRankHigh()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var budget = new Budget(_userId, "Ngân sách tháng", 10_000_000m, today.AddDays(-10), today.AddDays(20), BudgetPeriodType.Monthly, BudgetMethod.Rule503020);
        
        // Food category allocation: 3,000,000; already spent 2,500,000 in 10 days (250k/day) -> will deplete soon
        budget.AddAllocation(_foodCategory.Id, 3_000_000m);
        // Rent category: 4,000,000; spent 0 -> safe
        budget.AddAllocation(_rentCategory.Id, 4_000_000m);

        budget.Allocations.First(a => a.CategoryId == _foodCategory.Id).RecordExpense(2_500_000m);
        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetCategoryDeficitRisksAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var topRisk = result.Value.First();
        topRisk.CategoryId.Should().Be(_foodCategory.Id);
        topRisk.RiskSeverity.Should().Be("HighRisk");
        topRisk.RiskRatio.Should().BeGreaterThan(115m);
        topRisk.EstimatedExhaustionDate.Should().NotBeNull();
    }

    // UC-08.04 Test 8: What-If simulation for LumpSumExpense is purely in-memory and reduces DSA
    [Fact]
    public async Task SimulateWhatIf_LumpSumExpense_ShouldReduceDsaAndAdvanceDepletionDateWithoutDbMutation()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var budget = new Budget(_userId, "Ngân sách active", 10_000_000m, today.AddDays(-5), today.AddDays(25), BudgetPeriodType.Monthly, BudgetMethod.Rule503020);
        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        var request = new WhatIfSimulationRequestDto
        {
            ScenarioType = WhatIfScenarioType.LumpSumExpense,
            LumpSumAmount = 3_000_000m
        };

        // Act
        var result = await _service.SimulateWhatIfScenarioAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.DsaAfter.Should().BeLessThan(result.Value.DsaBefore);
        result.Value.NetWorthAfter.Should().Be(result.Value.NetWorthBefore - 3_000_000m);
        result.Value.ImpactSummaryMessage.Should().Contain("bị siết giảm");

        // Verify In-Memory Sandbox guarantee: Database was NOT modified
        var freshBudget = await _dbContext.Budgets.AsNoTracking().FirstAsync(b => b.Id == budget.Id);
        freshBudget.TotalAmount.Should().Be(10_000_000m);
    }

    // UC-08.04 Test 9: What-If simulation for InstallmentLoan recalculates DTI
    [Fact]
    public async Task SimulateWhatIf_InstallmentLoan_ShouldRecalculateDtiRatioAccurately()
    {
        // Arrange
        var request = new WhatIfSimulationRequestDto
        {
            ScenarioType = WhatIfScenarioType.InstallmentLoan,
            MonthlyInstallmentAmount = 3_000_000m,
            InstallmentMonths = 12
        };

        // Act
        var result = await _service.SimulateWhatIfScenarioAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DtiAfter.Should().BeGreaterThan(result.Value.DtiBefore);
        result.Value.ImpactSummaryMessage.Should().Contain("DTI");
    }

    // UC-08.04 Test 10: What-If simulation for IncomeReduction reduces DSA
    [Fact]
    public async Task SimulateWhatIf_IncomeReduction_ShouldAdjustDsaAndDtiAccurately()
    {
        // Arrange
        var request = new WhatIfSimulationRequestDto
        {
            ScenarioType = WhatIfScenarioType.IncomeReduction,
            IncomeReductionPercentage = 30m // 30% reduction
        };

        // Act
        var result = await _service.SimulateWhatIfScenarioAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DsaAfter.Should().BeLessThan(result.Value.DsaBefore);
        result.Value.IsHighRiskDecision.Should().BeTrue();
    }

    // UC-08.04 Test 11: EmergencyShock scenario stress-tests emergency fund and calculates runway accurately
    [Fact]
    public async Task SimulateWhatIf_EmergencyShock_ShouldStressTestEmergencyFundAndRunway()
    {
        // Arrange: Create Emergency Fund goal with 30,000,000 VND
        var emergencyGoal = new SavingsGoal(
            _userId,
            "Quỹ Dự Phòng 3 Tháng",
            30_000_000m,
            DateTime.UtcNow.AddMonths(-3),
            DateTime.UtcNow.AddMonths(3),
            SavingsGoalType.EmergencyFund,
            SavingsGoalPriority.High,
            initialAmount: 30_000_000m);

        _dbContext.SavingsGoals.Add(emergencyGoal);
        await _dbContext.SaveChangesAsync();

        var request = new WhatIfSimulationRequestDto
        {
            ScenarioType = WhatIfScenarioType.EmergencyShock,
            EmergencyExpenseAmount = 15_000_000m, // 15M medical shock
            EmergencyIncomeDropPercentage = 0m
        };

        // Act
        var result = await _service.SimulateWhatIfScenarioAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.EmergencyFundRemainingBefore.Should().Be(30_000_000m);
        result.Value.EmergencyFundRemainingAfter.Should().Be(15_000_000m);
        result.Value.RunwayMonthsAfter.Should().BeLessThan(result.Value.RunwayMonthsBefore);
        result.Value.NetWorthAfter.Should().Be(result.Value.NetWorthBefore - 15_000_000m);
        result.Value.ImpactSummaryMessage.Should().Contain("Sau biến cố khẩn cấp");

        // Verify In-Memory Sandbox: Database Emergency Fund remains unchanged
        var freshGoal = await _dbContext.SavingsGoals.AsNoTracking().FirstAsync(g => g.Id == emergencyGoal.Id);
        freshGoal.CurrentAmount.Should().Be(30_000_000m);
    }

    // UC-08.04 Test 12: Exception Flow E1 - Lump sum exceeding liquid wallet balance triggers critical deficit alert
    [Fact]
    public async Task SimulateWhatIf_LumpSum_WhenExceedsLiquidWallets_ShouldFlagLiquidDeficitRisk_ExceptionE1()
    {
        // Arrange: Total liquid funds = 20,000,000. Simulate a 35,000,000 purchase
        var request = new WhatIfSimulationRequestDto
        {
            ScenarioType = WhatIfScenarioType.LumpSumExpense,
            LumpSumAmount = 35_000_000m
        };

        // Act
        var result = await _service.SimulateWhatIfScenarioAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IsLiquidDeficitRisk.Should().BeTrue();
        result.Value.IsHighRiskDecision.Should().BeTrue();
        result.Value.ImpactSummaryMessage.Should().Contain("vượt quá tổng tiền mặt khả dụng");
    }

    // UC-08.04 Test 13: Installment loan computes accurate DTI utilizing real Debts catalog from DB
    [Fact]
    public async Task SimulateWhatIf_InstallmentLoan_UsesRealDebtsToComputeAccurateDti()
    {
        // Arrange: Add a real Debt with 2,000,000/month minimum payment
        var debt = new Debt(
            _userId,
            "Vay Mua Xe Máy",
            DebtType.InstallmentLoan,
            "FE Credit",
            30_000_000m,
            12.0m,
            2_000_000m,
            15,
            DateTime.UtcNow.AddMonths(-2),
            currentBalance: 20_000_000m);

        _dbContext.Debts.Add(debt);

        // Add RecurringIncome: 20,000,000/month
        var income = new RecurringIncome(
            _userId,
            "Lương Công Ty",
            20_000_000m,
            _bankWallet.Id,
            _foodCategory.Id,
            RecurringFrequency.Monthly,
            25);

        _dbContext.RecurringIncomes.Add(income);
        await _dbContext.SaveChangesAsync();

        var request = new WhatIfSimulationRequestDto
        {
            ScenarioType = WhatIfScenarioType.InstallmentLoan,
            MonthlyInstallmentAmount = 2_000_000m,
            InstallmentMonths = 12
        };

        // Act
        var result = await _service.SimulateWhatIfScenarioAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Baseline DTI: 2M / 20M = 10%
        result.Value!.DtiBefore.Should().Be(10.0m);
        // After DTI: (2M existing + 2M new) / 20M = 20%
        result.Value.DtiAfter.Should().Be(20.0m);
    }

    // UC-08.04 Test 14: Alternate Flow A1 - Convert simulated purchase to SavingsGoal
    [Fact]
    public async Task ConvertWhatIfToSavingsGoal_ValidInput_CreatesSavingsGoalSuccessfully()
    {
        // Arrange
        var targetDate = DateTime.UtcNow.Date.AddMonths(6);
        var request = new ConvertToSavingsGoalRequestDto
        {
            GoalName = "MacBook Pro M4",
            TargetAmount = 30_000_000m,
            TargetDate = targetDate,
            Notes = "Chuyển từ What-If mô phỏng mua sắm"
        };

        // Act
        var result = await _service.ConvertWhatIfToSavingsGoalAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.GoalName.Should().Be("MacBook Pro M4");
        result.Value.TargetAmount.Should().Be(30_000_000m);
        result.Value.MonthlyCommitment.Should().BeGreaterThan(0);
        result.Value.Message.Should().Contain("MacBook Pro M4");

        // Verify DB persistence
        var created = await _dbContext.SavingsGoals.FirstOrDefaultAsync(g => g.Id == result.Value.GoalId);
        created.Should().NotBeNull();
        created!.Name.Should().Be("MacBook Pro M4");
        created.TargetAmount.Should().Be(30_000_000m);
        created.Type.Should().Be(SavingsGoalType.MajorPurchase);
        created.Status.Should().Be(SavingsGoalStatus.Active);
    }
}
