using ExpenseTracker.Maui.Models.Autonomous;
using ExpenseTracker.Maui.Repositories.Autonomous;
using ExpenseTracker.Maui.ViewModels.Autonomous;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests;

// Unit tests validating ActionInboxViewModel behavior (Server-Driven Action Protocol)
public class ActionInboxViewModelTests
{
    private readonly Mock<IAutonomousActionRepository> _mockRepo;
    private readonly ActionInboxViewModel _viewModel;

    public ActionInboxViewModelTests()
    {
        _mockRepo = new Mock<IAutonomousActionRepository>();
        _viewModel = new ActionInboxViewModel(_mockRepo.Object);
    }

    [Fact]
    public async Task LoadInboxAsync_WhenActionsExist_ShouldPopulatePendingActionsAndSetCount()
    {
        // Arrange
        var mockActions = new List<AutonomousActionModel>
        {
            new(
                ActionId: "act_01",
                ActionType: AutonomousActionTypeConstants.QuickTransactionConfirm,
                ConfidenceScore: 0.92,
                Display: new ActionDisplayModel("Shopee: 185k", "ACB • 23:15", "Mua sắm", "#F59E0B", 185000m),
                PrimaryAction: new ActionCommandModel("Lưu nhanh", ActionKindConstants.QuickCommit)
            )
        };

        _mockRepo.Setup(r => r.GetInboxAsync())
            .ReturnsAsync(new AutonomousActionInboxResponseModel(1, mockActions));

        // Act
        await _viewModel.LoadInboxAsync();

        // Assert
        _viewModel.PendingActions.Should().HaveCount(1);
        _viewModel.ActionCount.Should().Be(1);
        _viewModel.HasPendingActions.Should().BeTrue();
    }

    [Fact]
    public async Task QuickCommitAsync_WhenInvoked_ShouldCallRepositoryAndRemoveAction()
    {
        // Arrange
        var action = new AutonomousActionModel(
            ActionId: "act_shopee",
            ActionType: AutonomousActionTypeConstants.QuickTransactionConfirm,
            ConfidenceScore: 0.95,
            Display: new ActionDisplayModel("Shopee: 185k", "ACB", "Mua sắm", "#F59E0B", 185000m),
            PrimaryAction: new ActionCommandModel("Lưu nhanh", ActionKindConstants.QuickCommit)
        );

        _viewModel.PendingActions.Add(action);

        _mockRepo.Setup(r => r.QuickCommitAsync(It.IsAny<QuickCommitActionRequestModel>()))
            .ReturnsAsync(action with { ActionType = AutonomousActionTypeConstants.Committed });

        // Act
        await _viewModel.QuickCommitAsync(action);

        // Assert
        _viewModel.PendingActions.Should().NotContain(action);
        _viewModel.HasPendingActions.Should().BeFalse();
        _viewModel.ActionStatusMessage.Should().Contain("185.000 đ");
    }

    [Fact]
    public async Task DismissAsync_WhenInvoked_ShouldRemoveActionFromPendingList()
    {
        // Arrange
        var action = new AutonomousActionModel(
            ActionId: "act_dismiss_me",
            ActionType: AutonomousActionTypeConstants.UnclassifiedNotice,
            ConfidenceScore: 0.40,
            Display: new ActionDisplayModel("Chi tiêu 50k", "ACB", "Chờ phân loại", "#6B7280", 50000m)
        );

        _viewModel.PendingActions.Add(action);
        _mockRepo.Setup(r => r.DismissActionAsync("act_dismiss_me")).ReturnsAsync(true);

        // Act
        await _viewModel.DismissAsync(action);

        // Assert
        _viewModel.PendingActions.Should().BeEmpty();
        _viewModel.HasPendingActions.Should().BeFalse();
    }

    [Fact]
    public async Task SelectChoiceAsync_WhenChoiceSelected_ShouldCommitWithChosenCategory()
    {
        // Arrange
        var choice = new ActionChoiceModel("c1", "Ăn uống", 0.60, "60%", new Dictionary<string, string> { ["categoryName"] = "Ăn uống" });
        var action = new AutonomousActionModel(
            ActionId: "act_circlek",
            ActionType: AutonomousActionTypeConstants.ChoiceDisambiguation,
            ConfidenceScore: 0.65,
            Display: new ActionDisplayModel("Circle K: 35k", "MoMo", "Cửa hàng tiện lợi", "#3B82F6", 35000m),
            Choices: new List<ActionChoiceModel> { choice }
        );

        _viewModel.PendingActions.Add(action);

        _mockRepo.Setup(r => r.QuickCommitAsync(It.IsAny<QuickCommitActionRequestModel>()))
            .ReturnsAsync(action with { ActionType = AutonomousActionTypeConstants.Committed });

        // Act
        await _viewModel.SelectChoiceAsync(choice);

        // Assert
        _viewModel.PendingActions.Should().BeEmpty();
        _viewModel.ActionStatusMessage.Should().Contain("Ăn uống");
        _mockRepo.Verify(r => r.QuickCommitAsync(It.Is<QuickCommitActionRequestModel>(req => req.CategoryName == "Ăn uống")), Times.Once);
    }
}
