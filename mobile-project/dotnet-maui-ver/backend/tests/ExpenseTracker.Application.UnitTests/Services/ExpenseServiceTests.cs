using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating expense ledger operations and ACID wallet debit constraints (UC-03.01 - 03.06)
public class ExpenseServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TransactionService _transactionService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;
    private readonly Category _category;

    public ExpenseServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _transactionService = new TransactionService(_dbContext);

        _wallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 500_000m);
        _dbContext.Wallets.Add(_wallet);

        _category = new Category("Ăn uống", TransactionType.Expense, _userId);
        _dbContext.Categories.Add(_category);

        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task CreateExpenseAsync_ShouldDebitWalletAndRecordTransaction()
    {
        // Arrange
        var request = new CreateExpenseTransactionRequest(
            WalletId: _wallet.Id,
            CategoryId: _category.Id,
            Amount: 150_000m,
            TransactionDate: DateTime.UtcNow,
            Note: "Ăn trưa bún bò",
            IsEssential: true
        );

        // Act
        var result = await _transactionService.CreateExpenseAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Amount.Should().Be(150_000m);
        result.Value.TransactionType.Should().Be(TransactionType.Expense);
        result.Value.IsEssential.Should().BeTrue();

        var updatedWallet = await _dbContext.Wallets.FirstAsync(w => w.Id == _wallet.Id);
        updatedWallet.Balance.Should().Be(350_000m); // 500k - 150k
    }

    [Fact]
    public async Task CreateExpenseAsync_WhenInsufficientBalance_ShouldFail()
    {
        // Arrange: Try to spend 600k when balance is 500k
        var request = new CreateExpenseTransactionRequest(
            WalletId: _wallet.Id,
            CategoryId: _category.Id,
            Amount: 600_000m,
            TransactionDate: DateTime.UtcNow
        );

        // Act
        var result = await _transactionService.CreateExpenseAsync(_userId, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Số dư khả dụng trong ví không đủ");

        var unchangedWallet = await _dbContext.Wallets.FirstAsync(w => w.Id == _wallet.Id);
        unchangedWallet.Balance.Should().Be(500_000m);
    }

    [Fact]
    public async Task CreateExpenseAsync_WithReceiptItems_ShouldSaveItemsCorrectly()
    {
        // Arrange
        var items = new List<CreateReceiptItemRequest>
        {
            new("Bánh mì", 2, 15000m, 30000m),
            new("Sữa chua", 1, 20000m, 20000m)
        };

        var request = new CreateExpenseTransactionRequest(
            WalletId: _wallet.Id,
            CategoryId: _category.Id,
            Amount: 50_000m,
            TransactionDate: DateTime.UtcNow,
            Items: items
        );

        // Act
        var result = await _transactionService.CreateExpenseAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var savedItems = await _dbContext.ReceiptItems
            .Where(r => r.TransactionId == result.Value!.Id)
            .ToListAsync();

        savedItems.Should().HaveCount(2);
        savedItems.Should().Contain(i => i.ItemName == "Bánh mì" && i.TotalPrice == 30000m);
    }

    [Fact]
    public async Task UpdateExpenseAsync_WhenAmountChanges_ShouldReconcileWalletBalance()
    {
        // Arrange: Spend 100k first (Balance becomes 400k)
        var createReq = new CreateExpenseTransactionRequest(_wallet.Id, _category.Id, 100_000m, DateTime.UtcNow);
        var created = await _transactionService.CreateExpenseAsync(_userId, createReq);

        // Act: Update to 150k (Delta = +50k, Balance should become 350k)
        var updateReq = new UpdateExpenseTransactionRequest(_wallet.Id, _category.Id, 150_000m, DateTime.UtcNow);
        var result = await _transactionService.UpdateExpenseAsync(_userId, created.Value!.Id, updateReq);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(150_000m);

        var updatedWallet = await _dbContext.Wallets.FirstAsync(w => w.Id == _wallet.Id);
        updatedWallet.Balance.Should().Be(350_000m);
    }

    [Fact]
    public async Task DeleteExpenseAsync_ShouldRevertSpentMoneyBackToWallet()
    {
        // Arrange: Spend 200k (Balance becomes 300k)
        var createReq = new CreateExpenseTransactionRequest(_wallet.Id, _category.Id, 200_000m, DateTime.UtcNow);
        var created = await _transactionService.CreateExpenseAsync(_userId, createReq);

        // Act: Delete transaction
        var result = await _transactionService.DeleteExpenseAsync(_userId, created.Value!.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var restoredWallet = await _dbContext.Wallets.FirstAsync(w => w.Id == _wallet.Id);
        restoredWallet.Balance.Should().Be(500_000m); // Reverted back to 500k
    }

    [Fact]
    public async Task GetExpenseDetailsAsync_ShouldReturnItemizedLines()
    {
        // Arrange
        var items = new List<CreateReceiptItemRequest>
        {
            new("Trà sen vàng", 2, 45000m, 90000m)
        };
        var createReq = new CreateExpenseTransactionRequest(_wallet.Id, _category.Id, 90_000m, DateTime.UtcNow, Note: "Highlands", Items: items);
        var created = await _transactionService.CreateExpenseAsync(_userId, createReq);

        // Act
        var result = await _transactionService.GetExpenseDetailsAsync(_userId, created.Value!.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().ContainSingle(i => i.ItemName == "Trà sen vàng" && i.TotalPrice == 90000m);
        result.Value.Note.Should().Be("Highlands");
    }
}
