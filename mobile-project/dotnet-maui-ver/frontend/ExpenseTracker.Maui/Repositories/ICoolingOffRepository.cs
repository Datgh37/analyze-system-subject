using ExpenseTracker.Maui.Models.Behavioral;

namespace ExpenseTracker.Maui.Repositories;

// Clean MVVM Repository Contract for 24h Positive Friction Cooling-Off Mechanism (UC-09.05)
public interface ICoolingOffRepository
{
    Task<CoolingOffEvaluationResultModel> EvaluateExpenseAsync(CoolingOffEvaluationRequestModel request);
    Task<CoolingOffTransactionModel> CreateCoolingOffAsync(CreateCoolingOffRequestModel request);
    Task<List<CoolingOffTransactionModel>> GetPendingCoolingOffAsync();
    Task<bool> ConfirmPurchaseAsync(Guid id);
    Task<bool> CancelPurchaseAsync(Guid id);
    Task<bool> BypassCoolingOffAsync(Guid id, BypassCoolingOffRequestModel request);
}
