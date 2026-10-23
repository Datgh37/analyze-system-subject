using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Nudges;

// DTO for Financial Nudge presentation (UC-09.01)
public class FinancialNudgeDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NudgeType NudgeType { get; set; }
    public string NudgeTypeName => NudgeType.ToString();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TriggerEvent { get; set; } = string.Empty;
    public NudgeStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public bool? IsApprovedByUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public bool HasRationale { get; set; }
    public bool HasRebalanceProposal { get; set; }
}

// Structured 3-component Explainable AI (XAI) rationale (UC-09.02)
public class XaiRationaleDto
{
    public Guid NudgeId { get; set; }
    public NudgeType NudgeType { get; set; }
    public string Title { get; set; } = string.Empty;
    
    // Component 1: Trigger Event context
    public string TriggerContext { get; set; } = string.Empty;
    
    // Component 2: Mathematical Evidence & Empirical Data
    public string MathematicalFormula { get; set; } = string.Empty;
    public string EmpiricalEvidence { get; set; } = string.Empty;
    public Dictionary<string, string> SupportingMetrics { get; set; } = new();

    // Component 3: Projected Consequences if unadjusted
    public string ProjectedConsequences { get; set; } = string.Empty;
    public decimal ProjectedDailyDsaAfter { get; set; }
    public DateTime? ProjectedExhaustionDate { get; set; }

    public string ToneOfVoiceNote { get; set; } = "Khuyến nghị mang tính đồng hành và khích lệ kỷ luật tự nguyện.";
}

// Dynamic Budget Rebalance proposal DTO (UC-09.03)
public class DynamicRebalanceProposalDto
{
    public Guid ProposalId { get; set; }
    public Guid SourceCategoryId { get; set; }
    public string SourceCategoryName { get; set; } = string.Empty;
    public decimal SourceCategoryOldLimit { get; set; }
    public decimal SourceCategoryNewLimit { get; set; }

    public Guid TargetCategoryId { get; set; }
    public string TargetCategoryName { get; set; } = string.Empty;
    public decimal TargetCategoryOldLimit { get; set; }
    public decimal TargetCategoryNewLimit { get; set; }

    public decimal TransferAmount { get; set; }
    public decimal TotalBudgetInvariantCheck { get; set; } // Verifies sum before == sum after
    public string ExplanationMessage { get; set; } = string.Empty;
}

// Request DTO to approve a nudge (UC-09.04)
public class ApproveNudgeRequestDto
{
    public bool ConfirmApplication { get; set; } = true;
}
