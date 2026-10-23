using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Wallets;

// Data transfer object representing wallet summary and presentation attributes
public record WalletDto(
    Guid Id,
    Guid UserId,
    string Name,
    WalletType WalletType,
    decimal Balance,
    decimal InitialBalance,
    string Currency,
    string? AccountNumber,
    string? BankName,
    decimal? CreditLimit,
    int? StatementDay,
    int? PaymentDueDay,
    string ColorHex,
    string IconKey,
    int DisplayOrder,
    bool IsExcludedFromTotal,
    WalletStatus Status,
    int Version,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
