using ExpenseTracker.Maui.Models.Wallets;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for wallet data operations (Clean MVVM Architecture)
public interface IWalletRepository
{
    Task<List<WalletModel>> GetWalletsAsync();
    Task<WalletModel> GetWalletByIdAsync(Guid id);
    Task<WalletModel> CreateWalletAsync(CreateWalletModel request);
    Task<WalletModel> UpdateWalletAsync(Guid id, UpdateWalletModel request);
    Task DeleteWalletAsync(Guid id);
    Task TransferMoneyAsync(TransferMoneyModel request);
    Task<BalanceAdjustmentModel> AdjustBalanceAsync(Guid id, AdjustBalanceModel request);
}
