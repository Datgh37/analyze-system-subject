namespace ExpenseTracker.Maui.Models.RecurringIncomes;

// Recurring income presentation model matching backend DTO (UC-02.07)
public record RecurringIncomeModel(
    Guid Id,
    Guid UserId,
    string Name,
    decimal ExpectedAmount,
    Guid WalletId,
    string WalletName,
    Guid CategoryId,
    string CategoryName,
    string CategoryIcon,
    string CategoryColor,
    int Frequency,
    int DayOfMonth,
    bool AutoRecord,
    bool IsActive,
    DateTime NextDueDate,
    DateTime? LastGeneratedDate,
    DateTime CreatedAt
);

public record CreateRecurringIncomeModel(
    string Name,
    decimal ExpectedAmount,
    Guid WalletId,
    Guid CategoryId,
    int Frequency = 3, // Monthly
    int DayOfMonth = 1,
    bool AutoRecord = true,
    DateTime? StartDate = null
);

public record UpdateRecurringIncomeModel(
    string Name,
    decimal ExpectedAmount,
    Guid WalletId,
    Guid CategoryId,
    int Frequency,
    int DayOfMonth,
    bool AutoRecord
);

public record ProcessRecurringIncomesModel(
    DateTime? AsOfDate = null
);

public record ProcessRecurringIncomeResultModel(
    int ProcessedCount,
    decimal TotalAmountRecorded,
    IReadOnlyList<Guid> GeneratedTransactionIds,
    string Message
);
