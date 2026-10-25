using ExpenseTracker.Maui.Models.AiAgent;

namespace ExpenseTracker.Maui.Repositories.AiAgent;

public class MockVoiceAgentRepository : IVoiceAgentRepository
{
    public Task<VoiceIntentResultModel> ParseIntentAsync(VoiceIntentCommandRequest request)
    {
        string text = request.Transcript.ToLowerInvariant();

        if (text.Contains("danh mục") || text.Contains("mục chi tiêu"))
        {
            var navModel = new VoiceIntentResultModel(
                DetectedIntent: "NavigateTo",
                ActionType: "Navigation",
                TargetRoute: "CreateCategoryPage",
                PrefillData: new Dictionary<string, string>
                {
                    ["categoryName"] = "Nuôi mèo",
                    ["type"] = "Expense"
                },
                ExtractedAmount: null,
                SuggestedCategory: "Nuôi mèo",
                SuggestedWalletId: request.CurrentWalletId,
                ConfirmationMessage: "Đang mở giao diện tạo danh mục mới: 'Nuôi mèo'",
                Confidence: 0.95
            );
            return Task.FromResult(navModel);
        }

        var txModel = new VoiceIntentResultModel(
            DetectedIntent: "AddTransaction",
            ActionType: "Expense",
            TargetRoute: "AddTransactionPage",
            PrefillData: new Dictionary<string, string>
            {
                ["amount"] = "50000",
                ["categoryName"] = "Ăn uống",
                ["type"] = "Expense",
                ["note"] = request.Transcript
            },
            ExtractedAmount: 50000m,
            SuggestedCategory: "Ăn uống",
            SuggestedWalletId: request.CurrentWalletId,
            ConfirmationMessage: "Nhận diện chi tiêu 50.000 VND cho Ăn uống",
            Confidence: 0.92
        );

        return Task.FromResult(txModel);
    }
}
