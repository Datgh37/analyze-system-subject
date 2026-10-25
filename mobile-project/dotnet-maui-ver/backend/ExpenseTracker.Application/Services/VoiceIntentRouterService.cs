using System.Text.Json;
using System.Text.RegularExpressions;
using ExpenseTracker.Application.DTOs.AiAgent;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Services;

// Implementation of Voice-Driven Intent Router Agent
// Built on Hybrid AI architecture: Local Fast-Path & Hardened Regex Engine + Cloud Smart-Path Google Gemini 2.5 Flash NLU
public partial class VoiceIntentRouterService : IVoiceIntentRouterService
{
    private readonly IAutoCategorizationService _autoRuleService;
    private readonly IGeminiApiClient? _geminiClient;

    public VoiceIntentRouterService(
        IAutoCategorizationService autoRuleService,
        IGeminiApiClient? geminiClient = null)
    {
        _autoRuleService = autoRuleService;
        _geminiClient = geminiClient;
    }

    public async Task<Result<VoiceIntentResultDto>> RouteVoiceCommandAsync(
        Guid userId,
        VoiceIntentCommandRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Transcript))
            return Result<VoiceIntentResultDto>.Failure("Nội dung giọng nói trống. Vui lòng nói rõ yêu cầu.");

        string transcript = request.Transcript.Trim();

        // 1. CLOUD SMART-PATH: Try Gemini 2.5 Flash NLU if client is available
        if (_geminiClient != null)
        {
            try
            {
                var geminiResult = await TryProcessWithGeminiNluAsync(userId, transcript, request.CurrentWalletId, ct);
                if (geminiResult != null)
                {
                    return Result<VoiceIntentResultDto>.Success(geminiResult);
                }
            }
            catch
            {
                // Resilient Fallback: If Gemini is offline or encounters transient issues, seamlessly drop down to Hardened Regex Engine
            }
        }

        // 2. LOCAL FAST-PATH / HARDENED SAFE FALLBACK: Process via comprehensive Vietnamese Regex & Heuristics
        return await ProcessWithHardenedRegexEngineAsync(userId, transcript, request.CurrentWalletId, ct);
    }

    // Cloud Smart-Path: Gemini 2.5 Flash NLU for deep semantic parsing of complex conversational voice transcripts
    private async Task<VoiceIntentResultDto?> TryProcessWithGeminiNluAsync(
        Guid userId,
        string transcript,
        Guid? currentWalletId,
        CancellationToken ct)
    {
        if (_geminiClient == null) return null;

        const string systemPrompt = """
        Bạn là trợ lý điều hướng giọng nói tài chính cá nhân thông minh (Voice Intent Router AI Agent).
        Nhiệm vụ: Phân tích khẩu lệnh tiếng Việt tự nhiên của người dùng (có thể chứa tiếng lóng tiền tệ: '50k', '2 củ rưỡi', 'năm xị', 'hai trẹo', '1 lít', 'nửa lít', 'ba chục', 'đi chợ', 'trả tiền trọ', 'mở báo cáo', 'xem ngân sách', 'tạo quỹ tiết kiệm mua xe', 'tạo danh mục Nuôi Mèo'...).
        
        YÊU CẦU ĐỊNH DẠNG:
        Trả về DUY NHẤT một chuỗi JSON thuần túy (không kèm markdown code block, không có lời dẫn) theo schema:
        {
          "detectedIntent": "AddTransaction" | "NavigateTo" | "QueryStatus",
          "actionType": "Expense" | "Income" | "Navigation" | "Query",
          "targetRoute": "AddTransactionPage" | "CreateCategoryPage" | "CreateGoalPage" | "AnalyticsReportPage" | "BudgetHubPage" | "DebtTrackerPage" | "NudgeFeedPage" | "CoolingOffHubPage" | "WalletsPage" | "ReceiptOcrPage",
          "extractedAmount": 0.0,
          "suggestedCategory": "Tên danh mục",
          "confirmationMessage": "Lời phản hồi thân thiện, xác nhận ngắn gọn",
          "confidence": 0.95,
          "prefillData": {
            "amount": "50000",
            "categoryName": "Ăn uống",
            "type": "Expense",
            "note": "Nội dung ghi chú"
          }
        }
        """;

        string userPrompt = $"Khẩu lệnh người dùng: \"{transcript}\"";

        var response = await _geminiClient.GenerateTextAsync(userPrompt, systemPrompt, ct);
        if (response.IsFailure || string.IsNullOrWhiteSpace(response.Value))
            return null;

        string cleanJson = CleanJsonMarkdown(response.Value);

        using var doc = JsonDocument.Parse(cleanJson);
        var root = doc.RootElement;

        string intent = root.TryGetProperty("detectedIntent", out var ip) ? ip.GetString() ?? VoiceIntents.AddTransaction : VoiceIntents.AddTransaction;
        string actionType = root.TryGetProperty("actionType", out var at) ? at.GetString() ?? "Expense" : "Expense";
        string? targetRoute = root.TryGetProperty("targetRoute", out var tr) ? tr.GetString() : "AddTransactionPage";
        string confirmMsg = root.TryGetProperty("confirmationMessage", out var cm) ? cm.GetString() ?? $"Đã ghi nhận: '{transcript}'" : $"Đã ghi nhận: '{transcript}'";
        double confidence = root.TryGetProperty("confidence", out var cf) && cf.TryGetDouble(out double cd) ? cd : 0.95;

        decimal? amount = null;
        if (root.TryGetProperty("extractedAmount", out var am) && am.ValueKind != JsonValueKind.Null && am.TryGetDecimal(out decimal av))
            amount = av;

        string? suggestedCat = root.TryGetProperty("suggestedCategory", out var sc) ? sc.GetString() : null;

        var prefillDict = new Dictionary<string, string>();
        if (root.TryGetProperty("prefillData", out var pf) && pf.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in pf.EnumerateObject())
            {
                prefillDict[prop.Name] = prop.Value.ToString();
            }
        }

        // Ensure essential fields exist if intent is AddTransaction
        if (intent == VoiceIntents.AddTransaction && amount.HasValue && !prefillDict.ContainsKey("amount"))
        {
            prefillDict["amount"] = amount.Value.ToString("G29");
            prefillDict["categoryName"] = suggestedCat ?? "Chi tiêu khác";
            prefillDict["type"] = actionType;
            prefillDict["note"] = transcript;
        }

        return new VoiceIntentResultDto(
            DetectedIntent: intent,
            ActionType: actionType,
            TargetRoute: targetRoute,
            PrefillData: prefillDict,
            ExtractedAmount: amount,
            SuggestedCategory: suggestedCat,
            SuggestedWalletId: currentWalletId,
            ConfirmationMessage: confirmMsg,
            Confidence: confidence
        );
    }

    // Hardened Safe-Path: 100% Offline-capable Regex & Heuristic Engine for Vietnamese Spoken Transcripts
    private async Task<Result<VoiceIntentResultDto>> ProcessWithHardenedRegexEngineAsync(
        Guid userId,
        string transcript,
        Guid? currentWalletId,
        CancellationToken ct)
    {
        string lower = transcript.ToLowerInvariant();

        // 1. Navigation Intents across all system modules
        if (lower.Contains("tạo danh mục") || lower.Contains("mục chi tiêu mới") || lower.Contains("thêm danh mục"))
        {
            string categoryName = ExtractCategoryNameFromCommand(transcript);
            return Result<VoiceIntentResultDto>.Success(new VoiceIntentResultDto(
                DetectedIntent: VoiceIntents.NavigateTo,
                ActionType: "Navigation",
                TargetRoute: "CreateCategoryPage",
                PrefillData: new Dictionary<string, string> { ["categoryName"] = categoryName, ["type"] = "Expense" },
                ExtractedAmount: null,
                SuggestedCategory: categoryName,
                SuggestedWalletId: currentWalletId,
                ConfirmationMessage: $"Đang mở giao diện tạo danh mục mới: '{categoryName}'",
                Confidence: 0.96
            ));
        }

        if (lower.Contains("tạo mục tiêu") || lower.Contains("quỹ tiết kiệm") || lower.Contains("mục tiêu mới"))
        {
            return Result<VoiceIntentResultDto>.Success(new VoiceIntentResultDto(
                DetectedIntent: VoiceIntents.NavigateTo,
                ActionType: "Navigation",
                TargetRoute: "CreateGoalPage",
                PrefillData: new Dictionary<string, string> { ["goalName"] = "Mục tiêu tích lũy mới" },
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: currentWalletId,
                ConfirmationMessage: "Đang mở giao diện tạo Mục tiêu tích lũy mới",
                Confidence: 0.95
            ));
        }

        if (lower.Contains("xem báo cáo") || lower.Contains("báo cáo tài chính") || lower.Contains("phân tích chi tiêu") || lower.Contains("báo cáo thu chi") || lower.Contains("mở báo cáo"))
        {
            return Result<VoiceIntentResultDto>.Success(new VoiceIntentResultDto(
                DetectedIntent: VoiceIntents.NavigateTo,
                ActionType: "Navigation",
                TargetRoute: "AnalyticsReportPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: currentWalletId,
                ConfirmationMessage: "Đang chuyển đến màn hình Báo cáo & Phân tích tài chính",
                Confidence: 0.98
            ));
        }

        if (lower.Contains("ngân sách") || lower.Contains("hạn mức") || lower.Contains("thiết lập ngân sách"))
        {
            return Result<VoiceIntentResultDto>.Success(new VoiceIntentResultDto(
                DetectedIntent: VoiceIntents.NavigateTo,
                ActionType: "Navigation",
                TargetRoute: "BudgetHubPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: currentWalletId,
                ConfirmationMessage: "Đang chuyển đến Trung tâm Quản lý Ngân sách & Hạn mức",
                Confidence: 0.95
            ));
        }

        if (lower.Contains("sổ nợ") || lower.Contains("khoản nợ") || lower.Contains("khoản vay") || lower.Contains("trả nợ") || lower.Contains("danh sách nợ"))
        {
            return Result<VoiceIntentResultDto>.Success(new VoiceIntentResultDto(
                DetectedIntent: VoiceIntents.NavigateTo,
                ActionType: "Navigation",
                TargetRoute: "DebtTrackerPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: currentWalletId,
                ConfirmationMessage: "Đang mở Sổ Quản lý Nợ & Kế hoạch Trả nợ",
                Confidence: 0.95
            ));
        }

        if (lower.Contains("cú hích") || lower.Contains("thaler") || lower.Contains("lời khuyên") || lower.Contains("gợi ý tài chính"))
        {
            return Result<VoiceIntentResultDto>.Success(new VoiceIntentResultDto(
                DetectedIntent: VoiceIntents.NavigateTo,
                ActionType: "Navigation",
                TargetRoute: "NudgeFeedPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: currentWalletId,
                ConfirmationMessage: "Đang mở Bảng tin Cú Hích Tài Chính Thaler Nudge",
                Confidence: 0.95
            ));
        }

        if (lower.Contains("khoảng trễ") || lower.Contains("danh sách chờ") || lower.Contains("chờ mua") || lower.Contains("cooling off"))
        {
            return Result<VoiceIntentResultDto>.Success(new VoiceIntentResultDto(
                DetectedIntent: VoiceIntents.NavigateTo,
                ActionType: "Navigation",
                TargetRoute: "CoolingOffHubPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: currentWalletId,
                ConfirmationMessage: "Đang mở Không gian Khoảng Trễ 24h Cooling-Off",
                Confidence: 0.95
            ));
        }

        if (lower.Contains("quản lý ví") || lower.Contains("danh sách ví") || lower.Contains("tài khoản thanh toán") || lower.Contains("mở ví"))
        {
            return Result<VoiceIntentResultDto>.Success(new VoiceIntentResultDto(
                DetectedIntent: VoiceIntents.NavigateTo,
                ActionType: "Navigation",
                TargetRoute: "WalletsPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: currentWalletId,
                ConfirmationMessage: "Đang mở Danh sách Ví & Tài khoản",
                Confidence: 0.95
            ));
        }

        if (lower.Contains("quét hóa đơn") || lower.Contains("chụp hóa đơn") || lower.Contains("ocr hóa đơn") || lower.Contains("bóc tách hóa đơn"))
        {
            return Result<VoiceIntentResultDto>.Success(new VoiceIntentResultDto(
                DetectedIntent: VoiceIntents.NavigateTo,
                ActionType: "Navigation",
                TargetRoute: "ReceiptOcrPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: currentWalletId,
                ConfirmationMessage: "Đang mở máy ảnh quét bóc tách hóa đơn AI",
                Confidence: 0.96
            ));
        }

        // 2. Transaction Commands (Income vs Expense)
        bool isIncome = lower.Contains("nhận lương") || lower.Contains("thu nhập") || lower.Contains("thưởng") || lower.Contains("nhận tiền") || lower.Contains("tiền vào");
        decimal? amount = ParseVietnameseSlangAmount(lower);

        // Attempt category auto-matching based on keywords in transcript
        var catMatch = await _autoRuleService.MatchCategoryAsync(userId, transcript, ct);
        string? suggestedCat = catMatch.Value?.CategoryName;
        if (string.IsNullOrWhiteSpace(suggestedCat))
        {
            if (lower.Contains("đi chợ") || lower.Contains("ăn") || lower.Contains("cafe") || lower.Contains("uống") || lower.Contains("bánh mì") || lower.Contains("trà đá"))
                suggestedCat = "Ăn uống";
            else if (lower.Contains("xăng") || lower.Contains("xe") || lower.Contains("grab") || lower.Contains("xe ôm") || lower.Contains("taxi"))
                suggestedCat = "Đi lại";
            else if (lower.Contains("tiền nhà") || lower.Contains("trọ") || lower.Contains("điện") || lower.Contains("nước") || lower.Contains("internet"))
                suggestedCat = "Hóa đơn & Tiện ích";
            else if (lower.Contains("mua sắm") || lower.Contains("quần áo") || lower.Contains("shopee") || lower.Contains("lazada"))
                suggestedCat = "Mua sắm";
            else
                suggestedCat = isIncome ? "Lương" : "Chi tiêu khác";
        }

        var prefillData = new Dictionary<string, string>
        {
            ["amount"] = (amount ?? 0m).ToString("G29"),
            ["categoryName"] = suggestedCat,
            ["type"] = isIncome ? "Income" : "Expense",
            ["note"] = transcript
        };

        string confirmMsg = amount.HasValue
            ? $"Nhận diện: {(isIncome ? "Thu nhập" : "Chi tiêu")} {amount.Value:N0} VND cho '{suggestedCat}'"
            : $"Đã ghi nhận yêu cầu: '{transcript}'. Vui lòng xác nhận thông tin.";

        var txResult = new VoiceIntentResultDto(
            DetectedIntent: VoiceIntents.AddTransaction,
            ActionType: isIncome ? "Income" : "Expense",
            TargetRoute: "AddTransactionPage",
            PrefillData: prefillData,
            ExtractedAmount: amount,
            SuggestedCategory: suggestedCat,
            SuggestedWalletId: currentWalletId,
            ConfirmationMessage: confirmMsg,
            Confidence: amount.HasValue ? 0.94 : 0.75
        );

        return Result<VoiceIntentResultDto>.Success(txResult);
    }

    private static string ExtractCategoryNameFromCommand(string transcript)
    {
        var patterns = new[]
        {
            @"(?:tạo|thêm)\s+danh\s+mục(?:\s+chi\s+tiêu)?(?:\s+mới)?(?:\s+về|\s+tên)?\s+(.+)$",
            @"mục\s+chi\s+tiêu\s+mới\s+(?:về\s+)?(.+)$"
        };

        foreach (var pat in patterns)
        {
            var match = Regex.Match(transcript, pat, RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 1)
            {
                string raw = match.Groups[1].Value.Trim(' ', '.', ',', '!');
                if (!string.IsNullOrWhiteSpace(raw))
                    return char.ToUpper(raw[0]) + raw[1..];
            }
        }

        return "Danh mục mới";
    }

    // Comprehensive Vietnamese slang amount parser supporting all regional dialects
    public static decimal? ParseVietnameseSlangAmount(string text)
    {
        // 1. Spoken number words in Vietnamese
        if (text.Contains("năm trăm ngàn") || text.Contains("năm trăm nghìn")) return 500_000m;
        if (text.Contains("hai trăm ngàn") || text.Contains("hai trăm nghìn")) return 200_000m;
        if (text.Contains("một trăm ngàn") || text.Contains("một trăm nghìn")) return 100_000m;
        if (text.Contains("năm mươi ngàn") || text.Contains("năm mươi nghìn")) return 50_000m;
        if (text.Contains("hai mươi ngàn") || text.Contains("hai mươi nghìn")) return 20_000m;
        if (text.Contains("một triệu")) return 1_000_000m;

        // 2. Northern dialect "lít" (1 lít = 100k, 5 lít = 500k, nửa lít = 50k)
        if (text.Contains("nửa lít")) return 50_000m;
        if (text.Contains("năm lít") || text.Contains("5 lít")) return 500_000m;
        if (text.Contains("hai lít") || text.Contains("2 lít")) return 200_000m;
        if (text.Contains("một lít") || text.Contains("1 lít")) return 100_000m;
        var litMatch = Regex.Match(text, @"(\d+)\s*lít", RegexOptions.IgnoreCase);
        if (litMatch.Success && decimal.TryParse(litMatch.Groups[1].Value, out decimal litCount))
            return litCount * 100_000m;

        // 3. Southern dialect "xị" (1 xị = 100k, 5 xị = 500k)
        if (text.Contains("năm xị") || text.Contains("5 xị")) return 500_000m;
        if (text.Contains("hai xị") || text.Contains("2 xị")) return 200_000m;
        if (text.Contains("một xị") || text.Contains("1 xị")) return 100_000m;
        var xiMatch = Regex.Match(text, @"(\d+)\s*xị", RegexOptions.IgnoreCase);
        if (xiMatch.Success && decimal.TryParse(xiMatch.Groups[1].Value, out decimal xiCount))
            return xiCount * 100_000m;

        // 4. Tens unit "chục" (3 chục = 30k, 5 chục = 50k, ba chục = 30k)
        if (text.Contains("năm chục") || text.Contains("5 chục")) return 50_000m;
        if (text.Contains("ba chục") || text.Contains("3 chục")) return 30_000m;
        if (text.Contains("hai chục") || text.Contains("2 chục")) return 20_000m;
        if (text.Contains("bốn chục") || text.Contains("4 chục")) return 40_000m;
        var chucMatch = Regex.Match(text, @"(\d+)\s*chục", RegexOptions.IgnoreCase);
        if (chucMatch.Success && decimal.TryParse(chucMatch.Groups[1].Value, out decimal chucCount))
            return chucCount * 10_000m;

        // 5. Millions: "củ", "triệu", "trẹo"
        // Case: "2 trẹo rưỡi" or "2 triệu rưỡi" or "2 củ rưỡi"
        var rưỡiMatch = Regex.Match(text, @"(\d+)\s*(?:triệu|trẹo|củ)\s*rưỡi", RegexOptions.IgnoreCase);
        if (rưỡiMatch.Success && decimal.TryParse(rưỡiMatch.Groups[1].Value, out decimal rưỡiBase))
            return rưỡiBase * 1_000_000m + 500_000m;

        // Case: "2 triệu 5" or "2 trẹo 500" or "1 củ 2"
        var splitMatch = Regex.Match(text, @"(\d+)\s*(?:triệu|trẹo|củ)\s*(\d+)", RegexOptions.IgnoreCase);
        if (splitMatch.Success && decimal.TryParse(splitMatch.Groups[1].Value, out decimal mBase) && decimal.TryParse(splitMatch.Groups[2].Value, out decimal mSub))
        {
            if (mSub < 10) mSub *= 100_000m;
            else if (mSub < 100) mSub *= 10_000m;
            else if (mSub < 1000) mSub *= 1_000m;
            return mBase * 1_000_000m + mSub;
        }

        // Case: "2 triệu", "2 trẹo", "2 củ"
        var milMatch = Regex.Match(text, @"(\d+(?:[.,]\d+)?)\s*(?:triệu|trẹo|củ|tr\b)", RegexOptions.IgnoreCase);
        if (milMatch.Success)
        {
            string numStr = milMatch.Groups[1].Value.Replace(',', '.');
            if (decimal.TryParse(numStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal mVal))
                return mVal * 1_000_000m;
        }

        // 6. Thousands: "k", "ka", "nghìn", "ngàn"
        var kMatch = Regex.Match(text, @"(\d+(?:[.,]\d+)?)\s*(?:k\b|ka\b|nghìn|ngàn)", RegexOptions.IgnoreCase);
        if (kMatch.Success)
        {
            string numStr = kMatch.Groups[1].Value.Replace(',', '.');
            if (decimal.TryParse(numStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal kVal))
                return kVal * 1_000m;
        }

        // 7. Standalone formatted number: e.g. "50,000" or "50000"
        var plainMatch = Regex.Match(text, @"\b(\d{1,3}(?:[.,]\d{3})+|\d{4,})\b");
        if (plainMatch.Success)
        {
            string clean = plainMatch.Groups[1].Value.Replace(".", "").Replace(",", "");
            if (decimal.TryParse(clean, out decimal plainVal))
                return plainVal;
        }

        return null;
    }

    private static string CleanJsonMarkdown(string raw)
    {
        string trimmed = raw.Trim();
        if (trimmed.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            trimmed = trimmed[7..].Trim();
        else if (trimmed.StartsWith("```"))
            trimmed = trimmed[3..].Trim();

        if (trimmed.EndsWith("```"))
            trimmed = trimmed[..^3].Trim();

        return trimmed;
    }
}
