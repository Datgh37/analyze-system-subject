using ExpenseTracker.Application.DTOs.RecurringIncomes;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Service implementing recurring income management and scheduled transaction processing (UC-02.07)
public class RecurringIncomeService : IRecurringIncomeService
{
    private readonly IApplicationDbContext _context;

    public RecurringIncomeService(IApplicationDbContext context)
    {
        _context = context;
    }

    // Retrieves all recurring income schedules for a specific user
    public async Task<Result<List<RecurringIncomeDto>>> GetRecurringIncomesAsync(Guid userId, CancellationToken ct = default)
    {
        var list = await _context.RecurringIncomes
            .AsNoTracking()
            .Include(r => r.Wallet)
            .Include(r => r.Category)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.NextDueDate)
            .Select(r => MapToDto(r))
            .ToListAsync(ct);

        return Result<List<RecurringIncomeDto>>.Success(list);
    }

    // Retrieves a specific recurring income schedule by ID
    public async Task<Result<RecurringIncomeDto>> GetRecurringIncomeByIdAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var item = await _context.RecurringIncomes
            .AsNoTracking()
            .Include(r => r.Wallet)
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId, ct);

        if (item == null)
            return Result<RecurringIncomeDto>.Failure("Nguồn thu định kỳ không tồn tại hoặc bạn không có quyền truy cập.", 404);

        return Result<RecurringIncomeDto>.Success(MapToDto(item));
    }

    // Creates a new recurring income schedule with initial due date calculation (UC-02.07)
    public async Task<Result<RecurringIncomeDto>> CreateRecurringIncomeAsync(Guid userId, CreateRecurringIncomeRequest request, CancellationToken ct = default)
    {
        var wallet = await _context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == request.WalletId && w.UserId == userId, ct);

        if (wallet == null)
            return Result<RecurringIncomeDto>.Failure("Ví nhận tiền không tồn tại hoặc bạn không có quyền truy cập.");

        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.IsSystem || c.UserId == userId), ct);

        if (category == null)
            return Result<RecurringIncomeDto>.Failure("Danh mục thu nhập không hợp lệ.");

        if (category.TransactionType != TransactionType.Income)
            return Result<RecurringIncomeDto>.Failure("Danh mục đã chọn không thuộc loại Thu nhập.");

        try
        {
            var recurringIncome = new RecurringIncome(
                userId: userId,
                name: request.Name,
                expectedAmount: request.ExpectedAmount,
                walletId: wallet.Id,
                categoryId: category.Id,
                frequency: request.Frequency,
                dayOfMonth: request.DayOfMonth,
                autoRecord: request.AutoRecord,
                startDate: request.StartDate
            );

            await _context.RecurringIncomes.AddAsync(recurringIncome, ct);
            await _context.SaveChangesAsync(ct);

            var dto = new RecurringIncomeDto(
                recurringIncome.Id,
                recurringIncome.UserId,
                recurringIncome.Name,
                recurringIncome.ExpectedAmount,
                wallet.Id,
                wallet.Name,
                category.Id,
                category.Name,
                category.IconKey,
                category.ColorHex,
                recurringIncome.Frequency,
                recurringIncome.DayOfMonth,
                recurringIncome.AutoRecord,
                recurringIncome.IsActive,
                recurringIncome.NextDueDate,
                recurringIncome.LastGeneratedDate,
                recurringIncome.CreatedAt
            );

            return Result<RecurringIncomeDto>.Success(dto);
        }
        catch (ArgumentException ex)
        {
            return Result<RecurringIncomeDto>.Failure(ex.Message);
        }
    }

    // Updates an existing recurring income schedule and recalculates the next due date
    public async Task<Result<RecurringIncomeDto>> UpdateRecurringIncomeAsync(Guid userId, Guid id, UpdateRecurringIncomeRequest request, CancellationToken ct = default)
    {
        var item = await _context.RecurringIncomes
            .Include(r => r.Wallet)
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId, ct);

        if (item == null)
            return Result<RecurringIncomeDto>.Failure("Nguồn thu định kỳ không tồn tại.", 404);

        var wallet = await _context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == request.WalletId && w.UserId == userId, ct);

        if (wallet == null)
            return Result<RecurringIncomeDto>.Failure("Ví nhận tiền không tồn tại hoặc bạn không có quyền truy cập.");

        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.IsSystem || c.UserId == userId), ct);

        if (category == null || category.TransactionType != TransactionType.Income)
            return Result<RecurringIncomeDto>.Failure("Danh mục thu nhập không hợp lệ hoặc không thuộc loại Thu nhập.");

        try
        {
            item.UpdateDetails(
                request.Name,
                request.ExpectedAmount,
                wallet.Id,
                category.Id,
                request.Frequency,
                request.DayOfMonth,
                request.AutoRecord
            );

            await _context.SaveChangesAsync(ct);

            var dto = new RecurringIncomeDto(
                item.Id,
                item.UserId,
                item.Name,
                item.ExpectedAmount,
                wallet.Id,
                wallet.Name,
                category.Id,
                category.Name,
                category.IconKey,
                category.ColorHex,
                item.Frequency,
                item.DayOfMonth,
                item.AutoRecord,
                item.IsActive,
                item.NextDueDate,
                item.LastGeneratedDate,
                item.CreatedAt
            );

            return Result<RecurringIncomeDto>.Success(dto);
        }
        catch (ArgumentException ex)
        {
            return Result<RecurringIncomeDto>.Failure(ex.Message);
        }
    }

    // Toggles active state of a recurring income schedule (UC-02.07 A1)
    public async Task<Result<RecurringIncomeDto>> ToggleRecurringIncomeStatusAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var item = await _context.RecurringIncomes
            .Include(r => r.Wallet)
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId, ct);

        if (item == null)
            return Result<RecurringIncomeDto>.Failure("Nguồn thu định kỳ không tồn tại.", 404);

        item.ToggleStatus();
        await _context.SaveChangesAsync(ct);

        return Result<RecurringIncomeDto>.Success(MapToDto(item));
    }

    // Soft-deletes recurring income schedule (preserves prior generated transactions)
    public async Task<Result<bool>> DeleteRecurringIncomeAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var item = await _context.RecurringIncomes
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId, ct);

        if (item == null)
            return Result<bool>.Failure("Nguồn thu định kỳ không tồn tại.", 404);

        item.MarkAsDeleted();
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    // Scans and automatically records due recurring incomes, crediting wallet balances atomically (UC-02.07 A2)
    public async Task<Result<ProcessRecurringIncomeResultDto>> ProcessDueRecurringIncomesAsync(
        Guid? userId = null, 
        DateTime? asOfDate = null, 
        CancellationToken ct = default)
    {
        var targetDate = asOfDate.HasValue 
            ? (asOfDate.Value.Kind == DateTimeKind.Utc ? asOfDate.Value : DateTime.SpecifyKind(asOfDate.Value, DateTimeKind.Utc))
            : DateTime.UtcNow;

        await using var dbTransaction = await _context.BeginTransactionAsync(ct);
        try
        {
            var query = _context.RecurringIncomes
                .Include(r => r.Wallet)
                .Include(r => r.Category)
                .Where(r => r.IsActive && r.AutoRecord && r.NextDueDate <= targetDate);

            if (userId.HasValue)
                query = query.Where(r => r.UserId == userId.Value);

            var dueIncomes = await query.ToListAsync(ct);

            if (dueIncomes.Count == 0)
            {
                return Result<ProcessRecurringIncomeResultDto>.Success(
                    new ProcessRecurringIncomeResultDto(0, 0m, new List<Guid>(), "Không có khoản thu định kỳ nào đến hạn cần ghi nhận."));
            }

            var generatedIds = new List<Guid>();
            decimal totalAmount = 0m;

            foreach (var income in dueIncomes)
            {
                // Verify wallet is available and not deleted
                if (income.Wallet == null || income.Wallet.IsDeleted)
                    continue;

                // Credit destination wallet balance
                income.Wallet.Credit(income.ExpectedAmount);

                // Create ledger transaction
                var transaction = new Transaction(
                    userId: income.UserId,
                    walletId: income.WalletId,
                    categoryId: income.CategoryId,
                    amount: income.ExpectedAmount,
                    transactionType: TransactionType.Income,
                    transactionDate: income.NextDueDate,
                    note: $"[Tự động] {income.Name}",
                    isEssential: false,
                    isWindfall: false,
                    isRecurring: true,
                    excludeFromReport: false
                );

                await _context.Transactions.AddAsync(transaction, ct);
                generatedIds.Add(transaction.Id);
                totalAmount += income.ExpectedAmount;

                // Advance due date for next cycle
                income.MarkProcessed(income.NextDueDate);
            }

            await _context.SaveChangesAsync(ct);
            await dbTransaction.CommitAsync(ct);

            return Result<ProcessRecurringIncomeResultDto>.Success(
                new ProcessRecurringIncomeResultDto(
                    generatedIds.Count,
                    totalAmount,
                    generatedIds,
                    $"Đã tự động ghi nhận thành công {generatedIds.Count} khoản thu định kỳ với tổng số tiền {totalAmount:N0} đ."));
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(ct);
            return Result<ProcessRecurringIncomeResultDto>.Failure($"Lỗi khi tự động xử lý thu nhập định kỳ: {ex.Message}");
        }
    }

    private static RecurringIncomeDto MapToDto(RecurringIncome r)
    {
        return new RecurringIncomeDto(
            r.Id,
            r.UserId,
            r.Name,
            r.ExpectedAmount,
            r.WalletId,
            r.Wallet?.Name ?? string.Empty,
            r.CategoryId,
            r.Category?.Name ?? string.Empty,
            r.Category?.IconKey ?? "repeat",
            r.Category?.ColorHex ?? "#10B981",
            r.Frequency,
            r.DayOfMonth,
            r.AutoRecord,
            r.IsActive,
            r.NextDueDate,
            r.LastGeneratedDate,
            r.CreatedAt
        );
    }
}
