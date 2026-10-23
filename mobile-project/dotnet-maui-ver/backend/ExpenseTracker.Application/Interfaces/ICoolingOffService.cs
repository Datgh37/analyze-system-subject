using ExpenseTracker.Domain.Common;
using ExpenseTracker.Application.DTOs.Nudges;

namespace ExpenseTracker.Application.Interfaces;

// Application contract for Friction-by-Design Cooling-Off Period (UC-09.05)
public interface ICoolingOffService
{
    Task<Result<CoolingOffEvaluationResultDto>> EvaluateExpenseForCoolingOffAsync(Guid userId, CoolingOffEvaluationRequestDto request, CancellationToken ct = default);
    Task<Result<CoolingOffTransactionDto>> CreateCoolingOffTransactionAsync(Guid userId, CreateCoolingOffRequestDto request, CancellationToken ct = default);
    Task<Result<List<CoolingOffTransactionDto>>> GetPendingCoolingOffTransactionsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<bool>> ConfirmPurchaseAsync(Guid userId, Guid coolingOffId, CancellationToken ct = default);
    Task<Result<bool>> CancelPurchaseAsync(Guid userId, Guid coolingOffId, CancellationToken ct = default);
    Task<Result<bool>> BypassCoolingOffAsync(Guid userId, Guid coolingOffId, BypassCoolingOffRequestDto request, CancellationToken ct = default);
}
