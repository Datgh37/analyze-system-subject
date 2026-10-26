using System.Text.RegularExpressions;
using ExpenseTracker.Maui.Models.AiAgent;

namespace ExpenseTracker.Maui.Repositories.AiAgent;

public class MockVoiceAgentRepository : IVoiceAgentRepository
{
    public Task<VoiceIntentResultModel> ParseIntentAsync(VoiceIntentCommandRequest request)
    {
        string text = request.Transcript.ToLowerInvariant().Trim();

        // 1. Navigation intents
        if (text.Contains("báo cáo") || text.Contains("phân tích"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "NavigateTo",
                ActionType: "Navigation",
                TargetRoute: "ReportsPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở màn hình Báo cáo & Phân tích tài chính",
                Confidence: 0.98
            );
            return Task.FromResult(navModel);
        }

        if (text.Contains("ngân sách") || text.Contains("hạn mức"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "NavigateTo",
                ActionType: "Navigation",
                TargetRoute: "BudgetsPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở màn hình Ngân sách & Hạn mức",
                Confidence: 0.96
            );
            return Task.FromResult(navModel);
        }

        if (text.Contains("danh mục") || text.Contains("mục chi tiêu"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "NavigateTo",
                ActionType: "Navigation",
                TargetRoute: "CategoriesPage",
                PrefillData: new Dictionary<string, string>
                {
                    ["categoryName"] = "Nuôi mèo",
                    ["type"] = "Expense"
                },
                ExtractedAmount: null,
                SuggestedCategory: "Nuôi mèo",
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở Cây danh mục thu chi",
                Confidence: 0.95
            );
            return Task.FromResult(navModel);
        }

        if (text.Contains("quét hóa đơn") || text.Contains("ocr") || text.Contains("chụp hóa đơn"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "NavigateTo",
                ActionType: "Navigation",
                TargetRoute: "ReceiptOcrPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở máy ảnh quét bóc tách hóa đơn AI",
                Confidence: 0.97
            );
            return Task.FromResult(navModel);
        }

        if (text.Contains("ví") || text.Contains("tài khoản"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "NavigateTo",
                ActionType: "Navigation",
                TargetRoute: "WalletsPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở Quản lý Đa Ví",
                Confidence: 0.96
            );
            return Task.FromResult(navModel);
        }

        // 2. Expense / Transaction Intents
        decimal amount = 50000m;
        var kMatch = Regex.Match(text, @"(\d+)\s*k\b");
        if (kMatch.Success && decimal.TryParse(kMatch.Groups[1].Value, out var kVal))
        {
            amount = kVal * 1000m;
        }
        else
        {
            var numMatch = Regex.Match(text, @"\b(\d{4,})\b");
            if (numMatch.Success && decimal.TryParse(numMatch.Groups[1].Value, out var nVal))
            {
                amount = nVal;
            }
        }

        string category = "Ăn uống";
        if (text.Contains("xăng") || text.Contains("grab") || text.Contains("xe") || text.Contains("taxi"))
        {
            category = "Đi lại";
        }
        else if (text.Contains("mua sắm") || text.Contains("shopee") || text.Contains("áo") || text.Contains("quần"))
        {
            category = "Mua sắm";
        }

        var txModel = new VoiceIntentResultModel(
            DetectedIntent: "AddTransaction",
            ActionType: "Expense",
            TargetRoute: "ExpensesPage",
            PrefillData: new Dictionary<string, string>
            {
                ["amount"] = amount.ToString("G29"),
                ["categoryName"] = category,
                ["type"] = "Expense",
                ["note"] = request.Transcript
            },
            ExtractedAmount: amount,
            SuggestedCategory: category,
            SuggestedWalletId: request.CurrentWalletId,
            ConfirmationMessage: $"Nhận diện chi tiêu {amount:N0} đ cho '{category}'",
            Confidence: 0.95
        );

        return Task.FromResult(txModel);
    }
}
