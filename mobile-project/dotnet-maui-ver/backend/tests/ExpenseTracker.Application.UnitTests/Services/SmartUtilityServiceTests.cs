using ExpenseTracker.Application.DTOs.SmartUtilities;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating Subscription Trap Detector, VietQR Napas 247, and Voice STT Parser (UC-03.09 - 03.11)
public class SmartUtilityServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AutoCategorizationService _autoRuleService;
    private readonly SubscriptionDetectorService _subDetectorService;
    private readonly VietQrService _vietQrService;
    private readonly VoiceExpenseParserService _voiceParserService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _cashWallet;
    private readonly Category _entertainmentCat;

    public SmartUtilityServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _autoRuleService = new AutoCategorizationService(_dbContext);
        _subDetectorService = new SubscriptionDetectorService(_dbContext);
        _vietQrService = new VietQrService();
        _voiceParserService = new VoiceExpenseParserService(_dbContext, _autoRuleService);

        _cashWallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 1_000_000m);
        _dbContext.Wallets.Add(_cashWallet);

        _entertainmentCat = new Category("Dịch vụ giải trí", TransactionType.Expense, _userId);
        _dbContext.Categories.Add(_entertainmentCat);

        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task DetectSubscriptionsAsync_ShouldIdentifyNetflixAndCalculateAnnualDrain()
    {
        // Arrange: User has two monthly payments for Netflix
        var tx1 = new Transaction(_userId, _cashWallet.Id, _entertainmentCat.Id, 260_000m, TransactionType.Expense, DateTime.UtcNow.AddMonths(-1), note: "Thuê bao Netflix Premium");
        var tx2 = new Transaction(_userId, _cashWallet.Id, _entertainmentCat.Id, 260_000m, TransactionType.Expense, DateTime.UtcNow, note: "Thanh toán gói Netflix");
        await _dbContext.Transactions.AddRangeAsync(tx1, tx2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _subDetectorService.DetectSubscriptionsAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Subscriptions.Should().ContainSingle(s => s.ServiceName == "Netflix");

        var netflix = result.Value.Subscriptions.First(s => s.ServiceName == "Netflix");
        netflix.RecurringAmount.Should().Be(260_000m);
        netflix.EstimatedAnnualCost.Should().Be(3_120_000m); // 260k * 12
        netflix.AdviceMessage.Should().Contain("Cú hích Thaler");
        result.Value.TotalAnnualDrain.Should().Be(3_120_000m);
    }

    [Fact]
    public async Task GenerateVietQrAsync_ShouldGenerateValidUrlAndPayload()
    {
        // Arrange
        var request = new GenerateVietQrRequest(
            BankBin: "970422", // MBBank
            AccountNumber: "0987654321",
            AccountName: "NGUYEN VAN A",
            Amount: 150_000m,
            Note: "Tien an trua"
        );

        // Act
        var result = await _vietQrService.GenerateVietQrAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.BankName.Should().Contain("MBBank");
        result.Value.QrImageUrl.Should().Contain("img.vietqr.io/image/970422-0987654321-compact2.png");
        result.Value.QrImageUrl.Should().Contain("amount=150000");
        result.Value.PayloadString.Should().Contain("00020101021238540010A00000072701240006970422");
    }

    [Fact]
    public async Task ParseVoiceExpenseAsync_ShouldExtractAmountAndWalletFromSentence()
    {
        // Arrange: Sentence: "Ăn trưa bún bò 45 nghìn ví tiền mặt"
        var request = new ParseVoiceExpenseRequest("Ăn trưa bún bò 45 nghìn ví tiền mặt");

        // Act
        var result = await _voiceParserService.ParseVoiceExpenseAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Amount.Should().Be(45_000m);
        result.Value.SuggestedWalletId.Should().Be(_cashWallet.Id);
        result.Value.Note.Should().Contain("Ăn trưa bún bò");
    }

    [Fact]
    public async Task ParseVoiceExpenseAsync_WhenMillionSpoken_ShouldExtractMillion()
    {
        // Arrange: Sentence: "Mua tivi mới 12.5 triệu"
        var request = new ParseVoiceExpenseRequest("Mua tivi mới 12.5 triệu");

        // Act
        var result = await _voiceParserService.ParseVoiceExpenseAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(12_500_000m);
    }
}
