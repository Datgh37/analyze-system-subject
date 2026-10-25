using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Wallets;

// Request model for creating a new wallet
public record CreateWalletRequest(
    string Name,
    WalletType WalletType,
    decimal InitialBalance,
    string Currency = "VND",
    string? AccountNumber = null,
    string? BankName = null,
    decimal? CreditLimit = null,
    int? StatementDay = null,
    int? PaymentDueDay = null,
    string ColorHex = "#0066FF",
    string IconKey = "wallet",
    bool IsExcludedFromTotal = false
);

// Request model for updating an existing wallet
public record UpdateWalletRequest(
    string Name,
    string? AccountNumber = null,
    string? BankName = null,
    decimal? CreditLimit = null,
    int? StatementDay = null,
    int? PaymentDueDay = null,
    string ColorHex = "#0066FF",
    string IconKey = "wallet",
    bool IsExcludedFromTotal = false
);

// Request model for partially updating an existing wallet (HTTP PATCH RFC 7396)
public record PatchWalletRequest(
    string? Name = null,
    string? ColorHex = null,
    string? IconKey = null,
    bool? IsExcludedFromTotal = null,
    WalletStatus? Status = null
);
