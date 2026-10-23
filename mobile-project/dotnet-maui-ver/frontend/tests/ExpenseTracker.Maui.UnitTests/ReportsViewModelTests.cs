using ExpenseTracker.Maui.Models.Reports;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests;

public class ReportsViewModelTests
{
    private readonly Mock<IAnalyticsReportRepository> _mockRepository;
    private readonly ReportsViewModel _viewModel;

    public ReportsViewModelTests()
    {
        _mockRepository = new Mock<IAnalyticsReportRepository>();
        _viewModel = new ReportsViewModel(_mockRepository.Object);
    }

    [Fact]
    public async Task LoadReports_WhenSuccessful_ShouldPopulateAllReportData()
    {
        // Arrange
        var incomeExpense = new IncomeExpenseComparisonModel
        {
            TotalIncome = 30_000_000m,
            TotalExpense = 18_000_000m,
            NetSurplus = 12_000_000m,
            Breakdown = new List<PeriodDataPointModel>
            {
                new() { PeriodLabel = "2026-10", Income = 30_000_000m, Expense = 18_000_000m, NetSurplus = 12_000_000m }
            }
        };

        var netWorth = new NetWorthReportModel
        {
            TotalAssets = 100_000_000m,
            TotalLiabilities = 10_000_000m,
            CurrentNetWorth = 90_000_000m
        };

        var cashflow = new NetCashflowReportModel
        {
            TotalIncome = 30_000_000m,
            TotalExpense = 18_000_000m,
            NetCashflow = 12_000_000m,
            SavingsRate = 40.0m,
            HealthStatus = "Excellent",
            WaterfallSteps = new List<WaterfallStepModel>
            {
                new() { StepName = "Thu nhập", Amount = 30_000_000m, RunningBalance = 30_000_000m, StepType = "Inflow" }
            }
        };

        var structure = new ExpenseStructureReportModel
        {
            TotalExpense = 18_000_000m,
            BySubCategory = new List<CategorySpendingShareModel>
            {
                new() { CategoryName = "Thực phẩm", Amount = 8_000_000m, Percentage = 44.4m, IsDominantAlert = true }
            }
        };

        var outliers = new OutlierDetectionReportModel
        {
            TotalAnalyzedTransactions = 20,
            OutlierCount = 1,
            Outliers = new List<OutlierTransactionModel>
            {
                new() { Amount = 5_000_000m, ZScore = 3.5, AlertSeverity = "High" }
            }
        };

        _mockRepository.Setup(r => r.GetIncomeExpenseAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "month"))
            .ReturnsAsync(incomeExpense);
        _mockRepository.Setup(r => r.GetNetWorthAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(netWorth);
        _mockRepository.Setup(r => r.GetCashflowAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(cashflow);
        _mockRepository.Setup(r => r.GetExpenseStructureAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(structure);
        _mockRepository.Setup(r => r.GetOutliersAsync(30, 2.0))
            .ReturnsAsync(outliers);

        // Act
        await _viewModel.LoadReportsAsync();

        // Assert
        _viewModel.IsBusy.Should().BeFalse();
        _viewModel.ErrorMessage.Should().BeNull();
        _viewModel.IncomeExpenseReport.Should().NotBeNull();
        _viewModel.IncomeExpenseReport!.TotalIncome.Should().Be(30_000_000m);
        _viewModel.NetWorthReport.Should().NotBeNull();
        _viewModel.NetWorthReport!.CurrentNetWorth.Should().Be(90_000_000m);
        _viewModel.CashflowReport.Should().NotBeNull();
        _viewModel.CashflowReport!.SavingsRate.Should().Be(40.0m);
        _viewModel.ExpenseStructureReport.Should().NotBeNull();
        _viewModel.OutlierReport.Should().NotBeNull();

        _viewModel.BreakdownItems.Should().HaveCount(1);
        _viewModel.WaterfallSteps.Should().HaveCount(1);
        _viewModel.SubCategories.Should().HaveCount(1);
        _viewModel.Outliers.Should().HaveCount(1);
    }

    [Fact]
    public async Task SelectTab_ShouldUpdateActiveTabState()
    {
        // Act
        await _viewModel.SelectTabAsync("Cashflow");

        // Assert
        _viewModel.ActiveTab.Should().Be("Cashflow");
    }

    [Fact]
    public async Task TakeSnapshot_WhenSuccessful_ShouldUpdateCurrentNetWorth()
    {
        // Arrange
        var snapshot = new NetWorthSnapshotModel
        {
            Id = Guid.NewGuid(),
            SnapshotDate = DateTime.UtcNow.Date,
            TotalAssets = 120_000_000m,
            TotalLiabilities = 15_000_000m,
            NetWorth = 105_000_000m
        };

        _mockRepository.Setup(r => r.RecordNetWorthSnapshotAsync(It.IsAny<string>()))
            .ReturnsAsync(snapshot);

        _mockRepository.Setup(r => r.GetNetWorthAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(new NetWorthReportModel { CurrentNetWorth = 105_000_000m });

        // Act
        await _viewModel.TakeSnapshotAsync();

        // Assert
        _viewModel.IsBusy.Should().BeFalse();
        _viewModel.ErrorMessage.Should().BeNull();
        _viewModel.StatusMessage.Should().Contain("Đã ghi nhận ảnh chụp số dư");
        _viewModel.NetWorthReport.Should().NotBeNull();
        _viewModel.NetWorthReport!.CurrentNetWorth.Should().Be(105_000_000m);
    }

    [Fact]
    public async Task LoadReports_WhenRepositoryFails_ShouldSetErrorMessageAndResetLoading()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetIncomeExpenseAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Mạng không ổn định"));

        // Act
        await _viewModel.LoadReportsAsync();

        // Assert
        _viewModel.IsBusy.Should().BeFalse();
        _viewModel.ErrorMessage.Should().Contain("Mạng không ổn định");
    }
}
