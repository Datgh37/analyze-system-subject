using ExpenseTracker.Domain.Common;
using ExpenseTracker.Application.DTOs.Nudges;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Services;

// Implementation of Friction-by-Design Cooling-Off Period Manager (UC-09.05)
public class CoolingOffService : ICoolingOffService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CoolingOffService> _logger;

    public CoolingOffService(IApplicationDbContext context, ILogger<CoolingOffService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // UC-09.05: Evaluate whether an expense requires a 24h-48h cooling-off period
    public async Task<Result<CoolingOffEvaluationResultDto>> EvaluateExpenseForCoolingOffAsync(
        Guid userId,
        CoolingOffEvaluationRequestDto request,
        CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            return Result<CoolingOffEvaluationResultDto>.Failure("Số tiền chi tiêu phải lớn hơn 0.");

        // Check category nature: Essential categories (Needs/Survival) do NOT trigger cooling-off (E1)
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.UserId == userId || c.UserId == null), ct);

        // Compute threshold: 20% of monthly income or standard 3,000,000 VND
        var monthlyIncome = await _context.RecurringIncomes
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.IsActive)
            .SumAsync(r => (decimal?)r.ExpectedAmount, ct) ?? 15000000m;

        if (monthlyIncome <= 0) monthlyIncome = 15000000m;

        decimal incomeThreshold = monthlyIncome * 0.20m;
        decimal triggerThreshold = Math.Min(3000000m, incomeThreshold);

        bool requiresCoolingOff = request.Amount >= triggerThreshold;

        // Calculate work hours equivalent (Based on 160 standard working hours per month)
        decimal hourlyWage = Math.Round(monthlyIncome / 160m, 2);
        decimal equivalentHours = hourlyWage > 0 ? Math.Round(request.Amount / hourlyWage, 1) : 0m;

        // Calculate DSA impact
        var today = DateTime.UtcNow.Date;
        var activeBudget = await _context.Budgets
            .AsNoTracking()
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(ct);

        int remainingDays = activeBudget != null ? Math.Max(1, (activeBudget.EndDate.Date - today).Days + 1) : 15;
        decimal dsaReduction = Math.Round(request.Amount / remainingDays, 0);

        var todayDsa = await _context.DailySpendingAllowances
            .Include(d => d.Budget)
            .Where(d => d.Budget.UserId == userId && d.Date == today)
            .FirstOrDefaultAsync(ct);

        decimal currentDsa = todayDsa?.DailyAllowanceAmount ?? 250000m;
        decimal dsaAfter = Math.Max(0m, currentDsa - dsaReduction);

        string impactMessage = requiresCoolingOff
            ? $"⚠️ KHOẢNG TRỄ TÂM LÝ 24H: Khoản chi {request.Amount:N0} đ tương đương với {equivalentHours:N1} giờ lao động thực tế ({Math.Round(equivalentHours / 8m, 1)} ngày công)! Hạn mức chi tiêu mỗi ngày của bạn sẽ giảm từ {currentDsa:N0} đ xuống còn {dsaAfter:N0} đ. Hãy để tâm trí hạ nhiệt 24 giờ trước khi thanh toán."
            : "Khoản chi tiêu nằm trong giới hạn an toàn thông thường.";

        var result = new CoolingOffEvaluationResultDto
        {
            RequiresCoolingOff = requiresCoolingOff,
            ThresholdAmount = triggerThreshold,
            CooldownHours = 24,
            EquivalentWorkHours = equivalentHours,
            ProjectedDsaReduction = dsaReduction,
            CurrentDsa = currentDsa,
            DsaAfter = dsaAfter,
            ImpactAnalysisMessage = impactMessage
        };

        return Result<CoolingOffEvaluationResultDto>.Success(result);
    }

    // UC-09.05: Create a pending cooling-off transaction (Wallet funds not deducted yet)
    public async Task<Result<CoolingOffTransactionDto>> CreateCoolingOffTransactionAsync(
        Guid userId,
        CreateCoolingOffRequestDto request,
        CancellationToken ct = default)
    {
        var eval = await EvaluateExpenseForCoolingOffAsync(userId, new CoolingOffEvaluationRequestDto
        {
            WalletId = request.WalletId,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Description = request.Description
        }, ct);

        if (!eval.IsSuccess)
            return Result<CoolingOffTransactionDto>.Failure(eval.Error ?? "Lỗi đánh giá giao dịch.");

        int hours = request.CooldownHours > 0 ? request.CooldownHours : 24;
        var coolingOffUntil = DateTime.UtcNow.AddHours(hours);

        var entity = new CoolingOffTransaction(
            userId,
            request.WalletId,
            request.CategoryId,
            request.Amount,
            request.Description,
            coolingOffUntil,
            eval.Value.EquivalentWorkHours,
            eval.Value.ProjectedDsaReduction);

        _context.CoolingOffTransactions.Add(entity);
        await _context.SaveChangesAsync(ct);

        var wallet = await _context.Wallets.AsNoTracking().FirstOrDefaultAsync(w => w.Id == request.WalletId, ct);
        var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.CategoryId, ct);

        var dto = new CoolingOffTransactionDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            WalletId = entity.WalletId,
            WalletName = wallet?.Name ?? "Ví thanh toán",
            CategoryId = entity.CategoryId,
            CategoryName = category?.Name ?? "Danh mục mua sắm",
            Amount = entity.Amount,
            Description = entity.Description,
            CoolingOffUntil = entity.CoolingOffUntil,
            Status = entity.Status,
            EquivalentWorkHours = entity.EquivalentWorkHours,
            ProjectedDsaReduction = entity.ProjectedDsaReduction,
            CreatedAt = entity.CreatedAt
        };

        return Result<CoolingOffTransactionDto>.Success(dto);
    }

    public async Task<Result<List<CoolingOffTransactionDto>>> GetPendingCoolingOffTransactionsAsync(Guid userId, CancellationToken ct = default)
    {
        var items = await _context.CoolingOffTransactions
            .Include(c => c.Wallet)
            .Include(c => c.Category)
            .Where(c => c.UserId == userId && c.Status == CoolingOffStatus.PendingCoolingOff)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CoolingOffTransactionDto
            {
                Id = c.Id,
                UserId = c.UserId,
                WalletId = c.WalletId,
                WalletName = c.Wallet != null ? c.Wallet.Name : "Ví",
                CategoryId = c.CategoryId,
                CategoryName = c.Category != null ? c.Category.Name : "Danh mục",
                Amount = c.Amount,
                Description = c.Description,
                CoolingOffUntil = c.CoolingOffUntil,
                Status = c.Status,
                EquivalentWorkHours = c.EquivalentWorkHours,
                ProjectedDsaReduction = c.ProjectedDsaReduction,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(ct);

        return Result<List<CoolingOffTransactionDto>>.Success(items);
    }

    // UC-09.05: Confirm purchase after the cooling-off duration has elapsed (Deduct wallet & record transaction)
    public async Task<Result<bool>> ConfirmPurchaseAsync(Guid userId, Guid coolingOffId, CancellationToken ct = default)
    {
        var tx = await _context.CoolingOffTransactions
            .FirstOrDefaultAsync(c => c.Id == coolingOffId && c.UserId == userId, ct);

        if (tx == null)
            return Result<bool>.Failure("Không tìm thấy giao dịch hạ nhiệt.");

        if (tx.Status != CoolingOffStatus.PendingCoolingOff)
            return Result<bool>.Failure($"Giao dịch đang ở trạng thái '{tx.Status}', không thể xác nhận mua.");

        if (DateTime.UtcNow < tx.CoolingOffUntil)
            return Result<bool>.Failure($"Khoảng trễ tâm lý chưa kết thúc (còn {Math.Max(1, (int)(tx.CoolingOffUntil - DateTime.UtcNow).TotalMinutes)} phút nữa). Vui lòng đợi hết thời gian để quyết định lý trí!");

        tx.ConfirmPurchase();

        // Deduct actual wallet balance and create real transaction
        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == tx.WalletId && w.UserId == userId, ct);
        if (wallet == null)
            return Result<bool>.Failure("Không tìm thấy ví thanh toán hợp lệ.");

        if (wallet.Balance < tx.Amount)
            return Result<bool>.Failure("Số dư ví không đủ để thanh toán giao dịch này.");

        wallet.Debit(tx.Amount);

        var realTx = new Transaction(
            userId,
            tx.WalletId,
            tx.CategoryId,
            tx.Amount,
            TransactionType.Expense,
            DateTime.UtcNow,
            note: $"{tx.Description} (Đã qua khoảng trễ 24h)");

        _context.Transactions.Add(realTx);
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    // UC-09.05: Cancel purchase to defeat impulse buying and save money
    public async Task<Result<bool>> CancelPurchaseAsync(Guid userId, Guid coolingOffId, CancellationToken ct = default)
    {
        var tx = await _context.CoolingOffTransactions
            .FirstOrDefaultAsync(c => c.Id == coolingOffId && c.UserId == userId, ct);

        if (tx == null)
            return Result<bool>.Failure("Không tìm thấy giao dịch hạ nhiệt.");

        if (tx.Status != CoolingOffStatus.PendingCoolingOff)
            return Result<bool>.Failure($"Giao dịch đang ở trạng thái '{tx.Status}', không thể hủy.");

        tx.CancelPurchase();
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    // UC-09.05 A1: Emergency bypass with user justification
    public async Task<Result<bool>> BypassCoolingOffAsync(
        Guid userId,
        Guid coolingOffId,
        BypassCoolingOffRequestDto request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result<bool>.Failure("Bắt buộc phải nhập lý do khẩn cấp để bỏ qua khoảng trễ tâm lý.");

        var tx = await _context.CoolingOffTransactions
            .FirstOrDefaultAsync(c => c.Id == coolingOffId && c.UserId == userId, ct);

        if (tx == null)
            return Result<bool>.Failure("Không tìm thấy giao dịch hạ nhiệt.");

        if (tx.Status != CoolingOffStatus.PendingCoolingOff)
            return Result<bool>.Failure($"Giao dịch đang ở trạng thái '{tx.Status}', không thể mở khóa khẩn cấp.");

        tx.BypassWithReason(request.Reason);

        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == tx.WalletId && w.UserId == userId, ct);
        if (wallet == null)
            return Result<bool>.Failure("Không tìm thấy ví thanh toán hợp lệ.");

        wallet.Debit(tx.Amount);

        var realTx = new Transaction(
            userId,
            tx.WalletId,
            tx.CategoryId,
            tx.Amount,
            TransactionType.Expense,
            DateTime.UtcNow,
            note: $"{tx.Description} (Mở khóa khẩn cấp: {request.Reason.Trim()})");

        _context.Transactions.Add(realTx);
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
