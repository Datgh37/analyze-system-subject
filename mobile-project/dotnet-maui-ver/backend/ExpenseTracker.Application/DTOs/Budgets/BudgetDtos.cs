namespace ExpenseTracker.Application.DTOs.Budgets;

// Individual category budget allocation DTO (UC-05.01/05.02)
public record BudgetAllocationDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string? CategoryIcon,
    string? CategoryColor,
    int? BudgetRuleMapping,
    decimal AllocatedAmount,
    decimal CurrentSpent,
    decimal RemainingAmount,
    decimal SpentPercent,
    decimal AlertThresholdPercent,
    bool IsAlertTriggered,
    bool IsExceeded
);

// Payload to allocate limit to category when creating a budget
public record CreateBudgetAllocationRequest(
    Guid CategoryId,
    decimal AllocatedAmount,
    decimal AlertThresholdPercent = 80m
);

// Payload to establish a new budget cycle (UC-05.01)
public record CreateBudgetRequest(
    string Name,
    int PeriodType, // 1 = Monthly, 2 = Weekly
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalAmount,
    int Method, // 1 = 50/30/20, 2 = ZBB, 3 = Kakeibo
    List<CreateBudgetAllocationRequest>? Allocations = null
);

// Payload to update budget name or total amount
public record UpdateBudgetRequest(
    string Name,
    decimal TotalAmount
);

// Single category rebalance request
public record RebalanceAllocationItem(
    Guid CategoryId,
    decimal NewAllocatedAmount
);

// Payload for dynamic rebalancing between categories (UC-05.03)
public record RebalanceBudgetRequest(
    List<RebalanceAllocationItem> Rebalances
);

// Comprehensive budget plan DTO with allocations and overall status
public record BudgetDto(
    Guid Id,
    Guid UserId,
    string Name,
    int PeriodType,
    string PeriodTypeName,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalAmount,
    int Method,
    string MethodName,
    int Status,
    string StatusName,
    decimal TotalAllocated,
    decimal TotalSpent,
    decimal TotalRemaining,
    decimal OverallSpentPercent,
    int TotalDays,
    List<BudgetAllocationDto> Allocations
);

// High-level budget summary card DTO
public record BudgetSummaryDto(
    Guid Id,
    string Name,
    decimal TotalAmount,
    decimal TotalSpent,
    decimal TotalRemaining,
    decimal OverallSpentPercent,
    string StatusName
);
