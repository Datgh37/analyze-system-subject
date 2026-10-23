using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Aggregate Root for Richard Thaler Behavioral Nudges and Explainable AI (UC-09.01 to UC-09.04)
public class FinancialNudge : BaseEntity
{
    public Guid UserId { get; private set; }
    public NudgeType NudgeType { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string TriggerEvent { get; private set; } = string.Empty;
    public string? RationaleExplanationJson { get; private set; } // Structured XAI rationale (UC-09.02)
    public string? RebalanceProposalJson { get; private set; }   // Structured rebalance plan (UC-09.03)
    public NudgeStatus Status { get; private set; }
    public bool? IsApprovedByUser { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    // Navigation property
    public User? User { get; private set; }

    private FinancialNudge() { } // EF Core constructor

    public FinancialNudge(
        Guid userId,
        NudgeType nudgeType,
        string title,
        string message,
        string triggerEvent,
        string? rationaleExplanationJson = null,
        string? rebalanceProposalJson = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty.", nameof(message));

        Id = Guid.NewGuid();
        UserId = userId;
        NudgeType = nudgeType;
        Title = title.Trim();
        Message = message.Trim();
        TriggerEvent = triggerEvent?.Trim() ?? string.Empty;
        RationaleExplanationJson = rationaleExplanationJson;
        RebalanceProposalJson = rebalanceProposalJson;
        Status = NudgeStatus.Active;
        IsApprovedByUser = null;
        CreatedAt = DateTime.UtcNow;
    }

    // UC-09.04: Human-in-the-loop approval of AI recommendation
    public void Approve()
    {
        if (Status != NudgeStatus.Active)
            throw new InvalidOperationException($"Cannot approve nudge in '{Status}' status.");

        Status = NudgeStatus.Resolved;
        IsApprovedByUser = true;
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // UC-09.04: User dismisses recommendation
    public void Dismiss()
    {
        if (Status != NudgeStatus.Active)
            throw new InvalidOperationException($"Cannot dismiss nudge in '{Status}' status.");

        Status = NudgeStatus.Dismissed;
        IsApprovedByUser = false;
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
