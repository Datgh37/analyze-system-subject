using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

// Snapshot entity recording a user's balance sheet point-in-time net worth (UC-07.02)
public class NetWorthSnapshot : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public DateTime SnapshotDate { get; private set; }
    public decimal TotalAssets { get; private set; }
    public decimal TotalLiabilities { get; private set; }
    public decimal NetWorth { get; private set; }
    public string? Notes { get; private set; }

    // Parameterless constructor for EF Core materialization
    private NetWorthSnapshot() { }

    public NetWorthSnapshot(
        Guid userId,
        DateTime snapshotDate,
        decimal totalAssets,
        decimal totalLiabilities,
        string? notes = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Mã người dùng không hợp lệ.", nameof(userId));

        if (totalAssets < 0)
            throw new ArgumentException("Tổng tài sản không được là số âm.", nameof(totalAssets));

        if (totalLiabilities < 0)
            throw new ArgumentException("Tổng nợ không được là số âm.", nameof(totalLiabilities));

        UserId = userId;
        SnapshotDate = snapshotDate.Kind == DateTimeKind.Utc
            ? snapshotDate.Date
            : DateTime.SpecifyKind(snapshotDate.Date, DateTimeKind.Utc);
        TotalAssets = totalAssets;
        TotalLiabilities = totalLiabilities;
        NetWorth = totalAssets - totalLiabilities;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Recalculate(decimal totalAssets, decimal totalLiabilities, string? notes = null)
    {
        if (totalAssets < 0)
            throw new ArgumentException("Tổng tài sản không được là số âm.", nameof(totalAssets));

        if (totalLiabilities < 0)
            throw new ArgumentException("Tổng nợ không được là số âm.", nameof(totalLiabilities));

        TotalAssets = totalAssets;
        TotalLiabilities = totalLiabilities;
        NetWorth = totalAssets - totalLiabilities;
        if (notes != null)
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }
        UpdatedAt = DateTime.UtcNow;
    }
}
