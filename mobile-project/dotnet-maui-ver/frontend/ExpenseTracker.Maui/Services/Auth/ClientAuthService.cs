using ExpenseTracker.Maui.Models.Auth;
using ExpenseTracker.Maui.Services.Api;
using ExpenseTracker.Maui.Services.Security;

namespace ExpenseTracker.Maui.Services.Auth;

// Client authentication service contract managing local session state
public interface IClientAuthService
{
    bool IsAuthenticated { get; }
    UserInfo? CurrentUser { get; }

    Task<bool> InitializeAsync();
    Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password);
    Task<(bool Success, string? ErrorMessage)> RegisterAsync(string fullName, string email, string password, string baseCurrency = "VND");
    Task<(bool Success, string? ErrorMessage)> VerifyOtpAsync(string email, string otpCode);
    Task<(bool Success, string? ErrorMessage)> ResendOtpAsync(string email);
    Task<(bool Success, string? ErrorMessage)> ForgotPasswordAsync(string email);
    Task<(bool Success, string? ErrorMessage)> ResetPasswordWithOtpAsync(string email, string otpCode, string newPassword);
    Task<(bool Success, string? ErrorMessage)> GoogleLoginAsync(string idToken, string email, string displayName);
    Task LogoutAsync();
}

// Implementation coordinating token persistence, Refit network calls, and session state
public class ClientAuthService : IClientAuthService
{
    private readonly IAuthApi _authApi;
    private readonly IUserApi _userApi;
    private readonly ITokenStorageService _tokenStorage;

    public bool IsAuthenticated { get; private set; }
    public UserInfo? CurrentUser { get; private set; }

    public ClientAuthService(
        IAuthApi authApi,
        IUserApi userApi,
        ITokenStorageService tokenStorage)
    {
        _authApi = authApi;
        _userApi = userApi;
        _tokenStorage = tokenStorage;
    }

    public async Task<bool> InitializeAsync()
    {
        var accessToken = await _tokenStorage.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            IsAuthenticated = false;
            CurrentUser = null;
            return false;
        }

        try
        {
            var profile = await _userApi.GetProfileAsync();
            CurrentUser = new UserInfo(profile.Id, profile.FullName, profile.Email, profile.BaseCurrency, true);
            await _tokenStorage.SaveUserSessionAsync(profile.Email, profile.FullName, null);
            await _tokenStorage.SetActiveSessionFlagAsync(true);
            IsAuthenticated = true;
            return true;
        }
        catch
        {
            // Token is invalid or expired
            await _tokenStorage.ClearTokensAsync();
            IsAuthenticated = false;
            CurrentUser = null;
            return false;
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest(email, password);
            var response = await _authApi.LoginAsync(request);

            await _tokenStorage.SaveTokensAsync(response.AccessToken, response.RefreshToken);
            await _tokenStorage.SaveUserSessionAsync(response.User.Email, response.User.FullName, null);
            CurrentUser = response.User;
            IsAuthenticated = true;

            return (true, null);
        }
        catch (Refit.ApiException apiEx)
        {
            return (false, ExtractErrorMessage(apiEx, "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin."));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(string fullName, string email, string password, string baseCurrency = "VND")
    {
        try
        {
            var request = new RegisterRequest(fullName, email, password, baseCurrency);
            var response = await _authApi.RegisterAsync(request);

            await _tokenStorage.SaveTokensAsync(response.AccessToken, response.RefreshToken);
            await _tokenStorage.SaveUserSessionAsync(response.User.Email, response.User.FullName, null);
            CurrentUser = response.User;
            IsAuthenticated = true;

            return (true, null);
        }
        catch (Refit.ApiException apiEx)
        {
            return (false, ExtractErrorMessage(apiEx, "Đăng ký không thành công. Vui lòng kiểm tra lại thông tin."));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> VerifyOtpAsync(string email, string otpCode)
    {
        try
        {
            var request = new VerifyOtpRequest(email, otpCode);
            var response = await _authApi.VerifyOtpAsync(request);

            await _tokenStorage.SaveTokensAsync(response.AccessToken, response.RefreshToken);
            await _tokenStorage.SaveUserSessionAsync(response.User.Email, response.User.FullName, null);
            CurrentUser = response.User;
            IsAuthenticated = true;

            return (true, null);
        }
        catch (Refit.ApiException apiEx)
        {
            return (false, ExtractErrorMessage(apiEx, "Mã xác thực không hợp lệ hoặc đã hết hạn."));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> ResendOtpAsync(string email)
    {
        try
        {
            var request = new ResendOtpRequest(email);
            await _authApi.ResendOtpAsync(request);
            return (true, null);
        }
        catch (Refit.ApiException apiEx)
        {
            return (false, ExtractErrorMessage(apiEx, "Không thể gửi lại mã xác thực vào lúc này."));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> ForgotPasswordAsync(string email)
    {
        try
        {
            var request = new ForgotPasswordRequest(email);
            await _authApi.ForgotPasswordAsync(request);
            return (true, null);
        }
        catch (Refit.ApiException apiEx)
        {
            return (false, ExtractErrorMessage(apiEx, "Yêu cầu khôi phục mật khẩu không thành công."));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> ResetPasswordWithOtpAsync(string email, string otpCode, string newPassword)
    {
        try
        {
            var request = new ResetPasswordWithOtpRequest(email, otpCode, newPassword);
            await _authApi.ResetPasswordWithOtpAsync(request);
            return (true, null);
        }
        catch (Refit.ApiException apiEx)
        {
            return (false, ExtractErrorMessage(apiEx, "Đặt lại mật khẩu không thành công. Vui lòng thử lại."));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> GoogleLoginAsync(string idToken, string email, string displayName)
    {
        try
        {
            var request = new GoogleAuthRequest(idToken, email, displayName);
            var response = await _authApi.GoogleLoginAsync(request);

            await _tokenStorage.SaveTokensAsync(response.AccessToken, response.RefreshToken);
            await _tokenStorage.SaveUserSessionAsync(response.User.Email, response.User.FullName, null);
            CurrentUser = response.User;
            IsAuthenticated = true;

            return (true, null);
        }
        catch (Refit.ApiException apiEx)
        {
            return (false, ExtractErrorMessage(apiEx, "Đăng nhập Google không thành công."));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var refreshToken = await _tokenStorage.GetRefreshTokenAsync();
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authApi.LogoutAsync(new { RefreshToken = refreshToken });
            }
        }
        catch
        {
            // Ignore network failures on logout
        }
        finally
        {
            await _tokenStorage.PurgeSessionOnLogoutAsync();
            IsAuthenticated = false;
            CurrentUser = null;
        }
    }

    private static string ExtractErrorMessage(Refit.ApiException apiEx, string fallback)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(apiEx.Content))
            {
                using var doc = System.Text.Json.JsonDocument.Parse(apiEx.Content);
                if (doc.RootElement.TryGetProperty("detail", out var detail) && !string.IsNullOrWhiteSpace(detail.GetString()))
                {
                    return detail.GetString()!;
                }
                if (doc.RootElement.TryGetProperty("title", out var title) && !string.IsNullOrWhiteSpace(title.GetString()))
                {
                    return title.GetString()!;
                }
            }
        }
        catch
        {
            // fallback if not valid JSON ProblemDetails
        }

        return !string.IsNullOrWhiteSpace(apiEx.Content) ? apiEx.Content : fallback;
    }
}
