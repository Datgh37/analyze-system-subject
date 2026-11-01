namespace ExpenseTracker.Maui.Services.Security;

// Service contract for securely persisting sensitive authentication tokens and session state
public interface ITokenStorageService
{
    Task SaveTokensAsync(string accessToken, string refreshToken);
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task ClearTokensAsync();

    // Session and Quick Login profile persistence
    Task SaveUserSessionAsync(string email, string fullName, string? avatarUrl);
    Task<(string? Email, string? FullName, string? AvatarUrl)> GetUserSessionAsync();
    Task SetQuickLoginEnabledAsync(bool enabled);
    Task<bool> IsQuickLoginEnabledAsync();
    Task SetActiveSessionFlagAsync(bool hasActiveSession);
    Task<bool> HasActiveSessionAsync();
    Task PurgeSessionOnLogoutAsync();
}

// Implementation leveraging MAUI SecureStorage backed by platform hardware keychains
public class SecureStorageTokenService : ITokenStorageService
{
    private const string AccessTokenKey = "auth_access_token";
    private const string RefreshTokenKey = "auth_refresh_token";
    private const string UserEmailKey = "user_email";
    private const string UserFullNameKey = "user_fullname";
    private const string UserAvatarUrlKey = "user_avatar_url";
    private const string QuickLoginEnabledKey = "quick_login_enabled";
    private const string HasActiveSessionKey = "has_active_session";

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        await SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);
        await SecureStorage.Default.SetAsync(RefreshTokenKey, refreshToken);
        await SetActiveSessionFlagAsync(true);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await SecureStorage.Default.GetAsync(AccessTokenKey);
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        return await SecureStorage.Default.GetAsync(RefreshTokenKey);
    }

    public Task ClearTokensAsync()
    {
        SecureStorage.Default.Remove(AccessTokenKey);
        SecureStorage.Default.Remove(RefreshTokenKey);
        return Task.CompletedTask;
    }

    public async Task SaveUserSessionAsync(string email, string fullName, string? avatarUrl)
    {
        await SecureStorage.Default.SetAsync(UserEmailKey, email);
        await SecureStorage.Default.SetAsync(UserFullNameKey, fullName);
        if (!string.IsNullOrWhiteSpace(avatarUrl))
        {
            await SecureStorage.Default.SetAsync(UserAvatarUrlKey, avatarUrl);
        }
    }

    public async Task<(string? Email, string? FullName, string? AvatarUrl)> GetUserSessionAsync()
    {
        var email = await SecureStorage.Default.GetAsync(UserEmailKey);
        var fullName = await SecureStorage.Default.GetAsync(UserFullNameKey);
        var avatarUrl = await SecureStorage.Default.GetAsync(UserAvatarUrlKey);
        return (email, fullName, avatarUrl);
    }

    public async Task SetQuickLoginEnabledAsync(bool enabled)
    {
        await SecureStorage.Default.SetAsync(QuickLoginEnabledKey, enabled ? "true" : "false");
    }

    public async Task<bool> IsQuickLoginEnabledAsync()
    {
        var value = await SecureStorage.Default.GetAsync(QuickLoginEnabledKey);
        // Mặc định bật nếu chưa thiết lập, hoặc trả về giá trị đã lưu
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    public async Task SetActiveSessionFlagAsync(bool hasActiveSession)
    {
        await SecureStorage.Default.SetAsync(HasActiveSessionKey, hasActiveSession ? "true" : "false");
    }

    public async Task<bool> HasActiveSessionAsync()
    {
        var value = await SecureStorage.Default.GetAsync(HasActiveSessionKey);
        if (string.IsNullOrWhiteSpace(value))
        {
            // Kiểm tra fallback: nếu có access token thì coi như có session
            var token = await GetAccessTokenAsync();
            return !string.IsNullOrWhiteSpace(token);
        }
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    public async Task PurgeSessionOnLogoutAsync()
    {
        // 1. Xóa toàn bộ token nhạy cảm
        SecureStorage.Default.Remove(AccessTokenKey);
        SecureStorage.Default.Remove(RefreshTokenKey);

        // 2. Đặt cờ phiên thành false để TUYỆT ĐỐI KHÔNG HIỆN Quick Login
        await SetActiveSessionFlagAsync(false);
    }
}
