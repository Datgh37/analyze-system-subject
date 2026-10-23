using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Realtime.Messages;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing user's multi-wallet overview, balance aggregation and quick operations (MVVM Architecture)
public partial class WalletsViewModel : BaseViewModel
{
    private readonly IWalletRepository _walletRepository;

    public ObservableCollection<WalletModel> Wallets { get; } = new();

    [ObservableProperty]
    private decimal _totalBalance;

    [ObservableProperty]
    private string _formattedTotalBalance = "0 VND";

    [ObservableProperty]
    private WalletModel? _selectedWallet;

    [ObservableProperty]
    private bool _isRealtimeConnected;

    [ObservableProperty]
    private string _realtimeStatusText = "Chờ kết nối Realtime...";

    [ObservableProperty]
    private string _realtimeDotColor = "#9CA3AF";

    [ObservableProperty]
    private string? _statusMessage;

    public WalletsViewModel(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
        Title = "Quản lý Đa Ví";

        // Listen for realtime connection state changes (Indicator Dot)
        WeakReferenceMessenger.Default.Register<RealtimeConnectionChangedMessage>(this, (r, m) =>
        {
            IsRealtimeConnected = m.Value.IsConnected;
            RealtimeStatusText = m.Value.StatusText;
            RealtimeDotColor = m.Value.ColorHex;
        });

        // Listen for realtime balance updates (Instant 0ms balance update without page reload)
        WeakReferenceMessenger.Default.Register<WalletBalanceUpdatedMessage>(this, (r, m) =>
        {
            var existing = Wallets.FirstOrDefault(w => w.Id == m.Value.WalletId);
            if (existing != null)
            {
                var index = Wallets.IndexOf(existing);
                var updated = existing with { Balance = m.Value.NewBalance };
                Wallets[index] = updated;

                decimal total = 0;
                foreach (var w in Wallets)
                {
                    if (!w.IsExcludedFromTotal)
                    {
                        total += w.Balance;
                    }
                }
                TotalBalance = total;
                FormattedTotalBalance = $"{total:N0} VND";
                StatusMessage = $"[Realtime] {existing.Name}: {m.Value.NewBalance:N0} VND";
            }
        });
    }

    // Fetches active wallets from repository and calculates total net worth
    [RelayCommand]
    public async Task LoadWalletsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await InternalLoadWalletsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải danh sách ví: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task InternalLoadWalletsAsync()
    {
        var walletsList = await _walletRepository.GetWalletsAsync();
        Wallets.Clear();

        decimal total = 0;
        foreach (var w in walletsList)
        {
            Wallets.Add(w);
            if (!w.IsExcludedFromTotal)
            {
                total += w.Balance;
            }
        }

        TotalBalance = total;
        FormattedTotalBalance = $"{total:N0} VND";
    }

    // Executes quick money transfer between wallets
    [RelayCommand]
    public async Task TransferMoneyAsync(TransferMoneyModel transfer)
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _walletRepository.TransferMoneyAsync(transfer);
            await InternalLoadWalletsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Chuyển tiền thất bại: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Reconciles wallet balance
    [RelayCommand]
    public async Task AdjustBalanceAsync(AdjustBalanceModel adjustment)
    {
        if (SelectedWallet == null || IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _walletRepository.AdjustBalanceAsync(SelectedWallet.Id, adjustment);
            await InternalLoadWalletsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Cân chỉnh số dư thất bại: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Soft-deletes a selected wallet
    [RelayCommand]
    public async Task DeleteWalletAsync(WalletModel wallet)
    {
        if (wallet == null || IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _walletRepository.DeleteWalletAsync(wallet.Id);
            Wallets.Remove(wallet);
            await InternalLoadWalletsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể xóa ví: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

