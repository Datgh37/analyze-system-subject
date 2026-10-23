using ExpenseTracker.Maui.Models.Behavioral;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

public class ApiNudgeRepository : INudgeRepository
{
    private readonly INudgeApi _api;

    public ApiNudgeRepository(INudgeApi api)
    {
        _api = api;
    }

    public async Task<List<FinancialNudgeModel>> GetActiveNudgesAsync()
    {
        return await _api.GetActiveNudgesAsync();
    }

    public async Task<XaiRationaleModel> GetXaiRationaleAsync(Guid id)
    {
        return await _api.GetXaiRationaleAsync(id);
    }

    public async Task<DynamicRebalanceProposalModel> GenerateRebalanceProposalAsync(Guid deficitCategoryId)
    {
        return await _api.GenerateRebalanceProposalAsync(deficitCategoryId);
    }

    public async Task<bool> ApproveNudgeAsync(Guid id)
    {
        return await _api.ApproveNudgeAsync(id);
    }

    public async Task<bool> DismissNudgeAsync(Guid id)
    {
        return await _api.DismissNudgeAsync(id);
    }
}
