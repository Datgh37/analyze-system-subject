using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of user authentication, credential validation, OTP verification, and token rotation
public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailService _emailService;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;

    public AuthService(
        IApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IEmailService emailService,
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailService = emailService;
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
        var user = new User(request.FullName, normalizedEmail, passwordHash, request.BaseCurrency, isEmailVerified: false);

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

        // Generate initial 6-digit OTP code for email verification
        var otpCode = Random.Shared.Next(100000, 999999).ToString("D6");
        var otpEntity = new UserOtpCode(
            normalizedEmail,
            otpCode,
            OtpPurpose.EmailVerification,
            TimeSpan.FromMinutes(5),
            user.Id
        );
        _dbContext.UserOtpCodes.Add(otpEntity);

        // Dispatch OTP via email service (SMTP or server console fallback)
        await _emailService.SendOtpEmailAsync(normalizedEmail, otpCode, OtpPurpose.EmailVerification, cancellationToken);

        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id);

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto(
            accessToken,
            refreshToken.Token,
            expiresAt,
            new UserInfoDto(user.Id, user.FullName, user.Email, user.BaseCurrency, user.IsEmailVerified)
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
            new UserInfoDto(user.Id, user.FullName, user.Email, user.BaseCurrency, user.IsEmailVerified)
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
        if (user == null || user.Status == UserStatus.Locked)
        {
            return Result<AuthResponseDto>.Failure("User account is inactive or locked.", 403);
        }

        var (newAccessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id);

        existingToken.Revoke(newRefreshToken.Token);
        _dbContext.RefreshTokens.Add(newRefreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto(
            newAccessToken,
            newRefreshToken.Token,
            expiresAt,
            new UserInfoDto(user.Id, user.FullName, user.Email, user.BaseCurrency, user.IsEmailVerified)
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

    public async Task<Result<AuthResponseDto>> VerifyOtpAsync(VerifyOtpRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.OtpCode))
        {
            return Result<AuthResponseDto>.Failure("Email và mã OTP không được để trống.", 400);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var otp = await _dbContext.UserOtpCodes
            .Where(o => o.Email == normalizedEmail && o.Purpose == request.Purpose && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp == null || !otp.IsValid(request.OtpCode))
        {
            return Result<AuthResponseDto>.Failure("Mã xác thực OTP không chính xác hoặc đã hết hạn.", 400);
        }

        otp.MarkAsUsed();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return Result<AuthResponseDto>.Failure("Không tìm thấy thông tin tài khoản người dùng.", 404);
        }

        if (request.Purpose == OtpPurpose.EmailVerification)
        {
            user.VerifyEmail();
        }

        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto(
            accessToken,
            refreshToken.Token,
            expiresAt,
            new UserInfoDto(user.Id, user.FullName, user.Email, user.BaseCurrency, user.IsEmailVerified)
        );

        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result> ResendOtpAsync(ResendOtpRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure("Email không được để trống.", 400);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Enforce 60-second cooldown rate limit between OTP requests
        var recentOtp = await _dbContext.UserOtpCodes
            .Where(o => o.Email == normalizedEmail && o.Purpose == request.Purpose)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (recentOtp != null && recentOtp.CreatedAt.AddSeconds(60) > DateTime.UtcNow)
        {
            var secondsRemaining = (int)Math.Ceiling((recentOtp.CreatedAt.AddSeconds(60) - DateTime.UtcNow).TotalSeconds);
            return Result.Failure($"Vui lòng đợi {secondsRemaining} giây trước khi yêu cầu gửi lại mã.", 429);
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        var otpCode = Random.Shared.Next(100000, 999999).ToString("D6");
        var newOtp = new UserOtpCode(
            normalizedEmail,
            otpCode,
            request.Purpose,
            TimeSpan.FromMinutes(5),
            user?.Id
        );

        _dbContext.UserOtpCodes.Add(newOtp);
        await _emailService.SendOtpEmailAsync(normalizedEmail, otpCode, request.Purpose, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure("Email không được để trống.", 400);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return Result.Failure("Không tìm thấy tài khoản với địa chỉ email này.", 404);
        }

        var otpCode = Random.Shared.Next(100000, 999999).ToString("D6");
        var otp = new UserOtpCode(
            normalizedEmail,
            otpCode,
            OtpPurpose.PasswordReset,
            TimeSpan.FromMinutes(10),
            user.Id
        );

        _dbContext.UserOtpCodes.Add(otp);
        await _emailService.SendOtpEmailAsync(normalizedEmail, otpCode, OtpPurpose.PasswordReset, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordWithOtpAsync(ResetPasswordWithOtpRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.OtpCode) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return Result.Failure("Vui lòng nhập đầy đủ email, mã OTP và mật khẩu mới.", 400);
        }

        if (request.NewPassword.Length < 6)
        {
            return Result.Failure("Mật khẩu mới phải có ít nhất 6 ký tự.", 400);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var otp = await _dbContext.UserOtpCodes
            .Where(o => o.Email == normalizedEmail && o.Purpose == OtpPurpose.PasswordReset && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp == null || !otp.IsValid(request.OtpCode))
        {
            return Result.Failure("Mã xác thực OTP không chính xác hoặc đã hết hạn.", 400);
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return Result.Failure("Không tìm thấy thông tin tài khoản người dùng.", 404);
        }

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(newHash);
        otp.MarkAsUsed();

        // Invalidate active refresh tokens for security upon password reset
        var activeTokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == user.Id && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke("password_reset_invalidation");
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<AuthResponseDto>> GoogleLoginAsync(GoogleAuthRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result<AuthResponseDto>.Failure("Email từ tài khoản Google không hợp lệ.", 400);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user == null)
        {
            // Auto-provision a new user account verified through Google provider
            var fullName = string.IsNullOrWhiteSpace(request.FullName) ? "Google User" : request.FullName.Trim();
            var randomPassword = _passwordHasher.HashPassword(Guid.NewGuid().ToString("N"));
            user = new User(fullName, normalizedEmail, randomPassword, "VND", isEmailVerified: true);

            _dbContext.Users.Add(user);

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
        }
        else
        {
            if (user.Status == UserStatus.Locked)
            {
                return Result<AuthResponseDto>.Failure("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ hỗ trợ.", 403);
            }

            if (!user.IsEmailVerified)
            {
                user.VerifyEmail();
            }
        }

        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto(
            accessToken,
            refreshToken.Token,
            expiresAt,
            new UserInfoDto(user.Id, user.FullName, user.Email, user.BaseCurrency, user.IsEmailVerified)
        );

        return Result<AuthResponseDto>.Success(response);
    }
}
