using ExpenseTracker.Application.DTOs.Wallets;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service interface managing wallet operations and ACID internal transfers
public interface IWalletService
{
    Task<Result<List<WalletDto>>> GetUserWalletsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<WalletDto>> GetWalletByIdAsync(Guid userId, Guid walletId, CancellationToken ct = default);
    Task<Result<WalletDto>> CreateWalletAsync(Guid userId, CreateWalletRequest request, CancellationToken ct = default);
    Task<Result<WalletDto>> UpdateWalletAsync(Guid userId, Guid walletId, UpdateWalletRequest request, CancellationToken ct = default);
    Task<Result<WalletDto>> PatchWalletAsync(Guid userId, Guid walletId, PatchWalletRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteWalletAsync(Guid userId, Guid walletId, CancellationToken ct = default);
    Task<Result<bool>> TransferMoneyAsync(Guid userId, TransferMoneyRequest request, CancellationToken ct = default);
    Task<Result<BalanceAdjustmentDto>> AdjustBalanceAsync(Guid userId, Guid walletId, AdjustBalanceRequest request, CancellationToken ct = default);
    Task<Result<List<BalanceAdjustmentDto>>> GetAdjustmentHistoryAsync(Guid userId, Guid walletId, CancellationToken ct = default);
}
