using System.Text.Json;
using ExpenseTracker.Maui.Models.Behavioral;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests;

public class NudgeFeedViewModelTests
{
    private readonly Mock<INudgeRepository> _mockRepository;
    private readonly NudgeFeedViewModel _viewModel;

    public NudgeFeedViewModelTests()
    {
        _mockRepository = new Mock<INudgeRepository>();
        _viewModel = new NudgeFeedViewModel(_mockRepository.Object);
    }

    [Fact]
    public async Task LoadNudges_WhenSuccessful_ShouldPopulateNudgesCollection()
    {
        // Arrange
        var nudges = new List<FinancialNudgeModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                NudgeType = NudgeType.DsaThreshold,
                Title = "Chạm trần DSA",
                Message = "Đã chi 88% hạn mức an toàn",
                Status = NudgeStatus.Active
            },
            new()
            {
                Id = Guid.NewGuid(),
                NudgeType = NudgeType.PayYourselfFirst,
                Title = "Tiết kiệm trước khi tiêu",
                Message = "Trích 20% lương vào hũ tích lũy",
                Status = NudgeStatus.Active
            }
        };

        _mockRepository.Setup(r => r.GetActiveNudgesAsync())
            .ReturnsAsync(nudges);

        // Act
        await _viewModel.LoadNudgesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsBusy.Should().BeFalse();
        _viewModel.Nudges.Should().HaveCount(2);
        _viewModel.Nudges[0].Title.Should().Be("Chạm trần DSA");
    }

    [Fact]
    public async Task ViewXaiRationale_WithValidJson_ShouldOpenModalDirectly()
    {
        // Arrange
        var xai = new XaiRationaleModel
        {
            Title = "Toán học XAI",
            MathematicalFormula = "DSA_t = Ngân_sách / Số_ngày",
            EmpiricalEvidence = "Chi tiêu 92%",
            ProjectedDailyDsaAfter = 200000m
        };

        var nudge = new FinancialNudgeModel
        {
            Id = Guid.NewGuid(),
            Title = "Cảnh báo hạn mức",
            RationaleExplanationJson = JsonSerializer.Serialize(xai)
        };

        // Act
        await _viewModel.ViewXaiRationaleCommand.ExecuteAsync(nudge);

        // Assert
        _viewModel.IsXaiModalVisible.Should().BeTrue();
        _viewModel.ActiveXaiRationale.Should().NotBeNull();
        _viewModel.ActiveXaiRationale!.MathematicalFormula.Should().Contain("DSA_t");
    }

    [Fact]
    public async Task ApproveNudge_WhenSuccessful_ShouldRemoveFromCollectionAndSetSuccessMessage()
    {
        // Arrange
        var nudge = new FinancialNudgeModel
        {
            Id = Guid.NewGuid(),
            Title = "Tái cân đối ngân sách",
            RebalanceProposalJson = "{}"
        };
        _viewModel.Nudges.Add(nudge);

        _mockRepository.Setup(r => r.ApproveNudgeAsync(nudge.Id))
            .ReturnsAsync(true);

        // Act
        await _viewModel.ApproveNudgeCommand.ExecuteAsync(nudge);

        // Assert
        _viewModel.Nudges.Should().NotContain(nudge);
        _viewModel.StatusMessage.Should().Contain("thành công");
    }

    [Fact]
    public async Task DismissNudge_WhenSuccessful_ShouldRemoveFromCollection()
    {
        // Arrange
        var nudge = new FinancialNudgeModel
        {
            Id = Guid.NewGuid(),
            Title = "Bỏ qua cảnh báo"
        };
        _viewModel.Nudges.Add(nudge);

        _mockRepository.Setup(r => r.DismissNudgeAsync(nudge.Id))
            .ReturnsAsync(true);

        // Act
        await _viewModel.DismissNudgeCommand.ExecuteAsync(nudge);

        // Assert
        _viewModel.Nudges.Should().BeEmpty();
    }
}
