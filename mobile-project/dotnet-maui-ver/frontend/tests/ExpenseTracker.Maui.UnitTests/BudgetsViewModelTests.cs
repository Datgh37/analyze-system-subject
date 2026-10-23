using ExpenseTracker.Maui.Models.Budgets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Maui.UnitTests;

public class BudgetsViewModelTests
{
    private readonly Mock<IBudgetRepository> _mockBudgetRepo;
    private readonly BudgetsViewModel _viewModel;

    public BudgetsViewModelTests()
    {
        _mockBudgetRepo = new Mock<IBudgetRepository>();
        _viewModel = new BudgetsViewModel(_mockBudgetRepo.Object);
    }

    [Fact]
    public async Task LoadActiveBudgetProgressAsync_WhenActiveBudgetExists_ShouldPopulateOverviewAndDsa()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var activeBudget = new BudgetModel(
            Id: budgetId,
            UserId: Guid.NewGuid(),
            Name: "Ngân Sách Tháng 10",
            PeriodType: 1,
            PeriodTypeName: "Tháng",
            StartDate: DateTime.Today.AddDays(-10),
            EndDate: DateTime.Today.AddDays(20),
            TotalAmount: 10000000m,
            Method: 1,
            MethodName: "50/30/20",
            Status: 1,
            StatusName: "Hoạt động",
            TotalAllocated: 10000000m,
            TotalSpent: 4000000m,
            TotalRemaining: 6000000m,
            OverallSpentPercent: 40m,
            TotalDays: 31,
            Allocations: new List<BudgetAllocationModel>()
        );

        var dsa = new DsaStatusModel(
            Date: DateTime.Today,
            DailyAllowanceAmount: 200000m,
            ActualSpentToday: 80000m,
            RemainingAllowanceToday: 120000m,
            RemainingFlexibleBudget: 6000000m,
            RemainingDays: 20,
            IsExceeded: false,
            AdviceMessage: "An toàn"
        );

        var velocity = new VelocityStatusModel(
            VelocityIndex: 0.9m,
            LevelName: "Ổn định",
            LevelColorHex: "#10B981",
            ActualVelocityPerDay: 180000m,
            TargetVelocityPerDay: 200000m,
            OutOfMoneyForecastDate: null,
            DaysUntilDepletion: null,
            ForecastMessage: "Tốc độ an toàn"
        );

        var overview = new BudgetProgressModel(
            BudgetId: budgetId,
            BudgetName: "Ngân Sách Tháng 10",
            TotalAmount: 10000000m,
            TotalSpent: 4000000m,
            TotalRemaining: 6000000m,
            OverallSpentPercent: 40m,
            DaysPassed: 10,
            TotalDays: 31,
            Dsa: dsa,
            Velocity: velocity,
            Allocations: new List<BudgetAllocationModel>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "Ăn uống",
                    AllocatedAmount = 4000000m,
                    CurrentSpent = 2500000m,
                    RemainingAmount = 1500000m,
                    SpentPercent = 62.5m
                }
            }
        );

        _mockBudgetRepo.Setup(r => r.GetActiveBudgetAsync()).ReturnsAsync(activeBudget);
        _mockBudgetRepo.Setup(r => r.GetBudgetProgressOverviewAsync()).ReturnsAsync(overview);

        // Act
        await _viewModel.LoadActiveBudgetProgressAsync();

        // Assert
        _viewModel.HasActiveBudget.Should().BeTrue();
        _viewModel.ActiveBudget.Should().NotBeNull();
        _viewModel.ActiveBudget!.Name.Should().Be("Ngân Sách Tháng 10");
        _viewModel.DsaStatus.Should().NotBeNull();
        _viewModel.DsaStatus!.DailyAllowanceAmount.Should().Be(200000m);
        _viewModel.Allocations.Should().HaveCount(1);
        _viewModel.Allocations[0].CategoryName.Should().Be("Ăn uống");
        _viewModel.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task LoadActiveBudgetProgressAsync_WhenErrorOccurs_ShouldSetErrorMessage()
    {
        // Arrange
        _mockBudgetRepo.Setup(r => r.GetBudgetProgressOverviewAsync()).ThrowsAsync(new InvalidOperationException("Database connection timeout"));

        // Act
        await _viewModel.LoadActiveBudgetProgressAsync();

        // Assert
        _viewModel.ErrorMessage.Should().Contain("Database connection timeout");
    }
}
