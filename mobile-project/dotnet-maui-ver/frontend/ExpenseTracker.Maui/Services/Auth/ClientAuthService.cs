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
            CurrentUser = new UserInfo(profile.Id, profile.FullName, profile.Email, profile.BaseCurrency);
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
            CurrentUser = response.User;
            IsAuthenticated = true;

            return (true, null);
        }
        catch (Refit.ApiException apiEx)
        {
            return (false, apiEx.Content ?? "Invalid email or password.");
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
            CurrentUser = response.User;
            IsAuthenticated = true;

            return (true, null);
        }
        catch (Refit.ApiException apiEx)
        {
            return (false, apiEx.Content ?? "Registration failed. Please verify your inputs.");
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
            await _tokenStorage.ClearTokensAsync();
            IsAuthenticated = false;
            CurrentUser = null;
        }
    }
}
