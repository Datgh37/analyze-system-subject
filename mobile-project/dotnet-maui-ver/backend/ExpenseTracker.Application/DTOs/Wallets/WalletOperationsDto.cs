namespace ExpenseTracker.Application.DTOs.Wallets;

// Request model for executing an atomic internal transfer between two wallets
public record TransferMoneyRequest(
    Guid SourceWalletId,
    Guid DestinationWalletId,
    decimal Amount,
    DateTime TransferDate,
    string? Note = null
);

// Request model for manual or OCR-assisted balance reconciliation
public record AdjustBalanceRequest(
    decimal NewBalance,
    string? Reason = null,
    DateTime? AdjustmentDate = null,
    string? ProofImageUrl = null
);

// Data transfer object representing balance reconciliation history
public record BalanceAdjustmentDto(
    Guid Id,
    Guid WalletId,
    decimal OldBalance,
    decimal NewBalance,
    decimal DifferenceAmount,
    string? Reason,
    DateTime AdjustmentDate,
    string? ProofImageUrl,
    DateTime CreatedAt
);

// Result model from AI/OCR scanning of account balance screenshots
public record OcrBalanceScanResponse(
    decimal DetectedBalance,
    double Confidence,
    string? DetectedBankName,
    string? DetectedAccountNumber,
    string RawText
);
