using ExpenseTracker.Application.DTOs.RecurringIncomes;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating RecurringIncome lifecycle, validation rules, and automated batch transaction generation (UC-02.07)
public class RecurringIncomeServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RecurringIncomeService _service;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;
    private readonly Category _incomeCategory;
    private readonly Category _expenseCategory;

    public RecurringIncomeServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _service = new RecurringIncomeService(_dbContext);

        _wallet = new Wallet(_userId, "Ví Lương MB Bank", WalletType.BankAccount, 5_000_000m);
        _incomeCategory = new Category("Tiền lương & Thưởng", TransactionType.Income, isSystem: true);
        _expenseCategory = new Category("Ăn uống", TransactionType.Expense, isSystem: true);

        _dbContext.Wallets.Add(_wallet);
        _dbContext.Categories.AddRange(_incomeCategory, _expenseCategory);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task CreateRecurringIncomeAsync_WithValidInput_ShouldCreateAndReturnDto()
    {
        // Arrange
        var request = new CreateRecurringIncomeRequest(
            Name: "Lương Công Ty A",
            ExpectedAmount: 20_000_000m,
            WalletId: _wallet.Id,
            CategoryId: _incomeCategory.Id,
            Frequency: RecurringFrequency.Monthly,
            DayOfMonth: 10,
            AutoRecord: true
        );

        // Act
        var result = await _service.CreateRecurringIncomeAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Lương Công Ty A");
        result.Value.ExpectedAmount.Should().Be(20_000_000m);
        result.Value.WalletName.Should().Be("Ví Lương MB Bank");
        result.Value.CategoryName.Should().Be("Tiền lương & Thưởng");
        result.Value.IsActive.Should().BeTrue();
        result.Value.AutoRecord.Should().BeTrue();

        var dbItem = await _dbContext.RecurringIncomes.FindAsync(result.Value.Id);
        dbItem.Should().NotBeNull();
        dbItem!.ExpectedAmount.Should().Be(20_000_000m);
    }

    [Fact]
    public async Task CreateRecurringIncomeAsync_WithInvalidWallet_ShouldReturnFailure()
    {
        // Arrange
        var request = new CreateRecurringIncomeRequest(
            Name: "Lương Công Ty",
            ExpectedAmount: 20_000_000m,
            WalletId: Guid.NewGuid(), // Non-existent wallet
            CategoryId: _incomeCategory.Id,
            Frequency: RecurringFrequency.Monthly,
            DayOfMonth: 5
        );

        // Act
        var result = await _service.CreateRecurringIncomeAsync(_userId, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Ví nhận tiền không tồn tại");
    }

    [Fact]
    public async Task CreateRecurringIncomeAsync_WithExpenseCategory_ShouldReturnFailure()
    {
        // Arrange
        var request = new CreateRecurringIncomeRequest(
            Name: "Lương Công Ty",
            ExpectedAmount: 20_000_000m,
            WalletId: _wallet.Id,
            CategoryId: _expenseCategory.Id, // Expense category
            Frequency: RecurringFrequency.Monthly,
            DayOfMonth: 5
        );

        // Act
        var result = await _service.CreateRecurringIncomeAsync(_userId, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Danh mục đã chọn không thuộc loại Thu nhập");
    }

    [Fact]
    public async Task UpdateRecurringIncomeAsync_ShouldUpdateFieldsAndRecalculateDue()
    {
        // Arrange
        var createReq = new CreateRecurringIncomeRequest(
            "Tiền nhà trọ", 10_000_000m, _wallet.Id, _incomeCategory.Id, RecurringFrequency.Monthly, 1);
        var created = await _service.CreateRecurringIncomeAsync(_userId, createReq);

        var updateReq = new UpdateRecurringIncomeRequest(
            "Tiền nhà trọ đã tăng", 12_000_000m, _wallet.Id, _incomeCategory.Id, RecurringFrequency.Monthly, 15, false);

        // Act
        var result = await _service.UpdateRecurringIncomeAsync(_userId, created.Value!.Id, updateReq);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Tiền nhà trọ đã tăng");
        result.Value.ExpectedAmount.Should().Be(12_000_000m);
        result.Value.DayOfMonth.Should().Be(15);
        result.Value.AutoRecord.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleRecurringIncomeStatusAsync_ShouldToggleActiveStatus()
    {
        // Arrange
        var createReq = new CreateRecurringIncomeRequest(
            "Cổ tức cổ phiếu", 3_000_000m, _wallet.Id, _incomeCategory.Id, RecurringFrequency.Quarterly, 20);
        var created = await _service.CreateRecurringIncomeAsync(_userId, createReq);
        created.Value!.IsActive.Should().BeTrue();

        // Act: Toggle to false
        var toggleOffResult = await _service.ToggleRecurringIncomeStatusAsync(_userId, created.Value.Id);
        toggleOffResult.IsSuccess.Should().BeTrue();
        toggleOffResult.Value!.IsActive.Should().BeFalse();

        // Act: Toggle back to true
        var toggleOnResult = await _service.ToggleRecurringIncomeStatusAsync(_userId, created.Value.Id);
        toggleOnResult.IsSuccess.Should().BeTrue();
        toggleOnResult.Value!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRecurringIncomeAsync_ShouldSoftDelete()
    {
        // Arrange
        var createReq = new CreateRecurringIncomeRequest(
            "Freelance", 5_000_000m, _wallet.Id, _incomeCategory.Id, RecurringFrequency.Monthly, 1);
        var created = await _service.CreateRecurringIncomeAsync(_userId, createReq);

        // Act
        var result = await _service.DeleteRecurringIncomeAsync(_userId, created.Value!.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var deletedInDb = await _dbContext.RecurringIncomes.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == created.Value.Id);
        deletedInDb.Should().NotBeNull();
        deletedInDb!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task ProcessDueRecurringIncomesAsync_WhenDueIncomesExist_ShouldGenerateTransactionsAndCreditWallet()
    {
        // Arrange: Start with initial balance 5,000,000
        var baseDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var income = new RecurringIncome(
            _userId,
            "Lương định kỳ",
            15_000_000m,
            _wallet.Id,
            _incomeCategory.Id,
            RecurringFrequency.Monthly,
            dayOfMonth: 5,
            autoRecord: true,
            startDate: baseDate
        );
        // Due date is set to 2026-10-05
        await _dbContext.RecurringIncomes.AddAsync(income);
        await _dbContext.SaveChangesAsync();

        // Act: Process on 2026-10-06 (due date 2026-10-05 has arrived)
        var asOfDate = new DateTime(2026, 10, 6, 0, 0, 0, DateTimeKind.Utc);
        var result = await _service.ProcessDueRecurringIncomesAsync(_userId, asOfDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ProcessedCount.Should().Be(1);
        result.Value.TotalAmountRecorded.Should().Be(15_000_000m);
        result.Value.GeneratedTransactionIds.Should().HaveCount(1);

        // Verify destination wallet balance is credited: 5M + 15M = 20M
        var updatedWallet = await _dbContext.Wallets.FindAsync(_wallet.Id);
        updatedWallet!.Balance.Should().Be(20_000_000m);

        // Verify transaction is created in ledger
        var tx = await _dbContext.Transactions.FindAsync(result.Value.GeneratedTransactionIds[0]);
        tx.Should().NotBeNull();
        tx!.Amount.Should().Be(15_000_000m);
        tx.TransactionType.Should().Be(TransactionType.Income);
        tx.IsRecurring.Should().BeTrue();
        tx.Note.Should().Be("[Tự động] Lương định kỳ");

        // Verify income due date advanced to next month (2026-11-05)
        var updatedIncome = await _dbContext.RecurringIncomes.FindAsync(income.Id);
        updatedIncome!.NextDueDate.Should().Be(new DateTime(2026, 11, 5, 0, 0, 0, DateTimeKind.Utc));
        updatedIncome.LastGeneratedDate.Should().Be(new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task ProcessDueRecurringIncomesAsync_WhenNotDueYet_ShouldNotProcess()
    {
        // Arrange: Due date is 2026-10-25
        var baseDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var income = new RecurringIncome(
            _userId,
            "Lương ngày 25",
            10_000_000m,
            _wallet.Id,
            _incomeCategory.Id,
            RecurringFrequency.Monthly,
            dayOfMonth: 25,
            autoRecord: true,
            startDate: baseDate
        );
        await _dbContext.RecurringIncomes.AddAsync(income);
        await _dbContext.SaveChangesAsync();

        // Act: Process on 2026-10-15 (not due yet)
        var asOfDate = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        var result = await _service.ProcessDueRecurringIncomesAsync(_userId, asOfDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ProcessedCount.Should().Be(0);

        // Wallet balance remains unchanged
        var wallet = await _dbContext.Wallets.FindAsync(_wallet.Id);
        wallet!.Balance.Should().Be(5_000_000m);
    }

    [Fact]
    public async Task ProcessDueRecurringIncomesAsync_WhenAutoRecordIsFalse_ShouldSkip()
    {
        // Arrange: Due date is arrived, but AutoRecord = false
        var baseDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var income = new RecurringIncome(
            _userId,
            "Thưởng doanh số thủ công",
            5_000_000m,
            _wallet.Id,
            _incomeCategory.Id,
            RecurringFrequency.Monthly,
            dayOfMonth: 5,
            autoRecord: false,
            startDate: baseDate
        );
        await _dbContext.RecurringIncomes.AddAsync(income);
        await _dbContext.SaveChangesAsync();

        // Act: Process on 2026-10-06
        var asOfDate = new DateTime(2026, 10, 6, 0, 0, 0, DateTimeKind.Utc);
        var result = await _service.ProcessDueRecurringIncomesAsync(_userId, asOfDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ProcessedCount.Should().Be(0);
    }
}
