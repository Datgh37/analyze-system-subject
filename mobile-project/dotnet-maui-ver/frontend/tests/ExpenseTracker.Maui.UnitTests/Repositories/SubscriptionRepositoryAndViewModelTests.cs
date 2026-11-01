using ExpenseTracker.Maui.Models.SmartUtilities;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests.Repositories;

public class SubscriptionRepositoryAndViewModelTests
{
    [Fact]
    public async Task MockSubscriptionRepository_PerformsFullCrudLifecycle()
    {
        // Arrange
        var repo = new MockSubscriptionRepository();

        // 1. Get initial subscriptions
        var list = await repo.GetSubscriptionsAsync();
        list.Should().HaveCount(4);

        // 2. Create new subscription
        var created = await repo.CreateSubscriptionAsync(new CreateSubscriptionRequest
        {
            Name = "VieON VIP",
            Amount = 69_000m,
            BillingCycle = 2,
            NextBillingDate = DateTime.UtcNow.AddDays(25),
            CategoryId = Guid.NewGuid()
        });
        created.Should().NotBeNull();
        created.Name.Should().Be("VieON VIP");

        // 3. Update
        var updated = await repo.UpdateSubscriptionAsync(created.Id, new UpdateSubscriptionRequest
        {
            Name = "VieON All Access",
            Amount = 99_000m,
            BillingCycle = 2,
            NextBillingDate = DateTime.UtcNow.AddDays(25),
            CategoryId = created.CategoryId
        });
        updated.Name.Should().Be("VieON All Access");
        updated.Amount.Should().Be(99_000m);

        // 4. Archive
        var archived = await repo.ArchiveSubscriptionAsync(created.Id);
        archived.Should().BeTrue();
    }

    [Fact]
    public async Task MockSubscriptionRepository_AuditAndUpcoming_ReturnsAccurateMetrics()
    {
        // Arrange
        var repo = new MockSubscriptionRepository();

        // 1. Audit report
        var audit = await repo.AuditSubscriptionsAsync();
        audit.Should().NotBeNull();
        audit.ActiveSubscriptionsCount.Should().BeGreaterThan(0);
        audit.TotalMonthlyDrain.Should().BeGreaterThan(0);
        audit.TotalAnnualDrain.Should().BeGreaterThan(0);
        audit.DetectedCandidates.Should().NotBeEmpty();

        // 2. Upcoming renewals in 7 days
        var upcoming = await repo.GetUpcomingRenewalsAsync(7);
        upcoming.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SubscriptionsViewModel_LoadsData_AndHandlesTabSwitching()
    {
        // Arrange
        var repo = new MockSubscriptionRepository();
        var vm = new SubscriptionsViewModel(repo);

        // Act
        await vm.LoadDataAsync();

        // Assert
        vm.Subscriptions.Should().HaveCount(4);
        vm.UpcomingRenewals.Should().NotBeEmpty();
        vm.AuditReport.Should().NotBeNull();
        vm.SelectedTab.Should().Be("MANAGED");

        // Switch tabs
        vm.SelectTab("AUDIT");
        vm.SelectedTab.Should().Be("AUDIT");
    }

    [Fact]
    public async Task SubscriptionsViewModel_HandlesCandidateConversion()
    {
        // Arrange
        var repo = new MockSubscriptionRepository();
        var vm = new SubscriptionsViewModel(repo);
        await vm.LoadDataAsync();

        var candidate = new SubscriptionCandidateModel
        {
            ServiceName = "OpenAI ChatGPT Plus",
            SuggestedAmount = 500_000m,
            SuggestedBillingCycle = 2,
            CategoryId = Guid.NewGuid()
        };

        // Act
        await vm.ConvertCandidateAsync(candidate);

        // Assert
        vm.FeedbackMessage.Should().Contain("thành công");
        vm.Subscriptions.Should().HaveCount(5);
        vm.Subscriptions.Should().Contain(s => s.Name == "OpenAI ChatGPT Plus");
    }
}

