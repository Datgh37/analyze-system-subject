using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service interface for user authentication and session management
public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}

// Service interface for user profile operations
public interface IUserService
{
    Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default);
}
