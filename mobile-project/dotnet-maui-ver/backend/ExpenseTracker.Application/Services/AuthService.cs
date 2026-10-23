using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of user authentication, credential validation, and token rotation
public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;

    public AuthService(
        IApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<AuthResponseDto>.Failure(validationResult.Errors[0].ErrorMessage, 400);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var emailExists = await _dbContext.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (emailExists)
        {
            return Result<AuthResponseDto>.Failure("An account with this email already exists.", 409);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = new User(request.FullName, normalizedEmail, passwordHash, request.BaseCurrency);

        _dbContext.Users.Add(user);

        // UC-01.01: Automatically initialize a default cash wallet with zero balance
        var defaultWallet = new Wallet(
            userId: user.Id,
            name: "Ví Tiền Mặt",
            walletType: WalletType.Cash,
            initialBalance: 0m,
            currency: user.BaseCurrency,
            colorHex: "#10B981",
            iconKey: "wallet",
            isExcludedFromTotal: false
        );
        _dbContext.Wallets.Add(defaultWallet);

        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id);

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto(
            accessToken,
            refreshToken.Token,
            expiresAt,
            new UserInfoDto(user.Id, user.FullName, user.Email, user.BaseCurrency)
        );

        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<AuthResponseDto>.Failure(validationResult.Errors[0].ErrorMessage, 400);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password.", 401);
        }

        if (user.Status == UserStatus.Locked)
        {
            return Result<AuthResponseDto>.Failure("Your account has been locked. Please contact support.", 403);
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password.", 401);
        }

        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto(
            accessToken,
            refreshToken.Token,
            expiresAt,
            new UserInfoDto(user.Id, user.FullName, user.Email, user.BaseCurrency)
        );

        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result<AuthResponseDto>.Failure("Refresh token is required.", 400);
        }

        var existingToken = await _dbContext.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

        if (existingToken == null)
        {
            return Result<AuthResponseDto>.Failure("Invalid refresh token.", 401);
        }

        // Security check: If a revoked token is presented, invalidate all refresh tokens for this user
        if (existingToken.IsRevoked)
        {
            var userTokens = await _dbContext.RefreshTokens
                .Where(t => t.UserId == existingToken.UserId && t.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in userTokens)
            {
                token.Revoke("compromised_reuse_detected");
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<AuthResponseDto>.Failure("Invalid refresh token state. Please login again.", 401);
        }

        if (existingToken.IsExpired)
        {
            return Result<AuthResponseDto>.Failure("Refresh token has expired. Please login again.", 401);
        }

        var user = existingToken.User;
        if (user.Status == UserStatus.Locked)
        {
            return Result<AuthResponseDto>.Failure("Account is locked.", 403);
        }

        var (newAccessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id);

        // Rotate token: revoke old token and save new token
        existingToken.Revoke(newRefreshToken.Token);
        _dbContext.RefreshTokens.Add(newRefreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto(
            newAccessToken,
            newRefreshToken.Token,
            expiresAt,
            new UserInfoDto(user.Id, user.FullName, user.Email, user.BaseCurrency)
        );

        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Success();
        }

        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

        if (token != null && token.IsActive)
        {
            token.Revoke();
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
