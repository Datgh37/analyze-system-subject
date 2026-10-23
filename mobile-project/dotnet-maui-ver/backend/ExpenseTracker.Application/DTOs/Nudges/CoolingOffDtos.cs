using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Nudges;

// Request to evaluate whether an upcoming expense requires cooling-off (UC-09.05)
public class CoolingOffEvaluationRequestDto
{
    public Guid WalletId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

// Result of cooling-off evaluation with behavioral impact analysis (UC-09.05)
public class CoolingOffEvaluationResultDto
{
    public bool RequiresCoolingOff { get; set; }
    public decimal ThresholdAmount { get; set; }
    public int CooldownHours { get; set; } = 24;
    public decimal EquivalentWorkHours { get; set; }
    public decimal ProjectedDsaReduction { get; set; }
    public decimal CurrentDsa { get; set; }
    public decimal DsaAfter { get; set; }
    public string ImpactAnalysisMessage { get; set; } = string.Empty;
}

// DTO representing an active or resolved cooling-off transaction (UC-09.05)
public class CoolingOffTransactionDto
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
    public CoolingOffStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public int RemainingSeconds => Math.Max(0, (int)(CoolingOffUntil - DateTime.UtcNow).TotalSeconds);
    public bool IsExpired => DateTime.UtcNow >= CoolingOffUntil;
    public decimal EquivalentWorkHours { get; set; }
    public decimal ProjectedDsaReduction { get; set; }
    public string? BypassReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

// Request to create a cooling-off pending transaction (UC-09.05)
public class CreateCoolingOffRequestDto
{
    public Guid WalletId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CooldownHours { get; set; } = 24;
}

// Request to bypass cooling-off in emergency (UC-09.05 A1)
public class BypassCoolingOffRequestDto
{
    public string Reason { get; set; } = string.Empty;
}
