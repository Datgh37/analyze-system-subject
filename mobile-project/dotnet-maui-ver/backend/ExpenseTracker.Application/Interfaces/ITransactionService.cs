using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service contract for financial transaction ledger operations and ACID wallet synchronization
public interface ITransactionService
{
    Task<Result<List<TransactionDto>>> GetTransactionsAsync(Guid userId, TransactionFilterRequest filter, CancellationToken ct = default);
    Task<Result<TransactionDto>> GetTransactionByIdAsync(Guid userId, Guid transactionId, CancellationToken ct = default);
    Task<Result<TransactionDto>> CreateIncomeAsync(Guid userId, CreateIncomeTransactionRequest request, CancellationToken ct = default);
    Task<Result<TransactionDto>> UpdateIncomeAsync(Guid userId, Guid transactionId, UpdateIncomeTransactionRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteIncomeAsync(Guid userId, Guid transactionId, CancellationToken ct = default);
    Task<Result<decimal>> GetTotalIncomeAmountAsync(Guid userId, int month, int year, CancellationToken ct = default);

    Task<Result<TransactionDto>> CreateExpenseAsync(Guid userId, CreateExpenseTransactionRequest request, CancellationToken ct = default);
    Task<Result<TransactionDto>> UpdateExpenseAsync(Guid userId, Guid transactionId, UpdateExpenseTransactionRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteExpenseAsync(Guid userId, Guid transactionId, CancellationToken ct = default);
    Task<Result<ExpenseDetailDto>> GetExpenseDetailsAsync(Guid userId, Guid transactionId, CancellationToken ct = default);
    Task<Result<decimal>> GetTotalExpenseAmountAsync(Guid userId, int month, int year, CancellationToken ct = default);
}
