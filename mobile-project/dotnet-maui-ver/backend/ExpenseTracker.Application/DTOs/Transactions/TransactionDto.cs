using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Transactions;

// DTO representing financial transaction details with wallet and category references
public record TransactionDto(
    Guid Id,
    Guid UserId,
    Guid WalletId,
    string WalletName,
    Guid CategoryId,
    string CategoryName,
    string CategoryIcon,
    string CategoryColor,
    decimal Amount,
    TransactionType TransactionType,
    DateTime TransactionDate,
    string? Note,
    bool IsEssential,
    bool IsWindfall,
    bool IsRecurring,
    bool ExcludeFromReport,
    DateTime CreatedAt
);
