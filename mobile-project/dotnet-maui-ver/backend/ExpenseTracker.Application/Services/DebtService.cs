using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.DTOs.Debts;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Services;

// Dịch vụ quản lý danh mục nợ cá nhân và thanh toán nợ (UC-06.05 & UC-06.07)
public class DebtService : IDebtService
{
    private readonly IApplicationDbContext _context;
    private readonly IFinancialRealtimeNotifier? _notifier;
    private readonly IValidator<CreateDebtDto>? _createValidator;
    private readonly IValidator<UpdateDebtDto>? _updateValidator;
    private readonly IValidator<RecordDebtPaymentDto>? _paymentValidator;
    private readonly ILogger<DebtService>? _logger;

    public DebtService(
        IApplicationDbContext context,
        IFinancialRealtimeNotifier? notifier = null,
        IValidator<CreateDebtDto>? createValidator = null,
        IValidator<UpdateDebtDto>? updateValidator = null,
        IValidator<RecordDebtPaymentDto>? paymentValidator = null,
        ILogger<DebtService>? logger = null)
    {
        _context = context;
        _notifier = notifier;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _paymentValidator = paymentValidator;
        _logger = logger;
    }

    public async Task<List<DebtDto>> GetDebtsAsync(Guid userId, CancellationToken ct = default)
    {
        var debts = await _context.Debts
            .Where(d => d.UserId == userId)
            .OrderBy(d => d.Status)
            .ThenBy(d => d.DueDateMonthly)
            .ToListAsync(ct);

        return debts.Select(MapToDto).ToList();
    }

