using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Transactions;

// Request to record a new income transaction
public record CreateIncomeTransactionRequest(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? Note = null,
    bool IsWindfall = false,
    bool IsRecurring = false,
    bool ExcludeFromReport = false
);

// Request to update an existing income transaction
public record UpdateIncomeTransactionRequest(
    Guid WalletId,
    Guid CategoryId,
    decimal Amount,
    DateTime TransactionDate,
    string? Note = null,
    bool IsWindfall = false,
    bool IsRecurring = false,
    bool ExcludeFromReport = false
);

// Query filter parameters for transaction listing
public record TransactionFilterRequest(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    Guid? WalletId = null,
    Guid? CategoryId = null,
    TransactionType? TransactionType = null,
    int Page = 1,
    int PageSize = 50
);
