namespace ExpenseTracker.Maui.Models.Behavioral;

public enum CoolingOffStatus
{
    PendingCoolingOff = 1,
    Confirmed = 2,
    Cancelled = 3,
    Bypassed = 4
}

public class CoolingOffTransactionModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public string WalletName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CoolingOffUntil { get; set; }
    public CoolingOffStatus Status { get; set; } = CoolingOffStatus.PendingCoolingOff;
    public decimal EquivalentWorkHours { get; set; }
    public decimal ProjectedDsaReduction { get; set; }
    public DateTime CreatedAt { get; set; }

    public int RemainingMinutes => Math.Max(0, (int)(CoolingOffUntil - DateTime.UtcNow).TotalMinutes);
    public bool IsExpired => DateTime.UtcNow >= CoolingOffUntil;

    public string RemainingTimeDisplay
    {
        get
        {
            if (IsExpired) return "Đã hết thời gian hạ nhiệt";
            int hours = RemainingMinutes / 60;
            int mins = RemainingMinutes % 60;
            return $"{hours:D2}h {mins:D2}m còn lại";
        }
    }

    public string StatusBadgeColor => Status switch
    {
        CoolingOffStatus.PendingCoolingOff => "#00E5FF", // Cyan glowing
        CoolingOffStatus.Confirmed => "#FF9800",        // Orange
        CoolingOffStatus.Cancelled => "#4CAF50",        // Green (Saved money)
        CoolingOffStatus.Bypassed => "#E91E63",         // Pink/Red emergency
        _ => "#9E9E9E"
    };

    public string StatusDisplay => Status switch
    {
        CoolingOffStatus.PendingCoolingOff => "Đang phong tỏa 24h",
        CoolingOffStatus.Confirmed => "Đã xác nhận mua",
        CoolingOffStatus.Cancelled => "Đã hủy (Tiết kiệm thành công)",
        CoolingOffStatus.Bypassed => "Mở khóa khẩn cấp",
        _ => "Không xác định"
    };
}

public class CoolingOffEvaluationRequestModel
{
    public Guid WalletId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CoolingOffEvaluationResultModel
{
    public bool RequiresCoolingOff { get; set; }
    public decimal ThresholdAmount { get; set; }
    public int CooldownHours { get; set; }
    public decimal EquivalentWorkHours { get; set; }
    public decimal ProjectedDsaReduction { get; set; }
    public decimal CurrentDsa { get; set; }
    public decimal DsaAfter { get; set; }
    public string ImpactAnalysisMessage { get; set; } = string.Empty;
}

public class CreateCoolingOffRequestModel
{
    public Guid WalletId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CooldownHours { get; set; } = 24;
}

public class BypassCoolingOffRequestModel
{
    public string Reason { get; set; } = string.Empty;
}
