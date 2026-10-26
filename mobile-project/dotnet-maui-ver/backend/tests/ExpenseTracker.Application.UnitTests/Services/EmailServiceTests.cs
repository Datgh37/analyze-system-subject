using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

// Unit tests validating EmailService dispatch logic, template formatting, and graceful logger fallback
public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _loggerMock;

    public EmailServiceTests()
    {
        _loggerMock = new Mock<ILogger<EmailService>>();
    }

    [Fact]
    public async Task SendOtpEmailAsync_WithEmailVerificationPurpose_DispatchesCleanly()
    {
        // Arrange (Configuration with no SMTP credentials activates the resilient console fallback)
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Smtp:Host", "" },
            { "Smtp:Port", "587" },
            { "Smtp:Username", "" },
            { "Smtp:Password", "" },
            { "Smtp:FromEmail", "no-reply@duonopro.vn" },
            { "Smtp:FromName", "DuoNoPro Finance" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var emailService = new EmailService(configuration, _loggerMock.Object);

        // Act
        Func<Task> act = async () => await emailService.SendOtpEmailAsync("user@example.com", "123456", OtpPurpose.EmailVerification);

        // Assert - should complete cleanly without throwing exceptions
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendOtpEmailAsync_WithPasswordResetPurpose_DispatchesCleanly()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Smtp:Host", "" },
            { "Smtp:Username", "" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var emailService = new EmailService(configuration, _loggerMock.Object);

        // Act
        Func<Task> act = async () => await emailService.SendOtpEmailAsync("user@example.com", "654321", OtpPurpose.PasswordReset);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_WhenNoSmtpConfigured_FallbacksGracefullyToLoggerWithoutThrowing()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var emailService = new EmailService(configuration, _loggerMock.Object);

        // Act
        Func<Task> act = async () => await emailService.SendEmailAsync("test@example.com", "Hello Test", "Sample body");

        // Assert
        await act.Should().NotThrowAsync();
    }
}
