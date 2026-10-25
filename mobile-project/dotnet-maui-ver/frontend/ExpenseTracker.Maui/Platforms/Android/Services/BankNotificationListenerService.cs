#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using Android.Service.Notification;
using System.Text.RegularExpressions;

namespace ExpenseTracker.Maui.Platforms.Android.Services;

// Android Native Notification Listener Service (UC-03.12 Prototype)
// Listens for balance variation notifications from target banking & e-wallet applications
[Service(Label = "BankNotificationListenerService",
    Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",
    Exported = true)]
[IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
public class BankNotificationListenerService : NotificationListenerService
{
    // Target Banking & E-Wallet Application Package Identifiers
    private static readonly HashSet<string> TargetBankPackages = new(StringComparer.OrdinalIgnoreCase)
    {
        "com.acb.mobile",              // ACB ONE
        "com.mservice.momotransfer",   // MoMo
        "vn.com.vng.zalopay",          // ZaloPay
        "com.VCB",                     // Vietcombank VCB Digibank
        "com.mbmobile"                 // MBBank
    };

    public override void OnNotificationPosted(StatusBarNotification? sbn)
    {
        if (sbn == null || string.IsNullOrWhiteSpace(sbn.PackageName))
            return;

        if (!TargetBankPackages.Contains(sbn.PackageName))
            return;

        var extras = sbn.Notification?.Extras;
        if (extras == null)
            return;

        string title = extras.GetCharSequence(Notification.ExtraTitle)?.ToString() ?? string.Empty;
        string text = extras.GetCharSequence(Notification.ExtraText)?.ToString() ?? string.Empty;
        string fullContent = $"{title} {text}";

        // Filter: Detect balance deduction patterns (e.g. "-50,000 VND", "GD: -100.000d", "Thanh toán 45.000")
        var match = Regex.Match(fullContent, @"(?:-|\bchi\b|\btt\b|\bthanh toan\b)?\s*([0-9]{1,3}(?:[.,][0-9]{3})+)\s*(?:vnd|vnđ|d|đ)?", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            string amountStr = match.Groups[1].Value.Replace(".", "").Replace(",", "");
            if (decimal.TryParse(amountStr, out decimal detectedAmount) && detectedAmount > 0)
            {
                string bankName = ResolveBankName(sbn.PackageName);
                System.Diagnostics.Debug.WriteLine($"[BankNotificationListener] Phát hiện biến động số dư từ {bankName}: -{detectedAmount:N0} VND ({text})");
            }
        }
    }

    private static string ResolveBankName(string packageName)
    {
        return packageName switch
        {
            "com.acb.mobile" => "Ngân hàng ACB",
            "com.mservice.momotransfer" => "Ví MoMo",
            "vn.com.vng.zalopay" => "Ví ZaloPay",
            "com.VCB" => "Vietcombank",
            "com.mbmobile" => "MBBank",
            _ => "Ngân hàng"
        };
    }
}
#endif
