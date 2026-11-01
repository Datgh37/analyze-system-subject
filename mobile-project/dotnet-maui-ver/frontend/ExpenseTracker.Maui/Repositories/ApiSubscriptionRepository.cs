using ExpenseTracker.Maui.Models.SmartUtilities;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

public class ApiSubscriptionRepository : ISubscriptionRepository
{
    private readonly ISubscriptionApi _api;

    public ApiSubscriptionRepository(ISubscriptionApi api)
    {
        _api = api;
    }

    public Task<List<SubscriptionModel>> GetSubscriptionsAsync(int? status = null)
        => _api.GetSubscriptionsAsync(status);

    public Task<SubscriptionModel> GetSubscriptionByIdAsync(Guid id)
        => _api.GetSubscriptionByIdAsync(id);

    public Task<SubscriptionModel> CreateSubscriptionAsync(CreateSubscriptionRequest request)
        => _api.CreateSubscriptionAsync(request);

    public Task<SubscriptionModel> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionRequest request)
        => _api.UpdateSubscriptionAsync(id, request);

    public Task<bool> ArchiveSubscriptionAsync(Guid id)
        => _api.ArchiveSubscriptionAsync(id);

    public Task<SubscriptionAuditReportModel> AuditSubscriptionsAsync()
        => _api.AuditSubscriptionsAsync();

    public Task<SubscriptionModel> ConvertCandidateAsync(CreateSubscriptionRequest request)
        => _api.ConvertCandidateAsync(request);

    public Task<List<UpcomingRenewalModel>> GetUpcomingRenewalsAsync(int daysAhead = 7)
        => _api.GetUpcomingRenewalsAsync(daysAhead);
}

