namespace ExpenseTracker.Application.DTOs.Auth;

// Request payload for registering a new user account
public record RegisterRequestDto(string FullName, string Email, string Password, string BaseCurrency = "VND");

// Request payload for authenticating existing credentials
public record LoginRequestDto(string Email, string Password);

// Request payload for rotating an expired access token using a refresh token
public record RefreshTokenRequestDto(string AccessToken, string RefreshToken);

// User profile snippet returned within authentication responses
public record UserInfoDto(Guid Id, string FullName, string Email, string BaseCurrency);

// Authentication response containing JWT tokens and user metadata
public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfoDto User
);
