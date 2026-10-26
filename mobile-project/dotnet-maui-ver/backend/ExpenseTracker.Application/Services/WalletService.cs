using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.DTOs.Wallets;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Service implementing multi-wallet lifecycle, ACID transfers and balance reconciliation
public class WalletService : IWalletService
{
    private readonly IApplicationDbContext _context;
    private readonly IFinancialRealtimeNotifier? _realtimeNotifier;

    public WalletService(
        IApplicationDbContext context,
        IFinancialRealtimeNotifier? realtimeNotifier = null)
    {
        _context = context;
        _realtimeNotifier = realtimeNotifier;
    }

    // Retrieves active wallets belonging to a user ordered by user preference
    public async Task<Result<List<WalletDto>>> GetUserWalletsAsync(Guid userId, CancellationToken ct = default)
    {
        var wallets = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .OrderBy(w => w.DisplayOrder)
            .ThenBy(w => w.CreatedAt)
            .Select(w => MapToDto(w))
            .ToListAsync(ct);

        return Result<List<WalletDto>>.Success(wallets);
    }

    // Retrieves specific wallet by identifier
    public async Task<Result<WalletDto>> GetWalletByIdAsync(Guid userId, Guid walletId, CancellationToken ct = default)
    {
        var wallet = await _context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);

        if (wallet == null)
            return Result<WalletDto>.Failure("Ví không tồn tại hoặc bạn không có quyền truy cập.", 404);

