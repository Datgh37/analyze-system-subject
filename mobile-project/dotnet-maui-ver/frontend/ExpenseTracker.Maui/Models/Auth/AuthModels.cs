namespace ExpenseTracker.Maui.Models.Auth;

// Data contracts matching backend authentication endpoints
public record RegisterRequest(string FullName, string Email, string Password, string BaseCurrency = "VND");

public record LoginRequest(string Email, string Password);

public record RefreshTokenRequest(string AccessToken, string RefreshToken);

public record UserInfo(Guid Id, string FullName, string Email, string BaseCurrency);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfo User
);

public record UserProfile(
    Guid Id,
    string FullName,
    string Email,
    string BaseCurrency,
    string Status,
    DateTime CreatedAt
);

public record UpdateProfileRequest(string FullName, string BaseCurrency);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
