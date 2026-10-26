using ExpenseTracker.Maui.Models.SavingsGoals;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for savings goals and Kakeibo financial jars (Clean MVVM Architecture)
public interface ISavingsGoalRepository
{
    Task<List<SavingsGoalSummaryModel>> GetSavingsGoalsAsync(SavingsGoalStatus? status = null);
    Task<SavingsGoalModel> GetSavingsGoalByIdAsync(Guid id);
    Task<SavingsGoalModel> CreateSavingsGoalAsync(CreateSavingsGoalModel request);
    Task<SavingsGoalModel> UpdateSavingsGoalAsync(Guid id, UpdateSavingsGoalModel request);
    Task<SavingsGoalModel> PatchSavingsGoalAsync(Guid id, PatchSavingsGoalModel request);
    Task<SavingsGoalModel> PatchGoalStatusAsync(Guid id, PatchSavingsGoalStatusModel request);
    Task DeleteSavingsGoalAsync(Guid id);
    Task<SavingsGoalContributionModel> DepositAsync(Guid id, DepositSavingsGoalModel request);
    Task<SavingsGoalContributionModel> WithdrawAsync(Guid id, WithdrawSavingsGoalModel request);
    Task<List<SavingsGoalContributionModel>> GetGoalContributionsAsync(Guid id);
    Task<SavingsGoalModel> ConfigureAutoDepositAsync(Guid id, ConfigureAutoDepositModel request);
    Task<SavingsFeasibilityModel> GetGoalFeasibilityAsync(Guid id);
    Task<JarsSummaryModel> GetJarsSummaryAsync();
}
