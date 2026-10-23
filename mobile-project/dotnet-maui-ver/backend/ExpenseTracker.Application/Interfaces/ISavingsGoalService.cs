using ExpenseTracker.Application.DTOs.SavingsGoals;

namespace ExpenseTracker.Application.Interfaces;

// Service interface managing smart savings goals, deposits/withdrawals, feasibility checks, and financial jars (UC-06.01 - 06.05)
public interface ISavingsGoalService
{
    Task<List<SavingsGoalDto>> GetUserGoalsAsync(Guid userId, CancellationToken ct = default);
    Task<SavingsGoalDto?> GetGoalByIdAsync(Guid userId, Guid goalId, CancellationToken ct = default);
    Task<SavingsGoalDto> CreateGoalAsync(Guid userId, CreateSavingsGoalDto request, CancellationToken ct = default);
    Task<SavingsGoalDto> UpdateGoalAsync(Guid userId, Guid goalId, UpdateSavingsGoalDto request, CancellationToken ct = default);
    Task DeleteGoalAsync(Guid userId, Guid goalId, CancellationToken ct = default);
    
    // Core transactional operations (UC-06.02, UC-06.03)
    Task<SavingsGoalContributionDto> DepositAsync(Guid userId, Guid goalId, DepositSavingsGoalDto request, CancellationToken ct = default);
    Task<SavingsGoalContributionDto> WithdrawAsync(Guid userId, Guid goalId, WithdrawSavingsGoalDto request, CancellationToken ct = default);
    Task<List<SavingsGoalContributionDto>> GetGoalContributionsAsync(Guid userId, Guid goalId, CancellationToken ct = default);

    // Business intelligence & Financial discipline (UC-06.01, UC-06.05)
    Task<GoalFeasibilityDto> EvaluateGoalFeasibilityAsync(Guid userId, Guid goalId, CancellationToken ct = default);
    Task<JarsSummaryDto> GetJarsSummaryAsync(Guid userId, CancellationToken ct = default);
    Task<SavingsGoalDto> ConfigureAutoDepositAsync(Guid userId, Guid goalId, ConfigureAutoDepositDto request, CancellationToken ct = default);
}
