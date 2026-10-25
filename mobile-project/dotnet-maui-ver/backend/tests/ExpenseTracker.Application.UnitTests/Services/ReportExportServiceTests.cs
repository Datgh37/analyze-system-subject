using ClosedXML.Excel;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests for ReportExportService (UC-07.01 Alternative Flow A1: Fast Raw Export vs AI Executive Report)
public class ReportExportServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ReportExportService _exportService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;
    private readonly Category _category;

    public ReportExportServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _exportService = new ReportExportService(_dbContext);

        _wallet = new Wallet(_userId, "Ví Chính", WalletType.BankAccount, 10_000_000m);
        _category = new Category("Tiền ăn", TransactionType.Expense, _userId);
        _dbContext.Wallets.Add(_wallet);
        _dbContext.Categories.Add(_category);

        var date = new DateTime(2026, 10, 15, 12, 0, 0, DateTimeKind.Utc);
        var expense = new Transaction(_userId, _wallet.Id, _category.Id, 2_000_000m, TransactionType.Expense, date, "Chi tiêu ăn uống");
        var income = new Transaction(_userId, _wallet.Id, _category.Id, 10_000_000m, TransactionType.Income, date, "Tiền lương tháng");
        _dbContext.Transactions.AddRange(expense, income);

        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task ExportRawTransactionsExcelAsync_ShouldGenerateValidExcelBytes()
    {
        // Act: Export Raw Data for October 2026
        var result = await _exportService.ExportRawTransactionsExcelAsync(_userId, 2026, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FileBytes.Should().NotBeNullOrEmpty();
        result.Value.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        result.Value.FileName.Should().Contain("BaoCao_TaiChinh_10_2026.xlsx");
    }

    [Fact]
    public async Task ExportSmartExecutiveReportAsync_WhenFallback_ShouldGenerateExcelWithDefaultAiAdvisorSummary()
    {
        // Act: Export Smart Executive Report for October 2026 using fallback template
        var result = await _exportService.ExportSmartExecutiveReportAsync(_userId, 2026, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FileBytes.Should().NotBeNullOrEmpty();
        result.Value.FileName.Should().Contain("BaoCao_CoVanAI_10_2026.xlsx");
    }

    [Fact]
    public async Task ExportSmartExecutiveReportAsync_WithGeminiAdvisorCommentary_ShouldEmbedAiCommentaryIntoExcel()
    {
        // Arrange
        const string mockCommentary = "Tháng 10/2026, Bạn đã xuất sắc duy trì tỷ lệ tiết kiệm 80%, vượt xa khuyến nghị vàng 20% của mô hình 50/30/20. Cú hích cho bạn: Hãy tự động hóa trích một phần thặng dư vào Quỹ tích lũy để bảo toàn dòng tiền.";

        var mockGemini = new Mock<IGeminiApiClient>();
        mockGemini.Setup(g => g.GenerateTextAsync(
            It.Is<string>(p => p.Contains("10/2026")),
            It.Is<string>(s => s.Contains("Richard Thaler")),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(mockCommentary));

        var service = new ReportExportService(_dbContext, mockGemini.Object);

        // Act
        var result = await service.ExportSmartExecutiveReportAsync(_userId, 2026, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FileBytes.Should().NotBeNullOrEmpty();

        // Inspect generated Excel binary to verify that AI commentary was properly embedded in Executive Box (A4)
        using var memoryStream = new MemoryStream(result.Value.FileBytes);
        using var workbook = new XLWorkbook(memoryStream);
        var aiSheet = workbook.Worksheet("⭐ TongQuan_CoVanAI");
        aiSheet.Should().NotBeNull();
        string execBoxValue = aiSheet.Cell("A4").GetString();
        execBoxValue.Should().Contain("Bạn đã xuất sắc duy trì tỷ lệ tiết kiệm 80%");
    }
}
