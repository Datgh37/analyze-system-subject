using ExpenseTracker.Maui.Models.AiAgent;
using ExpenseTracker.Maui.Repositories.AiAgent;
using ExpenseTracker.Maui.ViewModels.AiAgent;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests;

// Unit tests for VoiceFloatingMicViewModel (UC-03.11 Enhancement: Voice Intent Routing)
public class VoiceFloatingMicViewModelTests
{
    private readonly Mock<IVoiceAgentRepository> _mockVoiceRepo;
    private readonly VoiceFloatingMicViewModel _vm;

    public VoiceFloatingMicViewModelTests()
    {
        _mockVoiceRepo = new Mock<IVoiceAgentRepository>();
        _vm = new VoiceFloatingMicViewModel(_mockVoiceRepo.Object);
    }

    [Fact]
    public void StartListening_ShouldSetIsListeningTrueAndInstructions()
    {
        // Act
        _vm.StartListening();

        // Assert
        _vm.IsListening.Should().BeTrue();
        _vm.Transcript.Should().BeEmpty();
        _vm.FeedbackMessage.Should().Contain("Đang lắng nghe");
    }

    [Fact]
    public async Task ProcessTranscriptAsync_WhenValidTranscript_ShouldCallRepoAndSetResult()
    {
        // Arrange
        var intentResult = new VoiceIntentResultModel(
            DetectedIntent: "NavigateTo",
            ActionType: "Navigation",
            TargetRoute: "CreateCategoryPage",
            PrefillData: new Dictionary<string, string> { ["categoryName"] = "Nuôi mèo" },
            ExtractedAmount: null,
            SuggestedCategory: "Nuôi mèo",
            SuggestedWalletId: null,
            ConfirmationMessage: "Đang mở giao diện tạo danh mục mới: 'Nuôi mèo'",
            Confidence: 0.95
        );

        _mockVoiceRepo.Setup(r => r.ParseIntentAsync(It.IsAny<VoiceIntentCommandRequest>()))
                      .ReturnsAsync(intentResult);

        // Act
        await _vm.ProcessTranscriptAsync("Tạo danh mục mới Nuôi mèo");

        // Assert
        _vm.IsListening.Should().BeFalse();
        _vm.LastResult.Should().NotBeNull();
        _vm.LastResult!.DetectedIntent.Should().Be("NavigateTo");
        _vm.FeedbackMessage.Should().Contain("Nuôi mèo");
    }

    [Fact]
    public async Task ProcessTranscriptAsync_WhenEmpty_ShouldSetWarning()
    {
        // Act
        await _vm.ProcessTranscriptAsync("");

        // Assert
        _vm.FeedbackMessage.Should().Contain("Không nhận diện được giọng nói");
        _mockVoiceRepo.Verify(r => r.ParseIntentAsync(It.IsAny<VoiceIntentCommandRequest>()), Times.Never);
    }

    [Fact]
    public async Task ProcessTranscriptAsync_WhenExpenseTranscript_ShouldExtractAmountAndSetExpenseResult()
    {
        // Arrange
        var intentResult = new VoiceIntentResultModel(
            DetectedIntent: "AddTransaction",
            ActionType: "Expense",
            TargetRoute: "ExpensesPage",
            PrefillData: new Dictionary<string, string>
            {
                ["amount"] = "50000",
                ["note"] = "Chi 50k ăn trưa Highland",
                ["categoryName"] = "Ăn uống"
            },
            ExtractedAmount: 50000m,
            SuggestedCategory: "Ăn uống",
            SuggestedWalletId: null,
            ConfirmationMessage: "Nhận diện chi tiêu 50.000 đ cho 'Ăn uống'",
            Confidence: 0.95
        );

        _mockVoiceRepo.Setup(r => r.ParseIntentAsync(It.IsAny<VoiceIntentCommandRequest>()))
                      .ReturnsAsync(intentResult);

        // Act
        await _vm.ProcessTranscriptAsync("Chi 50k ăn trưa Highland");

        // Assert
        _vm.Transcript.Should().Be("Chi 50k ăn trưa Highland");
        _vm.LastResult.Should().NotBeNull();
        _vm.LastResult!.TargetRoute.Should().Be("ExpensesPage");
        _vm.LastResult.ExtractedAmount.Should().Be(50000m);
        _vm.LastResult.SuggestedCategory.Should().Be("Ăn uống");
        _vm.FeedbackMessage.Should().Contain("50.000");
    }

