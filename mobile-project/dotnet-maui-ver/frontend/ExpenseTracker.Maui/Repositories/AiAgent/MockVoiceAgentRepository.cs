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
                DetectedIntent: "ReceiptOcr",
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

        if (text.Contains("trả nợ") || text.Contains("thanh toán nợ") || text.Contains("sổ nợ") || text.Contains("khoản nợ"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "DebtPayment",
                ActionType: "Expense",
                TargetRoute: "DebtTrackerPage",
                PrefillData: new Dictionary<string, string> { ["note"] = request.Transcript },
                ExtractedAmount: text.Contains("2 triệu") ? 2000000m : 500000m,
                SuggestedCategory: "Trả nợ",
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở Sổ Quản lý Nợ & Kế hoạch Trả nợ",
                Confidence: 0.96
            );
            return Task.FromResult(navModel);
        }

        if (text.Contains("quỹ khẩn cấp") || text.Contains("quỹ dự phòng") || text.Contains("tiền dự phòng") || text.Contains("runway"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "EmergencyFund",
                ActionType: "Navigation",
                TargetRoute: "EmergencyFundPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: "Quỹ khẩn cấp",
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở Báo cáo Quỹ Dự Phòng Khẩn Cấp",
                Confidence: 0.96
            );
            return Task.FromResult(navModel);
        }

        if (text.Contains("bẫy thuê bao") || text.Contains("thuê bao định kỳ") || text.Contains("dịch vụ đăng ký") || text.Contains("subscription"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "SubscriptionAudit",
                ActionType: "Navigation",
                TargetRoute: "SubscriptionsPage",
                PrefillData: new Dictionary<string, string>(),
                ExtractedAmount: null,
                SuggestedCategory: null,
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở Radar Kiểm Toán Bẫy Thuê Bao Định Kỳ",
                Confidence: 0.96
            );
            return Task.FromResult(navModel);
        }

        if (text.Contains("mô phỏng") || text.Contains("kịch bản") || text.Contains("what-if") || text.Contains("thử mua"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "WhatIfSimulation",
                ActionType: "Navigation",
                TargetRoute: "ForecastingPage",
                PrefillData: new Dictionary<string, string> { ["note"] = request.Transcript },
                ExtractedAmount: 30000000m,
                SuggestedCategory: null,
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở Phòng Thí Nghiệm Mô Phỏng Dòng Tiền What-If",
                Confidence: 0.96
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
