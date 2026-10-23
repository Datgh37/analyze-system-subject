using ExpenseTracker.Maui.Models.Transactions;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for transaction & expense operations (Clean MVVM Architecture)
public interface ITransactionRepository
{
    Task<List<TransactionModel>> GetTransactionsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        Guid? walletId = null,
        Guid? categoryId = null,
        int? transactionType = null,
        int page = 1,
        int pageSize = 50);

    Task<TransactionModel> GetTransactionByIdAsync(Guid id);
    Task<TransactionModel> CreateIncomeAsync(CreateIncomeModel request);
    Task<TransactionModel> CreateExpenseAsync(CreateExpenseModel request);
    Task DeleteIncomeAsync(Guid id);
    Task DeleteExpenseAsync(Guid id);
    Task<decimal> GetMonthlyIncomeTotalAsync(int month, int year);
    Task<decimal> GetMonthlyExpenseTotalAsync(int month, int year);
}
