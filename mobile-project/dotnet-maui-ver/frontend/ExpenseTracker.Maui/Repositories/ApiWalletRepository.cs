using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Live API implementation of IWalletRepository delegating to Refit IWalletApi
public class ApiWalletRepository : IWalletRepository
{
    private readonly IWalletApi _walletApi;

    public ApiWalletRepository(IWalletApi walletApi)
    {
        _walletApi = walletApi;
    }

    public Task<List<WalletModel>> GetWalletsAsync() => _walletApi.GetWalletsAsync();
    public Task<WalletModel> GetWalletByIdAsync(Guid id) => _walletApi.GetWalletByIdAsync(id);
    public Task<WalletModel> CreateWalletAsync(CreateWalletModel request) => _walletApi.CreateWalletAsync(request);
    public Task<WalletModel> UpdateWalletAsync(Guid id, UpdateWalletModel request) => _walletApi.UpdateWalletAsync(id, request);
    public Task DeleteWalletAsync(Guid id) => _walletApi.DeleteWalletAsync(id);
    public Task TransferMoneyAsync(TransferMoneyModel request) => _walletApi.TransferMoneyAsync(request);
    public Task<BalanceAdjustmentModel> AdjustBalanceAsync(Guid id, AdjustBalanceModel request) => _walletApi.AdjustBalanceAsync(id, request);
}
