using ExpenseTracker.Maui.Models.Reports;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

public class ApiNetWorthRepository : INetWorthRepository
{
    private readonly INetWorthApi _api;

    public ApiNetWorthRepository(INetWorthApi api)
    {
        _api = api;
    }

    public Task<NetWorthBreakdownModel> GetCurrentNetWorthAsync()
        => _api.GetCurrentNetWorthAsync();

    public Task<List<NetWorthHistoryItemModel>> GetNetWorthHistoryAsync(int limit = 12)
        => _api.GetNetWorthHistoryAsync(limit);

    public Task<NetWorthHistoryItemModel> RecordSnapshotAsync(RecordNetWorthSnapshotRequest request)
        => _api.RecordSnapshotAsync(request);
}

