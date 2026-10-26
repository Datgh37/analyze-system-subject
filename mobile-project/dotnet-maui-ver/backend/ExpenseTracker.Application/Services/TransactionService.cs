using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Service implementing financial ledger operations and ACID wallet balance reconciliation
public class TransactionService : ITransactionService
{
    private readonly IApplicationDbContext _context;
    private readonly IFinancialRealtimeNotifier? _realtimeNotifier;
    private readonly INotificationService? _notificationService;

    public TransactionService(
        IApplicationDbContext context,
        IFinancialRealtimeNotifier? realtimeNotifier = null,
        INotificationService? notificationService = null)
    {
        _context = context;
        _realtimeNotifier = realtimeNotifier;
        _notificationService = notificationService;
    }

    // Queries financial transactions matching multi-criteria filter
    public async Task<Result<List<TransactionDto>>> GetTransactionsAsync(Guid userId, TransactionFilterRequest filter, CancellationToken ct = default)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Include(t => t.Wallet)
            .Include(t => t.Category)
            .Where(t => t.UserId == userId);

        if (filter.TransactionType.HasValue)
            query = query.Where(t => t.TransactionType == filter.TransactionType.Value);

        if (filter.WalletId.HasValue)
            query = query.Where(t => t.WalletId == filter.WalletId.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);

        if (filter.FromDate.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(filter.FromDate.Value, DateTimeKind.Utc);
            query = query.Where(t => t.TransactionDate >= fromUtc);
        }

