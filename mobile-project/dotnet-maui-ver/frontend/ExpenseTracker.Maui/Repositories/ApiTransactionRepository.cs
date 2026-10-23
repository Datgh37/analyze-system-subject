using ExpenseTracker.Maui.Models.Transactions;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Live API implementation of ITransactionRepository delegating to Refit ITransactionApi
public class ApiTransactionRepository : ITransactionRepository
{
    private readonly ITransactionApi _transactionApi;

    public ApiTransactionRepository(ITransactionApi transactionApi)
    {
        _transactionApi = transactionApi;
    }

    public Task<List<TransactionModel>> GetTransactionsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        Guid? walletId = null,
        Guid? categoryId = null,
        int? transactionType = null,
        int page = 1,
        int pageSize = 50) =>
        _transactionApi.GetTransactionsAsync(fromDate, toDate, walletId, categoryId, transactionType, page, pageSize);

    public Task<TransactionModel> GetTransactionByIdAsync(Guid id) =>
        _transactionApi.GetTransactionByIdAsync(id);

    public Task<TransactionModel> CreateIncomeAsync(CreateIncomeModel request) =>
        _transactionApi.CreateIncomeAsync(request);

    public Task<TransactionModel> CreateExpenseAsync(CreateExpenseModel request) =>
        _transactionApi.CreateExpenseAsync(request);

    public Task DeleteIncomeAsync(Guid id) =>
        _transactionApi.DeleteIncomeAsync(id);

    public Task DeleteExpenseAsync(Guid id) =>
        _transactionApi.DeleteExpenseAsync(id);

    public Task<decimal> GetMonthlyIncomeTotalAsync(int month, int year) =>
        _transactionApi.GetMonthlyIncomeTotalAsync(month, year);

    public Task<decimal> GetMonthlyExpenseTotalAsync(int month, int year) =>
        _transactionApi.GetMonthlyExpenseTotalAsync(month, year);
}
