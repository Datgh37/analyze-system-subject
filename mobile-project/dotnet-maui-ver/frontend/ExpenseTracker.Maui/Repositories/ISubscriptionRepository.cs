using ExpenseTracker.Maui.Models.SmartUtilities;

namespace ExpenseTracker.Maui.Repositories;

public interface ISubscriptionRepository
{
    Task<List<SubscriptionModel>> GetSubscriptionsAsync(int? status = null);
    Task<SubscriptionModel> GetSubscriptionByIdAsync(Guid id);
    Task<SubscriptionModel> CreateSubscriptionAsync(CreateSubscriptionRequest request);
    Task<SubscriptionModel> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionRequest request);
    Task<bool> ArchiveSubscriptionAsync(Guid id);
    Task<SubscriptionAuditReportModel> AuditSubscriptionsAsync();
    Task<SubscriptionModel> ConvertCandidateAsync(CreateSubscriptionRequest request);
    Task<List<UpcomingRenewalModel>> GetUpcomingRenewalsAsync(int daysAhead = 7);
}

