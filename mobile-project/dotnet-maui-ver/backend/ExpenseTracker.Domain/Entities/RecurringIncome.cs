using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Recurring income aggregate root managing scheduled recurring income streams (UC-02.07)
public class RecurringIncome : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;
    public decimal ExpectedAmount { get; private set; }

    public Guid WalletId { get; private set; }
    public Wallet Wallet { get; private set; } = null!;

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public RecurringFrequency Frequency { get; private set; } = RecurringFrequency.Monthly;
    public int DayOfMonth { get; private set; } = 1;
    public bool AutoRecord { get; private set; } = true;
    public bool IsActive { get; private set; } = true;
    public DateTime NextDueDate { get; private set; }
    public DateTime? LastGeneratedDate { get; private set; }

    // Parameterless constructor for EF Core materialization
    private RecurringIncome() { }

    public RecurringIncome(
        Guid userId,
        string name,
        decimal expectedAmount,
        Guid walletId,
        Guid categoryId,
        RecurringFrequency frequency,
        int dayOfMonth,
        bool autoRecord = true,
        DateTime? startDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên khoản thu định kỳ không được để trống.", nameof(name));

        if (expectedAmount <= 0)
            throw new ArgumentException("Số tiền dự kiến phải lớn hơn 0.", nameof(expectedAmount));

        if (dayOfMonth < 1 || dayOfMonth > 31)
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "Ngày nhận tiền phải từ 1 đến 31.");

        UserId = userId;
        Name = name.Trim();
        ExpectedAmount = expectedAmount;
        WalletId = walletId;
        CategoryId = categoryId;
        Frequency = frequency;
        DayOfMonth = dayOfMonth;
        AutoRecord = autoRecord;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        var baseDate = startDate ?? DateTime.UtcNow;
        NextDueDate = CalculateNextDueDate(baseDate);
    }

    // Calculates the next due date with short month alignment (e.g., Feb 28/29, April 30 for day 31)
    public DateTime CalculateNextDueDate(DateTime fromDate)
    {
        var fromUtc = fromDate.Kind == DateTimeKind.Utc ? fromDate : DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);

        switch (Frequency)
        {
            case RecurringFrequency.Daily:
                return fromUtc.Date.AddDays(1);

            case RecurringFrequency.Weekly:
                return fromUtc.Date.AddDays(7);

            case RecurringFrequency.Monthly:
            {
                int targetYear = fromUtc.Year;
                int targetMonth = fromUtc.Month;

                // If fromDate's day is already on or past the target day, move to next month
                if (fromUtc.Day >= DayOfMonth)
                {
                    targetMonth++;
                    if (targetMonth > 12)
                    {
                        targetMonth = 1;
                        targetYear++;
                    }
                }

                // Short month alignment (UC-02.07 E1)
                int maxDaysInTargetMonth = DateTime.DaysInMonth(targetYear, targetMonth);
                int adjustedDay = Math.Min(DayOfMonth, maxDaysInTargetMonth);

                return new DateTime(targetYear, targetMonth, adjustedDay, 0, 0, 0, DateTimeKind.Utc);
            }

            case RecurringFrequency.Quarterly:
            {
                var nextQuarter = fromUtc.Date.AddMonths(3);
                int maxDays = DateTime.DaysInMonth(nextQuarter.Year, nextQuarter.Month);
                int adjustedDay = Math.Min(DayOfMonth, maxDays);
                return new DateTime(nextQuarter.Year, nextQuarter.Month, adjustedDay, 0, 0, 0, DateTimeKind.Utc);
            }

            case RecurringFrequency.Yearly:
            {
                var nextYear = fromUtc.Date.AddYears(1);
                int maxDays = DateTime.DaysInMonth(nextYear.Year, nextYear.Month);
                int adjustedDay = Math.Min(DayOfMonth, maxDays);
                return new DateTime(nextYear.Year, nextYear.Month, adjustedDay, 0, 0, 0, DateTimeKind.Utc);
            }

            default:
                return fromUtc.Date.AddMonths(1);
        }
    }

    // Marks recurring income as processed and advances next due date
    public void MarkProcessed(DateTime executionDate)
    {
        LastGeneratedDate = executionDate;
        NextDueDate = CalculateNextDueDate(executionDate);
        UpdatedAt = DateTime.UtcNow;
    }

    // Toggles active state (UC-02.07 A1)
    public void ToggleStatus()
    {
        IsActive = !IsActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // Updates recurring income schedule configuration
    public void UpdateDetails(
        string name,
        decimal expectedAmount,
        Guid walletId,
        Guid categoryId,
        RecurringFrequency frequency,
        int dayOfMonth,
        bool autoRecord)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên khoản thu định kỳ không được để trống.", nameof(name));

        if (expectedAmount <= 0)
            throw new ArgumentException("Số tiền dự kiến phải lớn hơn 0.", nameof(expectedAmount));

        if (dayOfMonth < 1 || dayOfMonth > 31)
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "Ngày nhận tiền phải từ 1 đến 31.");

        Name = name.Trim();
        ExpectedAmount = expectedAmount;
        WalletId = walletId;
        CategoryId = categoryId;
        Frequency = frequency;
        DayOfMonth = dayOfMonth;
        AutoRecord = autoRecord;
        UpdatedAt = DateTime.UtcNow;
        NextDueDate = CalculateNextDueDate(DateTime.UtcNow);
    }
}
