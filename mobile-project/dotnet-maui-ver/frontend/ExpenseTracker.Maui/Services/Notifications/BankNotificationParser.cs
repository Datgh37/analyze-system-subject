using System.Text.RegularExpressions;

namespace ExpenseTracker.Maui.Services.Notifications;

public record BankDeductionResult
{
    public bool IsSuccess { get; init; }
    public decimal Amount { get; init; }
    public string BankName { get; init; } = string.Empty;
    public string ExtractedNote { get; init; } = string.Empty;
    public string RawContent { get; init; } = string.Empty;

    public static BankDeductionResult Failed(string raw = "") => new()
    {
        IsSuccess = false,
        RawContent = raw
    };
}

public static class BankNotificationParser
{
    private static readonly HashSet<string> KnownBankPackages = new(StringComparer.OrdinalIgnoreCase)
    {
        "com.acb.mobile",              // ACB ONE
        "com.mservice.momotransfer",   // Ví MoMo
        "vn.com.vng.zalopay",          // Ví ZaloPay
        "com.VCB",                     // Vietcombank VCB Digibank
        "com.mbmobile",                // MBBank
        "nhan.techcombank.mobile",     // Techcombank Mobile
        "com.techcombank.mobile",      // Techcombank Mobile Alternative
        "vn.com.techcombank.bb.app",   // Techcombank Business/New
        "com.vnpay.bidv",              // BIDV SmartBanking
        "com.vpb.vpbankneo",           // VPBank NEO
        "com.tpb.mb.gprsandroid"       // TPBank Mobile
    };

    public static bool IsSupportedPackage(string packageName)
    {
        return !string.IsNullOrWhiteSpace(packageName) && KnownBankPackages.Contains(packageName);
    }

    public static string ResolveBankName(string packageName)
    {
        return packageName switch
        {
            "com.acb.mobile" => "Ngân hàng ACB",
            "com.mservice.momotransfer" => "Ví MoMo",
            "vn.com.vng.zalopay" => "Ví ZaloPay",
            "com.VCB" => "Vietcombank",
            "com.mbmobile" => "MBBank",
            "nhan.techcombank.mobile" or "com.techcombank.mobile" or "vn.com.techcombank.bb.app" => "Techcombank",
            "com.vnpay.bidv" => "BIDV",
            "com.vpb.vpbankneo" => "VPBank",
            "com.tpb.mb.gprsandroid" => "TPBank",
            _ => "Tài khoản Ngân hàng"
        };
    }

    public static BankDeductionResult Parse(string packageName, string title, string text)
    {
        string fullContent = $"{title} {text}".Trim();
        if (string.IsNullOrWhiteSpace(fullContent))
        {
            return BankDeductionResult.Failed(fullContent);
        }

        // 1. Loại trừ các thông báo cộng tiền hoặc quảng cáo/khuyến mãi
        var incomeOrPromoPattern = @"(?:\bcộng\b|\bnhận được\b|\bhoàn tiền\b|\bđược tặng\b|\bvoucher\b|\bưu đãi\b|\bkhuyến mãi\b|\bvay\b|\bmở thẻ\b)";
        if (Regex.IsMatch(fullContent, incomeOrPromoPattern, RegexOptions.IgnoreCase))
        {
            // Ngoại lệ: nếu nội dung có rõ ràng dấu trừ "- [tiền]" thì vẫn xét tiếp
            if (!Regex.IsMatch(fullContent, @"-\s*[0-9]{1,3}(?:[.,][0-9]{3})+", RegexOptions.IgnoreCase))
            {
                return BankDeductionResult.Failed(fullContent);
            }
        }

        // 2. Nhận diện số tiền bị trừ
        // Ví dụ: -50.000 VND, thanh toan 120,000 đ, GD: -500.000d, SD TK -250,000VND
        var amountRegex = new Regex(@"(?:-|\bchi\b|\btt\b|\bthanh toan\b|\bchuyen khoan\b|\bchuyen tien\b|\brút\b|\bphi\b)?\s*([0-9]{1,3}(?:[.,][0-9]{3})+)\s*(?:vnd|vnđ|d|đ)?", RegexOptions.IgnoreCase);
        var match = amountRegex.Match(fullContent);

        if (!match.Success)
        {
            return BankDeductionResult.Failed(fullContent);
        }

        string amountStr = match.Groups[1].Value.Replace(".", "").Replace(",", "");
        if (!decimal.TryParse(amountStr, out decimal detectedAmount) || detectedAmount <= 0)
        {
            return BankDeductionResult.Failed(fullContent);
        }

        string bankName = ResolveBankName(packageName);

        // 3. Bóc tách nội dung ghi chú (Merchant hoặc nội dung chuyển khoản)
        string extractedNote = string.Empty;
        var noteRegex = new Regex(@"(?:ND:|nội dung:|cho|tại|tai)\s*([^;\n\r]+)", RegexOptions.IgnoreCase);
        var noteMatch = noteRegex.Match(fullContent);
        if (noteMatch.Success)
        {
            extractedNote = noteMatch.Groups[1].Value.Trim().TrimEnd('.');
        }

        if (string.IsNullOrWhiteSpace(extractedNote))
        {
            extractedNote = $"Chi tiêu qua {bankName}";
        }

        return new BankDeductionResult
        {
            IsSuccess = true,
            Amount = detectedAmount,
            BankName = bankName,
            ExtractedNote = extractedNote,
            RawContent = fullContent
        };
    }
}
