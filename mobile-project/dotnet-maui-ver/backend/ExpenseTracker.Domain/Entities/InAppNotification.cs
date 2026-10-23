using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class InAppNotification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string Type { get; private set; } = "System"; // BudgetAlert, BalanceUpdate, AiReceiptProcessed, DailyReminder, System
    public string? PayloadJson { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    // Navigation property
    public User? User { get; private set; }

    private InAppNotification() { } // Required by EF Core

    public InAppNotification(
        Guid userId,
        string title,
        string message,
        string type = "System",
        string? payloadJson = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        PayloadJson = payloadJson;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
