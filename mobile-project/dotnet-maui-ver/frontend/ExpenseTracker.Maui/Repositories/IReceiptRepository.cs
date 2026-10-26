using ExpenseTracker.Maui.Models.Receipts;
using ExpenseTracker.Maui.Models.Transactions;
using Refit;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for receipt AI vision OCR and ledger confirmation (Clean MVVM Architecture)
public interface IReceiptRepository
{
    Task<ReceiptParsedDataModel> ParseReceiptAsync(StreamPart file);
    Task<TransactionModel> ConfirmReceiptAsync(ConfirmReceiptModel request);
}
