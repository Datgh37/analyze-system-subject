using System.Text.Json;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.DTOs.Nudges;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Services;

// Implementation of Richard Thaler Behavioral Nudge Engine & Explainable AI (UC-09.01 to UC-09.04)
public class NudgeEngineService : INudgeEngineService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<NudgeEngineService> _logger;
    private readonly IFinancialRealtimeNotifier? _realtimeNotifier;

    public NudgeEngineService(
        IApplicationDbContext context,
        ILogger<NudgeEngineService> logger,
        IFinancialRealtimeNotifier? realtimeNotifier = null)
    {
        _context = context;
        _logger = logger;
        _realtimeNotifier = realtimeNotifier;
    }


    public async Task<Result<List<FinancialNudgeDto>>> GetActiveNudgesAsync(Guid userId, CancellationToken ct = default)
    {
        var nudges = await _context.FinancialNudges
            .AsNoTracking()
            .Where(n => n.UserId == userId && n.Status == NudgeStatus.Active)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new FinancialNudgeDto
            {
                Id = n.Id,
                UserId = n.UserId,
                NudgeType = n.NudgeType,
                Title = n.Title,
                Message = n.Message,
                TriggerEvent = n.TriggerEvent,
                Status = n.Status,
                IsApprovedByUser = n.IsApprovedByUser,
                CreatedAt = n.CreatedAt,
                ResolvedAt = n.ResolvedAt,
                HasRationale = !string.IsNullOrEmpty(n.RationaleExplanationJson),
                HasRebalanceProposal = !string.IsNullOrEmpty(n.RebalanceProposalJson)
            })
            .ToListAsync(ct);

        return Result<List<FinancialNudgeDto>>.Success(nudges);
    }

    // UC-09.02: Retrieve structured 3-component Explainable AI rationale
    public async Task<Result<XaiRationaleDto>> GetNudgeRationaleAsync(Guid userId, Guid nudgeId, CancellationToken ct = default)
    {
        var nudge = await _context.FinancialNudges
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == nudgeId && n.UserId == userId, ct);

        if (nudge == null)
            return Result<XaiRationaleDto>.Failure("Không tìm thấy Cú hích tài chính tương ứng.");

        if (!string.IsNullOrEmpty(nudge.RationaleExplanationJson))
        {
            try
            {
                var dto = JsonSerializer.Deserialize<XaiRationaleDto>(nudge.RationaleExplanationJson);
                if (dto != null) return Result<XaiRationaleDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize XAI rationale for nudge {NudgeId}", nudgeId);
            }
        }

        // Fallback: Generate dynamic transparent rationale based on nudge type
        var rationale = new XaiRationaleDto
        {
            NudgeId = nudge.Id,
            NudgeType = nudge.NudgeType,
            Title = nudge.Title,
            TriggerContext = $"Cú hích được kích hoạt tự động từ sự kiện: '{nudge.TriggerEvent}'.",
            MathematicalFormula = nudge.NudgeType switch
            {
                NudgeType.DsaThreshold => "DSA_t = Ngân_sách_còn_lại / Số_ngày_còn_lại. Ngưỡng cảnh báo: Thực_chi >= 85% DSA_t.",
                NudgeType.DeficitRebalance => "Tái cân đối: Giảm allocatedAmount_nguồn và tăng allocatedAmount_đích, bảo toàn Sum(Allocated).",
                NudgeType.PayYourselfFirst => "Tỷ lệ tích lũy vàng: Tiết kiệm tối thiểu = 20% x Thu_nhập_thực_nhận.",
                _ => "Kiểm tra giới hạn an toàn tài chính cá nhân."
            },
            EmpiricalEvidence = nudge.Message,
            ProjectedConsequences = "Nếu duy trì hành vi kỷ luật, bạn sẽ hoàn toàn bảo toàn ngân sách đến cuối tháng.",
            ProjectedDailyDsaAfter = 200000m
        };

        return Result<XaiRationaleDto>.Success(rationale);
    }

    // UC-09.03: Propose Dynamic Budget Rebalance (Needs thâm hụt <- Wants dôi dư)
    public async Task<Result<DynamicRebalanceProposalDto>> GenerateRebalanceProposalAsync(
        Guid userId,
        Guid? deficitCategoryId = null,
        CancellationToken ct = default)
    {
        var activeBudget = await _context.Budgets
            .Include(b => b.Allocations)
            .ThenInclude(a => a.Category)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (activeBudget == null)
            return Result<DynamicRebalanceProposalDto>.Failure("Người dùng chưa có kế hoạch ngân sách hoạt động.");

        // Identify target deficit category
        BudgetAllocation? targetAlloc = null;
        if (deficitCategoryId.HasValue)
        {
            targetAlloc = activeBudget.Allocations.FirstOrDefault(a => a.CategoryId == deficitCategoryId.Value);
        }

        if (targetAlloc == null)
        {
            targetAlloc = activeBudget.Allocations
                .OrderByDescending(a => a.CurrentSpent - a.AllocatedAmount)
                .FirstOrDefault(a => a.CurrentSpent > a.AllocatedAmount);
        }

        if (targetAlloc == null)
        {
            return Result<DynamicRebalanceProposalDto>.Failure("Không có danh mục nào đang bị thâm hụt ngân sách cần bù trừ.");
        }

        decimal deficitAmount = Math.Max(200000m, targetAlloc.CurrentSpent - targetAlloc.AllocatedAmount);

        // Find candidate source category from Wants / Optional categories with positive surplus
        var candidateSources = activeBudget.Allocations
            .Where(a => a.CategoryId != targetAlloc.CategoryId && (a.AllocatedAmount - a.CurrentSpent) > 50000m)
            .OrderByDescending(a => a.AllocatedAmount - a.CurrentSpent)
            .ToList();

        if (!candidateSources.Any())
        {
            return Result<DynamicRebalanceProposalDto>.Failure("Không có danh mục tùy chọn nào còn dư ngân sách để bù trừ.");
        }

        var sourceAlloc = candidateSources.First();
        decimal surplus = sourceAlloc.AllocatedAmount - sourceAlloc.CurrentSpent;
        decimal transferAmount = Math.Min(surplus, deficitAmount);

        var proposal = new DynamicRebalanceProposalDto
        {
            ProposalId = Guid.NewGuid(),
            SourceCategoryId = sourceAlloc.CategoryId,
            SourceCategoryName = sourceAlloc.Category?.Name ?? "Danh mục dôi dư",
            SourceCategoryOldLimit = sourceAlloc.AllocatedAmount,
            SourceCategoryNewLimit = sourceAlloc.AllocatedAmount - transferAmount,
            TargetCategoryId = targetAlloc.CategoryId,
            TargetCategoryName = targetAlloc.Category?.Name ?? "Danh mục thâm hụt",
            TargetCategoryOldLimit = targetAlloc.AllocatedAmount,
            TargetCategoryNewLimit = targetAlloc.AllocatedAmount + transferAmount,
            TransferAmount = transferAmount,
            TotalBudgetInvariantCheck = activeBudget.Allocations.Sum(a => a.AllocatedAmount),
            ExplanationMessage = $"Đề xuất điều chuyển {transferAmount:N0} đ từ '{sourceAlloc.Category?.Name}' sang '{targetAlloc.Category?.Name}' để bảo toàn ngân sách thiết yếu."
        };

        return Result<DynamicRebalanceProposalDto>.Success(proposal);
    }

    // UC-09.04: Human-in-the-loop approval of nudge and atomic database execution
    public async Task<Result<bool>> ApproveNudgeAsync(Guid userId, Guid nudgeId, CancellationToken ct = default)
    {
        var nudge = await _context.FinancialNudges
            .FirstOrDefaultAsync(n => n.Id == nudgeId && n.UserId == userId, ct);

        if (nudge == null)
            return Result<bool>.Failure("Không tìm thấy Cú hích tài chính.");

        if (nudge.Status != NudgeStatus.Active)
            return Result<bool>.Failure($"Cú hích đang ở trạng thái '{nudge.Status}', không thể phê duyệt.");

        nudge.Approve();

        // If the nudge contains a rebalance proposal, execute the budget adjustment in atomic transaction
        if (!string.IsNullOrEmpty(nudge.RebalanceProposalJson))
        {
            try
            {
                var proposal = JsonSerializer.Deserialize<DynamicRebalanceProposalDto>(nudge.RebalanceProposalJson);
                if (proposal != null)
                {
                    var source = await _context.BudgetAllocations
                        .Include(a => a.Budget)
                        .FirstOrDefaultAsync(a => a.CategoryId == proposal.SourceCategoryId && a.Budget != null && a.Budget.UserId == userId && a.Budget.Status == BudgetStatus.Active, ct);

                    var target = await _context.BudgetAllocations
                        .Include(a => a.Budget)
                        .FirstOrDefaultAsync(a => a.CategoryId == proposal.TargetCategoryId && a.Budget != null && a.Budget.UserId == userId && a.Budget.Status == BudgetStatus.Active, ct);

                    if (source != null && target != null)
                    {
                        source.Rebalance(Math.Max(0m, source.AllocatedAmount - proposal.TransferAmount));
                        target.Rebalance(target.AllocatedAmount + proposal.TransferAmount);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply rebalance proposal for nudge {NudgeId}", nudgeId);
                return Result<bool>.Failure("Lỗi thực thi giao dịch điều chuyển hạn mức ngân sách.");
            }
        }

        await _context.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    // UC-09.04: Human-in-the-loop dismissal of nudge
    public async Task<Result<bool>> DismissNudgeAsync(Guid userId, Guid nudgeId, CancellationToken ct = default)
    {
        var nudge = await _context.FinancialNudges
            .FirstOrDefaultAsync(n => n.Id == nudgeId && n.UserId == userId, ct);

        if (nudge == null)
            return Result<bool>.Failure("Không tìm thấy Cú hích tài chính.");

        if (nudge.Status != NudgeStatus.Active)
            return Result<bool>.Failure($"Cú hích đang ở trạng thái '{nudge.Status}', không thể hủy.");

        nudge.Dismiss();
        await _context.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    // Trigger evaluation on expense events with Anti-Spam protection (UC-09.01 E1)
    public async Task EvaluateTriggersOnExpenseAsync(Guid userId, decimal expenseAmount, Guid categoryId, CancellationToken ct = default)
    {
        var fourHoursAgo = DateTime.UtcNow.AddHours(-4);

        // Anti-Spam check: Avoid sending DSA nudges within 4 hours
        bool hasRecentDsaNudge = await _context.FinancialNudges
            .AnyAsync(n => n.UserId == userId && n.NudgeType == NudgeType.DsaThreshold && n.CreatedAt >= fourHoursAgo, ct);

        if (!hasRecentDsaNudge)
        {
            var today = DateTime.UtcNow.Date;
            var dsa = await _context.DailySpendingAllowances
                .Include(d => d.Budget)
                .Where(d => d.Budget != null && d.Budget.UserId == userId && d.Date == today)
                .FirstOrDefaultAsync(ct);

            if (dsa != null && dsa.DailyAllowanceAmount > 0)
            {
                decimal spentRatio = (dsa.ActualSpentToday + expenseAmount) / dsa.DailyAllowanceAmount;
                if (spentRatio >= 0.85m)
                {
                    var xai = new XaiRationaleDto
                    {
                        NudgeType = NudgeType.DsaThreshold,
                        Title = "Cảnh Báo Chạm Trần Hạn Mức Ngày (DSA)",
                        TriggerContext = $"Giao dịch chi tiêu mới: {expenseAmount:N0} đ đưa tổng chi hôm nay lên {dsa.ActualSpentToday + expenseAmount:N0} đ.",
                        MathematicalFormula = "DSA_t = Ngân_sách_còn_lại / Số_ngày_còn_lại. Mức cảnh báo: Thực_chi >= 85% DSA_t.",
                        EmpiricalEvidence = $"Bạn đã sử dụng {Math.Round(spentRatio * 100m, 1)}% hạn mức an toàn của ngày hôm nay ({dsa.DailyAllowanceAmount:N0} đ).",
                        ProjectedConsequences = "Nếu tiếp tục chi tiêu tối nay, hạn mức của các ngày còn lại trong tuần sẽ bị siết giảm tương ứng.",
                        ProjectedDailyDsaAfter = Math.Max(0m, dsa.DailyAllowanceAmount - 50000m)
                    };

                    var nudge = new FinancialNudge(
                        userId,
                        NudgeType.DsaThreshold,
                        "Cảnh Báo Chạm Trần Hạn Mức Ngày (DSA)",
                        $"Bạn đã tiêu {Math.Round(spentRatio * 100m, 1)}% hạn mức an toàn hôm nay ({dsa.DailyAllowanceAmount:N0} đ). Hãy tạm hoãn các khoản chi tùy chọn tối nay nhé!",
                        "ExpenseCreated",
                        rationaleExplanationJson: JsonSerializer.Serialize(xai));

                    _context.FinancialNudges.Add(nudge);
                    await _context.SaveChangesAsync(ct);

                    if (_realtimeNotifier != null)
                    {
                        _ = _realtimeNotifier.NotifyNudgeAlertAsync(userId, new NudgeAlertNotificationDto(
                            nudge.Id, nudge.Title, nudge.Message, "Warning", "app://nudge-feed", DateTime.UtcNow), ct);
                    }
                }
            }
        }
    }

    // Trigger evaluation on income events (Pay Yourself First) (UC-09.01)
    public async Task EvaluateTriggersOnIncomeAsync(Guid userId, decimal incomeAmount, CancellationToken ct = default)
    {
        if (incomeAmount <= 0) return;

        var fourHoursAgo = DateTime.UtcNow.AddHours(-4);
        bool hasRecentIncomeNudge = await _context.FinancialNudges
            .AnyAsync(n => n.UserId == userId && n.NudgeType == NudgeType.PayYourselfFirst && n.CreatedAt >= fourHoursAgo, ct);

        if (hasRecentIncomeNudge) return;

        decimal suggestedSavings = Math.Round(incomeAmount * 0.20m, 0);

        var xai = new XaiRationaleDto
        {
            NudgeType = NudgeType.PayYourselfFirst,
            Title = "Nguyên Tắc Vàng: Trả Cho Mình Trước",
            TriggerContext = $"Bạn vừa ghi nhận một khoản thu nhập mới: {incomeAmount:N0} đ.",
            MathematicalFormula = "Tiết kiệm tối thiểu = 20% x Thu_nhập_thực_nhận (Theo quy tắc 50/30/20 và GS. Thaler).",
            EmpiricalEvidence = $"Gợi ý trích ngay {suggestedSavings:N0} đ (20%) vào Quỹ tích lũy trước khi phân bổ vào các khoản chi tùy chọn.",
            ProjectedConsequences = "Hành động này giúp bạn rút ngắn 1.2 tháng để hoàn thành Mục tiêu tài chính cá nhân."
        };

        var nudge = new FinancialNudge(
            userId,
            NudgeType.PayYourselfFirst,
            "Cú Hích Tích Lũy: Hãy Trả Cho Mình Trước!",
            $"Bạn vừa nhận {incomeAmount:N0} đ. Hãy trích ngay {suggestedSavings:N0} đ (20%) vào Quỹ tích lũy trước khi tiêu xài nhé!",
            "IncomeCreated",
            rationaleExplanationJson: JsonSerializer.Serialize(xai));

        _context.FinancialNudges.Add(nudge);
        await _context.SaveChangesAsync(ct);

        if (_realtimeNotifier != null)
        {
            _ = _realtimeNotifier.NotifyNudgeAlertAsync(userId, new NudgeAlertNotificationDto(
                nudge.Id, nudge.Title, nudge.Message, "Info", "app://nudge-feed", DateTime.UtcNow), ct);
        }
    }
}