    public async Task<DebtDetailDto?> GetDebtByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var debt = await _context.Debts
            .Include(d => d.Transactions)
                .ThenInclude(t => t.Wallet)
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId, ct);

        if (debt == null) return null;

        var transactionDtos = debt.Transactions
            .OrderByDescending(t => t.PaymentDate)
            .Select(t => new DebtTransactionDto(
                t.Id,
                t.DebtId,
                t.WalletId,
                t.Wallet?.Name ?? "Ví không xác định",
                t.Amount,
                t.PrincipalPaid,
                t.InterestPaid,
                t.PaymentDate,
                t.Note,
                t.CreatedAt
            ))
            .ToList();

        return new DebtDetailDto(MapToDto(debt), transactionDtos);
    }

    public async Task<DebtSummaryDto> GetDebtSummaryAsync(Guid userId, CancellationToken ct = default)
    {
        var debts = await _context.Debts
            .Where(d => d.UserId == userId)
            .ToListAsync(ct);

        var totalCurrent = debts.Where(d => d.Status != DebtStatus.PaidOff).Sum(d => d.CurrentBalance);
        var totalPrincipal = debts.Sum(d => d.PrincipalAmount);
        var totalPaid = debts.Sum(d => Math.Max(0, d.PrincipalAmount - d.CurrentBalance));
        var totalMonthlyMin = debts.Where(d => d.Status != DebtStatus.PaidOff).Sum(d => d.MinimumPayment);
        var activeCount = debts.Count(d => d.Status != DebtStatus.PaidOff);
        var paidOffCount = debts.Count(d => d.Status == DebtStatus.PaidOff);

        return new DebtSummaryDto(
            totalCurrent,
            totalPrincipal,
            totalPaid,
            totalMonthlyMin,
            activeCount,
            paidOffCount
        );
    }

    public async Task<DebtDto> CreateDebtAsync(Guid userId, CreateDebtDto dto, CancellationToken ct = default)
    {
        if (_createValidator != null)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, ct);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }
        }

        var debt = new Debt(
            userId,
            dto.Name,
            (DebtType)dto.DebtType,
            dto.CreditorName,
            dto.PrincipalAmount,
            dto.InterestRateAnnual,
            dto.MinimumPayment,
            dto.DueDateMonthly,
            dto.StartDate,
            dto.ExpectedPayoffDate,
            dto.CurrentBalance,
            dto.Notes
        );

        _context.Debts.Add(debt);
        await _context.SaveChangesAsync(ct);

        return MapToDto(debt);
    }

    public async Task<DebtDto> UpdateDebtAsync(Guid id, Guid userId, UpdateDebtDto dto, CancellationToken ct = default)
    {
        var debt = await _context.Debts
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId, ct);

        if (debt == null)
            throw new KeyNotFoundException("Không tìm thấy khoản nợ được yêu cầu.");

        if (_updateValidator != null)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto, ct);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }
        }

        debt.UpdateDetails(
            dto.Name,
            (DebtType)dto.DebtType,
            dto.CreditorName,
            dto.InterestRateAnnual,
            dto.MinimumPayment,
            dto.DueDateMonthly,
            dto.ExpectedPayoffDate,
            dto.Notes
        );

        await _context.SaveChangesAsync(ct);

        return MapToDto(debt);
    }

    public async Task<DebtTransactionDto> RecordPaymentAsync(Guid debtId, Guid userId, RecordDebtPaymentDto dto, CancellationToken ct = default)
    {
        var debt = await _context.Debts
            .FirstOrDefaultAsync(d => d.Id == debtId && d.UserId == userId, ct);

        if (debt == null)
            throw new KeyNotFoundException("Không tìm thấy khoản nợ được yêu cầu.");

        if (_paymentValidator != null)
        {
            var validationResult = await _paymentValidator.ValidateAsync(dto, ct);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }
        }

        // Khởi tạo Transaction đảm bảo tính toàn vẹn ACID
        using var tx = await _context.BeginTransactionAsync(ct);

        // Khóa bi quan (pessimistic lock) ví để tránh xung đột trừ tiền
        var wallet = await _context.GetWalletForUpdateAsync(dto.WalletId, userId, ct);
        if (wallet == null)
            throw new KeyNotFoundException("Không tìm thấy ví thanh toán được chỉ định.");

        if (wallet.Status != WalletStatus.Active)
            throw new InvalidOperationException("Ví thanh toán đang ở trạng thái lưu trữ hoặc bị vô hiệu hóa.");

        // Trừ tiền từ ví (Domain method của Wallet sẽ ném InvalidOperationException nếu số dư không đủ)
        wallet.Debit(dto.Amount);

        // Ghi nhận trả nợ trong Domain entity Debt
        var transaction = debt.RecordPayment(
            dto.WalletId,
            dto.Amount,
            dto.PrincipalPaid,
            dto.InterestPaid,
            dto.PaymentDate,
            dto.Note
        );

        await _context.DebtTransactions.AddAsync(transaction, ct);
        await _context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        // Bắn tín hiệu SignalR cập nhật số dư ví tức thì về Mobile (Bọc try-catch chống gãy luồng thanh toán)
        if (_notifier != null)
        {
            try
            {
                await _notifier.NotifyBalanceUpdateAsync(userId, new BalanceUpdateNotificationDto(
                    wallet.Id,
                    wallet.Balance,
                    -dto.Amount,
                    $"Thanh toán nợ: {debt.Name}",
                    DateTime.UtcNow
                ), ct);

                if (debt.Status == DebtStatus.PaidOff)
                {
                    await _notifier.NotifyInAppAsync(userId, new InAppNotificationDto(
                        Guid.NewGuid(),
                        "🎉 Chúc mừng! Đã tất toán khoản nợ",
                        $"Bạn đã hoàn thành việc trả sạch khoản nợ '{debt.Name}' trị giá {debt.PrincipalAmount:N0}đ!",
                        "debt_paidoff",
                        null,
                        false,
                        null,
                        DateTime.UtcNow
                    ), ct);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Không thể gửi tín hiệu SignalR khi thanh toán nợ {DebtId}: {Message}", debt.Id, ex.Message);
            }
        }

        return new DebtTransactionDto(
            transaction.Id,
            transaction.DebtId,
            transaction.WalletId,
            wallet.Name,
            transaction.Amount,
            transaction.PrincipalPaid,
            transaction.InterestPaid,
            transaction.PaymentDate,
            transaction.Note,
            transaction.CreatedAt
        );
    }

    public async Task<bool> ArchiveDebtAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var debt = await _context.Debts
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId, ct);

        if (debt == null)
            return false;

        debt.MarkAsDeleted();
        await _context.SaveChangesAsync(ct);

        if (_notifier != null)
        {
            try
            {
                await _notifier.NotifyEntityArchivedAsync(userId, new EntityArchivedNotificationDto(
                    "Debt",
                    debt.Id,
                    debt.Name,
                    DateTime.UtcNow
                ), ct);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Không thể phát SignalR EntityArchived cho nợ {DebtId}", debt.Id);
            }
        }

        return true;
    }

    private static DebtDto MapToDto(Debt debt)
    {
        var paidAmount = Math.Max(0, debt.PrincipalAmount - debt.CurrentBalance);
        var progressPercent = debt.PrincipalAmount > 0 
            ? Math.Round(paidAmount / debt.PrincipalAmount * 100m, 2) 
            : 100m;

        var typeName = debt.DebtType switch
        {
            DebtType.Personal => "Vay mượn cá nhân",
            DebtType.CreditCard => "Thẻ tín dụng",
            DebtType.InstallmentLoan => "Vay tiêu dùng / Trả góp",
            DebtType.MortgageOrBankLoan => "Vay ngân hàng / Thế chấp",
            _ => "Khác"
        };

        // Nhận diện trạng thái quá hạn nếu ngày dự kiến đã qua và còn dư nợ
        var isOverdue = debt.Status == DebtStatus.Overdue ||
            (debt.Status == DebtStatus.Active && debt.ExpectedPayoffDate.HasValue && debt.ExpectedPayoffDate.Value.Date < DateTime.UtcNow.Date && debt.CurrentBalance > 0);

        var statusName = debt.Status switch
        {
            DebtStatus.PaidOff => "Đã tất toán",
            DebtStatus.Overdue => "Quá hạn",
            _ => isOverdue ? "Quá hạn" : "Đang trả nợ"
        };

        var effectiveStatus = isOverdue && debt.Status == DebtStatus.Active ? (int)DebtStatus.Overdue : (int)debt.Status;

        return new DebtDto(
            debt.Id,
            debt.UserId,
            debt.Name,
            (int)debt.DebtType,
            typeName,
            debt.CreditorName,
            debt.PrincipalAmount,
            debt.CurrentBalance,
            paidAmount,
            progressPercent,
            debt.InterestRateAnnual,
            debt.MinimumPayment,
            debt.DueDateMonthly,
            debt.StartDate,
            debt.ExpectedPayoffDate,
            effectiveStatus,
            statusName,
            debt.Notes,
            debt.CreatedAt,
            debt.UpdatedAt
        );
    }
}
