using ExpenseTracker.Maui.Models.SavingsGoals;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests.Repositories;

public class EmergencyFundAndNetWorthTests
{
    [Fact]
    public async Task MockEmergencyFundRepository_PerformsFullDepositAndWithdrawCycle()
    {
        // Arrange
        var repo = new MockEmergencyFundRepository();

        // 1. Get recommendation
        var rec = await repo.GetRecommendationAsync();
        rec.Should().NotBeNull();
        rec.MonthlyEssentialExpenses.Should().Be(6_000_000m);
        rec.SixMonthsAmount.Should().Be(36_000_000m);

        // 2. Deposit
        var afterDeposit = await repo.DepositAsync(new DepositEmergencyFundRequest
        {
            Amount = 4_000_000m,
            SourceWalletId = Guid.NewGuid()
        });
        afterDeposit.CurrentAmount.Should().Be(22_000_000m); // 18tr + 4tr

        // 3. Withdraw
        var afterWithdraw = await repo.WithdrawAsync(new WithdrawEmergencyFundRequest
        {
            Amount = 2_000_000m,
            TargetWalletId = Guid.NewGuid(),
            EmergencyReason = "Medical",
            DetailedExplanation = "Viện phí khẩn cấp cấp cứu"
        });
        afterWithdraw.CurrentAmount.Should().Be(20_000_000m);
    }

    [Fact]
    public async Task EmergencyFundViewModel_LoadsData_AndHandlesSetup()
    {
        // Arrange
        var repo = new MockEmergencyFundRepository();
        var vm = new EmergencyFundViewModel(repo);

        // Act
        await vm.LoadDataAsync();

        // Assert
        vm.Recommendation.Should().NotBeNull();
        vm.Status.Should().NotBeNull();
        vm.Status!.HasFundConfigured.Should().BeTrue();
        vm.SelectedMonths.Should().Be(6);

        // Setup with 12 months
        vm.SelectedMonths = 12;
        await vm.SetupFundAsync();

        vm.Status.TargetMonths.Should().Be(12);
        vm.FeedbackMessage.Should().Contain("thành công");
    }

    [Fact]
    public async Task MockNetWorthRepository_CalculatesRealtimeBreakdownAndSnapshots()
    {
        // Arrange
        var repo = new MockNetWorthRepository();

        // 1. Get current net worth
        var current = await repo.GetCurrentNetWorthAsync();
        current.Should().NotBeNull();
        current.TotalAssets.Should().Be(150_000_000m);
        current.TotalLiabilities.Should().Be(30_000_000m);
        current.NetWorth.Should().Be(120_000_000m);
        current.DebtToAssetRatio.Should().Be(20.0m);
        current.FinancialHealthRating.Should().Be("Healthy");

        // 2. History
        var history = await repo.GetNetWorthHistoryAsync(12);
        history.Should().HaveCount(3);

        // 3. Record snapshot
        var newSnapshot = await repo.RecordSnapshotAsync(new Models.Reports.RecordNetWorthSnapshotRequest { Notes = "Tháng 11 mới" });
        newSnapshot.NetWorth.Should().Be(120_000_000m);
        newSnapshot.Notes.Should().Be("Tháng 11 mới");

        var updatedHistory = await repo.GetNetWorthHistoryAsync(12);
        updatedHistory.Should().HaveCount(4);
    }

    [Fact]
    public async Task NetWorthViewModel_LoadsData_AndRecordsSnapshot()
    {
        // Arrange
        var repo = new MockNetWorthRepository();
        var vm = new NetWorthViewModel(repo);

        // Act
        await vm.LoadDataAsync();

        // Assert
        vm.Breakdown.Should().NotBeNull();
        vm.Breakdown!.NetWorth.Should().Be(120_000_000m);
        vm.History.Should().HaveCount(3);

        // Record snapshot
        vm.SnapshotNotes = "Ghi chú thử nghiệm";
        await vm.RecordSnapshotCommand.ExecuteAsync(null);

        vm.FeedbackMessage.Should().Contain("thành công");
        vm.History.Should().HaveCount(4);
    }
}

