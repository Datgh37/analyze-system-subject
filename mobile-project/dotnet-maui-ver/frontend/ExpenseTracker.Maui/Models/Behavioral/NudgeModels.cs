using System.Text.Json.Serialization;

namespace ExpenseTracker.Maui.Models.Behavioral;

public enum NudgeType
{
    DsaThreshold = 1,
    WindfallSavings = 2,
    DeficitRebalance = 3,
    CoolingOffSuggestion = 4,
    PayYourselfFirst = 5,
    WeekendSpendingAlert = 6
}

public enum NudgeStatus
{
    Active = 1,
    Dismissed = 2,
    Resolved = 3,
    Expired = 4
}

public class FinancialNudgeModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NudgeType NudgeType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? TriggerContext { get; set; }
    public NudgeStatus Status { get; set; } = NudgeStatus.Active;
    public string? RebalanceProposalJson { get; set; }
    public string? RationaleExplanationJson { get; set; }
    public bool IsApprovedByUser { get; set; }
    public DateTime CreatedAt { get; set; }

    public string NudgeTypeDisplay => NudgeType switch
    {
        NudgeType.DsaThreshold => "⚠️ Cảnh Báo Hạn Mức DSA",
        NudgeType.WindfallSavings => "🎉 Quản Trị Tiền Bất Thường",
        NudgeType.DeficitRebalance => "⚖️ Đề Xuất Tái Cân Đối Ngân Sách",
        NudgeType.CoolingOffSuggestion => "⏳ Đề Xuất Khoảng Trễ 24h",
        NudgeType.PayYourselfFirst => "💎 Tiết Kiệm Trước Khi Tiêu",
        NudgeType.WeekendSpendingAlert => "📅 Cảnh Báo Cuối Tuần",
        _ => "💡 Gợi Ý Tài Chính"
    };

    public string NudgeBadgeColor => NudgeType switch
    {
        NudgeType.DsaThreshold => "#FF5252",
        NudgeType.WindfallSavings => "#4CAF50",
        NudgeType.DeficitRebalance => "#FF9800",
        NudgeType.CoolingOffSuggestion => "#00E5FF",
        NudgeType.PayYourselfFirst => "#9C27B0",
        _ => "#2196F3"
    };

    public bool HasRebalanceProposal => !string.IsNullOrEmpty(RebalanceProposalJson);
    public bool HasXaiRationale => !string.IsNullOrEmpty(RationaleExplanationJson);
}

public class XaiRationaleModel
{
    public NudgeType NudgeType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TriggerContext { get; set; } = string.Empty;
    public string MathematicalFormula { get; set; } = string.Empty;
    public string EmpiricalEvidence { get; set; } = string.Empty;
    public string ProjectedConsequences { get; set; } = string.Empty;
    public decimal ProjectedDailyDsaAfter { get; set; }
}

public class DynamicRebalanceProposalModel
{
    public Guid ProposalId { get; set; }
    public Guid SourceCategoryId { get; set; }
    public string SourceCategoryName { get; set; } = string.Empty;
    public Guid TargetCategoryId { get; set; }
    public string TargetCategoryName { get; set; } = string.Empty;
    public decimal TransferAmount { get; set; }
    public decimal SourceCategoryOldLimit { get; set; }
    public decimal SourceCategoryNewLimit { get; set; }
    public decimal TargetCategoryOldLimit { get; set; }
    public decimal TargetCategoryNewLimit { get; set; }
}
