using ExpenseTracker.Domain.Common;
using ExpenseTracker.Application.DTOs.Nudges;

namespace ExpenseTracker.Application.Interfaces;

// Application contract for Richard Thaler Behavioral Nudge Engine & Explainable AI (UC-09.01 to UC-09.04)
public interface INudgeEngineService
{
    Task<Result<List<FinancialNudgeDto>>> GetActiveNudgesAsync(Guid userId, CancellationToken ct = default);
    Task<Result<XaiRationaleDto>> GetNudgeRationaleAsync(Guid userId, Guid nudgeId, CancellationToken ct = default);
    Task<Result<DynamicRebalanceProposalDto>> GenerateRebalanceProposalAsync(Guid userId, Guid? deficitCategoryId = null, CancellationToken ct = default);
    Task<Result<bool>> ApproveNudgeAsync(Guid userId, Guid nudgeId, CancellationToken ct = default);
    Task<Result<bool>> DismissNudgeAsync(Guid userId, Guid nudgeId, CancellationToken ct = default);
    Task EvaluateTriggersOnExpenseAsync(Guid userId, decimal expenseAmount, Guid categoryId, CancellationToken ct = default);
    Task EvaluateTriggersOnIncomeAsync(Guid userId, decimal incomeAmount, CancellationToken ct = default);
}
