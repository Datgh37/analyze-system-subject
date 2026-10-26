using ExpenseTracker.Maui.Models.SmartUtilities;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock repository for smart financial utilities with realistic subscriptions and VietQR data
public class MockSmartUtilityRepository : ISmartUtilityRepository
{
    private static readonly List<DetectedSubscriptionModel> _subscriptions = new()
    {
        new DetectedSubscriptionModel(
            ServiceName: "Netflix Standard HD",
            CategoryId: Guid.Parse("33333333-3333-3333-3333-333333333306"), // Giải trí
            CategoryName: "Giải trí & Sự kiện",
            RecurringAmount: 260_000m,
            EstimatedAnnualCost: 3_120_000m,
            OccurrencesCount: 6,
            LastChargedDate: DateTime.Now.AddDays(-12),
            AdviceMessage: "Gói cước trừ tiền đều đặn mỗi tháng. Bạn có thể hạ gói sang Basic nếu ít dùng."
        ),
        new DetectedSubscriptionModel(
            ServiceName: "Spotify Premium Family",
            CategoryId: Guid.Parse("33333333-3333-3333-3333-333333333306"),
            CategoryName: "Giải trí & Sự kiện",
            RecurringAmount: 89_000m,
            EstimatedAnnualCost: 1_068_000m,
            OccurrencesCount: 8,
            LastChargedDate: DateTime.Now.AddDays(-5),
            AdviceMessage: "Chi phí hợp lý, tần suất sử dụng nghe nhạc mỗi ngày ổn định."
        ),
        new DetectedSubscriptionModel(
            ServiceName: "iCloud+ 200GB Storage",
            CategoryId: Guid.Parse("33333333-3333-3333-3333-333333333304"), // Hóa đơn & Tiện ích
            CategoryName: "Hóa đơn & Tiện ích",
            RecurringAmount: 69_000m,
            EstimatedAnnualCost: 828_000m,
            OccurrencesCount: 12,
            LastChargedDate: DateTime.Now.AddDays(-18),
            AdviceMessage: "Dung lượng lưu trữ ảnh và tài liệu đám mây gia đình."
        ),
        new DetectedSubscriptionModel(
            ServiceName: "ChatGPT Plus Subscription",
            CategoryId: Guid.Parse("33333333-3333-3333-3333-333333333305"), // Giáo dục & Tri thức
            CategoryName: "Giáo dục & Phát triển",
            RecurringAmount: 520_000m,
            EstimatedAnnualCost: 6_240_000m,
            OccurrencesCount: 4,
            LastChargedDate: DateTime.Now.AddDays(-2),
            AdviceMessage: "Công cụ trợ lý hỗ trợ công việc và học tập lập trình."
        )
    };

    public Task<SubscriptionAnalysisResultModel> DetectSubscriptionsAsync()
    {
        decimal monthlyTotal = _subscriptions.Sum(s => s.RecurringAmount);
        decimal annualTotal = _subscriptions.Sum(s => s.EstimatedAnnualCost);

        var result = new SubscriptionAnalysisResultModel(
            Subscriptions: new List<DetectedSubscriptionModel>(_subscriptions),
            TotalMonthlyDrain: monthlyTotal,
            TotalAnnualDrain: annualTotal,
            FinancialHealthAlert: $"Hệ thống phát hiện {_subscriptions.Count} dịch vụ định kỳ với tổng mức tiêu hao {monthlyTotal:N0} đ/tháng (~{annualTotal:N0} đ/năm). Hãy rà soát định kỳ để tránh rò rỉ dòng tiền."
        );

        return Task.FromResult(result);
    }

    public Task<VietQrResultModel> GenerateVietQrAsync(GenerateVietQrModel request)
    {
        string bankName = request.BankBin switch
        {
            "970422" => "MBBank (Quân Đội)",
            "970415" => "VietinBank (Công Thương)",
            "970436" => "Vietcombank (Ngoại Thương)",
            "970418" => "BIDV (Đầu Tư & Phát Triển)",
            "970407" => "Techcombank",
            _ => "Napas 247 Bank"
        };

        string amountParam = request.Amount.HasValue && request.Amount.Value > 0 ? $"?amount={request.Amount.Value:F0}" : "";
        string noteParam = !string.IsNullOrEmpty(request.Note) ? $"{(string.IsNullOrEmpty(amountParam) ? "?" : "&")}description={Uri.EscapeDataString(request.Note)}" : "";
        string qrImageUrl = $"https://img.vietqr.io/image/{request.BankBin}-{request.AccountNumber}-compact2.png{amountParam}{noteParam}";
        string payload = $"00020101021238540010A00000072701240006{request.BankBin}0110{request.AccountNumber}530370454{request.Amount:F0}5802VN62{request.Note}6304";

        var result = new VietQrResultModel(
            QrImageUrl: qrImageUrl,
            PayloadString: payload,
            BankBin: request.BankBin,
            BankName: bankName,
            AccountNumber: request.AccountNumber,
            AccountName: request.AccountName ?? "CHU TAI KHOAN",
            Amount: request.Amount,
            Note: request.Note
        );

        return Task.FromResult(result);
    }

    public Task<ParsedVoiceExpenseModel> ParseVoiceExpenseAsync(ParseVoiceExpenseModel request)
    {
        var result = new ParsedVoiceExpenseModel(
            Amount: 45_000m,
            SuggestedWalletId: Guid.Parse("11111111-1111-1111-1111-111111111101"),
            SuggestedWalletName: "Ví Tiền Mặt",
            SuggestedCategoryId: Guid.Parse("33333333-3333-3333-3333-333333333301"),
            SuggestedCategoryName: "Ăn uống & Thực phẩm",
            Note: request.Transcript,
            RawTranscript: request.Transcript,
            Confidence: 0.94
        );

        return Task.FromResult(result);
    }
}
