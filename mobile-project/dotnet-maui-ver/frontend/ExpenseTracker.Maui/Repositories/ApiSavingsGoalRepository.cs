using ExpenseTracker.Maui.Models.SavingsGoals;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Production API repository connecting to backend ASP.NET Core WebApi
public class ApiSavingsGoalRepository : ISavingsGoalRepository
{
    private readonly ISavingsGoalApi _api;

    public ApiSavingsGoalRepository(ISavingsGoalApi api)
    {
        _api = api;
    }

    public Task<List<SavingsGoalSummaryModel>> GetSavingsGoalsAsync(SavingsGoalStatus? status = null)
        => _api.GetSavingsGoalsAsync(status);

    public Task<SavingsGoalModel> GetSavingsGoalByIdAsync(Guid id)
        => _api.GetSavingsGoalByIdAsync(id);

    public Task<SavingsGoalModel> CreateSavingsGoalAsync(CreateSavingsGoalModel request)
        => _api.CreateSavingsGoalAsync(request);

    public Task<SavingsGoalModel> UpdateSavingsGoalAsync(Guid id, UpdateSavingsGoalModel request)
        => _api.UpdateSavingsGoalAsync(id, request);

    public Task<SavingsGoalModel> PatchSavingsGoalAsync(Guid id, PatchSavingsGoalModel request)
        => _api.PatchSavingsGoalAsync(id, request);

    public Task<SavingsGoalModel> PatchGoalStatusAsync(Guid id, PatchSavingsGoalStatusModel request)
        => _api.PatchGoalStatusAsync(id, request);

    public Task DeleteSavingsGoalAsync(Guid id)
        => _api.DeleteSavingsGoalAsync(id);

    public Task<SavingsGoalContributionModel> DepositAsync(Guid id, DepositSavingsGoalModel request)
        => _api.DepositAsync(id, request);

    public Task<SavingsGoalContributionModel> WithdrawAsync(Guid id, WithdrawSavingsGoalModel request)
        => _api.WithdrawAsync(id, request);

    public Task<List<SavingsGoalContributionModel>> GetGoalContributionsAsync(Guid id)
        => _api.GetGoalContributionsAsync(id);

    public Task<SavingsGoalModel> ConfigureAutoDepositAsync(Guid id, ConfigureAutoDepositModel request)
        => _api.ConfigureAutoDepositAsync(id, request);

    public Task<SavingsFeasibilityModel> GetGoalFeasibilityAsync(Guid id)
        => _api.GetGoalFeasibilityAsync(id);

    public Task<JarsSummaryModel> GetJarsSummaryAsync()
        => _api.GetJarsSummaryAsync();
}
