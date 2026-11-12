using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Services.Auth;
using ExpenseTracker.Maui.Services.Biometrics;
using ExpenseTracker.Maui.Services.Realtime;
using ExpenseTracker.Maui.Services.Security;

namespace ExpenseTracker.Maui.ViewModels;

public partial class QuickLoginViewModel : BaseViewModel
{
    private readonly IBiometricAuthService _biometricAuth;
    private readonly ITokenStorageService _tokenStorage;
    private readonly IClientAuthService _authService;
    private readonly ISignalRRealtimeService? _realtimeService;

    [ObservableProperty]
    private string _userFullName = "Người dùng";

    [ObservableProperty]
    private string _userEmail = string.Empty;

    [ObservableProperty]
    private string? _userAvatarUrl;

    [ObservableProperty]
    private string _statusMessage = "Chạm vào cảm biến để đăng nhập";

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private bool _canRetry = true;

    [ObservableProperty]
    private string _greetingText = "Chào mừng bạn trở lại";

    public QuickLoginViewModel(
        IBiometricAuthService biometricAuth,
        ITokenStorageService tokenStorage,
        IClientAuthService authService,
        ISignalRRealtimeService? realtimeService = null)
    {
        _biometricAuth = biometricAuth;
        _tokenStorage = tokenStorage;
        _authService = authService;
        _realtimeService = realtimeService;
        Title = "Đăng Nhập Nhanh";

        UpdateGreeting();
    }

    private void UpdateGreeting()
    {
        var hour = DateTime.Now.Hour;
        GreetingText = hour switch
        {
            >= 5 and < 12 => "Chào buổi sáng",
            >= 12 and < 18 => "Chào buổi chiều",
            _ => "Chào buổi tối"
        };
    }

    public async Task InitializeAsync()
    {
        var (email, fullName, avatar) = await _tokenStorage.GetUserSessionAsync();
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            UserFullName = fullName;
        }
        if (!string.IsNullOrWhiteSpace(email))
        {
            UserEmail = email;
        }
        UserAvatarUrl = avatar;

        UpdateGreeting();

        // Tự động kích hoạt quét vân tay 300ms sau khi mở trang
        await Task.Delay(300);
        await ScanBiometricAsync();
    }

    [RelayCommand]
    private async Task ScanBiometricAsync()
    {
        if (IsScanning)
        {
            return;
        }

        try
        {
            IsScanning = true;
            StatusMessage = "Đang chờ quét vân tay / FaceID...";

            var capability = await _biometricAuth.CheckCapabilityAsync();
            if (capability == BiometricCapabilityStatus.Unsupported)
            {
                StatusMessage = "Thiết bị không hỗ trợ sinh trắc học.";
                CanRetry = false;
                return;
            }

            if (capability == BiometricCapabilityStatus.NotEnrolled)
            {
                StatusMessage = "Bạn chưa đăng ký vân tay trong cài đặt máy.";
                CanRetry = false;
                return;
            }

            var result = await _biometricAuth.AuthenticateAsync(
                title: "Xác Thực Sinh Trắc Học",
                subtitle: $"Đăng nhập nhanh cho {UserFullName}",
                negativeButtonText: "Sử dụng Mật khẩu");

            if (result.IsSuccess)
            {
                StatusMessage = "Xác thực thành công! Đang vào ứng dụng...";

                // Khởi tạo kết nối Realtime nếu có
                if (_realtimeService != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try { await _realtimeService.ConnectAsync(); } catch { }
                    });
                }

                // Chuyển hướng thẳng vào trang chính
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("//WalletsPage");
                }
            }
            else
            {
                if (result.IsUserCanceled)
                {
                    StatusMessage = "Bạn đã hủy quét. Hãy chạm vào vân tay để thử lại hoặc chọn nhập mật khẩu.";
                }
                else
                {
                    StatusMessage = result.ErrorMessage ?? "Xác thực không thành công. Hãy thử lại.";
                }
                CanRetry = true;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi: {ex.Message}";
            CanRetry = true;
        }
        finally
        {
            IsScanning = false;
        }
    }

    [RelayCommand]
    private async Task UsePasswordAsync()
    {
        // Điều hướng sang LoginPage truyền thống
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    [RelayCommand]
    private async Task SwitchAccountAsync()
    {
        // Xóa sạch session của tài khoản cũ và điều hướng về LoginPage
        await _authService.LogoutAsync();
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
