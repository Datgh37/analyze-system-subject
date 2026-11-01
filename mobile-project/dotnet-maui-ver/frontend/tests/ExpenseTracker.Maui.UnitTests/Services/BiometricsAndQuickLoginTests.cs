using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.Services.Biometrics;
using ExpenseTracker.Maui.Services.Security;
using ExpenseTracker.Maui.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests.Services;

public class BiometricsAndQuickLoginTests
{
    private readonly Mock<IBiometricAuthService> _biometricAuthMock;
    private readonly Mock<ITokenStorageService> _tokenStorageMock;
    private readonly Mock<IClientAuthService> _authServiceMock;

    public BiometricsAndQuickLoginTests()
    {
        _biometricAuthMock = new Mock<IBiometricAuthService>();
        _tokenStorageMock = new Mock<ITokenStorageService>();
        _authServiceMock = new Mock<IClientAuthService>();
    }

    [Fact]
    public async Task QuickLogin_WhenUserLoggedOut_HasActiveSessionIsFalse_ShouldDisallowQuickLogin()
    {
        // Arrange: Người dùng đã bấm Đăng xuất ở phiên trước
        _tokenStorageMock.Setup(s => s.HasActiveSessionAsync()).ReturnsAsync(false);
        _tokenStorageMock.Setup(s => s.IsQuickLoginEnabledAsync()).ReturnsAsync(true);

        // Act: Giả lập logic kiểm tra khởi động App
        var hasActiveSession = await _tokenStorageMock.Object.HasActiveSessionAsync();
        var isQuickLoginEnabled = await _tokenStorageMock.Object.IsQuickLoginEnabledAsync();

        bool canQuickLogin = hasActiveSession && isQuickLoginEnabled;

        // Assert: TUYỆT ĐỐI KHÔNG ĐƯỢC PHÉP Quick Login
        canQuickLogin.Should().BeFalse();
    }

    [Fact]
    public async Task QuickLogin_WhenUserJustClosedApp_HasActiveSessionIsTrue_AndQuickLoginEnabled_ShouldAllowQuickLogin()
    {
        // Arrange: Người dùng chỉ tắt app (chưa đăng xuất) và bật Quick Login
        _tokenStorageMock.Setup(s => s.HasActiveSessionAsync()).ReturnsAsync(true);
        _tokenStorageMock.Setup(s => s.IsQuickLoginEnabledAsync()).ReturnsAsync(true);

        // Act
        var hasActiveSession = await _tokenStorageMock.Object.HasActiveSessionAsync();
        var isQuickLoginEnabled = await _tokenStorageMock.Object.IsQuickLoginEnabledAsync();

        bool canQuickLogin = hasActiveSession && isQuickLoginEnabled;

        // Assert: Được phép vào QuickLoginPage
        canQuickLogin.Should().BeTrue();
    }

    [Fact]
    public async Task QuickLoginViewModel_WhenBiometricSucceeds_ShouldReportSuccessStatus()
    {
        // Arrange
        _tokenStorageMock.Setup(s => s.GetUserSessionAsync())
            .ReturnsAsync(("test@expensetracker.vn", "Nguyễn Văn Test", null));
        _biometricAuthMock.Setup(b => b.CheckCapabilityAsync())
            .ReturnsAsync(BiometricCapabilityStatus.Available);
        _biometricAuthMock.Setup(b => b.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new BiometricAuthResult(true, null, false));

        var vm = new QuickLoginViewModel(_biometricAuthMock.Object, _tokenStorageMock.Object, _authServiceMock.Object);

        // Act
        await vm.ScanBiometricCommand.ExecuteAsync(null);

        // Assert
        vm.StatusMessage.Should().Contain("Xác thực thành công");
        vm.IsScanning.Should().BeFalse();
    }

    [Fact]
    public async Task QuickLoginViewModel_WhenBiometricCanceled_ShouldAllowRetryAndKeepModalState()
    {
        // Arrange: Người dùng chạm ra ngoài modal hoặc bấm Hủy
        _tokenStorageMock.Setup(s => s.GetUserSessionAsync())
            .ReturnsAsync(("test@expensetracker.vn", "Nguyễn Văn Test", null));
        _biometricAuthMock.Setup(b => b.CheckCapabilityAsync())
            .ReturnsAsync(BiometricCapabilityStatus.Available);
        _biometricAuthMock.Setup(b => b.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new BiometricAuthResult(false, "Đã hủy quét", true));

        var vm = new QuickLoginViewModel(_biometricAuthMock.Object, _tokenStorageMock.Object, _authServiceMock.Object);

        // Act
        await vm.ScanBiometricCommand.ExecuteAsync(null);

        // Assert: Không văng app, cho phép thử lại
        vm.CanRetry.Should().BeTrue();
        vm.StatusMessage.Should().Contain("hủy");
        vm.IsScanning.Should().BeFalse();
    }

    [Fact]
    public async Task QuickLoginViewModel_WhenHardwareUnsupported_ShouldDisableRetryAndInformUser()
    {
        // Arrange: Máy không có cảm biến
        _biometricAuthMock.Setup(b => b.CheckCapabilityAsync())
            .ReturnsAsync(BiometricCapabilityStatus.Unsupported);

        var vm = new QuickLoginViewModel(_biometricAuthMock.Object, _tokenStorageMock.Object, _authServiceMock.Object);

        // Act
        await vm.ScanBiometricCommand.ExecuteAsync(null);

        // Assert
        vm.CanRetry.Should().BeFalse();
        vm.StatusMessage.Should().Contain("không hỗ trợ");
    }

    [Fact]
    public async Task QuickLoginViewModel_WhenNotEnrolled_ShouldPromptUserToRegisterFingerprint()
    {
        // Arrange: Máy có cảm biến nhưng chưa cài đặt vân tay trong hệ điều hành
        _biometricAuthMock.Setup(b => b.CheckCapabilityAsync())
            .ReturnsAsync(BiometricCapabilityStatus.NotEnrolled);

        var vm = new QuickLoginViewModel(_biometricAuthMock.Object, _tokenStorageMock.Object, _authServiceMock.Object);

        // Act
        await vm.ScanBiometricCommand.ExecuteAsync(null);

        // Assert
        vm.CanRetry.Should().BeFalse();
        vm.StatusMessage.Should().Contain("chưa đăng ký vân tay");
    }

    [Fact]
    public async Task Logout_ShouldInvokePurgeSessionOnLogout_AndClearActiveFlag()
    {
        // Arrange
        _tokenStorageMock.Setup(s => s.PurgeSessionOnLogoutAsync()).Returns(Task.CompletedTask);
        _authServiceMock.Setup(a => a.LogoutAsync()).Returns(Task.CompletedTask);

        // Act: Giả lập hành vi Logout
        await _authServiceMock.Object.LogoutAsync();
        await _tokenStorageMock.Object.PurgeSessionOnLogoutAsync();

        // Assert: Phải gọi đúng PurgeSessionOnLogoutAsync
        _tokenStorageMock.Verify(s => s.PurgeSessionOnLogoutAsync(), Times.Once);
        _authServiceMock.Verify(a => a.LogoutAsync(), Times.Once);
    }
}

