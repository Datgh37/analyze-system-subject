using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Auth;

// Request payload for registering a new user account
public record RegisterRequestDto(string FullName, string Email, string Password, string BaseCurrency = "VND");

// Request payload for authenticating existing credentials
public record LoginRequestDto(string Email, string Password);

// Request payload for rotating an expired access token using a refresh token
public record RefreshTokenRequestDto(string AccessToken, string RefreshToken);

// User profile snippet returned within authentication responses
public record UserInfoDto(Guid Id, string FullName, string Email, string BaseCurrency, bool IsEmailVerified = false);

// Authentication response containing JWT tokens and user metadata
public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfoDto User
);

// Request payload for verifying a 6-digit OTP token
public record VerifyOtpRequestDto(string Email, string OtpCode, OtpPurpose Purpose);

// Request payload for re-dispatching a fresh OTP token
public record ResendOtpRequestDto(string Email, OtpPurpose Purpose);

// Request payload for requesting account recovery via OTP
public record ForgotPasswordRequestDto(string Email);

// Request payload for resetting password using a verified OTP code
public record ResetPasswordWithOtpRequestDto(string Email, string OtpCode, string NewPassword);

// Request payload for authenticating with Google OAuth 2.0
public record GoogleAuthRequestDto(string IdToken, string? Email = null, string? FullName = null);

