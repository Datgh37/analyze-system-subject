using ExpenseTracker.Application.DTOs.SmartUtilities;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class SubscriptionServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SubscriptionService _service;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    public SubscriptionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _service = new SubscriptionService(_dbContext);

        // Seed default category
        var category = new Category("Dịch vụ số", TransactionType.Expense, _userId);
        _dbContext.Categories.Add(category);
        _dbContext.SaveChanges();
        _categoryId = category.Id;
    }

    [Fact]
    public async Task CreateSubscription_WithValidData_ReturnsCreatedDto()
    {
        // Arrange
        var request = new CreateSubscriptionDto
        {
            Name = "Netflix 4K",
            Amount = 260_000m,
            BillingCycle = SubscriptionBillingCycle.Monthly,
            NextBillingDate = DateTime.UtcNow.AddDays(15),
            CategoryId = _categoryId,
            Status = SubscriptionStatus.Active
        };

        // Act
        var result = await _service.CreateSubscriptionAsync(_userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Netflix 4K");
        result.Amount.Should().Be(260_000m);
        result.MonthlyCost.Should().Be(260_000m);
        result.AnnualCost.Should().Be(3_120_000m);
        result.DaysUntilNextBilling.Should().Be(15);
    }

    [Fact]
    public async Task GetSubscriptions_FiltersByStatusCorrectly()
    {
        // Arrange
        await _service.CreateSubscriptionAsync(_userId, new CreateSubscriptionDto { Name = "Active 1", Amount = 100_000m, CategoryId = _categoryId, Status = SubscriptionStatus.Active });
        await _service.CreateSubscriptionAsync(_userId, new CreateSubscriptionDto { Name = "Active 2", Amount = 200_000m, CategoryId = _categoryId, Status = SubscriptionStatus.Active });
        await _service.CreateSubscriptionAsync(_userId, new CreateSubscriptionDto { Name = "Paused 1", Amount = 300_000m, CategoryId = _categoryId, Status = SubscriptionStatus.Paused });

        // Act
        var activeOnly = await _service.GetSubscriptionsAsync(_userId, SubscriptionStatus.Active);
        var all = await _service.GetSubscriptionsAsync(_userId);

        // Assert
        activeOnly.Should().HaveCount(2);
        all.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdateSubscription_UpdatesPropertiesAccurately()
    {
        // Arrange
        var created = await _service.CreateSubscriptionAsync(_userId, new CreateSubscriptionDto { Name = "Spotify Free", Amount = 59_000m, CategoryId = _categoryId });

        var updateReq = new UpdateSubscriptionDto
        {
            Name = "Spotify Duo",
            Amount = 79_000m,
            BillingCycle = SubscriptionBillingCycle.Monthly,
            NextBillingDate = DateTime.UtcNow.AddDays(20),
            CategoryId = _categoryId,
            Status = SubscriptionStatus.Active,
            AutoRenew = true,
            RemindDaysBefore = 5
        };

        // Act
        var updated = await _service.UpdateSubscriptionAsync(_userId, created.Id, updateReq);

        // Assert
        updated.Name.Should().Be("Spotify Duo");
        updated.Amount.Should().Be(79_000m);
        updated.RemindDaysBefore.Should().Be(5);
    }

    [Fact]
    public async Task ArchiveSubscription_MarksSubscriptionAsCancelled()
    {
        // Arrange
        var created = await _service.CreateSubscriptionAsync(_userId, new CreateSubscriptionDto { Name = "Adobe CC", Amount = 750_000m, CategoryId = _categoryId });

        // Act
        var res = await _service.ArchiveSubscriptionAsync(_userId, created.Id);

        // Assert
        res.Should().BeTrue();
        var fromDb = await _service.GetSubscriptionByIdAsync(_userId, created.Id);
        fromDb.Should().BeNull(); // Excluded by query filter due to soft-delete
    }

    [Fact]
    public async Task AuditSubscriptions_DetectsCatalogMatchFromTransactions()
    {
        // Arrange: Người dùng có 2 giao dịch Netflix trong lịch sử nhưng chưa quản lý
        var wallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 10_000_000m);
        _dbContext.Wallets.Add(wallet);

        var netflixCat = new Category("Xem phim", TransactionType.Expense, _userId);
        _dbContext.Categories.Add(netflixCat);

        var tx1 = new Transaction(_userId, wallet.Id, netflixCat.Id, 260_000m, TransactionType.Expense, DateTime.UtcNow.AddDays(-30), note: "Thanh toán gói Netflix");
        var tx2 = new Transaction(_userId, wallet.Id, netflixCat.Id, 260_000m, TransactionType.Expense, DateTime.UtcNow.AddDays(-2), note: "Gia hạn Netflix");
        _dbContext.Transactions.AddRange(tx1, tx2);
        await _dbContext.SaveChangesAsync();

        // Act
        var audit = await _service.AuditSubscriptionsAsync(_userId);

        // Assert
        audit.Should().NotBeNull();
        audit.DetectedCandidates.Should().Contain(c => c.ServiceName == "Netflix" && c.DetectionSource == "CatalogMatch");
        audit.FinancialHealthAlert.Should().Contain("Cảnh báo bẫy thuê bao");
    }

    [Fact]
    public async Task AuditSubscriptions_DetectsPeriodicClustering_WhenSameAmountRepeatsMonthly()
    {
        // Arrange: Người dùng trả tiền trọ 3.500.000đ lặp lại 2 tháng cách nhau 30 ngày
        var wallet = new Wallet(_userId, "Ví Ngân Hàng", WalletType.BankAccount, 50_000_000m);
        var rentCat = new Category("Tiền thuê nhà", TransactionType.Expense, _userId);
        _dbContext.Wallets.Add(wallet);
        _dbContext.Categories.Add(rentCat);

        var tx1 = new Transaction(_userId, wallet.Id, rentCat.Id, 3_500_000m, TransactionType.Expense, DateTime.UtcNow.AddDays(-60), note: "Tiền phòng trọ");
        var tx2 = new Transaction(_userId, wallet.Id, rentCat.Id, 3_500_000m, TransactionType.Expense, DateTime.UtcNow.AddDays(-30), note: "Tiền phòng trọ");
        _dbContext.Transactions.AddRange(tx1, tx2);
        await _dbContext.SaveChangesAsync();

        // Act
        var audit = await _service.AuditSubscriptionsAsync(_userId);

        // Assert
        audit.DetectedCandidates.Should().Contain(c => c.DetectionSource == "RecurringCluster" && c.SuggestedAmount == 3_500_000m);
    }

    [Fact]
    public async Task AuditSubscriptions_AlertsExpiringFreeTrials()
    {
        // Arrange: Gói dùng thử sẽ hết hạn sau 2 ngày
        await _service.CreateSubscriptionAsync(_userId, new CreateSubscriptionDto
        {
            Name = "Canva Pro Trial",
            Amount = 149_000m,
            CategoryId = _categoryId,
            Status = SubscriptionStatus.FreeTrial,
            TrialEndDate = DateTime.UtcNow.AddDays(2),
            NextBillingDate = DateTime.UtcNow.AddDays(2)
        });

        // Act
        var audit = await _service.AuditSubscriptionsAsync(_userId);

        // Assert
        audit.FreeTrialAlertsCount.Should().Be(1);
    }

    [Fact]
    public async Task GetUpcomingRenewals_ReturnsSubscriptionsDueSoon()
    {
        // Arrange
        await _service.CreateSubscriptionAsync(_userId, new CreateSubscriptionDto
        {
            Name = "Sắp đến hạn 3 ngày",
            Amount = 100_000m,
            CategoryId = _categoryId,
            NextBillingDate = DateTime.UtcNow.AddDays(3)
        });

        await _service.CreateSubscriptionAsync(_userId, new CreateSubscriptionDto
        {
            Name = "Còn xa 20 ngày",
            Amount = 200_000m,
            CategoryId = _categoryId,
            NextBillingDate = DateTime.UtcNow.AddDays(20)
        });

        // Act
        var upcoming = await _service.GetUpcomingRenewalsAsync(_userId, daysAhead: 7);

        // Assert
        upcoming.Should().HaveCount(1);
        upcoming[0].Name.Should().Be("Sắp đến hạn 3 ngày");
    }
}

