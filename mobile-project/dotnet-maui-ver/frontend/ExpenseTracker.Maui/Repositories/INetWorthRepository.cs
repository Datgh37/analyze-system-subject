using ExpenseTracker.Maui.Models.Reports;

namespace ExpenseTracker.Maui.Repositories;

public interface INetWorthRepository
{
    Task<NetWorthBreakdownModel> GetCurrentNetWorthAsync();
    Task<List<NetWorthHistoryItemModel>> GetNetWorthHistoryAsync(int limit = 12);
    Task<NetWorthHistoryItemModel> RecordSnapshotAsync(RecordNetWorthSnapshotRequest request);
}

