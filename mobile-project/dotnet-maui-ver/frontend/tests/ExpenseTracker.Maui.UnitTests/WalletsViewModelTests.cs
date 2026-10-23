using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Maui.UnitTests;

public class WalletsViewModelTests
{
    private readonly Mock<IWalletRepository> _mockRepo;
    private readonly WalletsViewModel _viewModel;

    public WalletsViewModelTests()
    {
        _mockRepo = new Mock<IWalletRepository>();
        _viewModel = new WalletsViewModel(_mockRepo.Object);
    }

    [Fact]
    public async Task LoadWalletsAsync_ShouldPopulateWallets_AndCalculateTotalBalanceCorrectly()
    {
        // Arrange: 2 ví với số dư khác nhau, 1 ví không loại trừ và 1 ví bị loại trừ
        var userId = Guid.NewGuid();
        var wallets = new List<WalletModel>
        {
            new(Guid.NewGuid(), userId, "Ví Tiền Mặt", MobileWalletType.Cash, 5000000m, 5000000m, "VND", null, null, null, null, null, "#10B981", "wallet", 1, false, 1, 1, DateTime.UtcNow, null),
            new(Guid.NewGuid(), userId, "Tài khoản VCB", MobileWalletType.BankAccount, 12000000m, 12000000m, "VND", "123456", "VCB", null, null, null, "#0066FF", "bank", 2, false, 1, 1, DateTime.UtcNow, null),
            new(Guid.NewGuid(), userId, "Quỹ Đầu Tư (Excluded)", MobileWalletType.Cash, 50000000m, 50000000m, "VND", null, null, null, null, null, "#888888", "lock", 3, true, 1, 1, DateTime.UtcNow, null)
        };

        _mockRepo.Setup(r => r.GetWalletsAsync()).ReturnsAsync(wallets);

        // Act
        await _viewModel.LoadWalletsAsync();

        // Assert
        _viewModel.Wallets.Should().HaveCount(3);
        _viewModel.TotalBalance.Should().Be(17000000m); // 5tr + 12tr (bỏ qua ví excluded 50tr)
        _viewModel.FormattedTotalBalance.Should().Contain("17").And.Contain("000").And.Contain("VND");
        _viewModel.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task LoadWalletsAsync_WhenRepositoryReturnsEmpty_ShouldSetBalanceZero()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetWalletsAsync()).ReturnsAsync(new List<WalletModel>());

        // Act
        await _viewModel.LoadWalletsAsync();

        // Assert
        _viewModel.Wallets.Should().BeEmpty();
        _viewModel.TotalBalance.Should().Be(0);
        _viewModel.FormattedTotalBalance.Should().Be("0 VND");
    }

    [Fact]
    public async Task LoadWalletsAsync_WhenRepositoryThrows_ShouldSetErrorMessage()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetWalletsAsync()).ThrowsAsync(new HttpRequestException("Network failure"));

        // Act
        await _viewModel.LoadWalletsAsync();

        // Assert
        _viewModel.ErrorMessage.Should().Contain("Network failure");
        _viewModel.Wallets.Should().BeEmpty();
    }

    [Fact]
    public async Task TransferMoneyAsync_WhenCalled_ShouldCallRepository_AndReloadWallets()
    {
        // Arrange
        var req = new TransferMoneyModel(
            SourceWalletId: Guid.NewGuid(),
            DestinationWalletId: Guid.NewGuid(),
            Amount: 1000000m,
            TransferDate: DateTime.Today,
            Note: "Chuyển tiền tiết kiệm"
        );

        _mockRepo.Setup(r => r.TransferMoneyAsync(req)).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.GetWalletsAsync()).ReturnsAsync(new List<WalletModel>());

        // Act
        await _viewModel.TransferMoneyAsync(req);

        // Assert
        _mockRepo.Verify(r => r.TransferMoneyAsync(req), Times.Once);
        _mockRepo.Verify(r => r.GetWalletsAsync(), Times.Once);
    }
}