        return Result<WalletDto>.Success(MapToDto(wallet));
    }

    // Creates a new wallet account with initial balance
    public async Task<Result<WalletDto>> CreateWalletAsync(Guid userId, CreateWalletRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<WalletDto>.Failure("Tên ví không được để trống.");

        var trimmedName = request.Name.Trim();
        var isDuplicate = await _context.Wallets
            .AnyAsync(w => w.UserId == userId && w.Name.ToLower() == trimmedName.ToLower(), ct);

        if (isDuplicate)
            return Result<WalletDto>.Failure("Tên ví này đã tồn tại. Vui lòng đặt tên khác để dễ phân biệt.");

        var wallet = new Wallet(
            userId: userId,
            name: trimmedName,
            walletType: request.WalletType,
            initialBalance: request.InitialBalance,
            currency: request.Currency,
            accountNumber: request.AccountNumber,
            bankName: request.BankName,
            creditLimit: request.CreditLimit,
            statementDay: request.StatementDay,
            paymentDueDay: request.PaymentDueDay,
            colorHex: request.ColorHex,
            iconKey: request.IconKey,
            isExcludedFromTotal: request.IsExcludedFromTotal
        );

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync(ct);

        return Result<WalletDto>.Success(MapToDto(wallet));
    }

    // Updates existing wallet presentation and metadata
    public async Task<Result<WalletDto>> UpdateWalletAsync(Guid userId, Guid walletId, UpdateWalletRequest request, CancellationToken ct = default)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);

        if (wallet == null)
            return Result<WalletDto>.Failure("Ví không tồn tại hoặc bạn không có quyền truy cập.");

        var trimmedName = request.Name.Trim();
        var isDuplicate = await _context.Wallets
            .AnyAsync(w => w.Id != walletId && w.UserId == userId && w.Name.ToLower() == trimmedName.ToLower(), ct);

        if (isDuplicate)
            return Result<WalletDto>.Failure("Tên ví này đã tồn tại. Vui lòng đặt tên khác để dễ phân biệt.");

        wallet.UpdateDetails(
            name: trimmedName,
            accountNumber: request.AccountNumber,
            bankName: request.BankName,
            creditLimit: request.CreditLimit,
            statementDay: request.StatementDay,
            paymentDueDay: request.PaymentDueDay,
            colorHex: request.ColorHex,
            iconKey: request.IconKey,
            isExcludedFromTotal: request.IsExcludedFromTotal
        );

        await _context.SaveChangesAsync(ct);

        return Result<WalletDto>.Success(MapToDto(wallet));
    }

    // Partially updates wallet properties via HTTP PATCH (RFC 7396)
    public async Task<Result<WalletDto>> PatchWalletAsync(Guid userId, Guid walletId, PatchWalletRequest request, CancellationToken ct = default)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);

        if (wallet == null)
            return Result<WalletDto>.Failure("Ví không tồn tại hoặc bạn không có quyền truy cập.");

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var trimmedName = request.Name.Trim();
            var isDuplicate = await _context.Wallets
                .AnyAsync(w => w.Id != walletId && w.UserId == userId && w.Name.ToLower() == trimmedName.ToLower(), ct);

            if (isDuplicate)
                return Result<WalletDto>.Failure("Tên ví này đã tồn tại. Vui lòng đặt tên khác để dễ phân biệt.");
        }

        wallet.PatchDetails(
            name: request.Name,
            colorHex: request.ColorHex,
            iconKey: request.IconKey,
            isExcludedFromTotal: request.IsExcludedFromTotal,
            status: request.Status
        );

        await _context.SaveChangesAsync(ct);

        return Result<WalletDto>.Success(MapToDto(wallet));
    }

    // Soft-deletes a wallet account
    public async Task<Result<bool>> DeleteWalletAsync(Guid userId, Guid walletId, CancellationToken ct = default)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);

        if (wallet == null)
            return Result<bool>.Failure("Ví không tồn tại hoặc bạn không có quyền truy cập.");

        if (wallet.Balance > 0)
            return Result<bool>.Failure("Không thể xóa ví đang có số dư lớn hơn 0. Vui lòng chuyển toàn bộ số dư sang ví khác trước khi xóa.");

        var hasTransactions = await _context.Transactions.AnyAsync(t => t.WalletId == walletId, ct);
        if (hasTransactions)
            return Result<bool>.Failure("Không thể xóa ví đã có phát sinh giao dịch thu chi. Bạn có thể chọn Lưu trữ (Archive) ví này.");

        wallet.MarkAsDeleted();
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    // Executes atomic internal money transfer with strict deadlock prevention ordering
    public async Task<Result<bool>> TransferMoneyAsync(Guid userId, TransferMoneyRequest request, CancellationToken ct = default)
    {
        if (request.SourceWalletId == request.DestinationWalletId)
            return Result<bool>.Failure("Ví nguồn và ví đích không được trùng nhau.");

        if (request.Amount <= 0)
            return Result<bool>.Failure("Số tiền chuyển phải lớn hơn 0.");

        await using var transaction = await _context.BeginTransactionAsync(ct);
        try
        {
            // Deadlock Prevention: Order wallet IDs to guarantee consistent locking sequence
            bool sourceIsFirst = request.SourceWalletId.CompareTo(request.DestinationWalletId) < 0;
            var firstLockId = sourceIsFirst ? request.SourceWalletId : request.DestinationWalletId;
            var secondLockId = sourceIsFirst ? request.DestinationWalletId : request.SourceWalletId;

            var firstWallet = await _context.GetWalletForUpdateAsync(firstLockId, userId, ct);
            var secondWallet = await _context.GetWalletForUpdateAsync(secondLockId, userId, ct);

            var sourceWallet = sourceIsFirst ? firstWallet : secondWallet;
            var destWallet = sourceIsFirst ? secondWallet : firstWallet;

            if (sourceWallet == null)
                return Result<bool>.Failure("Ví nguồn không tồn tại.");

            if (destWallet == null)
                return Result<bool>.Failure("Ví đích không tồn tại.");

            if (sourceWallet.Status != WalletStatus.Active || destWallet.Status != WalletStatus.Active)
                return Result<bool>.Failure("Một trong hai ví đang bị vô hiệu hóa hoặc lưu trữ.");

            sourceWallet.Debit(request.Amount);
            destWallet.Credit(request.Amount);

            // UC-01.08: Log internal transfer transaction record for immutable audit trail
            var transfer = new WalletTransfer(
                userId: userId,
                sourceWalletId: sourceWallet.Id,
                destinationWalletId: destWallet.Id,
                amount: request.Amount,
                transferDate: request.TransferDate,
                note: request.Note
            );
            await _context.WalletTransfers.AddAsync(transfer, ct);

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            if (_realtimeNotifier != null)
            {
                _ = _realtimeNotifier.NotifyBalanceUpdateAsync(userId, new BalanceUpdateNotificationDto(
                    sourceWallet.Id, sourceWallet.Balance, -request.Amount, $"Chuyển tiền tới {destWallet.Name}", DateTime.UtcNow), ct);
                _ = _realtimeNotifier.NotifyBalanceUpdateAsync(userId, new BalanceUpdateNotificationDto(
                    destWallet.Id, destWallet.Balance, request.Amount, $"Nhận tiền từ {sourceWallet.Name}", DateTime.UtcNow), ct);
            }

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            await transaction.RollbackAsync(ct);
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            return Result<bool>.Failure($"Lỗi giao dịch chuyển tiền: {ex.Message}");
        }
    }

    // Reconciles wallet balance against actual count and logs immutable audit trail
    public async Task<Result<BalanceAdjustmentDto>> AdjustBalanceAsync(Guid userId, Guid walletId, AdjustBalanceRequest request, CancellationToken ct = default)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);

        if (wallet == null)
            return Result<BalanceAdjustmentDto>.Failure("Ví không tồn tại.");

        // UC-01.09 E1: Reconcile error if difference is 0
        if (request.NewBalance == wallet.Balance)
            return Result<BalanceAdjustmentDto>.Failure("Số dư thực tế hoàn toàn trùng khớp với sổ sách hệ thống. Bạn không cần thực hiện cân chỉnh.");

        // UC-01.09 E2: Negative actual balance constraint for cash and bank accounts
        if (request.NewBalance < 0 && wallet.WalletType != WalletType.CreditCard)
            return Result<BalanceAdjustmentDto>.Failure("Số dư thực tế của ví tiền mặt hoặc tài khoản ngân hàng không thể là số âm.");

        // UC-01.09 E3: Mandatory justification reason
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result<BalanceAdjustmentDto>.Failure("Vui lòng chọn hoặc nhập lý do cân chỉnh để phục vụ việc theo dõi và minh bạch tài chính.");

        var oldBalance = wallet.Balance;
        var adjustment = new BalanceAdjustment(
            walletId: wallet.Id,
            oldBalance: oldBalance,
            newBalance: request.NewBalance,
            reason: request.Reason,
            adjustmentDate: request.AdjustmentDate,
            proofImageUrl: request.ProofImageUrl
        );

        _context.BalanceAdjustments.Add(adjustment);
        wallet.SetReconciledBalance(request.NewBalance);

        await _context.SaveChangesAsync(ct);

        if (_realtimeNotifier != null)
        {
            _ = _realtimeNotifier.NotifyBalanceUpdateAsync(userId, new BalanceUpdateNotificationDto(
                wallet.Id, wallet.Balance, adjustment.DifferenceAmount, $"Cân chỉnh số dư: {adjustment.Reason}", DateTime.UtcNow), ct);
        }

        var dto = new BalanceAdjustmentDto(
            adjustment.Id,
            adjustment.WalletId,
            adjustment.OldBalance,
            adjustment.NewBalance,
            adjustment.DifferenceAmount,
            adjustment.Reason,
            adjustment.AdjustmentDate,
            adjustment.ProofImageUrl,
            adjustment.CreatedAt
        );

        return Result<BalanceAdjustmentDto>.Success(dto);
    }

    // Retrieves historical balance adjustments for audit review
    public async Task<Result<List<BalanceAdjustmentDto>>> GetAdjustmentHistoryAsync(Guid userId, Guid walletId, CancellationToken ct = default)
    {
        var walletExists = await _context.Wallets
            .AnyAsync(w => w.Id == walletId && w.UserId == userId, ct);

        if (!walletExists)
            return Result<List<BalanceAdjustmentDto>>.Failure("Ví không tồn tại.");

        var adjustments = await _context.BalanceAdjustments
            .AsNoTracking()
            .Where(a => a.WalletId == walletId)
            .OrderByDescending(a => a.AdjustmentDate)
            .Select(a => new BalanceAdjustmentDto(
                a.Id,
                a.WalletId,
                a.OldBalance,
                a.NewBalance,
                a.DifferenceAmount,
                a.Reason,
                a.AdjustmentDate,
                a.ProofImageUrl,
                a.CreatedAt
            ))
            .ToListAsync(ct);

        return Result<List<BalanceAdjustmentDto>>.Success(adjustments);
    }

    // Internal projection helper
    private static WalletDto MapToDto(Wallet w) => new(
        w.Id,
        w.UserId,
        w.Name,
        w.WalletType,
        w.Balance,
        w.InitialBalance,
        w.Currency,
        w.AccountNumber,
        w.BankName,
        w.CreditLimit,
        w.StatementDay,
        w.PaymentDueDay,
        w.ColorHex,
        w.IconKey,
        w.DisplayOrder,
        w.IsExcludedFromTotal,
        w.Status,
        w.Version,
        w.CreatedAt,
        w.UpdatedAt
    );
}
