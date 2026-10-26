using ExpenseTracker.Maui.Models.SmartUtilities;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for smart financial utilities: Subscriptions, VietQR Napas 247, Voice NLP (UC-03.09 - 03.11)
public interface ISmartUtilityRepository
{
    Task<SubscriptionAnalysisResultModel> DetectSubscriptionsAsync();
    Task<VietQrResultModel> GenerateVietQrAsync(GenerateVietQrModel request);
    Task<ParsedVoiceExpenseModel> ParseVoiceExpenseAsync(ParseVoiceExpenseModel request);
}
