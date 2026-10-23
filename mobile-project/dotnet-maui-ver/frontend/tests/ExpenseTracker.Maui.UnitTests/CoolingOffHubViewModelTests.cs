using ExpenseTracker.Maui.Models.Behavioral;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests;

public class CoolingOffHubViewModelTests
{
    private readonly Mock<ICoolingOffRepository> _mockRepository;
    private readonly CoolingOffHubViewModel _viewModel;

    public CoolingOffHubViewModelTests()
    {
        _mockRepository = new Mock<ICoolingOffRepository>();
        _viewModel = new CoolingOffHubViewModel(_mockRepository.Object);
    }

    [Fact]
    public async Task LoadPendingTransactions_WhenSuccessful_ShouldPopulateCollection()
    {
        // Arrange
        var items = new List<CoolingOffTransactionModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Amount = 4_500_000m,
                Description = "Tai nghe Sony",
                Status = CoolingOffStatus.PendingCoolingOff,
                CoolingOffUntil = DateTime.UtcNow.AddHours(20)
            }
        };

        _mockRepository.Setup(r => r.GetPendingCoolingOffAsync())
            .ReturnsAsync(items);

        // Act
        await _viewModel.LoadPendingTransactionsCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsBusy.Should().BeFalse();
        _viewModel.PendingTransactions.Should().HaveCount(1);
        _viewModel.PendingTransactions[0].Amount.Should().Be(4_500_000m);
    }

    [Fact]
    public async Task EvaluateTestExpense_WithValidAmount_ShouldOpenModalWithEvaluation()
    {
        // Arrange
        var evalResult = new CoolingOffEvaluationResultModel
        {
            RequiresCoolingOff = true,
            ThresholdAmount = 3_000_000m,
            CooldownHours = 24,
            EquivalentWorkHours = 48m,
            ImpactAnalysisMessage = "Khoản chi yêu cầu 24h hạ nhiệt"
        };

        _mockRepository.Setup(r => r.EvaluateExpenseAsync(It.IsAny<CoolingOffEvaluationRequestModel>()))
            .ReturnsAsync(evalResult);

        _viewModel.TestExpenseAmount = 4_500_000m;
        _viewModel.TestDescription = "Smartwatch";

        // Act
        await _viewModel.EvaluateTestExpenseCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsEvaluateModalVisible.Should().BeTrue();
        _viewModel.EvaluationResult.Should().NotBeNull();
        _viewModel.EvaluationResult!.EquivalentWorkHours.Should().Be(48m);
    }

    [Fact]
    public async Task ConfirmPurchase_BeforeCooldownExpires_ShouldSetErrorMessage()
    {
        // Arrange: Cooldown in future
        var item = new CoolingOffTransactionModel
        {
            Id = Guid.NewGuid(),
            Amount = 4_000_000m,
            CoolingOffUntil = DateTime.UtcNow.AddHours(12)
        };
        _viewModel.PendingTransactions.Add(item);

        // Act
        await _viewModel.ConfirmPurchaseCommand.ExecuteAsync(item);

        // Assert
        _viewModel.ErrorMessage.Should().Contain("chưa hết thời gian");
        _viewModel.PendingTransactions.Should().Contain(item);
    }

    [Fact]
    public async Task ConfirmPurchase_WhenExpired_ShouldRemoveItemAndSetSuccess()
    {
        // Arrange: Cooldown expired in past
        var item = new CoolingOffTransactionModel
        {
            Id = Guid.NewGuid(),
            Amount = 4_000_000m,
            CoolingOffUntil = DateTime.UtcNow.AddMinutes(-5)
        };
        _viewModel.PendingTransactions.Add(item);

        _mockRepository.Setup(r => r.ConfirmPurchaseAsync(item.Id))
            .ReturnsAsync(true);

        // Act
        await _viewModel.ConfirmPurchaseCommand.ExecuteAsync(item);

        // Assert
        _viewModel.PendingTransactions.Should().NotContain(item);
        _viewModel.StatusMessage.Should().Contain("Đã xác nhận mua");
    }

    [Fact]
    public async Task CancelPurchase_ShouldRemoveItemAndCelebrateSavingMoney()
    {
        // Arrange
        var item = new CoolingOffTransactionModel
        {
            Id = Guid.NewGuid(),
            Amount = 3_500_000m,
            EquivalentWorkHours = 35m,
            CoolingOffUntil = DateTime.UtcNow.AddHours(15)
        };
        _viewModel.PendingTransactions.Add(item);

        _mockRepository.Setup(r => r.CancelPurchaseAsync(item.Id))
            .ReturnsAsync(true);

        // Act
        await _viewModel.CancelPurchaseCommand.ExecuteAsync(item);

        // Assert
        _viewModel.PendingTransactions.Should().NotContain(item);
        _viewModel.StatusMessage.Should().Contain("CHÚC MỪNG");
    }

    [Fact]
    public async Task SubmitBypass_WithValidReason_ShouldCallRepositoryAndRemoveItem()
    {
        // Arrange
        var item = new CoolingOffTransactionModel
        {
            Id = Guid.NewGuid(),
            Amount = 2_000_000m,
            CoolingOffUntil = DateTime.UtcNow.AddHours(20)
        };
        _viewModel.PendingTransactions.Add(item);
        _viewModel.SelectedItem = item;
        _viewModel.BypassReason = "Sửa xe máy bị hỏng giữa đường";

        _mockRepository.Setup(r => r.BypassCoolingOffAsync(item.Id, It.IsAny<BypassCoolingOffRequestModel>()))
            .ReturnsAsync(true);

        // Act
        await _viewModel.SubmitBypassCommand.ExecuteAsync(null);

        // Assert
        _viewModel.PendingTransactions.Should().NotContain(item);
        _viewModel.IsBypassModalVisible.Should().BeFalse();
        _viewModel.StatusMessage.Should().Contain("mở khóa khẩn cấp");
    }
}
