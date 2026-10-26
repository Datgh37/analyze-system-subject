using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Maui.UnitTests;

public class VerifyOtpViewModelTests
{
    private readonly Mock<IClientAuthService> _mockAuthService;
    private readonly VerifyOtpViewModel _viewModel;

    public VerifyOtpViewModelTests()
    {
        _mockAuthService = new Mock<IClientAuthService>();
        _viewModel = new VerifyOtpViewModel(_mockAuthService.Object);
    }

    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        _viewModel.Title.Should().Be("Xác Thực Tài Khoản");
        _viewModel.CountdownSeconds.Should().Be(60);
        _viewModel.CanResend.Should().BeFalse();
        _viewModel.ResendTextColor.Should().Be("#94A3B8");
        _viewModel.ErrorMessage.Should().BeNull();
        _viewModel.InfoMessage.Should().BeNull();
    }

    [Fact]
    public async Task VerifyOtpAsync_WhenEmailIsEmpty_ShouldSetErrorMessage()
    {
        // Arrange
        _viewModel.Email = "";
        _viewModel.OtpCode = "123456";

        // Act
        await _viewModel.VerifyOtpCommand.ExecuteAsync(null);

        // Assert
        _viewModel.ErrorMessage.Should().Contain("email không xác định");
        _mockAuthService.Verify(a => a.VerifyOtpAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("   ")]
    public async Task VerifyOtpAsync_WhenOtpCodeNotSixDigits_ShouldSetErrorMessage(string invalidCode)
    {
        // Arrange
        _viewModel.Email = "test@example.com";
        _viewModel.OtpCode = invalidCode;

        // Act
        await _viewModel.VerifyOtpCommand.ExecuteAsync(null);

        // Assert
        _viewModel.ErrorMessage.Should().Contain("đầy đủ mã OTP 6 chữ số");
        _mockAuthService.Verify(a => a.VerifyOtpAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task VerifyOtpAsync_WhenServiceReturnsFailure_ShouldSetErrorMessage()
    {
        // Arrange
        _viewModel.Email = "user@example.com";
        _viewModel.OtpCode = "999999";
        _mockAuthService
            .Setup(a => a.VerifyOtpAsync("user@example.com", "999999"))
            .ReturnsAsync((false, "Mã xác thực đã hết hạn."));

        // Act
        await _viewModel.VerifyOtpCommand.ExecuteAsync(null);

        // Assert
        _viewModel.ErrorMessage.Should().Be("Mã xác thực đã hết hạn.");
    }

    [Fact]
    public async Task ResendOtpAsync_WhenCanResendFalse_ShouldNotCallService()
    {
        // Arrange
        _viewModel.Email = "user@example.com";
        _viewModel.CanResend = false;

        // Act
        await _viewModel.ResendOtpCommand.ExecuteAsync(null);

        // Assert
        _mockAuthService.Verify(a => a.ResendOtpAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ResendOtpAsync_WhenCanResendTrue_AndServiceSucceeds_ShouldSetInfoMessage()
    {
        // Arrange
        _viewModel.Email = "user@example.com";
        _viewModel.CanResend = true;
        _mockAuthService
            .Setup(a => a.ResendOtpAsync("user@example.com"))
            .ReturnsAsync((true, null));

        // Act
        await _viewModel.ResendOtpCommand.ExecuteAsync(null);

        // Assert
        _viewModel.InfoMessage.Should().Contain("Mã xác thực mới đã được gửi");
        _viewModel.ErrorMessage.Should().BeNull();
        _mockAuthService.Verify(a => a.ResendOtpAsync("user@example.com"), Times.Once);
    }

    [Fact]
    public async Task ResendOtpAsync_WhenServiceFails_ShouldSetErrorMessage()
    {
        // Arrange
        _viewModel.Email = "user@example.com";
        _viewModel.CanResend = true;
        _mockAuthService
            .Setup(a => a.ResendOtpAsync("user@example.com"))
            .ReturnsAsync((false, "Vui lòng đợi 60 giây trước khi yêu cầu lại."));

        // Act
        await _viewModel.ResendOtpCommand.ExecuteAsync(null);

        // Assert
        _viewModel.ErrorMessage.Should().Be("Vui lòng đợi 60 giây trước khi yêu cầu lại.");
    }
}
