using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.DTOs.Autonomous;
using ExpenseTracker.Application.DTOs.Transactions;
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

// Unit tests validating Server-Driven Action Protocol & Jev Probabilistic Decision Gating (Chặng 10.5 / PLAN-47)
public class AutonomousActionEngineServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<IFinancialRealtimeNotifier> _realtimeNotifierMock;
    private readonly AutonomousActionEngineService _service;
    private readonly Guid _userId = Guid.NewGuid();

    public AutonomousActionEngineServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _transactionServiceMock = new Mock<ITransactionService>();
        _realtimeNotifierMock = new Mock<IFinancialRealtimeNotifier>();

        _service = new AutonomousActionEngineService(
            _dbContext,
            _transactionServiceMock.Object,
            _realtimeNotifierMock.Object
        );
    }

    [Fact]
    public async Task ProcessRawBankNotification_WhenHighConfidenceShopee_ShouldReturnQuickTransactionConfirmWithPrimaryQuickCommit()
    {
        // Arrange: Shopee notification with clear amount
        var request = new RawBankNotificationRequest(
            PackageName: "com.vnpay.acb",
            Title: "ACB Thông báo biến động số dư",
            Message: "TK 123456: -185,000 VND luc 23:15 tai Shopee Pay",
            NotificationTime: DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var action = result.Value!;

        action.ActionType.Should().Be(AutonomousActionTypes.QuickTransactionConfirm);
        action.ConfidenceScore.Should().BeGreaterThanOrEqualTo(0.85);
        action.Display.Title.Should().StartWith("Shopee:");
        action.Display.Title.Should().Contain("185");
        action.Display.BadgeText.Should().Be("Mua sắm");

        // Primary action MUST be Quick Commit 1-tap button
        action.PrimaryAction.Should().NotBeNull();
        action.PrimaryAction!.Label.Should().Be("Lưu nhanh");
        action.PrimaryAction.ActionKind.Should().Be(ActionKinds.QuickCommit);
        action.PrimaryAction.Endpoint.Should().Be("/api/actions/quick-commit");

        // Secondary action is Navigate
        action.SecondaryAction.Should().NotBeNull();
        action.SecondaryAction!.Label.Should().Be("Đổi");
        action.SecondaryAction.ActionKind.Should().Be(ActionKinds.Navigate);

        // Choices should be null for high confidence
        action.Choices.Should().BeNull();
    }

    [Theory]
    [InlineData("MoMo thanh toan GrabFood 45k", 45000, "Ăn uống")]
    [InlineData("ZaloPay: -120.000d tai WinMart", 120000, "Đi chợ / Thực phẩm")]
    [InlineData("Vietcombank: Giao dich -65k tai Highlands Coffee", 65000, "Ăn uống")]
    [InlineData("ACB: Tru 260.000 VND Netflix subscription", 260000, "Bẫy thuê bao")]
    public async Task ProcessRawBankNotification_WhenHighConfidenceKeywords_ShouldExtractAmountAndCategory(
        string message, decimal expectedAmount, string expectedCategory)
    {
        // Arrange
        var request = new RawBankNotificationRequest(
            PackageName: "com.mservice.momopay",
            Title: "Biến động số dư",
            Message: message,
            NotificationTime: DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.ConfidenceScore.Should().BeGreaterThanOrEqualTo(0.85);
        result.Value.Display.Amount.Should().Be(expectedAmount);
        result.Value.Display.BadgeText.Should().Be(expectedCategory);
        result.Value.PrimaryAction.Should().NotBeNull();
        result.Value.PrimaryAction!.Label.Should().Be("Lưu nhanh");
    }

    [Fact]
    public async Task ProcessRawBankNotification_WhenMediumConfidenceCircleK_ShouldGenerateJevChoiceDisambiguation()
    {
        // Arrange: Circle K convenience store (can be food or household items)
        var request = new RawBankNotificationRequest(
            PackageName: "com.vnpay.acb",
            Title: "ACB",
            Message: "GD: -35.000d tai Circle K Nguyen Trai",
            NotificationTime: DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var action = result.Value!;

        action.ActionType.Should().Be(AutonomousActionTypes.ChoiceDisambiguation);
        action.ConfidenceScore.Should().BeInRange(0.50, 0.84);
        action.Display.Amount.Should().Be(35000m);

        // Primary 1-tap action MUST be null to prevent premature assumption
        action.PrimaryAction.Should().BeNull();

        // Must offer calibrated Jev peer choices
        action.Choices.Should().NotBeNull();
        action.Choices.Should().HaveCount(2);
        action.Choices![0].Label.Should().Be("Ăn uống");
        action.Choices[0].FormattedProbability.Should().Be("60%");
        action.Choices[1].Label.Should().Be("Vật dụng sinh hoạt");
        action.Choices[1].FormattedProbability.Should().Be("40%");
    }

    [Fact]
    public async Task ProcessRawBankNotification_WhenPharmacy_ShouldGenerateJevChoiceDisambiguation()
    {
        // Arrange: Pharmacy can be Health or Personal Care
        var request = new RawBankNotificationRequest(
            PackageName: "com.vcb.digibank",
            Title: "VCB Digibank",
            Message: "Thanh toan 150.000 VND tai Nha thuoc Pharmacity",
            NotificationTime: DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.ActionType.Should().Be(AutonomousActionTypes.ChoiceDisambiguation);
        result.Value.Choices.Should().NotBeNull();
        result.Value.Choices.Should().Contain(c => c.Label == "Sức khỏe" && c.Probability == 0.75);
    }

    [Fact]
    public async Task ProcessRawBankNotification_WhenLowConfidenceAmbiguousMessage_ShouldReturnSilentNoticeWithNoPrimaryAction()
    {
        // Arrange: Unknown merchant / transfer
        var request = new RawBankNotificationRequest(
            PackageName: "com.mbmobile",
            Title: "MBBank",
            Message: "Chuyen tien -200,000 VND noi dung chuyen khoan ca nhan",
            NotificationTime: DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var action = result.Value!;

        action.ActionType.Should().Be(AutonomousActionTypes.UnclassifiedNotice);
        action.ConfidenceScore.Should().BeLessThan(0.50);
        action.Display.BadgeText.Should().Be("Chờ phân loại");
        action.PrimaryAction.Should().BeNull();
        action.Choices.Should().BeNull();
    }

    [Fact]
    public async Task ProcessRawBankNotification_WhenNoValidAmountInText_ShouldReturnFailure()
    {
        // Arrange: OTP SMS or notification without amount
        var request = new RawBankNotificationRequest(
            PackageName: "com.vnpay.acb",
            Title: "ACB OTP",
            Message: "Ma xac thuc OTP cua quy khach la 654321",
            NotificationTime: DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Không trích xuất được số tiền");
    }

    [Fact]
    public async Task ProcessRawBankNotification_WhenRealtimeNotifierProvided_ShouldSendInAppSignalR()
    {
        // Arrange
        var request = new RawBankNotificationRequest(
            PackageName: "com.vnpay.acb",
            Title: "ACB",
            Message: "TK 1234: -50.000 VND tai Shopee",
            NotificationTime: DateTime.UtcNow
        );

        // Act
        await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert: Realtime notifier was invoked
        _realtimeNotifierMock.Verify(n => n.NotifyInAppAsync(
            _userId,
            It.Is<InAppNotificationDto>(dto => dto.Type == "AUTONOMOUS_ACTION"),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetPendingActions_WhenActionsAdded_ShouldReturnInboxOrderedByCreatedAtDescending()
    {
        // Arrange
        var req1 = new RawBankNotificationRequest("com.vnpay.acb", "ACB", "Shopee -100k", NotificationTime: DateTime.UtcNow.AddMinutes(-5));
        var req2 = new RawBankNotificationRequest("com.vnpay.acb", "ACB", "GrabFood -50k", NotificationTime: DateTime.UtcNow);

        await _service.ProcessRawBankNotificationAsync(_userId, req1);
        await _service.ProcessRawBankNotificationAsync(_userId, req2);

        // Act
        var result = await _service.GetPendingActionsAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TotalCount.Should().BeGreaterThanOrEqualTo(2);
        result.Value.Actions[0].Display.Amount.Should().Be(50000m);
    }

    [Fact]
    public async Task CommitQuickAction_WhenValidRequest_ShouldCreateExpenseTransactionAndRemoveFromInbox()
    {
        // Arrange
        var wallet = new Wallet(_userId, "Ví ACB", WalletType.BankAccount, 5000000m);
        var category = new Category("Mua sắm", TransactionType.Expense, _userId);
        await _dbContext.Wallets.AddAsync(wallet);
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var notifRequest = new RawBankNotificationRequest(
            "com.vnpay.acb", "ACB", "TK 1234: -185,000 VND Shopee", NotificationTime: DateTime.UtcNow);
        var processResult = await _service.ProcessRawBankNotificationAsync(_userId, notifRequest);
        var actionId = processResult.Value!.ActionId;

        _transactionServiceMock
            .Setup(t => t.CreateExpenseAsync(_userId, It.IsAny<CreateExpenseTransactionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TransactionDto>.Success(new TransactionDto(
                Guid.NewGuid(), _userId, wallet.Id, wallet.Name, category.Id, category.Name,
                "cart", "#F59E0B", 185000m, TransactionType.Expense, DateTime.UtcNow,
                "Shopee", false, false, false, false, DateTime.UtcNow
            )));

        var commitRequest = new QuickCommitActionRequest(
            ActionId: actionId,
            CategoryName: "Mua sắm",
            Amount: 185000m,
            WalletName: "ACB",
            Note: "Shopee: 185.000 đ"
        );

        // Act
        var commitResult = await _service.CommitQuickActionAsync(_userId, commitRequest);

        // Assert
        commitResult.IsSuccess.Should().BeTrue();
        commitResult.Value.Should().NotBeNull();
        commitResult.Value.ActionType.Should().Be("COMMITTED");
        commitResult.Value.Display.Subtitle.Should().Contain("Đã lưu vào ví Ví ACB");

        // Verify transaction service was invoked with correct amount
        _transactionServiceMock.Verify(t => t.CreateExpenseAsync(
            _userId,
            It.Is<CreateExpenseTransactionRequest>(r => r.Amount == 185000m && r.WalletId == wallet.Id && r.CategoryId == category.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        // Verify action is removed from pending inbox
        var inboxResult = await _service.GetPendingActionsAsync(_userId);
        inboxResult.Value.Should().NotBeNull();
        inboxResult.Value.Actions.Should().NotContain(a => a.ActionId == actionId);
    }

    [Fact]
    public async Task DismissAction_WhenActionExists_ShouldRemoveActionFromInbox()
    {
        // Arrange
        var req = new RawBankNotificationRequest("com.vnpay.acb", "ACB", "Shopee -185k", NotificationTime: DateTime.UtcNow);
        var processResult = await _service.ProcessRawBankNotificationAsync(_userId, req);
        var actionId = processResult.Value!.ActionId;

        // Act
        var dismissResult = await _service.DismissActionAsync(_userId, actionId);

        // Assert
        dismissResult.IsSuccess.Should().BeTrue();
        dismissResult.Value.Should().BeTrue();

        var inboxResult = await _service.GetPendingActionsAsync(_userId);
        inboxResult.Value.Should().NotBeNull();
        inboxResult.Value.Actions.Should().NotContain(a => a.ActionId == actionId);
    }

    [Fact]
    public async Task ProcessRawBankNotification_WhenTechcombankHighlands_ShouldRecognizeTechcombankAndDining()
    {
        // Arrange
        var request = new RawBankNotificationRequest(
            PackageName: "com.techcombank.mobile",
            Title: "Techcombank Thông báo",
            Message: "GD: -120,000VND luc 14:15 tai Highlands Coffee. So du: 5,400,000VND",
            NotificationTime: DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var action = result.Value!;
        action.ActionType.Should().Be(AutonomousActionTypes.QuickTransactionConfirm);
        action.ConfidenceScore.Should().BeGreaterThanOrEqualTo(0.85);
        action.Display.Amount.Should().Be(120000m);
        action.Display.BadgeText.Should().Be("Ăn uống");
        action.Display.Subtitle.Should().Contain("Techcombank");
        action.PrimaryAction.Should().NotBeNull();
        action.PrimaryAction!.Payload!["walletName"].Should().Be("Techcombank");
    }

    [Fact]
    public async Task ProcessRawBankNotification_WhenBidvCoopmart_ShouldRecognizeBidvAndGroceries()
    {
        // Arrange
        var request = new RawBankNotificationRequest(
            PackageName: "com.vnpay.bidv",
            Title: "BIDV",
            Message: "TK 123456: -450,000 VND luc 17:00 tai Sieu thi Co.opmart",
            NotificationTime: DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var action = result.Value!;
        action.ConfidenceScore.Should().BeGreaterThanOrEqualTo(0.85);
        action.Display.Amount.Should().Be(450000m);
        action.Display.BadgeText.Should().Be("Đi chợ / Thực phẩm");
        action.Display.Subtitle.Should().Contain("BIDV");
        action.PrimaryAction!.Payload!["walletName"].Should().Be("BIDV");
    }

    [Theory]
    [InlineData("com.vpb.vpbankneo", "VPBank NEO", "VPBank: -250,000 VND tai CGV", "VPBank")]
    [InlineData("com.tpb.mb.gprsandroid", "TPBank", "GD -80,000 VND tai Phuc Long Coffee", "TPBank")]
    public async Task ProcessRawBankNotification_WhenVpBankAndTpBank_ShouldIdentifyBankName(
        string packageName, string title, string message, string expectedBankName)
    {
        // Arrange
        var request = new RawBankNotificationRequest(
            PackageName: packageName,
            Title: title,
            Message: message,
            NotificationTime: DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessRawBankNotificationAsync(_userId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Display.Subtitle.Should().Contain(expectedBankName);
        result.Value.PrimaryAction!.Payload!["walletName"].Should().Be(expectedBankName);
    }
}
