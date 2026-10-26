using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

// Unit tests validating UserOtpCode entity lifecycle, code validation and email verification
public class UserOtpCodeTests
{
    [Fact]
    public void UserOtpCode_Creation_SetsCorrectPropertiesAndExpiration()
    {
        // Arrange
        var email = "USER@EXAMPLE.COM";
        var otpCode = "123456";
        var validity = TimeSpan.FromMinutes(5);

        // Act
        var otp = new UserOtpCode(email, otpCode, OtpPurpose.EmailVerification, validity);

        // Assert
        otp.Email.Should().Be("user@example.com");
        otp.OtpCode.Should().Be("123456");
        otp.Purpose.Should().Be(OtpPurpose.EmailVerification);
        otp.IsUsed.Should().BeFalse();
        otp.UsedAt.Should().BeNull();
        otp.ExpiredAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(4));
    }

    [Fact]
    public void UserOtpCode_IsValid_WithMatchingCodeAndUnexpired_ReturnsTrue()
    {
        // Arrange
        var otp = new UserOtpCode("user@example.com", "889900", OtpPurpose.PasswordReset, TimeSpan.FromMinutes(10));

        // Act & Assert
        otp.IsValid("889900").Should().BeTrue();
    }

    [Fact]
    public void UserOtpCode_IsValid_WithWrongCode_ReturnsFalse()
    {
        // Arrange
        var otp = new UserOtpCode("user@example.com", "889900", OtpPurpose.PasswordReset, TimeSpan.FromMinutes(10));

        // Act & Assert
        otp.IsValid("112233").Should().BeFalse();
        otp.IsValid("").Should().BeFalse();
        otp.IsValid(null!).Should().BeFalse();
    }

    [Fact]
    public void UserOtpCode_IsValid_WhenAlreadyUsed_ReturnsFalse()
    {
        // Arrange
        var otp = new UserOtpCode("user@example.com", "889900", OtpPurpose.PasswordReset, TimeSpan.FromMinutes(10));
        otp.MarkAsUsed();

        // Act & Assert
        otp.IsUsed.Should().BeTrue();
        otp.UsedAt.Should().NotBeNull();
        otp.IsValid("889900").Should().BeFalse();
    }

    [Fact]
    public void UserOtpCode_IsValid_WhenExpired_ReturnsFalse()
    {
        // Arrange (create expired token with negative duration)
        var otp = new UserOtpCode("user@example.com", "889900", OtpPurpose.PasswordReset, TimeSpan.FromSeconds(-10));

        // Act & Assert
        otp.IsValid("889900").Should().BeFalse();
    }

    [Fact]
    public void User_VerifyEmail_SetsIsEmailVerifiedToTrue()
    {
        // Arrange
        var user = new User("Nguyen Van A", "a@example.com", "hash123", "VND", isEmailVerified: false);
        user.IsEmailVerified.Should().BeFalse();

        // Act
        user.VerifyEmail();

        // Assert
        user.IsEmailVerified.Should().BeTrue();
        user.UpdatedAt.Should().NotBeNull();
    }
}
