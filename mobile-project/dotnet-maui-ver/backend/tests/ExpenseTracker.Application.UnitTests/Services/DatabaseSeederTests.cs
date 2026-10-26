using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating DatabaseSeeder logic, admin initialization, and default categories
public class DatabaseSeederTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public DatabaseSeederTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>()))
            .Returns("hashed_secure_admin_password");
    }

    [Fact]
    public async Task SeedDefaultCategoriesAsync_WhenDatabaseEmpty_Seeds14Categories()
    {
        // Act
        await DatabaseSeeder.SeedDefaultCategoriesAsync(_dbContext, NullLogger.Instance);

        // Assert
        var categories = await _dbContext.Categories.ToListAsync();
        categories.Should().NotBeEmpty();
        categories.Count.Should().Be(14);
        categories.Should().Contain(c => c.Name == "Ăn uống & Thực phẩm" && c.IsSystem);
        categories.Should().Contain(c => c.Name == "Tiền lương" && c.IsSystem);
    }

    [Fact]
    public async Task SeedDefaultCategoriesAsync_WhenAlreadySeeded_DoesNotDuplicate()
    {
        // Arrange
        await DatabaseSeeder.SeedDefaultCategoriesAsync(_dbContext, NullLogger.Instance);

        // Act
        await DatabaseSeeder.SeedDefaultCategoriesAsync(_dbContext, NullLogger.Instance);

        // Assert
        var count = await _dbContext.Categories.CountAsync();
        count.Should().Be(14);
    }

    [Fact]
    public async Task SeedAdminUserAsync_WhenAdminDoesNotExist_SeedsAdminWithWalletsAndLedger()
    {
        // Arrange
        await DatabaseSeeder.SeedDefaultCategoriesAsync(_dbContext, NullLogger.Instance);

        // Act
        var admin = await DatabaseSeeder.SeedAdminUserAsync(_dbContext, _passwordHasherMock.Object, NullLogger.Instance);

        // Assert
        admin.Should().NotBeNull();
        admin.Email.Should().Be(DatabaseSeeder.DefaultAdminEmail);
        admin.FullName.Should().Contain("Admin");

        // Verify Wallets
        var wallets = await _dbContext.Wallets.Where(w => w.UserId == admin.Id).ToListAsync();
        wallets.Should().HaveCount(2);
        wallets.Should().Contain(w => w.Name == "Ví Tiền Mặt");
        wallets.Should().Contain(w => w.Name == "Tài Khoản Vietcombank");

        // Verify Transactions
        var transactions = await _dbContext.Transactions.Where(t => t.UserId == admin.Id).ToListAsync();
        transactions.Should().HaveCount(3);

        // Verify Budgets
        var budgets = await _dbContext.Budgets.Where(b => b.UserId == admin.Id).ToListAsync();
        budgets.Should().HaveCount(1);
    }

    [Fact]
    public async Task SeedAdminUserAsync_WhenAdminAlreadyExists_ReturnsExistingAdminIdempotently()
    {
        // Arrange
        await DatabaseSeeder.SeedDefaultCategoriesAsync(_dbContext, NullLogger.Instance);
        var firstAdmin = await DatabaseSeeder.SeedAdminUserAsync(_dbContext, _passwordHasherMock.Object, NullLogger.Instance);

        // Act
        var secondAdmin = await DatabaseSeeder.SeedAdminUserAsync(_dbContext, _passwordHasherMock.Object, NullLogger.Instance);

        // Assert
        secondAdmin.Id.Should().Be(firstAdmin.Id);
        var userCount = await _dbContext.Users.CountAsync();
        userCount.Should().Be(1);
    }

    [Fact]
    public async Task SeedDemoStoryAsync_WhenCalled_CreatesUserAndAllDemoEntitiesWithValidData()
    {
        // Act
        var demoUser = await DatabaseSeeder.SeedDemoStoryAsync(_dbContext, _passwordHasherMock.Object, NullLogger.Instance);

        // Assert User
        demoUser.Should().NotBeNull();
        demoUser.Email.Should().Be(DatabaseSeeder.DefaultDemoEmail);
        demoUser.FullName.Should().Be("Trịnh Thành Đạt");

        // Assert Wallets (3 wallets)
        var wallets = await _dbContext.Wallets.Where(w => w.UserId == demoUser.Id).ToListAsync();
        wallets.Should().HaveCount(3);
        wallets.Should().Contain(w => w.Name == "Tài Khoản Vietcombank" && w.Balance == 15_500_000m);
        wallets.Should().Contain(w => w.Name == "Ví Tiền Mặt" && w.Balance == 1_200_000m);
        wallets.Should().Contain(w => w.Name == "Quỹ Khẩn Cấp (Techcombank)" && w.Balance == 10_000_000m);

        // Assert Transactions (10 transactions)
        var transactions = await _dbContext.Transactions.Where(t => t.UserId == demoUser.Id).ToListAsync();
        transactions.Should().HaveCount(10);
        transactions.Count(t => t.TransactionType == Domain.Enums.TransactionType.Income).Should().Be(2);
        transactions.Count(t => t.TransactionType == Domain.Enums.TransactionType.Expense).Should().Be(8);

        // Assert Budget & 85% Food Alert Threshold
        var budget = await _dbContext.Budgets
            .Include(b => b.Allocations)
            .FirstOrDefaultAsync(b => b.UserId == demoUser.Id);
        budget.Should().NotBeNull();
        budget!.TotalAmount.Should().Be(12_000_000m);

        var foodCategory = await _dbContext.Categories.FirstAsync(c => c.Name == "Ăn uống & Thực phẩm");
        var foodAllocation = budget.Allocations.FirstOrDefault(a => a.CategoryId == foodCategory.Id);
        foodAllocation.Should().NotBeNull();
        foodAllocation!.AllocatedAmount.Should().Be(3_000_000m);
        foodAllocation.CurrentSpent.Should().Be(2_550_000m); // exactly 85%
        foodAllocation.SpentPercent.Should().Be(85.0m);
        foodAllocation.IsAlertTriggered.Should().BeTrue();

        // Assert Receipt Items (WinMart 4 items matching 385,000 VND)
        var winmartTx = transactions.First(t => t.Note == "Đi siêu thị WinMart");
        var receiptItems = await _dbContext.ReceiptItems.Where(r => r.TransactionId == winmartTx.Id).ToListAsync();
        receiptItems.Should().HaveCount(4);
        receiptItems.Sum(r => r.TotalPrice).Should().Be(385_000m);

        // Assert Salary Slip Detail (Net = 22,690,000 VND, IsMathAudited = true)
        var salarySlip = await _dbContext.SalarySlipDetails.FirstOrDefaultAsync(s => s.UserId == demoUser.Id);
        salarySlip.Should().NotBeNull();
        salarySlip!.NetSalary.Should().Be(22_690_000m);
        salarySlip.GrossSalary.Should().Be(22_000_000m);
        salarySlip.IsMathAudited.Should().BeTrue();
        salarySlip.DiscrepancyAmount.Should().Be(0m);

        // Assert Savings Goals (Laptop 48%, Emergency 50%)
        var savingsGoals = await _dbContext.SavingsGoals.Where(g => g.UserId == demoUser.Id).ToListAsync();
        savingsGoals.Should().HaveCount(2);
        var laptopGoal = savingsGoals.First(g => g.Name == "Mua Laptop Mới");
        laptopGoal.CurrentAmount.Should().Be(12_000_000m);
        laptopGoal.TargetAmount.Should().Be(25_000_000m);
        laptopGoal.ProgressPercent.Should().Be(48.0m);

        // Assert Cooling-Off (Sony Headphone 4.5M)
        var coolingOffTx = await _dbContext.CoolingOffTransactions.FirstOrDefaultAsync(c => c.UserId == demoUser.Id);
        coolingOffTx.Should().NotBeNull();
        coolingOffTx!.Amount.Should().Be(4_500_000m);
        coolingOffTx.Description.Should().Contain("Sony");
        coolingOffTx.Status.Should().Be(Domain.Enums.CoolingOffStatus.PendingCoolingOff);

        // Assert Nudges & InApp Notifications
        var nudges = await _dbContext.FinancialNudges.Where(n => n.UserId == demoUser.Id).ToListAsync();
        nudges.Should().HaveCount(2);

        var notifications = await _dbContext.InAppNotifications.Where(n => n.UserId == demoUser.Id).ToListAsync();
        notifications.Should().HaveCount(3);

        // Assert Net Worth Snapshots (3 historical snapshots)
        var snapshots = await _dbContext.NetWorthSnapshots.Where(n => n.UserId == demoUser.Id).ToListAsync();
        snapshots.Should().HaveCount(3);
    }

    [Fact]
    public async Task SeedDemoStoryAsync_WhenCalledMultipleTimes_CleansOldDataAndReseedsIdempotently()
    {
        // Act: Run seed twice
        var firstRun = await DatabaseSeeder.SeedDemoStoryAsync(_dbContext, _passwordHasherMock.Object, NullLogger.Instance);
        var secondRun = await DatabaseSeeder.SeedDemoStoryAsync(_dbContext, _passwordHasherMock.Object, NullLogger.Instance);

        // Assert: Idempotent and clean state
        secondRun.Id.Should().Be(firstRun.Id);

        var userCount = await _dbContext.Users.CountAsync(u => u.Email == DatabaseSeeder.DefaultDemoEmail);
        userCount.Should().Be(1);

        var walletsCount = await _dbContext.Wallets.CountAsync(w => w.UserId == secondRun.Id);
        walletsCount.Should().Be(3);

        var transactionsCount = await _dbContext.Transactions.CountAsync(t => t.UserId == secondRun.Id);
        transactionsCount.Should().Be(10);

        var receiptItemsCount = await _dbContext.ReceiptItems.CountAsync(r => r.Transaction.UserId == secondRun.Id);
        receiptItemsCount.Should().Be(4);

        var savingsGoalsCount = await _dbContext.SavingsGoals.CountAsync(g => g.UserId == secondRun.Id);
        savingsGoalsCount.Should().Be(2);

        var coolingOffCount = await _dbContext.CoolingOffTransactions.CountAsync(c => c.UserId == secondRun.Id);
        coolingOffCount.Should().Be(1);

        var notificationsCount = await _dbContext.InAppNotifications.CountAsync(n => n.UserId == secondRun.Id);
        notificationsCount.Should().Be(3);
    }
}
