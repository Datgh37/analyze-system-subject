namespace ExpenseTracker.Domain.Enums;

// Indicates the business objective for issuing an OTP verification code
public enum OtpPurpose
{
    EmailVerification = 1,
    PasswordReset = 2,
    PhoneVerification = 3
}
