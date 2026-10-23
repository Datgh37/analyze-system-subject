namespace ExpenseTracker.Application.DTOs.User;

// Detailed user profile response
public record UserProfileDto(
    Guid Id,
    string FullName,
    string Email,
    string BaseCurrency,
    string Status,
    DateTime CreatedAt
);

// Payload for updating basic profile information
public record UpdateProfileRequestDto(string FullName, string BaseCurrency);

// Payload for changing account password securely
public record ChangePasswordRequestDto(string CurrentPassword, string NewPassword);