        if (filter.ToDate.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(filter.ToDate.Value, DateTimeKind.Utc);
            query = query.Where(t => t.TransactionDate <= toUtc);
        }

        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => MapToDto(t))
            .ToListAsync(ct);

        return Result<List<TransactionDto>>.Success(transactions);
    }

    // Retrieves transaction by ID
    public async Task<Result<TransactionDto>> GetTransactionByIdAsync(Guid userId, Guid transactionId, CancellationToken ct = default)
    {
        var transaction = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Wallet)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId, ct);

        if (transaction == null)
            return Result<TransactionDto>.Failure("Giao dịch không tồn tại hoặc bạn không có quyền truy cập.", 404);

        return Result<TransactionDto>.Success(MapToDto(transaction));
    }

    // Records a new income transaction and credits the destination wallet balance atomically
    public async Task<Result<TransactionDto>> CreateIncomeAsync(Guid userId, CreateIncomeTransactionRequest request, CancellationToken ct = default)
    {
        await using var dbTransaction = await _context.BeginTransactionAsync(ct);
        try
        {
            var wallet = await _context.GetWalletForUpdateAsync(request.WalletId, userId, ct);

            if (wallet == null)
                return Result<TransactionDto>.Failure("Ví tiếp nhận không tồn tại hoặc bạn không có quyền sử dụng ví này.");

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.IsSystem || c.UserId == userId), ct);

            if (category == null)
                return Result<TransactionDto>.Failure("Danh mục thu nhập không hợp lệ.");

            if (category.TransactionType != TransactionType.Income)
                return Result<TransactionDto>.Failure("Danh mục đã chọn không thuộc loại Thu nhập.");

            // Credit destination wallet balance
            wallet.Credit(request.Amount);

            // Record income transaction in ledger
            var transaction = new Transaction(
                userId: userId,
                walletId: wallet.Id,
                categoryId: category.Id,
                amount: request.Amount,
                transactionType: TransactionType.Income,
                transactionDate: request.TransactionDate,
                note: request.Note,
                isEssential: false,
                isWindfall: request.IsWindfall,
                isRecurring: request.IsRecurring,
                excludeFromReport: request.ExcludeFromReport
            );

            await _context.Transactions.AddAsync(transaction, ct);
            await _context.SaveChangesAsync(ct);
            await dbTransaction.CommitAsync(ct);

            if (_realtimeNotifier != null)
            {
                _ = _realtimeNotifier.NotifyBalanceUpdateAsync(userId, new BalanceUpdateNotificationDto(
                    wallet.Id, wallet.Balance, request.Amount, $"Thu nhập: {category.Name}", DateTime.UtcNow), ct);

                _ = _realtimeNotifier.NotifyTransactionCreatedAsync(userId, new TransactionCreatedNotificationDto(
                    transaction.Id, wallet.Id, wallet.Name, category.Id, category.Name,
                    request.Amount, (int)transaction.TransactionType, transaction.TransactionDate, transaction.Note, DateTime.UtcNow), ct);
            }


            var dto = new TransactionDto(
                transaction.Id,
                transaction.UserId,
                wallet.Id,
                wallet.Name,
                category.Id,
                category.Name,
                category.IconKey,
                category.ColorHex,
                transaction.Amount,
                transaction.TransactionType,
                transaction.TransactionDate,
                transaction.Note,
                transaction.IsEssential,
                transaction.IsWindfall,
                transaction.IsRecurring,
                transaction.ExcludeFromReport,
                transaction.CreatedAt
            );

            return Result<TransactionDto>.Success(dto);
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<TransactionDto>.Failure($"Lỗi khi ghi nhận thu nhập: {ex.Message}");
        }
    }

    // Updates an existing income transaction and reconciles wallet balances atomically
    public async Task<Result<TransactionDto>> UpdateIncomeAsync(Guid userId, Guid transactionId, UpdateIncomeTransactionRequest request, CancellationToken ct = default)
    {
        await using var dbTransaction = await _context.BeginTransactionAsync(ct);
        try
        {
            var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId, ct);

            if (transaction == null)
                return Result<TransactionDto>.Failure("Giao dịch không tồn tại.", 404);

            if (transaction.TransactionType != TransactionType.Income)
                return Result<TransactionDto>.Failure("Chỉ có thể cập nhật giao dịch loại Thu nhập qua chức năng này.");

            var newCategory = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.IsSystem || c.UserId == userId), ct);

            if (newCategory == null || newCategory.TransactionType != TransactionType.Income)
                return Result<TransactionDto>.Failure("Danh mục thu nhập mới không hợp lệ.");

            // Wallet balance reconciliation logic
            if (transaction.WalletId != request.WalletId)
            {
                // Revert money from old wallet
                var oldWallet = transaction.Wallet;
                oldWallet.Debit(transaction.Amount);

                // Credit money to new destination wallet
                var newWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.Id == request.WalletId && w.UserId == userId, ct);

                if (newWallet == null)
                    return Result<TransactionDto>.Failure("Ví nhận mới không tồn tại hoặc không hợp lệ.");

                newWallet.Credit(request.Amount);
            }
            else
            {
                // Same wallet: compute delta
                var delta = request.Amount - transaction.Amount;
                if (delta > 0)
                {
                    transaction.Wallet.Credit(delta);
                }
                else if (delta < 0)
                {
                    transaction.Wallet.Debit(Math.Abs(delta));
                }
            }

            transaction.UpdateDetails(
                walletId: request.WalletId,
                categoryId: request.CategoryId,
                amount: request.Amount,
                transactionDate: request.TransactionDate,
                note: request.Note,
                isEssential: false,
                isWindfall: request.IsWindfall,
                isRecurring: request.IsRecurring,
                excludeFromReport: request.ExcludeFromReport
            );

            await _context.SaveChangesAsync(ct);
            await dbTransaction.CommitAsync(ct);

            var updatedWallet = await _context.Wallets.AsNoTracking().FirstAsync(w => w.Id == request.WalletId, ct);
            var dto = new TransactionDto(
                transaction.Id,
                transaction.UserId,
                updatedWallet.Id,
                updatedWallet.Name,
                newCategory.Id,
                newCategory.Name,
                newCategory.IconKey,
                newCategory.ColorHex,
                transaction.Amount,
                transaction.TransactionType,
                transaction.TransactionDate,
                transaction.Note,
                transaction.IsEssential,
                transaction.IsWindfall,
                transaction.IsRecurring,
                transaction.ExcludeFromReport,
                transaction.CreatedAt
            );

            return Result<TransactionDto>.Success(dto);
        }
        catch (InvalidOperationException)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<TransactionDto>.Failure("Không thể hoàn tác hoặc điều chỉnh khoản thu này vì số dư khả dụng hiện tại của ví không đủ để khấu trừ (bạn đã chi tiêu từ nguồn tiền này).");
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<TransactionDto>.Failure($"Lỗi khi cập nhật thu nhập: {ex.Message}");
        }
    }

    // Soft-deletes income transaction and reverts credited money from wallet
    public async Task<Result<bool>> DeleteIncomeAsync(Guid userId, Guid transactionId, CancellationToken ct = default)
    {
        await using var dbTransaction = await _context.BeginTransactionAsync(ct);
        try
        {
            var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId, ct);

            if (transaction == null)
                return Result<bool>.Failure("Giao dịch không tồn tại.", 404);

            if (transaction.TransactionType != TransactionType.Income)
                return Result<bool>.Failure("Chỉ áp dụng xóa đối với giao dịch Thu nhập.");

            // Revert income amount from wallet
            transaction.Wallet.Debit(transaction.Amount);

            // Stamp soft delete
            transaction.MarkAsDeleted();

            await _context.SaveChangesAsync(ct);
            await dbTransaction.CommitAsync(ct);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<bool>.Failure("Không thể hoàn tác khoản thu này vì số dư khả dụng hiện tại của ví không đủ để khấu trừ (bạn đã chi tiêu từ nguồn tiền này).");
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<bool>.Failure($"Lỗi khi xóa khoản thu nhập: {ex.Message}");
        }
    }

    // Calculates total net income for a specific month
    public async Task<Result<decimal>> GetTotalIncomeAmountAsync(Guid userId, int month, int year, CancellationToken ct = default)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        var total = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId 
                     && t.TransactionType == TransactionType.Income 
                     && !t.ExcludeFromReport
                     && t.TransactionDate >= startDate 
                     && t.TransactionDate < endDate)
            .SumAsync(t => t.Amount, ct);

        return Result<decimal>.Success(total);
    }

    // Records a new expense transaction and debits the source wallet balance atomically (UC-03.01)
    public async Task<Result<TransactionDto>> CreateExpenseAsync(Guid userId, CreateExpenseTransactionRequest request, CancellationToken ct = default)
    {
        await using var dbTransaction = await _context.BeginTransactionAsync(ct);
        try
        {
            var wallet = await _context.GetWalletForUpdateAsync(request.WalletId, userId, ct);

            if (wallet == null)
                return Result<TransactionDto>.Failure("Ví thanh toán không tồn tại hoặc bạn không có quyền sử dụng ví này.");

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.IsSystem || c.UserId == userId), ct);

            if (category == null)
                return Result<TransactionDto>.Failure("Danh mục chi tiêu không hợp lệ.");

            if (category.TransactionType != TransactionType.Expense)
                return Result<TransactionDto>.Failure("Danh mục đã chọn không thuộc loại Chi tiêu.");

            // Debit source wallet balance (handles non-negative constraint and credit limits)
            wallet.Debit(request.Amount);

            var transaction = new Transaction(
                userId: userId,
                walletId: wallet.Id,
                categoryId: category.Id,
                amount: request.Amount,
                transactionType: TransactionType.Expense,
                transactionDate: request.TransactionDate,
                note: request.Note,
                isEssential: request.IsEssential,
                isWindfall: false,
                isRecurring: request.IsRecurring,
                excludeFromReport: request.ExcludeFromReport
            );

            await _context.Transactions.AddAsync(transaction, ct);

            // Add itemized lines if provided
            if (request.Items != null && request.Items.Count > 0)
            {
                foreach (var item in request.Items)
                {
                    var receiptItem = new ReceiptItem(
                        transactionId: transaction.Id,
                        itemName: item.ItemName,
                        quantity: item.Quantity,
                        unitPrice: item.UnitPrice,
                        totalPrice: item.TotalPrice,
                        categoryId: item.CategoryId
                    );
                    await _context.ReceiptItems.AddAsync(receiptItem, ct);
                }
            }

            // Synchronize with active budget allocation (UC-05.02 / UC-05.04)
            var activeBudget = await _context.Budgets
                .Include(b => b.Allocations)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Status == BudgetStatus.Active && b.StartDate <= request.TransactionDate && b.EndDate >= request.TransactionDate, ct);

            if (activeBudget != null)
            {
                activeBudget.RecordExpense(category.Id, request.Amount);
            }

            await _context.SaveChangesAsync(ct);
            await dbTransaction.CommitAsync(ct);

            if (_realtimeNotifier != null)
            {
                _ = _realtimeNotifier.NotifyBalanceUpdateAsync(userId, new BalanceUpdateNotificationDto(
                    wallet.Id, wallet.Balance, -request.Amount, $"Chi tiêu: {category.Name}", DateTime.UtcNow), ct);

                _ = _realtimeNotifier.NotifyTransactionCreatedAsync(userId, new TransactionCreatedNotificationDto(
                    transaction.Id, wallet.Id, wallet.Name, category.Id, category.Name,
                    request.Amount, (int)transaction.TransactionType, transaction.TransactionDate, transaction.Note, DateTime.UtcNow), ct);
            }


            if (activeBudget != null)
            {
                var alloc = activeBudget.Allocations.FirstOrDefault(a => a.CategoryId == category.Id);
                if (alloc != null && alloc.AllocatedAmount > 0)
                {
                    var percent = Math.Round((alloc.CurrentSpent / alloc.AllocatedAmount) * 100, 1);
                    if (percent >= alloc.AlertThresholdPercent)
                    {
                        var isOver100 = percent >= 100;
                        var alertTitle = isOver100 ? "Vượt ngân sách danh mục!" : "Cảnh báo hạn mức ngân sách";
                        var alertMsg = isOver100
                            ? $"Bạn đã vượt {percent}% ngân sách '{category.Name}' ({alloc.CurrentSpent:N0}/{alloc.AllocatedAmount:N0} VND)!"
                            : $"Bạn đã sử dụng {percent}% ngân sách '{category.Name}' ({alloc.CurrentSpent:N0}/{alloc.AllocatedAmount:N0} VND).";

                        if (_notificationService != null)
                        {
                            _ = _notificationService.CreateAndSendNotificationAsync(
                                userId,
                                alertTitle,
                                alertMsg,
                                "BudgetAlert",
                                new { budgetId = activeBudget.Id, categoryId = category.Id, percentUsed = percent },
                                ct);
                        }

                        if (_realtimeNotifier != null)
                        {
                            _ = _realtimeNotifier.NotifyBudgetAlertAsync(userId, new BudgetAlertNotificationDto(
                                activeBudget.Id, category.Name, alloc.CurrentSpent, alloc.AllocatedAmount, percent, alertMsg, DateTime.UtcNow), ct);
                        }
                    }
                }
            }

            var dto = new TransactionDto(
                transaction.Id,
                transaction.UserId,
                wallet.Id,
                wallet.Name,
                category.Id,
                category.Name,
                category.IconKey,
                category.ColorHex,
                transaction.Amount,
                transaction.TransactionType,
                transaction.TransactionDate,
                transaction.Note,
                transaction.IsEssential,
                transaction.IsWindfall,
                transaction.IsRecurring,
                transaction.ExcludeFromReport,
                transaction.CreatedAt
            );

            return Result<TransactionDto>.Success(dto);
        }
        catch (InvalidOperationException ex)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<TransactionDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<TransactionDto>.Failure($"Lỗi khi ghi nhận chi tiêu: {ex.Message}");
        }
    }

    // Updates an existing expense transaction and reconciles wallet balances atomically (UC-03.04)
    public async Task<Result<TransactionDto>> UpdateExpenseAsync(Guid userId, Guid transactionId, UpdateExpenseTransactionRequest request, CancellationToken ct = default)
    {
        await using var dbTransaction = await _context.BeginTransactionAsync(ct);
        try
        {
            var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId, ct);

            if (transaction == null)
                return Result<TransactionDto>.Failure("Giao dịch không tồn tại.", 404);

            if (transaction.TransactionType != TransactionType.Expense)
                return Result<TransactionDto>.Failure("Chỉ có thể cập nhật giao dịch loại Chi tiêu qua chức năng này.");

            var newCategory = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.IsSystem || c.UserId == userId), ct);

            if (newCategory == null || newCategory.TransactionType != TransactionType.Expense)
                return Result<TransactionDto>.Failure("Danh mục chi tiêu mới không hợp lệ.");

            // Wallet reconciliation logic
            if (transaction.WalletId != request.WalletId)
            {
                // Revert money to old wallet
                transaction.Wallet.Credit(transaction.Amount);

                // Debit money from new wallet
                var newWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.Id == request.WalletId && w.UserId == userId, ct);

                if (newWallet == null)
                    return Result<TransactionDto>.Failure("Ví thanh toán mới không tồn tại hoặc không hợp lệ.");

                newWallet.Debit(request.Amount);
            }
            else
            {
                // Same wallet: compute delta
                var delta = request.Amount - transaction.Amount;
                if (delta > 0)
                {
                    transaction.Wallet.Debit(delta);
                }
                else if (delta < 0)
                {
                    transaction.Wallet.Credit(Math.Abs(delta));
                }
            }

            transaction.UpdateDetails(
                walletId: request.WalletId,
                categoryId: request.CategoryId,
                amount: request.Amount,
                transactionDate: request.TransactionDate,
                note: request.Note,
                isEssential: request.IsEssential,
                isWindfall: false,
                isRecurring: request.IsRecurring,
                excludeFromReport: request.ExcludeFromReport
            );

            // Update itemized lines if provided
            if (request.Items != null)
            {
                var existingItems = await _context.ReceiptItems
                    .Where(r => r.TransactionId == transactionId)
                    .ToListAsync(ct);

                _context.ReceiptItems.RemoveRange(existingItems);

                foreach (var item in request.Items)
                {
                    var receiptItem = new ReceiptItem(
                        transactionId: transaction.Id,
                        itemName: item.ItemName,
                        quantity: item.Quantity,
                        unitPrice: item.UnitPrice,
                        totalPrice: item.TotalPrice,
                        categoryId: item.CategoryId
                    );
                    await _context.ReceiptItems.AddAsync(receiptItem, ct);
                }
            }

            await _context.SaveChangesAsync(ct);
            await dbTransaction.CommitAsync(ct);

            var updatedWallet = await _context.Wallets.AsNoTracking().FirstAsync(w => w.Id == request.WalletId, ct);
            var dto = new TransactionDto(
                transaction.Id,
                transaction.UserId,
                updatedWallet.Id,
                updatedWallet.Name,
                newCategory.Id,
                newCategory.Name,
                newCategory.IconKey,
                newCategory.ColorHex,
                transaction.Amount,
                transaction.TransactionType,
                transaction.TransactionDate,
                transaction.Note,
                transaction.IsEssential,
                transaction.IsWindfall,
                transaction.IsRecurring,
                transaction.ExcludeFromReport,
                transaction.CreatedAt
            );

            return Result<TransactionDto>.Success(dto);
        }
        catch (InvalidOperationException ex)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<TransactionDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<TransactionDto>.Failure($"Lỗi khi cập nhật chi tiêu: {ex.Message}");
        }
    }

    // Soft-deletes expense transaction and reverts spent money to source wallet (UC-03.05)
    public async Task<Result<bool>> DeleteExpenseAsync(Guid userId, Guid transactionId, CancellationToken ct = default)
    {
        await using var dbTransaction = await _context.BeginTransactionAsync(ct);
        try
        {
            var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId, ct);

            if (transaction == null)
                return Result<bool>.Failure("Giao dịch không tồn tại.", 404);

            if (transaction.TransactionType != TransactionType.Expense)
                return Result<bool>.Failure("Chỉ có thể xóa giao dịch loại Chi tiêu qua chức năng này.");

            // Revert spent money back into wallet
            transaction.Wallet.Credit(transaction.Amount);

            // Soft-delete receipt items
            var receiptItems = await _context.ReceiptItems
                .Where(r => r.TransactionId == transactionId)
                .ToListAsync(ct);

            foreach (var item in receiptItems)
            {
                item.MarkAsDeleted();
            }

            transaction.MarkAsDeleted();

            // Synchronize with active budget allocation (UC-05.02 / UC-05.04)
            var activeBudget = await _context.Budgets
                .Include(b => b.Allocations)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Status == BudgetStatus.Active && b.StartDate <= transaction.TransactionDate && b.EndDate >= transaction.TransactionDate, ct);

            if (activeBudget != null)
            {
                activeBudget.RevertExpense(transaction.CategoryId, transaction.Amount);
            }

            await _context.SaveChangesAsync(ct);
            await dbTransaction.CommitAsync(ct);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<bool>.Failure($"Lỗi khi xóa khoản chi tiêu: {ex.Message}");
        }
    }

    // Retrieves comprehensive expense details including itemized receipt lines (UC-03.03)
    public async Task<Result<ExpenseDetailDto>> GetExpenseDetailsAsync(Guid userId, Guid transactionId, CancellationToken ct = default)
    {
        var transaction = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Wallet)
            .Include(t => t.Category)
            .Include(t => t.ReceiptItems)
                .ThenInclude(r => r.Category)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId, ct);

        if (transaction == null)
            return Result<ExpenseDetailDto>.Failure("Giao dịch không tồn tại hoặc bạn không có quyền truy cập.", 404);

        if (transaction.TransactionType != TransactionType.Expense)
            return Result<ExpenseDetailDto>.Failure("Giao dịch này không phải là giao dịch Chi tiêu.");

        var itemsDto = transaction.ReceiptItems
            .Select(i => new ReceiptItemDto(
                i.Id,
                i.ItemName,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice,
                i.CategoryId,
                i.Category?.Name
            ))
            .ToList();

        var dto = new ExpenseDetailDto(
            transaction.Id,
            transaction.UserId,
            transaction.WalletId,
            transaction.Wallet?.Name ?? string.Empty,
            transaction.CategoryId,
            transaction.Category?.Name ?? string.Empty,
            transaction.Category?.IconKey ?? "folder",
            transaction.Category?.ColorHex ?? "#EF4444",
            transaction.Amount,
            transaction.TransactionDate,
            transaction.Note,
            transaction.IsEssential,
            transaction.IsRecurring,
            transaction.ExcludeFromReport,
            itemsDto,
            transaction.CreatedAt
        );

        return Result<ExpenseDetailDto>.Success(dto);
    }

    // Calculates total net expenses for a specific month
    public async Task<Result<decimal>> GetTotalExpenseAmountAsync(Guid userId, int month, int year, CancellationToken ct = default)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        var total = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId 
                     && t.TransactionType == TransactionType.Expense 
                     && !t.ExcludeFromReport
                     && t.TransactionDate >= startDate 
                     && t.TransactionDate < endDate)
            .SumAsync(t => t.Amount, ct);

        return Result<decimal>.Success(total);
    }

    private static TransactionDto MapToDto(Transaction t)
    {
        return new TransactionDto(
            t.Id,
            t.UserId,
            t.WalletId,
            t.Wallet?.Name ?? string.Empty,
            t.CategoryId,
            t.Category?.Name ?? string.Empty,
            t.Category?.IconKey ?? "folder",
            t.Category?.ColorHex ?? "#10B981",
            t.Amount,
            t.TransactionType,
            t.TransactionDate,
            t.Note,
            t.IsEssential,
            t.IsWindfall,
            t.IsRecurring,
            t.ExcludeFromReport,
            t.CreatedAt
        );
    }
}
