using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Application.Validators;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests verifying user registration, authentication, and token rotation rules
public class AuthServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _emailServiceMock = new Mock<IEmailService>();

        _authService = new AuthService(
            _dbContext,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object,
            _emailServiceMock.Object,
            new RegisterRequestValidator(),
            new LoginRequestValidator()
        );
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsSuccessAndPersistsUser()
    {
        // Arrange
        var request = new RegisterRequestDto("John Doe", "john@example.com", "Password123!", "VND");
        _passwordHasherMock.Setup(h => h.HashPassword(request.Password)).Returns("hashed_password");
        _jwtTokenGeneratorMock.Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns(("access_token_123", DateTime.UtcNow.AddMinutes(60)));
        _jwtTokenGeneratorMock.Setup(j => j.GenerateRefreshToken(It.IsAny<Guid>()))
            .Returns(new RefreshToken(Guid.NewGuid(), "refresh_token_123", DateTime.UtcNow.AddDays(30)));

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessToken.Should().Be("access_token_123");
        result.Value.RefreshToken.Should().Be("refresh_token_123");
        result.Value.User.Email.Should().Be("john@example.com");

        var persistedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "john@example.com");
        persistedUser.Should().NotBeNull();
        persistedUser!.FullName.Should().Be("John Doe");

        // UC-01.01: Assert default cash wallet initialized with 0 balance
        var defaultWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == persistedUser.Id);
        defaultWallet.Should().NotBeNull();
        defaultWallet!.Name.Should().Be("Ví Tiền Mặt");
        defaultWallet.WalletType.Should().Be(ExpenseTracker.Domain.Enums.WalletType.Cash);
        defaultWallet.Balance.Should().Be(0m);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ReturnsConflict()
    {
        // Arrange
        var existingUser = new User("Jane Doe", "jane@example.com", "hash");
        _dbContext.Users.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        var request = new RegisterRequestDto("Duplicate User", "jane@example.com", "Password123!");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(409);
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessWithTokens()
    {
        // Arrange
        var user = new User("John Doe", "john@example.com", "valid_hash");
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new LoginRequestDto("john@example.com", "ValidPass123!");
        _passwordHasherMock.Setup(h => h.VerifyPassword(request.Password, "valid_hash")).Returns(true);
        _jwtTokenGeneratorMock.Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns(("new_access_token", DateTime.UtcNow.AddMinutes(60)));
        _jwtTokenGeneratorMock.Setup(j => j.GenerateRefreshToken(user.Id))
            .Returns(new RefreshToken(user.Id, "new_refresh_token", DateTime.UtcNow.AddDays(30)));

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("new_access_token");
        result.Value.RefreshToken.Should().Be("new_refresh_token");
    }

    [Fact]
    public async Task LoginAsync_WithIncorrectPassword_ReturnsUnauthorized()
    {
        // Arrange
        var user = new User("John Doe", "john@example.com", "valid_hash");
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new LoginRequestDto("john@example.com", "WrongPassword");
        _passwordHasherMock.Setup(h => h.VerifyPassword(request.Password, "valid_hash")).Returns(false);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(401);
        result.Error.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_WithLockedAccount_ReturnsForbidden()
    {
        // Arrange
        var user = new User("Locked User", "locked@example.com", "valid_hash");
        user.LockAccount();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new LoginRequestDto("locked@example.com", "AnyPassword");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(403);
        result.Error.Should().Contain("locked");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidActiveToken_RotatesAndReturnsNewTokens()
    {
        // Arrange
        var user = new User("John Doe", "john@example.com", "hash");
        _dbContext.Users.Add(user);

        var activeToken = new RefreshToken(user.Id, "old_valid_token", DateTime.UtcNow.AddDays(10));
        _dbContext.RefreshTokens.Add(activeToken);
        await _dbContext.SaveChangesAsync();

        var request = new RefreshTokenRequestDto("expired_access_token", "old_valid_token");
        _jwtTokenGeneratorMock.Setup(j => j.GenerateAccessToken(user))
            .Returns(("brand_new_access_token", DateTime.UtcNow.AddMinutes(60)));
        _jwtTokenGeneratorMock.Setup(j => j.GenerateRefreshToken(user.Id))
            .Returns(new RefreshToken(user.Id, "brand_new_refresh_token", DateTime.UtcNow.AddDays(30)));

        // Act
        var result = await _authService.RefreshTokenAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("brand_new_access_token");
        result.Value.RefreshToken.Should().Be("brand_new_refresh_token");

        activeToken.IsRevoked.Should().BeTrue();
        activeToken.ReplacedByToken.Should().Be("brand_new_refresh_token");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithRevokedToken_InvalidatesAllTokensAndReturnsUnauthorized()
    {
        // Arrange
        var user = new User("John Doe", "john@example.com", "hash");
        _dbContext.Users.Add(user);

        var compromisedToken = new RefreshToken(user.Id, "compromised_token", DateTime.UtcNow.AddDays(10));
        compromisedToken.Revoke("previous_rotation");

        var siblingToken = new RefreshToken(user.Id, "sibling_active_token", DateTime.UtcNow.AddDays(10));

        _dbContext.RefreshTokens.AddRange(compromisedToken, siblingToken);
        await _dbContext.SaveChangesAsync();

        var request = new RefreshTokenRequestDto("any", "compromised_token");

        // Act
        var result = await _authService.RefreshTokenAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(401);

        siblingToken.IsRevoked.Should().BeTrue();
        siblingToken.ReplacedByToken.Should().Be("compromised_reuse_detected");
    }

    [Fact]
    public async Task RegisterAsync_SendsOtpEmailAndPersistsOtpCode()
    {
        // Arrange
        var request = new RegisterRequestDto("Jane Doe", "jane.otp@example.com", "Password123!", "VND");
        _passwordHasherMock.Setup(h => h.HashPassword(request.Password)).Returns("hashed_pass");
        _jwtTokenGeneratorMock.Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns(("access_token_abc", DateTime.UtcNow.AddMinutes(60)));
        _jwtTokenGeneratorMock.Setup(j => j.GenerateRefreshToken(It.IsAny<Guid>()))
            .Returns(new RefreshToken(Guid.NewGuid(), "refresh_token_abc", DateTime.UtcNow.AddDays(30)));

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var otpRecord = await _dbContext.UserOtpCodes.FirstOrDefaultAsync(o => o.Email == "jane.otp@example.com");
        otpRecord.Should().NotBeNull();
        otpRecord!.Purpose.Should().Be(OtpPurpose.EmailVerification);
        otpRecord.IsUsed.Should().BeFalse();

        _emailServiceMock.Verify(m => m.SendOtpEmailAsync(
            "jane.otp@example.com",
            It.Is<string>(code => code.Length == 6),
            OtpPurpose.EmailVerification,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task VerifyOtpAsync_WithValidOtp_VerifiesUserAndReturnsToken()
    {
        // Arrange
        var user = new User("Test User", "verify@example.com", "hash", "VND", isEmailVerified: false);
        _dbContext.Users.Add(user);

        var otp = new UserOtpCode("verify@example.com", "123456", OtpPurpose.EmailVerification, TimeSpan.FromMinutes(5), user.Id);
        _dbContext.UserOtpCodes.Add(otp);
        await _dbContext.SaveChangesAsync();

        _jwtTokenGeneratorMock.Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns(("verified_access_token", DateTime.UtcNow.AddMinutes(60)));
        _jwtTokenGeneratorMock.Setup(j => j.GenerateRefreshToken(It.IsAny<Guid>()))
            .Returns(new RefreshToken(user.Id, "verified_refresh_token", DateTime.UtcNow.AddDays(30)));

        var request = new VerifyOtpRequestDto("verify@example.com", "123456", OtpPurpose.EmailVerification);

        // Act
        var result = await _authService.VerifyOtpAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("verified_access_token");
        result.Value.User.IsEmailVerified.Should().BeTrue();

        var updatedUser = await _dbContext.Users.FirstAsync(u => u.Id == user.Id);
        updatedUser.IsEmailVerified.Should().BeTrue();

        var updatedOtp = await _dbContext.UserOtpCodes.FirstAsync(o => o.Id == otp.Id);
        updatedOtp.IsUsed.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyOtpAsync_WithInvalidOtp_ReturnsBadRequest()
    {
        // Arrange
        var otp = new UserOtpCode("wrong@example.com", "123456", OtpPurpose.EmailVerification, TimeSpan.FromMinutes(5));
        _dbContext.UserOtpCodes.Add(otp);
        await _dbContext.SaveChangesAsync();

        var request = new VerifyOtpRequestDto("wrong@example.com", "999999", OtpPurpose.EmailVerification);

        // Act
        var result = await _authService.VerifyOtpAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ForgotPasswordAsync_WithValidEmail_DispatchesPasswordResetOtp()
    {
        // Arrange
        var user = new User("Recover User", "recover@example.com", "hash");
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new ForgotPasswordRequestDto("recover@example.com");

        // Act
        var result = await _authService.ForgotPasswordAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var otpRecord = await _dbContext.UserOtpCodes.FirstOrDefaultAsync(o => o.Email == "recover@example.com" && o.Purpose == OtpPurpose.PasswordReset);
        otpRecord.Should().NotBeNull();

        _emailServiceMock.Verify(m => m.SendOtpEmailAsync(
            "recover@example.com",
            It.Is<string>(c => c.Length == 6),
            OtpPurpose.PasswordReset,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordWithOtpAsync_WithValidOtp_UpdatesPassword()
    {
        // Arrange
        var user = new User("Reset User", "reset@example.com", "old_hash");
        _dbContext.Users.Add(user);

        var otp = new UserOtpCode("reset@example.com", "777888", OtpPurpose.PasswordReset, TimeSpan.FromMinutes(10), user.Id);
        _dbContext.UserOtpCodes.Add(otp);
        await _dbContext.SaveChangesAsync();

        _passwordHasherMock.Setup(h => h.HashPassword("NewSecretPass123")).Returns("new_hashed_password");

        var request = new ResetPasswordWithOtpRequestDto("reset@example.com", "777888", "NewSecretPass123");

        // Act
        var result = await _authService.ResetPasswordWithOtpAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var updatedUser = await _dbContext.Users.FirstAsync(u => u.Id == user.Id);
        updatedUser.PasswordHash.Should().Be("new_hashed_password");

        var updatedOtp = await _dbContext.UserOtpCodes.FirstAsync(o => o.Id == otp.Id);
        updatedOtp.IsUsed.Should().BeTrue();
    }

    [Fact]
    public async Task GoogleLoginAsync_WithNewUser_ProvisionsUserWithVerifiedEmailAndDefaultWallet()
    {
        // Arrange
        var request = new GoogleAuthRequestDto("google_id_token", "google.new@example.com", "Google Account User");
        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed_random_pass");
        _jwtTokenGeneratorMock.Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns(("google_jwt_token", DateTime.UtcNow.AddMinutes(60)));
        _jwtTokenGeneratorMock.Setup(j => j.GenerateRefreshToken(It.IsAny<Guid>()))
            .Returns(new RefreshToken(Guid.NewGuid(), "google_refresh_token", DateTime.UtcNow.AddDays(30)));

        // Act
        var result = await _authService.GoogleLoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("google_jwt_token");
        result.Value.User.Email.Should().Be("google.new@example.com");
        result.Value.User.FullName.Should().Be("Google Account User");
        result.Value.User.IsEmailVerified.Should().BeTrue();

        var createdUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "google.new@example.com");
        createdUser.Should().NotBeNull();
        createdUser!.IsEmailVerified.Should().BeTrue();

        var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == createdUser.Id);
        wallet.Should().NotBeNull();
        wallet!.Name.Should().Be("Ví Tiền Mặt");
    }
}

