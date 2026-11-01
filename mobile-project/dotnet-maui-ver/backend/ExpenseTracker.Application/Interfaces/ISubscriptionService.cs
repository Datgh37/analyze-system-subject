using ExpenseTracker.Application.DTOs.SmartUtilities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Interfaces;

// Giao diện dịch vụ quản lý thuê bao định kỳ và động cơ quét bẫy chi tiêu (UC-03.09)
public interface ISubscriptionService
{
    Task<List<SubscriptionDto>> GetSubscriptionsAsync(Guid userId, SubscriptionStatus? status = null, CancellationToken ct = default);
    Task<SubscriptionDto?> GetSubscriptionByIdAsync(Guid userId, Guid id, CancellationToken ct = default);
    Task<SubscriptionDto> CreateSubscriptionAsync(Guid userId, CreateSubscriptionDto request, CancellationToken ct = default);
    Task<SubscriptionDto> UpdateSubscriptionAsync(Guid userId, Guid id, UpdateSubscriptionDto request, CancellationToken ct = default);
    Task<bool> ArchiveSubscriptionAsync(Guid userId, Guid id, CancellationToken ct = default);

    // Audit engine & Intelligent detection
    Task<SubscriptionAuditReportDto> AuditSubscriptionsAsync(Guid userId, CancellationToken ct = default);
    Task<SubscriptionDto> ConvertCandidateToSubscriptionAsync(Guid userId, CreateSubscriptionDto request, CancellationToken ct = default);
    Task<List<UpcomingRenewalDto>> GetUpcomingRenewalsAsync(Guid userId, int daysAhead = 7, CancellationToken ct = default);
}

