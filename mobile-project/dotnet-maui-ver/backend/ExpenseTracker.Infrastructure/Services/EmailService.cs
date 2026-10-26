using System.Net;
using System.Net.Mail;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Services;

// Email delivery service supporting production SMTP relay with a resilient dev console fallback
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        var smtpHost = _configuration["Smtp:Host"];
        var smtpPortStr = _configuration["Smtp:Port"];
        var smtpUser = _configuration["Smtp:Username"];
        var smtpPass = _configuration["Smtp:Password"];
        var fromEmail = _configuration["Smtp:FromEmail"] ?? "no-reply@duonopro.vn";
        var fromName = _configuration["Smtp:FromName"] ?? "DuoNoPro Finance";
        var enableSsl = bool.TryParse(_configuration["Smtp:EnableSsl"], out var ssl) ? ssl : true;
        var port = int.TryParse(smtpPortStr, out var p) ? p : 587;

        var hasSmtpConfig = !string.IsNullOrWhiteSpace(smtpHost) &&
                            !string.IsNullOrWhiteSpace(smtpUser) &&
                            !string.IsNullOrWhiteSpace(smtpPass);

        if (hasSmtpConfig)
        {
            try
            {
                using var client = new SmtpClient(smtpHost, port)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = enableSsl,
                    Timeout = 10000 // 10 seconds timeout
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage, cancellationToken);
                _logger.LogInformation("Successfully sent email via SMTP to {ToEmail} with subject '{Subject}'", toEmail, subject);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to dispatch email via SMTP to {ToEmail}. Activating resilient dev fallback logger.", toEmail);
            }
        }

        // Resilient Fallback: Log email details cleanly to server console/outbox
        LogEmailToOutbox(fromName, fromEmail, toEmail, subject, body);
        await Task.CompletedTask;
    }

    public async Task SendOtpEmailAsync(string toEmail, string otpCode, OtpPurpose purpose, CancellationToken cancellationToken = default)
    {
        var (subject, purposeText) = purpose switch
        {
            OtpPurpose.EmailVerification => (
                $"[DuoNoPro] Mã kích hoạt tài khoản: {otpCode}",
                "kích hoạt tài khoản người dùng mới"
            ),
            OtpPurpose.PasswordReset => (
                $"[DuoNoPro] Mã OTP đặt lại mật khẩu: {otpCode}",
                "khôi phục và đặt lại mật khẩu mới"
            ),
            OtpPurpose.PhoneVerification => (
                $"[DuoNoPro] Mã xác thực số điện thoại: {otpCode}",
                "xác thực số điện thoại liên kết"
            ),
            _ => (
                $"[DuoNoPro] Mã xác thực OTP: {otpCode}",
                "xác thực thông tin tài khoản"
            )
        };

        var body =
            $"Xin chào,\n\n" +
            $"Bạn (hoặc ai đó) vừa yêu cầu {purposeText} trên ứng dụng DuoNoPro.\n\n" +
            $"Mã OTP xác thực 6 số của bạn là:\n\n" +
            $"      ========================\n" +
            $"      >>>   [ {otpCode} ]   <<<\n" +
            $"      ========================\n\n" +
            $"Lưu ý quan trọng:\n" +
            $"- Mã xác thực này có hiệu lực trong vòng 5 phút kể từ thời điểm gửi.\n" +
            $"- Tuyệt đối KHÔNG chia sẻ mã này cho bất kỳ ai, kể cả nhân viên hỗ trợ.\n" +
            $"- Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.\n\n" +
            $"Trân trọng,\n" +
            $"Đội ngũ DuoNoPro Personal Finance";

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    private void LogEmailToOutbox(string fromName, string fromEmail, string toEmail, string subject, string body)
    {
        _logger.LogInformation(
            "\n" +
            "================================================================================\n" +
            "📨 [EMAIL SERVICE OUTBOX - CONSOLE DISPATCH]\n" +
            "From:    {FromName} <{FromEmail}>\n" +
            "To:      {ToEmail}\n" +
            "Subject: {Subject}\n" +
            "Time:    {Timestamp:yyyy-MM-dd HH:mm:ss UTC}\n" +
            "--------------------------------------------------------------------------------\n" +
            "{Body}\n" +
            "================================================================================",
            fromName, fromEmail, toEmail, subject, DateTime.UtcNow, body);
    }
}
