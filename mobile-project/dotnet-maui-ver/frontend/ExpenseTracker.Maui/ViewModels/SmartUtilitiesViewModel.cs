using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.SmartUtilities;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing Smart Financial Utilities: Subscription Trap Audit, VietQR Napas 247, and Voice STT (UC-03.09 - 03.11)
public partial class SmartUtilitiesViewModel : BaseViewModel
{
    private readonly ISmartUtilityRepository _smartUtilityRepo;
    private readonly IWalletRepository _walletRepo;

    public ObservableCollection<DetectedSubscriptionModel> Subscriptions { get; } = new();
    public ObservableCollection<WalletModel> BankWallets { get; } = new();

    // UC-03.09: Subscription Trap Audit fields
    [ObservableProperty]
    private decimal _totalMonthlyDrain;

    [ObservableProperty]
    private decimal _totalAnnualDrain;

    [ObservableProperty]
    private string _financialHealthAlert = string.Empty;

    [ObservableProperty]
    private bool _hasSubscriptionAudit;

    // UC-03.10: VietQR Generator fields
    [ObservableProperty]
    private string _bankBin = "970422"; // Default MBBank

    [ObservableProperty]
    private string _accountNumber = string.Empty;

    [ObservableProperty]
    private string _accountName = string.Empty;

    [ObservableProperty]
    private decimal _qrAmount;

    [ObservableProperty]
    private string _qrNote = string.Empty;

    [ObservableProperty]
    private VietQrResultModel? _generatedQr;

    [ObservableProperty]
    private bool _hasGeneratedQr;

    [ObservableProperty]
    private WalletModel? _selectedBankWallet;

    // Status & Feedback
    [ObservableProperty]
    private string? _statusMessage;

    public SmartUtilitiesViewModel(
        ISmartUtilityRepository smartUtilityRepo,
        IWalletRepository walletRepo)
    {
        _smartUtilityRepo = smartUtilityRepo;
        _walletRepo = walletRepo;
        Title = "Tiện Ích Tài Chính";
    }

    [RelayCommand]
    public async Task InitializeDataAsync()
    {
        await LoadBankWalletsAsync();
        await AuditSubscriptionsAsync();
    }

    [RelayCommand]
    public async Task LoadBankWalletsAsync()
    {
        try
        {
            var wallets = await _walletRepo.GetWalletsAsync();
            BankWallets.Clear();
            foreach (var w in wallets.Where(x => x.WalletType == MobileWalletType.BankAccount || x.WalletType == MobileWalletType.EWallet))
            {
                BankWallets.Add(w);
            }

            if (BankWallets.Count > 0 && SelectedBankWallet == null)
            {
                SelectedBankWallet = BankWallets[0];
                AccountNumber = SelectedBankWallet.AccountNumber ?? string.Empty;
                AccountName = SelectedBankWallet.Name;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi nạp danh sách tài khoản ngân hàng: {ex.Message}";
        }
    }

    partial void OnSelectedBankWalletChanged(WalletModel? value)
    {
        if (value != null)
        {
            AccountNumber = value.AccountNumber ?? string.Empty;
            AccountName = value.Name;
        }
    }

    [RelayCommand]
    public async Task AuditSubscriptionsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var result = await _smartUtilityRepo.DetectSubscriptionsAsync();
            Subscriptions.Clear();
            foreach (var sub in result.Subscriptions)
            {
                Subscriptions.Add(sub);
            }

            TotalMonthlyDrain = result.TotalMonthlyDrain;
            TotalAnnualDrain = result.TotalAnnualDrain;
            FinancialHealthAlert = result.FinancialHealthAlert;
            HasSubscriptionAudit = Subscriptions.Count > 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi rà soát bẫy thuê bao: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task GenerateVietQrAsync()
    {
        if (string.IsNullOrWhiteSpace(AccountNumber))
        {
            ErrorMessage = "Vui lòng nhập số tài khoản ngân hàng.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var request = new GenerateVietQrModel(
                BankBin,
                AccountNumber.Trim(),
                string.IsNullOrWhiteSpace(AccountName) ? null : AccountName.Trim(),
                QrAmount > 0 ? QrAmount : null,
                string.IsNullOrWhiteSpace(QrNote) ? null : QrNote.Trim()
            );

            GeneratedQr = await _smartUtilityRepo.GenerateVietQrAsync(request);
            HasGeneratedQr = true;
            StatusMessage = "Đã tạo mã VietQR Napas 247 thành công.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tạo mã VietQR: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
