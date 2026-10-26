using ExpenseTracker.Maui.Models.Receipts;
using ExpenseTracker.Maui.Models.Transactions;
using ExpenseTracker.Maui.Services.Api;
using Refit;

namespace ExpenseTracker.Maui.Repositories;

// API implementation of receipt repository delegating to Refit IReceiptApi
public class ApiReceiptRepository : IReceiptRepository
{
    private readonly IReceiptApi _api;

    public ApiReceiptRepository(IReceiptApi api)
    {
        _api = api;
    }

    public Task<ReceiptParsedDataModel> ParseReceiptAsync(StreamPart file) => _api.ParseReceiptAsync(file);
    public Task<TransactionModel> ConfirmReceiptAsync(ConfirmReceiptModel request) => _api.ConfirmReceiptAsync(request);
}
