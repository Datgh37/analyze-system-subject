namespace ExpenseTracker.Maui.Services.Security;

// Service contract for securely persisting sensitive authentication tokens on the device
public interface ITokenStorageService
{
    Task SaveTokensAsync(string accessToken, string refreshToken);
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task ClearTokensAsync();
}

// Implementation leveraging MAUI SecureStorage backed by platform hardware keychains
public class SecureStorageTokenService : ITokenStorageService
{
    private const string AccessTokenKey = "auth_access_token";
    private const string RefreshTokenKey = "auth_refresh_token";

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        await SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);
        await SecureStorage.Default.SetAsync(RefreshTokenKey, refreshToken);
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
}
