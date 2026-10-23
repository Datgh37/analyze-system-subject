using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.RecurringIncomes;

// DTO representing recurring income configuration with wallet and category metadata
public record RecurringIncomeDto(
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
    RecurringFrequency Frequency,
    int DayOfMonth,
    bool AutoRecord,
    bool IsActive,
    DateTime NextDueDate,
    DateTime? LastGeneratedDate,
    DateTime CreatedAt
);
