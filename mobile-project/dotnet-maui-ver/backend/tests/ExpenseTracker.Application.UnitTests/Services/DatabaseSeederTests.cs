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
}