    [Theory]
    [InlineData("Chi 50k ăn trưa Highland", 50000, "Ăn uống", "ExpensesPage")]
    [InlineData("Uống trà sữa Phúc Long 65k", 65000, "Ăn uống", "ExpensesPage")]
    [InlineData("Đi grab hết 45k", 45000, "Đi lại", "ExpensesPage")]
    public async Task MockVoiceAgentRepository_WhenExpenseCommand_ShouldParseAmountAndCategoryCorrectly(
        string command, decimal expectedAmount, string expectedCategory, string expectedRoute)
    {
        // Arrange
        var repo = new MockVoiceAgentRepository();
        var request = new VoiceIntentCommandRequest(command);

        // Act
        var result = await repo.ParseIntentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TargetRoute.Should().Be(expectedRoute);
        result.ExtractedAmount.Should().Be(expectedAmount);
        result.SuggestedCategory.Should().Be(expectedCategory);
        result.DetectedIntent.Should().Be("AddTransaction");
    }

    [Theory]
    [InlineData("Xem báo cáo tài chính", "ReportsPage")]
    [InlineData("Thiết lập ngân sách", "BudgetsPage")]
    [InlineData("Tạo danh mục Nuôi mèo", "CategoriesPage")]
    [InlineData("Quét hóa đơn siêu thị", "ReceiptOcrPage")]
    [InlineData("Mở quản lý ví", "WalletsPage")]
    [InlineData("Xem quỹ khẩn cấp", "EmergencyFundPage")]
    [InlineData("Quét bẫy thuê bao", "SubscriptionsPage")]
    [InlineData("Thử mô phỏng mua xe", "ForecastingPage")]
    [InlineData("Trả nợ 2 triệu thẻ tín dụng", "DebtTrackerPage")]
    public async Task MockVoiceAgentRepository_WhenNavigationCommand_ShouldMapToCorrectTargetRoute(
        string command, string expectedRoute)
    {
        // Arrange
        var repo = new MockVoiceAgentRepository();
        var request = new VoiceIntentCommandRequest(command);

        // Act
        var result = await repo.ParseIntentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TargetRoute.Should().Be(expectedRoute);
    }

    [Fact]
    public async Task SendDirectTextInputAsync_WhenValidInput_ShouldTriggerProcessingAndClearInput()
    {
        // Arrange
        var intentResult = new VoiceIntentResultModel(
            DetectedIntent: "EmergencyFund",
            ActionType: "Navigation",
            TargetRoute: "EmergencyFundPage",
            PrefillData: new Dictionary<string, string>(),
            ExtractedAmount: null,
            SuggestedCategory: "Quỹ khẩn cấp",
            SuggestedWalletId: null,
            ConfirmationMessage: "Đang mở Báo cáo Quỹ Dự Phòng Khẩn Cấp",
            Confidence: 0.96
        );

        _mockVoiceRepo.Setup(r => r.ParseIntentAsync(It.IsAny<VoiceIntentCommandRequest>()))
                      .ReturnsAsync(intentResult);

        _vm.DirectTextInput = "Xem quỹ khẩn cấp của tôi";

        // Act
        await _vm.SendDirectTextInputAsync();

        // Assert
        _vm.DirectTextInput.Should().BeEmpty();
        _vm.Transcript.Should().Be("Xem quỹ khẩn cấp của tôi");
        _vm.LastResult.Should().NotBeNull();
        _vm.LastResult!.TargetRoute.Should().Be("EmergencyFundPage");
        _vm.FeedbackMessage.Should().Contain("Quỹ Dự Phòng Khẩn Cấp");
    }

    [Fact]
    public async Task StartListeningAsync_WhenCalled_ShouldSetListeningState()
    {
        // Act
        await _vm.StartListeningAsync();

        // Assert
        _vm.IsListening.Should().BeTrue();
        _vm.Transcript.Should().BeEmpty();
        _vm.FeedbackMessage.Should().Contain("Đang lắng nghe");
    }
}
