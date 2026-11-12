using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Realtime.Messages;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing comprehensive Wallet CRUD, balance reconciliation and configuration
public partial class ManageWalletsViewModel : BaseViewModel
{
    private readonly IWalletRepository _walletRepository;

    public ObservableCollection<WalletModel> Wallets { get; } = new();

    public ObservableCollection<string> ColorPresets { get; } = new()
    {
        "#0066FF", // Navy Blue (Vietcombank/ACB)
        "#10B981", // Emerald Green (Cash/Tien Mat)
        "#D82D8B", // MoMo Magenta
        "#6366F1", // Indigo Tech
        "#F59E0B", // Amber Gold
        "#EF4444", // Crimson Red
        "#0F172A", // Titanium Black
        "#8B5CF6"  // Purple Royal
    };

    public ObservableCollection<string> WalletTypeNames { get; } = new()
    {
        "Tiền mặt (Cash)",
        "Tài khoản Ngân hàng (Bank Account)",
        "Ví điện tử (E-Wallet)",
        "Thẻ tín dụng (Credit Card)"
    };

    [ObservableProperty]
    private decimal _totalBalance;

    [ObservableProperty]
    private string _formattedTotalBalance = "0 VND";

    // Form Modal State
    [ObservableProperty]
    private bool _isFormVisible;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _formTitle = "Thiết Lập Ví Mới";

    [ObservableProperty]
    private Guid? _editingWalletId;

    [ObservableProperty]
    private string _nameInput = string.Empty;

    [ObservableProperty]
    private int _selectedTypeIndex = 0; // 0=Cash, 1=Bank, 2=EWallet, 3=CreditCard

    [ObservableProperty]
    private string _balanceInput = string.Empty;

    [ObservableProperty]
    private string _bankNameInput = string.Empty;

    [ObservableProperty]
    private string _accountNumberInput = string.Empty;

    [ObservableProperty]
    private string _creditLimitInput = string.Empty;

    [ObservableProperty]
    private string _statementDayInput = string.Empty;

    [ObservableProperty]
    private string _paymentDueDayInput = string.Empty;

    [ObservableProperty]
    private string _selectedColorHex = "#0066FF";

    [ObservableProperty]
    private bool _isExcludedFromTotal;

    [ObservableProperty]
    private string? _statusMessage;

    // Adjust Balance Modal State
    [ObservableProperty]
    private bool _isAdjustModalVisible;

    [ObservableProperty]
    private WalletModel? _adjustingWallet;

    [ObservableProperty]
    private string _adjustNewBalanceInput = string.Empty;

    [ObservableProperty]
    private string _adjustReasonInput = string.Empty;

    public bool IsBankDetailsVisible => SelectedTypeIndex == 1 || SelectedTypeIndex == 3;
    public bool IsCreditCardDetailsVisible => SelectedTypeIndex == 3;

    public ManageWalletsViewModel(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
        Title = "Quản Trị Các Ví";

        // Listen for balance updates
        WeakReferenceMessenger.Default.Register<WalletBalanceUpdatedMessage>(this, (r, m) =>
        {
            var existing = Wallets.FirstOrDefault(w => w.Id == m.Value.WalletId);
            if (existing != null)
            {
                var index = Wallets.IndexOf(existing);
                Wallets[index] = existing with { Balance = m.Value.NewBalance };
                RecalculateTotalBalance();
            }
        });
    }

