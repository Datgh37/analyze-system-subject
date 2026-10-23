using ExpenseTracker.Maui.Models.Forecasting;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests;

public class ForecastingViewModelTests
{
    private readonly Mock<IForecastingRepository> _mockRepository;
    private readonly ForecastingViewModel _viewModel;

    public ForecastingViewModelTests()
    {
        _mockRepository = new Mock<IForecastingRepository>();
        _viewModel = new ForecastingViewModel(_mockRepository.Object);
    }

    [Fact]
    public async Task LoadForecastData_WhenSuccessful_ShouldPopulateCardsAndChartData()
    {
        // Arrange
        var forecast = new ExpenseForecastModel
        {
            BudgetId = Guid.NewGuid(),
            BudgetName = "Ngân sách tháng 10",
            TotalBudget = 15_000_000m,
            CurrentSpent = 9_450_000m,
            EstimatedTotalSpent = 16_850_000m,
            ProjectedOverspendAmount = 1_850_000m,
            WeekdayDailyVelocity = 420_000m,
            WeekendDailyVelocity = 650_000m,
            RiskLevel = "HighRisk",
            Trajectory = new List<DailyForecastPointModel>
            {
                new() { Date = DateTime.UtcNow.Date, DayLabel = "23/10", DailySpent = 420_000m, CumulativeSpent = 9_450_000m }
            }
        };

        var liquidity = new LiquidityDepletionModel
        {
            TotalLiquidFunds = 3_200_000m,
            Recent7DaysDailyVelocity = 485_000m,
            DaysUntilDepletion = 6,
            OutOfMoneyDate = DateTime.UtcNow.Date.AddDays(6),
            NextSalaryDate = DateTime.UtcNow.Date.AddDays(13),
            DeficitDaysBeforeSalary = 7,
            IsLiquidityCrisis = true,
            LiquidityHealthStatus = "Crisis"
        };

        var risks = new List<CategoryDeficitRiskModel>
        {
            new()
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Ăn uống",
                IsEssential = true,
                AllocatedAmount = 6_000_000m,
                CurrentSpent = 5_200_000m,
                ProjectedSpentEnd = 7_800_000m,
                RiskRatio = 130m,
                RiskSeverity = "HighRisk"
            }
        };

        var whatIf = new WhatIfSimulationResultModel
        {
            ScenarioType = WhatIfScenarioType.LumpSumExpense,
            DsaBefore = 450_000m,
            DsaAfter = 350_000m,
            ImpactSummaryMessage = "Hạn mức bị siết giảm"
        };

        _mockRepository.Setup(r => r.GetExpenditureForecastAsync()).ReturnsAsync(forecast);
        _mockRepository.Setup(r => r.GetLiquidityDepletionAsync()).ReturnsAsync(liquidity);
        _mockRepository.Setup(r => r.GetCategoryRisksAsync()).ReturnsAsync(risks);
        _mockRepository.Setup(r => r.SimulateWhatIfAsync(It.IsAny<WhatIfSimulationRequestModel>())).ReturnsAsync(whatIf);

        // Act
        await _viewModel.LoadForecastDataAsync();

        // Assert
        _viewModel.IsBusy.Should().BeFalse();
        _viewModel.HasError.Should().BeFalse();
        _viewModel.Forecast.Should().NotBeNull();
        _viewModel.Forecast!.EstimatedTotalSpent.Should().Be(16_850_000m);
        _viewModel.TrajectoryPoints.Should().HaveCount(1);

        _viewModel.Liquidity.Should().NotBeNull();
        _viewModel.Liquidity!.IsLiquidityCrisis.Should().BeTrue();
        _viewModel.Liquidity.DeficitDaysBeforeSalary.Should().Be(7);

        _viewModel.CategoryRisks.Should().HaveCount(1);
        _viewModel.CategoryRisks.First().CategoryName.Should().Be("Ăn uống");

        _viewModel.WhatIfResult.Should().NotBeNull();
        _viewModel.StatusMessage.Should().Contain("thời gian thực");
    }

    [Fact]
    public async Task RunWhatIfSimulation_WhenTriggered_ShouldUpdateComparisonResults()
    {
        // Arrange
        var whatIf = new WhatIfSimulationResultModel
        {
            ScenarioType = WhatIfScenarioType.LumpSumExpense,
            ScenarioDescription = "Mua sắm lớn: 5,000,000 đ",
            DsaBefore = 500_000m,
            DsaAfter = 350_000m,
            DsaChangePercentage = -30m,
            IsHighRiskDecision = true,
            ImpactSummaryMessage = "Hạn mức giảm mạnh"
        };

        _mockRepository.Setup(r => r.SimulateWhatIfAsync(It.IsAny<WhatIfSimulationRequestModel>()))
            .ReturnsAsync(whatIf);

        _viewModel.LumpSumAmount = 5_000_000m;
        _viewModel.SelectedScenarioType = WhatIfScenarioType.LumpSumExpense;

        // Act
        await _viewModel.RunWhatIfSimulationAsync();

        // Assert
        _viewModel.WhatIfResult.Should().NotBeNull();
        _viewModel.WhatIfResult!.DsaAfter.Should().Be(350_000m);
        _viewModel.WhatIfResult.IsHighRiskDecision.Should().BeTrue();
        _viewModel.WhatIfResult.ImpactSummaryMessage.Should().Contain("giảm");
    }

    [Fact]
    public async Task SelectScenario_ShouldSwitchScenarioTypeAndRecalculate()
    {
        // Arrange
        var whatIfInstallment = new WhatIfSimulationResultModel
        {
            ScenarioType = WhatIfScenarioType.InstallmentLoan,
            ScenarioDescription = "Vay trả góp",
            DtiBefore = 10m,
            DtiAfter = 35m
        };

        _mockRepository.Setup(r => r.SimulateWhatIfAsync(It.Is<WhatIfSimulationRequestModel>(q => q.ScenarioType == WhatIfScenarioType.InstallmentLoan)))
            .ReturnsAsync(whatIfInstallment);

        // Act
        await _viewModel.SelectScenarioAsync("Installment");

        // Assert
        _viewModel.SelectedScenarioType.Should().Be(WhatIfScenarioType.InstallmentLoan);
        _viewModel.WhatIfResult.Should().NotBeNull();
        _viewModel.WhatIfResult!.DtiAfter.Should().Be(35m);
    }

    [Fact]
    public async Task HandleRepositoryFailure_ShouldSetErrorMessageGracefully()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetExpenditureForecastAsync())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        // Act
        await _viewModel.LoadForecastDataAsync();

        // Assert
        _viewModel.IsBusy.Should().BeFalse();
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ErrorMessage.Should().Contain("Connection refused");
    }
}
