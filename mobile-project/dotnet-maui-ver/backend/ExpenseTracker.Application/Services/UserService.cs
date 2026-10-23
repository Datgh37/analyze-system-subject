using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of user profile retrieval, detail updates, and secure password changes
public class UserService : IUserService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator;

    public UserService(
        IApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IValidator<ChangePasswordRequestDto> changePasswordValidator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _changePasswordValidator = changePasswordValidator;
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return Result<UserProfileDto>.Failure("User not found.", 404);
        }

        var profile = new UserProfileDto(
            user.Id,
            user.FullName,
            user.Email,
            user.BaseCurrency,
            user.Status.ToString(),
            user.CreatedAt
        );

        return Result<UserProfileDto>.Success(profile);
    }

    public async Task<Result<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return Result<UserProfileDto>.Failure("Full name cannot be empty.", 400);
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return Result<UserProfileDto>.Failure("User not found.", 404);
        }

        user.UpdateProfile(request.FullName, request.BaseCurrency);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var profile = new UserProfileDto(
            user.Id,
            user.FullName,
            user.Email,
            user.BaseCurrency,
            user.Status.ToString(),
            user.CreatedAt
        );

        return Result<UserProfileDto>.Success(profile);
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _changePasswordValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.Errors[0].ErrorMessage, 400);
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return Result.Failure("User not found.", 404);
        }

        var isCurrentPasswordValid = _passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash);
        if (!isCurrentPasswordValid)
        {
            return Result.Failure("Current password is incorrect.", 400);
        }

        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(newPasswordHash);

        // Invalidate active refresh tokens across other sessions upon password change
        var activeTokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke("password_changed");
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
