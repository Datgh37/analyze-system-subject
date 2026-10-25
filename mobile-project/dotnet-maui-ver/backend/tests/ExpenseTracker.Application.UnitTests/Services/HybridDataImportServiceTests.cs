using ClosedXML.Excel;
using ExpenseTracker.Application.DTOs.ImportExport;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests for HybridDataImportService (UC-07.06: Hybrid Multi-Source Data Import)
public class HybridDataImportServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AutoCategorizationService _autoRuleService;
    private readonly TransactionService _transactionService;
    private readonly HybridDataImportService _importService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;

    public HybridDataImportServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _autoRuleService = new AutoCategorizationService(_dbContext);
        _transactionService = new TransactionService(_dbContext);
        _importService = new HybridDataImportService(_dbContext, _autoRuleService, _transactionService);

        _wallet = new Wallet(_userId, "Ví Chi Tiêu", WalletType.Cash, 5_000_000m);
        _dbContext.Wallets.Add(_wallet);

        var foodCat = new Category("Ăn uống", TransactionType.Expense, _userId);
        var salaryCat = new Category("Lương", TransactionType.Income, _userId);
        _dbContext.Categories.AddRange(foodCat, salaryCat);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task ParseSpreadsheetImportAsync_WithValidExcelStream_ShouldParseRowsAccurately()
    {
        // Arrange: Generate an in-memory Excel workbook with headers & 2 transactions
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Sheet1");
        ws.Cell("A1").Value = "Ngày";
        ws.Cell("B1").Value = "Số tiền";
        ws.Cell("C1").Value = "Danh mục";
        ws.Cell("D1").Value = "Ghi chú";

        ws.Cell("A2").Value = DateTime.UtcNow.ToString("yyyy-MM-dd");
        ws.Cell("B2").Value = 45000;
        ws.Cell("C2").Value = "Ăn uống";
        ws.Cell("D2").Value = "Ăn trưa cơm tấm";

        ws.Cell("A3").Value = DateTime.UtcNow.ToString("yyyy-MM-dd");
        ws.Cell("B3").Value = 50000;
        ws.Cell("C3").Value = "Đi lại";
        ws.Cell("D3").Value = "Đổ xăng xe máy";

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        ms.Position = 0;

        // Act
        var result = await _importService.ParseSpreadsheetImportAsync(_userId, ms, "test_expenses.xlsx");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRows.Should().Be(2);
        result.Value.ValidRows.Should().Be(2);
        result.Value.TotalAmount.Should().Be(95000m);
        result.Value.Rows[0].Amount.Should().Be(45000m);
        result.Value.Rows[1].Amount.Should().Be(50000m);
    }

    [Fact]
    public async Task ParseUnstructuredTextImportAsync_WithDailyNotes_ShouldExtractRows()
    {
        // Arrange: Free-form text note lines
        string rawNotes = "15/10: Cà phê Highlands 45k\nĐổ xăng xe 50 ka; Tiền ăn trưa 40000";

        // Act
        var result = await _importService.ParseUnstructuredTextImportAsync(_userId, rawNotes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRows.Should().Be(3);
        result.Value.ValidRows.Should().Be(3);
        result.Value.TotalAmount.Should().Be(135000m);
    }

    [Fact]
    public async Task CommitBulkImportAsync_WithSelectedRows_ShouldDebitWalletAndPersist()
    {
        // Arrange
        var rowsToCommit = new List<ParsedImportRowDto>
        {
            new ParsedImportRowDto(1, DateTime.UtcNow, 50000m, "Expense", "Ăn uống", "Ăn sáng", null, true, null),
            new ParsedImportRowDto(2, DateTime.UtcNow, 30000m, "Expense", "Ăn uống", "Cà phê", null, true, null)
        };
        var req = new BulkCommitImportRequestDto(_wallet.Id, rowsToCommit);

        // Act
        var result = await _importService.CommitBulkImportAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SuccessCount.Should().Be(2);
        result.Value.TotalCommittedAmount.Should().Be(80000m);

        // Verify wallet balance debited by 80,000 (5,000,000 - 80,000 = 4,920,000)
        var updatedWallet = await _dbContext.Wallets.FindAsync(_wallet.Id);
        updatedWallet!.Balance.Should().Be(4_920_000m);
    }
}
