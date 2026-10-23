using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.RecurringIncomes;

// Request to create a new recurring income schedule (UC-02.07)
public record CreateRecurringIncomeRequest(
    string Name,
    decimal ExpectedAmount,
    Guid WalletId,
    Guid CategoryId,
    RecurringFrequency Frequency = RecurringFrequency.Monthly,
    int DayOfMonth = 1,
    bool AutoRecord = true,
    DateTime? StartDate = null
);

// Request to update an existing recurring income schedule
public record UpdateRecurringIncomeRequest(
    string Name,
    decimal ExpectedAmount,
    Guid WalletId,
    Guid CategoryId,
    RecurringFrequency Frequency,
    int DayOfMonth,
    bool AutoRecord
);

// Request / parameters to trigger processing of due recurring incomes
public record ProcessRecurringIncomesRequest(
    DateTime? AsOfDate = null
);

// Summary result of running the due recurring income batch processing
public record ProcessRecurringIncomeResultDto(
    int ProcessedCount,
    decimal TotalAmountRecorded,
    IReadOnlyList<Guid> GeneratedTransactionIds,
    string Message
);
