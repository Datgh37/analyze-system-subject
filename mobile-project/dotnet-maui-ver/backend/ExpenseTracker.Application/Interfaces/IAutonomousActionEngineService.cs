using ExpenseTracker.Application.DTOs.Autonomous;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service contract for Server-Driven Action Protocol and Jev Probabilistic Decision Gating (Chặng 10.5 / PLAN-47)
public interface IAutonomousActionEngineService
{
    // Retrieve all active pending autonomous actions in user's action inbox
    Task<Result<AutonomousActionInboxResponse>> GetPendingActionsAsync(Guid userId, CancellationToken ct = default);

    // Process a raw incoming bank or e-wallet notification captured from Android Native Service
    Task<Result<AutonomousActionDto?>> ProcessRawBankNotificationAsync(Guid userId, RawBankNotificationRequest request, CancellationToken ct = default);

    // Commit a 1-tap quick action (creating transaction, adjusting wallet, removing action from pending)
    Task<Result<AutonomousActionDto>> CommitQuickActionAsync(Guid userId, QuickCommitActionRequest request, CancellationToken ct = default);

    // Dismiss an action from inbox
    Task<Result<bool>> DismissActionAsync(Guid userId, string actionId, CancellationToken ct = default);
}
