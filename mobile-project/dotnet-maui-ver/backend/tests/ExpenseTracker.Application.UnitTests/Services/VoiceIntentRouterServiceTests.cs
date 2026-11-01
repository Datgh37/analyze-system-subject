using ExpenseTracker.Application.DTOs.AiAgent;
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

// Unit tests for VoiceIntentRouterService (UC-03.11 Enhancement: Hybrid AI Voice Intent Agent)
public class VoiceIntentRouterServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AutoCategorizationService _autoRuleService;
    private readonly VoiceIntentRouterService _voiceRouter;
    private readonly Guid _userId = Guid.NewGuid();

    public VoiceIntentRouterServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _autoRuleService = new AutoCategorizationService(_dbContext);
        _voiceRouter = new VoiceIntentRouterService(_autoRuleService);

        var foodCat = new Category("Ăn uống", TransactionType.Expense, _userId);
        _dbContext.Categories.Add(foodCat);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task RouteVoiceCommand_WhenNavigateToCreateCategory_ShouldReturnNavigateIntentWithPrefill()
    {
        // Arrange: User says command to create category for pet
        var req = new VoiceIntentCommandRequest("Tôi muốn bạn tạo 1 mục chi tiêu mới về nuôi mèo");

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DetectedIntent.Should().Be(VoiceIntents.NavigateTo);
        result.Value.ActionType.Should().Be("Navigation");
        result.Value.TargetRoute.Should().Be("CreateCategoryPage");
        result.Value.PrefillData.Should().ContainKey("categoryName");
        result.Value.PrefillData["categoryName"].Should().Be("Nuôi mèo");
    }

    [Theory]
    [InlineData("Tôi vừa tiêu 50k cho việc đi chợ", 50000)]
    [InlineData("Ăn sáng hết 50 ka", 50000)]
    [InlineData("Khao Bạn năm xị tiền ăn lẩu", 500000)]
    [InlineData("Mua đồ gia dụng 2 trẹo rưỡi", 2500000)]
    [InlineData("Đổ xăng 100000 tiền mặt", 100000)]
    public async Task RouteVoiceCommand_WhenSpendingSlang_ShouldExtractCorrectAmount(string transcript, decimal expectedAmount)
    {
        // Arrange
        var req = new VoiceIntentCommandRequest(transcript);

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DetectedIntent.Should().Be(VoiceIntents.AddTransaction);
        result.Value.ActionType.Should().Be("Expense");
        result.Value.ExtractedAmount.Should().Be(expectedAmount);
        result.Value.TargetRoute.Should().Be("AddTransactionPage");
    }

    [Theory]
    [InlineData("Uống trà đá hết nửa lít", 50000)]
    [InlineData("Mua bánh mì 3 chục", 30000)]
    [InlineData("Góp quỹ hai lít đi picnic", 200000)]
    [InlineData("Ăn tối hết hai trăm ngàn", 200000)]
    public async Task RouteVoiceCommand_WhenSlangWithLitAndChuc_ShouldExtractCorrectAmount(string transcript, decimal expectedAmount)
    {
        // Arrange
        var req = new VoiceIntentCommandRequest(transcript);

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ExtractedAmount.Should().Be(expectedAmount);
    }

    [Theory]
    [InlineData("Mở ngân sách tháng này", "BudgetHubPage")]
    [InlineData("Xem sổ nợ cá nhân", "DebtTrackerPage")]
    [InlineData("Xem gợi ý cú hích", "NudgeFeedPage")]
    [InlineData("Mở không gian khoảng trễ", "CoolingOffHubPage")]
    [InlineData("Quét hóa đơn siêu thị", "ReceiptOcrPage")]
    public async Task RouteVoiceCommand_WhenAdvancedNavigationRoutes_ShouldNavigateCorrectly(string transcript, string expectedRoute)
    {
        // Arrange
        var req = new VoiceIntentCommandRequest(transcript);

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DetectedIntent.Should().Be(VoiceIntents.NavigateTo);
        result.Value.TargetRoute.Should().Be(expectedRoute);
    }

    [Fact]
    public async Task RouteVoiceCommand_WhenIncomeTranscript_ShouldReturnIncomeActionType()
    {
        // Arrange: User says income receipt
        var req = new VoiceIntentCommandRequest("Vừa nhận lương tháng này 15 triệu qua chuyển khoản");

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DetectedIntent.Should().Be(VoiceIntents.AddTransaction);
        result.Value.ActionType.Should().Be("Income");
        result.Value.ExtractedAmount.Should().Be(15_000_000m);
    }

    [Fact]
    public async Task RouteVoiceCommand_WhenGeminiNluResponse_ShouldParseComplexSemanticIntent()
    {
        // Arrange: Complex conversational transcript with wallet and contextual details
        const string transcript = "Hôm nay đi ăn tiệc liên hoan công ty hết 1 củ 2, ghi nhận chi tiêu ví Momo nhé";
        const string mockGeminiResponse = """
        {
          "detectedIntent": "AddTransaction",
          "actionType": "Expense",
          "targetRoute": "AddTransactionPage",
          "extractedAmount": 1200000,
          "suggestedCategory": "Ăn uống",
          "confirmationMessage": "Nhận diện: Chi tiêu 1,200,000 VND cho 'Ăn uống'",
          "confidence": 0.98,
          "prefillData": {
            "amount": "1200000",
            "categoryName": "Ăn uống",
            "type": "Expense",
            "note": "Hôm nay đi ăn tiệc liên hoan công ty hết 1 củ 2, ghi nhận chi tiêu ví Momo nhé"
          }
        }
        """;

        var mockGemini = new Mock<IGeminiApiClient>();
        mockGemini.Setup(g => g.GenerateTextAsync(
            It.Is<string>(p => p.Contains("1 củ 2")),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(mockGeminiResponse));

        var hybridRouter = new VoiceIntentRouterService(_autoRuleService, mockGemini.Object);
        var req = new VoiceIntentCommandRequest(transcript);

        // Act
        var result = await hybridRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DetectedIntent.Should().Be(VoiceIntents.AddTransaction);
        result.Value.ActionType.Should().Be("Expense");
        result.Value.ExtractedAmount.Should().Be(1_200_000m);
        result.Value.SuggestedCategory.Should().Be("Ăn uống");
        result.Value.Confidence.Should().Be(0.98);
        result.Value.PrefillData.Should().ContainKey("amount");
        result.Value.PrefillData["amount"].Should().Be("1200000");
    }

    [Fact]
    public async Task RouteVoiceCommand_WhenDebtPaymentWithAmount_ShouldReturnDebtPaymentIntent()
    {
        // Arrange: User says command to pay debt with slang amount
        var req = new VoiceIntentCommandRequest("Tôi vừa trả nợ 2 triệu thẻ tín dụng VIB");

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DetectedIntent.Should().Be(VoiceIntents.DebtPayment);
        result.Value.TargetRoute.Should().Be("DebtTrackerPage");
        result.Value.ExtractedAmount.Should().Be(2_000_000m);
        result.Value.PrefillData.Should().ContainKey("amount");
        result.Value.PrefillData["amount"].Should().Be("2000000");
        result.Value.ConfirmationMessage.Should().Contain("Nhận diện trả nợ");
    }

    [Fact]
    public async Task RouteVoiceCommand_WhenEmergencyFundCommand_ShouldReturnEmergencyFundIntent()
    {
        // Arrange
        var req = new VoiceIntentCommandRequest("Cho tôi xem quỹ khẩn cấp và chỉ số runway");

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DetectedIntent.Should().Be(VoiceIntents.EmergencyFund);
        result.Value.TargetRoute.Should().Be("EmergencyFundPage");
        result.Value.ConfirmationMessage.Should().Contain("Quỹ Dự Phòng Khẩn Cấp");
    }

    [Fact]
    public async Task RouteVoiceCommand_WhenSubscriptionAuditCommand_ShouldReturnSubscriptionAuditIntent()
    {
        // Arrange
        var req = new VoiceIntentCommandRequest("Kiểm tra và quét bẫy thuê bao định kỳ");

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DetectedIntent.Should().Be(VoiceIntents.SubscriptionAudit);
        result.Value.TargetRoute.Should().Be("SubscriptionsPage");
        result.Value.ConfirmationMessage.Should().Contain("Bẫy Thuê Bao");
    }

    [Fact]
    public async Task RouteVoiceCommand_WhenWhatIfSimulationCommand_ShouldReturnWhatIfSimulationIntent()
    {
        // Arrange
        var req = new VoiceIntentCommandRequest("Thử mô phỏng mua xe máy 30 triệu");

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DetectedIntent.Should().Be(VoiceIntents.WhatIfSimulation);
        result.Value.TargetRoute.Should().Be("ForecastingPage");
        result.Value.ExtractedAmount.Should().Be(30_000_000m);
        result.Value.ConfirmationMessage.Should().Contain("What-If");
    }

    [Fact]
    public async Task RouteVoiceCommand_WhenReceiptOcrCommand_ShouldReturnReceiptOcrRoute()
    {
        // Arrange
        var req = new VoiceIntentCommandRequest("Mở máy ảnh chụp hóa đơn siêu thị");

        // Act
        var result = await _voiceRouter.RouteVoiceCommandAsync(_userId, req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TargetRoute.Should().Be("ReceiptOcrPage");
        result.Value.ConfirmationMessage.Should().Contain("hóa đơn");
    }
}
