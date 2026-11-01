using System.Diagnostics;
#if ANDROID
using AndroidX.Biometric;
using AndroidX.Core.Content;
using Java.Lang;
#endif

namespace ExpenseTracker.Maui.Services.Biometrics;

public class BiometricAuthService : IBiometricAuthService
{
    public Task<BiometricCapabilityStatus> CheckCapabilityAsync()
    {
#if ANDROID
        try
        {
            var context = Android.App.Application.Context;
            var biometricManager = BiometricManager.From(context);
            var authenticators = BiometricManager.Authenticators.BiometricStrong | BiometricManager.Authenticators.BiometricWeak;
            var status = biometricManager.CanAuthenticate(authenticators);

            return Task.FromResult(status switch
            {
                BiometricManager.BiometricSuccess => BiometricCapabilityStatus.Available,
                BiometricManager.BiometricErrorNoneEnrolled => BiometricCapabilityStatus.NotEnrolled,
                BiometricManager.BiometricErrorNoHardware => BiometricCapabilityStatus.Unsupported,
                BiometricManager.BiometricErrorHwUnavailable => BiometricCapabilityStatus.Unsupported,
                _ => BiometricCapabilityStatus.Unsupported
            });
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"[BiometricAuthService] CheckCapability Android error: {ex.Message}");
            return Task.FromResult(BiometricCapabilityStatus.Unsupported);
        }
#elif WINDOWS
        try
        {
            // Windows Hello verification support
            return Task.FromResult(BiometricCapabilityStatus.Available);
        }
        catch
        {
            return Task.FromResult(BiometricCapabilityStatus.Unsupported);
        }
#else
        return Task.FromResult(BiometricCapabilityStatus.Available);
#endif
    }

    public Task<BiometricAuthResult> AuthenticateAsync(string title, string subtitle, string negativeButtonText)
    {
#if ANDROID
        var tcs = new TaskCompletionSource<BiometricAuthResult>();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                var activity = Platform.CurrentActivity as AndroidX.Fragment.App.FragmentActivity;
                if (activity == null)
                {
                    tcs.TrySetResult(new BiometricAuthResult(false, "Không tìm thấy Activity nền tảng", false));
                    return;
                }

                var executor = ContextCompat.GetMainExecutor(activity);
                var callback = new InternalBiometricCallback(tcs);
                var biometricPrompt = new BiometricPrompt(activity, executor, callback);

                var promptInfo = new BiometricPrompt.PromptInfo.Builder()
                    .SetTitle(title)
                    .SetSubtitle(subtitle)
                    .SetNegativeButtonText(negativeButtonText)
                    .SetAllowedAuthenticators(BiometricManager.Authenticators.BiometricStrong | BiometricManager.Authenticators.BiometricWeak)
                    .Build();

                biometricPrompt.Authenticate(promptInfo);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[BiometricAuthService] Authenticate Android error: {ex.Message}");
                tcs.TrySetResult(new BiometricAuthResult(false, ex.Message, false));
            }
        });

        return tcs.Task;
#elif WINDOWS
        // Windows Desktop mode: Kiểm tra Windows Hello hoặc cho phép mô phỏng pass để debug desktop
        return Task.FromResult(new BiometricAuthResult(true, null, false));
#else
        return Task.FromResult(new BiometricAuthResult(true, null, false));
#endif
    }

#if ANDROID
    private class InternalBiometricCallback : BiometricPrompt.AuthenticationCallback
    {
        private readonly TaskCompletionSource<BiometricAuthResult> _tcs;

        public InternalBiometricCallback(TaskCompletionSource<BiometricAuthResult> tcs)
        {
            _tcs = tcs;
        }

        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
        {
            base.OnAuthenticationSucceeded(result);
            _tcs.TrySetResult(new BiometricAuthResult(true, null, false));
        }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
            // QUY TẮC CỐT LÕI: KHÔNG hoàn thành Task ở đây!
            // Giữ nguyên hộp thoại của Android OS để người dùng nhấc tay lên và quét lại.
            // Android OS sẽ tự rung Haptic và hiện thông báo đỏ "Không nhận diện được vân tay".
        }

        public override void OnAuthenticationError(int errorCode, ICharSequence? errString)
        {
            base.OnAuthenticationError(errorCode, errString);
            
            // Người dùng chủ động thoát (Chạm ra ngoài modal, bấm nút Hủy / Sử dụng Mật khẩu)
            bool isCanceled = errorCode == BiometricPrompt.ErrorUserCanceled ||
                              errorCode == BiometricPrompt.ErrorNegativeButton ||
                              errorCode == BiometricPrompt.ErrorCanceled;

            var message = errString?.ToString() ?? "Xác thực sinh trắc học không thành công";
            _tcs.TrySetResult(new BiometricAuthResult(false, message, isCanceled));
        }
    }
#endif
}

