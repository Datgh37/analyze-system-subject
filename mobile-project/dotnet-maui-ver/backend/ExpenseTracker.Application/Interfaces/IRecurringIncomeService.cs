using ExpenseTracker.Application.DTOs.RecurringIncomes;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service contract for managing scheduled recurring income streams (UC-02.07)
public interface IRecurringIncomeService
{
    Task<Result<List<RecurringIncomeDto>>> GetRecurringIncomesAsync(Guid userId, CancellationToken ct = default);
    Task<Result<RecurringIncomeDto>> GetRecurringIncomeByIdAsync(Guid userId, Guid id, CancellationToken ct = default);
    Task<Result<RecurringIncomeDto>> CreateRecurringIncomeAsync(Guid userId, CreateRecurringIncomeRequest request, CancellationToken ct = default);
    Task<Result<RecurringIncomeDto>> UpdateRecurringIncomeAsync(Guid userId, Guid id, UpdateRecurringIncomeRequest request, CancellationToken ct = default);
    Task<Result<RecurringIncomeDto>> ToggleRecurringIncomeStatusAsync(Guid userId, Guid id, CancellationToken ct = default);
    Task<Result<bool>> DeleteRecurringIncomeAsync(Guid userId, Guid id, CancellationToken ct = default);
    Task<Result<ProcessRecurringIncomeResultDto>> ProcessDueRecurringIncomesAsync(Guid? userId = null, DateTime? asOfDate = null, CancellationToken ct = default);
}
