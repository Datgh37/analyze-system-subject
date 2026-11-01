namespace ExpenseTracker.Maui.Services.Biometrics;

public enum BiometricCapabilityStatus
{
    Available,       // Thiết bị có cảm biến và người dùng đã đăng ký vân tay/FaceID
    NotEnrolled,     // Có cảm biến nhưng chưa cài đặt vân tay trong hệ thống
    Unsupported,     // Thiết bị không hỗ trợ phần cứng sinh trắc học
    Denied           // Bị từ chối cấp quyền
}

public record BiometricAuthResult(
    bool IsSuccess,
    string? ErrorMessage,
    bool IsUserCanceled);

public interface IBiometricAuthService
{
    Task<BiometricCapabilityStatus> CheckCapabilityAsync();
    Task<BiometricAuthResult> AuthenticateAsync(string title, string subtitle, string negativeButtonText);
}

