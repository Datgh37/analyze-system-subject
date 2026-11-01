using ExpenseTracker.Application.DTOs.Reports;

namespace ExpenseTracker.Application.Interfaces;

// Service interface calculating real-time balance sheet Net Worth and historical snapshots (UC-07.02)
public interface INetWorthService
{
    Task<NetWorthBreakdownDto> GetCurrentNetWorthAsync(Guid userId, CancellationToken ct = default);
    Task<List<NetWorthHistoryItemDto>> GetNetWorthHistoryAsync(Guid userId, int limit = 12, CancellationToken ct = default);
    Task<NetWorthHistoryItemDto> RecordSnapshotAsync(Guid userId, RecordNetWorthSnapshotDto? request = null, CancellationToken ct = default);
}

