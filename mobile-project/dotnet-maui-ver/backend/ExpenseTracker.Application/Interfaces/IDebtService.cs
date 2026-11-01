using ExpenseTracker.Application.DTOs.Debts;

namespace ExpenseTracker.Application.Interfaces;

// Giao diện nghiệp vụ quản lý danh mục nợ cá nhân và thanh toán trả nợ (UC-06.05 & UC-06.07)
public interface IDebtService
{
    Task<List<DebtDto>> GetDebtsAsync(Guid userId, CancellationToken ct = default);
    Task<DebtDetailDto?> GetDebtByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<DebtSummaryDto> GetDebtSummaryAsync(Guid userId, CancellationToken ct = default);
    Task<DebtDto> CreateDebtAsync(Guid userId, CreateDebtDto dto, CancellationToken ct = default);
    Task<DebtDto> UpdateDebtAsync(Guid id, Guid userId, UpdateDebtDto dto, CancellationToken ct = default);
    Task<DebtTransactionDto> RecordPaymentAsync(Guid debtId, Guid userId, RecordDebtPaymentDto dto, CancellationToken ct = default);
    Task<bool> ArchiveDebtAsync(Guid id, Guid userId, CancellationToken ct = default);
}

