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
}