    partial void OnSelectedTypeIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsBankDetailsVisible));
        OnPropertyChanged(nameof(IsCreditCardDetailsVisible));
    }

    [RelayCommand]
    public async Task LoadWalletsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var list = await _walletRepository.GetWalletsAsync();
            Wallets.Clear();
            foreach (var w in list)
            {
                Wallets.Add(w);
            }
            RecalculateTotalBalance();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải danh sách ví: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void RecalculateTotalBalance()
    {
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
    }

    [RelayCommand]
    public void OpenCreateForm()
    {
        IsEditMode = false;
        EditingWalletId = null;
        FormTitle = "Thiết Lập Ví Mới";

        NameInput = string.Empty;
        SelectedTypeIndex = 0;
        BalanceInput = string.Empty;
        BankNameInput = string.Empty;
        AccountNumberInput = string.Empty;
        CreditLimitInput = string.Empty;
        StatementDayInput = string.Empty;
        PaymentDueDayInput = string.Empty;
        SelectedColorHex = "#0066FF";
        IsExcludedFromTotal = false;

        ErrorMessage = null;
        IsFormVisible = true;
    }

    [RelayCommand]
    public void OpenEditForm(WalletModel? wallet)
    {
        if (wallet == null) return;

        IsEditMode = true;
        EditingWalletId = wallet.Id;
        FormTitle = $"Chỉnh Sửa: {wallet.Name}";

        NameInput = wallet.Name;
        SelectedTypeIndex = wallet.WalletType switch
        {
            MobileWalletType.Cash => 0,
            MobileWalletType.BankAccount => 1,
            MobileWalletType.EWallet => 2,
            MobileWalletType.CreditCard => 3,
            _ => 0
        };
        BalanceInput = wallet.Balance.ToString("N0").Replace(",", "").Replace(".", "");
        BankNameInput = wallet.BankName ?? string.Empty;
        AccountNumberInput = wallet.AccountNumber ?? string.Empty;
        CreditLimitInput = wallet.CreditLimit.HasValue ? wallet.CreditLimit.Value.ToString("N0").Replace(",", "").Replace(".", "") : string.Empty;
        StatementDayInput = wallet.StatementDay.HasValue ? wallet.StatementDay.Value.ToString() : string.Empty;
        PaymentDueDayInput = wallet.PaymentDueDay.HasValue ? wallet.PaymentDueDay.Value.ToString() : string.Empty;
        SelectedColorHex = string.IsNullOrEmpty(wallet.ColorHex) ? "#0066FF" : wallet.ColorHex;
        IsExcludedFromTotal = wallet.IsExcludedFromTotal;

        ErrorMessage = null;
        IsFormVisible = true;
    }

    [RelayCommand]
    public void CloseForm()
    {
        IsFormVisible = false;
        ErrorMessage = null;
    }

    [RelayCommand]
    public void SelectColor(string colorHex)
    {
        if (!string.IsNullOrEmpty(colorHex))
        {
            SelectedColorHex = colorHex;
        }
    }

    [RelayCommand]
    public async Task SaveWalletAsync()
    {
        if (string.IsNullOrWhiteSpace(NameInput))
        {
            ErrorMessage = "Vui lòng nhập tên ví.";
            return;
        }

        var walletType = SelectedTypeIndex switch
        {
            0 => MobileWalletType.Cash,
            1 => MobileWalletType.BankAccount,
            2 => MobileWalletType.EWallet,
            3 => MobileWalletType.CreditCard,
            _ => MobileWalletType.Cash
        };

        decimal balance = 0;
        if (!string.IsNullOrWhiteSpace(BalanceInput))
        {
            var clean = BalanceInput.Replace(",", "").Replace(".", "").Trim();
            decimal.TryParse(clean, out balance);
        }

        decimal? creditLimit = null;
        if (!string.IsNullOrWhiteSpace(CreditLimitInput))
        {
            var clean = CreditLimitInput.Replace(",", "").Replace(".", "").Trim();
            if (decimal.TryParse(clean, out var cl)) creditLimit = cl;
        }

        int? statementDay = null;
        if (int.TryParse(StatementDayInput, out var sd) && sd >= 1 && sd <= 31) statementDay = sd;

        int? paymentDueDay = null;
        if (int.TryParse(PaymentDueDayInput, out var pdd) && pdd >= 1 && pdd <= 31) paymentDueDay = pdd;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (IsEditMode && EditingWalletId.HasValue)
            {
                var updateReq = new UpdateWalletModel(
                    Name: NameInput.Trim(),
                    AccountNumber: string.IsNullOrWhiteSpace(AccountNumberInput) ? null : AccountNumberInput.Trim(),
                    BankName: string.IsNullOrWhiteSpace(BankNameInput) ? null : BankNameInput.Trim(),
                    CreditLimit: creditLimit,
                    StatementDay: statementDay,
                    PaymentDueDay: paymentDueDay,
                    ColorHex: SelectedColorHex,
                    IconKey: walletType.ToString().ToLowerInvariant(),
                    IsExcludedFromTotal: IsExcludedFromTotal
                );

                await _walletRepository.UpdateWalletAsync(EditingWalletId.Value, updateReq);
                StatusMessage = $"Đã cập nhật ví '{NameInput.Trim()}' thành công!";
            }
            else
            {
                var createReq = new CreateWalletModel(
                    Name: NameInput.Trim(),
                    WalletType: walletType,
                    InitialBalance: balance,
                    Currency: "VND",
                    AccountNumber: string.IsNullOrWhiteSpace(AccountNumberInput) ? null : AccountNumberInput.Trim(),
                    BankName: string.IsNullOrWhiteSpace(BankNameInput) ? null : BankNameInput.Trim(),
                    CreditLimit: creditLimit,
                    StatementDay: statementDay,
                    PaymentDueDay: paymentDueDay,
                    ColorHex: SelectedColorHex,
                    IconKey: walletType.ToString().ToLowerInvariant(),
                    IsExcludedFromTotal: IsExcludedFromTotal
                );

                await _walletRepository.CreateWalletAsync(createReq);
                StatusMessage = $"Đã tạo ví mới '{NameInput.Trim()}' thành công!";
            }

            IsFormVisible = false;
            await LoadWalletsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi lưu ví: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteWalletAsync(WalletModel? wallet)
    {
        if (wallet == null) return;

        if (Wallets.Count <= 1)
        {
            ErrorMessage = "Bạn phải duy trì ít nhất 1 ví trong hệ thống.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _walletRepository.DeleteWalletAsync(wallet.Id);
            Wallets.Remove(wallet);
            RecalculateTotalBalance();
            StatusMessage = $"Đã xóa ví '{wallet.Name}' thành công.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xóa ví: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void OpenAdjustBalance(WalletModel? wallet)
    {
        if (wallet == null) return;

        AdjustingWallet = wallet;
        AdjustNewBalanceInput = wallet.Balance.ToString("N0").Replace(",", "").Replace(".", "");
        AdjustReasonInput = "Đối chiếu sao kê thực tế";
        ErrorMessage = null;
        IsAdjustModalVisible = true;
    }

    [RelayCommand]
    public void CloseAdjustBalance()
    {
        IsAdjustModalVisible = false;
        AdjustingWallet = null;
        ErrorMessage = null;
    }

    [RelayCommand]
    public async Task SaveAdjustBalanceAsync()
    {
        if (AdjustingWallet == null) return;

        var clean = AdjustNewBalanceInput.Replace(",", "").Replace(".", "").Trim();
        if (!decimal.TryParse(clean, out var newBalance) || newBalance < 0)
        {
            ErrorMessage = "Số dư thực tế mới không hợp lệ.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var req = new AdjustBalanceModel(
                NewBalance: newBalance,
                Reason: string.IsNullOrWhiteSpace(AdjustReasonInput) ? "Cân chỉnh số dư" : AdjustReasonInput.Trim(),
                AdjustmentDate: DateTime.UtcNow
            );

            await _walletRepository.AdjustBalanceAsync(AdjustingWallet.Id, req);
            StatusMessage = $"Đã cân chỉnh số dư ví '{AdjustingWallet.Name}' thành {newBalance:N0} VND!";
            IsAdjustModalVisible = false;
            AdjustingWallet = null;

            await LoadWalletsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi cân chỉnh số dư: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

