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
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        _authService = new AuthService(
            _dbContext,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object,
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
}
