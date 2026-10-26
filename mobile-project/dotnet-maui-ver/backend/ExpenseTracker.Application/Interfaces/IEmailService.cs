using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Interfaces;

// Service contract responsible for dispatching system emails and OTP verification messages
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
    Task SendOtpEmailAsync(string toEmail, string otpCode, OtpPurpose purpose, CancellationToken cancellationToken = default);
}
