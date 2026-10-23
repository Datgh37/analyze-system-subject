using ExpenseTracker.Maui.Models.Behavioral;

namespace ExpenseTracker.Maui.Repositories;

// Clean MVVM Repository Contract for Thaler Behavioral Nudges (UC-09.01 -> UC-09.04)
public interface INudgeRepository
{
    Task<List<FinancialNudgeModel>> GetActiveNudgesAsync();
    Task<XaiRationaleModel> GetXaiRationaleAsync(Guid id);
    Task<DynamicRebalanceProposalModel> GenerateRebalanceProposalAsync(Guid deficitCategoryId);
    Task<bool> ApproveNudgeAsync(Guid id);
    Task<bool> DismissNudgeAsync(Guid id);
}
