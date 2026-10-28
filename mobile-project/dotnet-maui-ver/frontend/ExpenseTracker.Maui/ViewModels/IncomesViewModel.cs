using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Models.Transactions;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Maui.Services.Realtime.Messages;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing personal income tracking, form submission and monthly aggregation
public partial class IncomesViewModel : BaseViewModel
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ObservableCollection<TransactionModel> Incomes { get; } = new();
    public ObservableCollection<WalletModel> Wallets { get; } = new();
    public ObservableCollection<CategoryModel> IncomeCategories { get; } = new();

    [ObservableProperty]
    private decimal _monthlyTotalIncome;

    [ObservableProperty]
    private string _formattedMonthlyTotal = "0 VND";

    // Form binding fields
    [ObservableProperty]
    private WalletModel? _selectedWallet;

    [ObservableProperty]
    private CategoryModel? _selectedCategory;

    [ObservableProperty]
    private string _amountInput = string.Empty;

    [ObservableProperty]
    private DateTime _transactionDate = DateTime.Now;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private bool _isWindfall;

    public IncomesViewModel(ITransactionRepository transactionRepository, IWalletRepository walletRepository, ICategoryRepository categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _categoryRepository = categoryRepository;
        Title = "Sổ Thu Nhập Cá Nhân";

        // Listen for realtime transaction created events (Instant 0ms timeline prepend)
        WeakReferenceMessenger.Default.Register<TransactionCreatedMessage>(this, (r, m) =>
        {
            if (m.Value.TransactionType == 1) // Income
            {
                var existing = Incomes.FirstOrDefault(e => e.Id == m.Value.TransactionId);
                if (existing == null)
                {
                    var newIncome = new TransactionModel(
                        m.Value.TransactionId,
                        Guid.Empty,
                        m.Value.WalletId,
                        m.Value.WalletName,
                        m.Value.CategoryId,
                        m.Value.CategoryName,
                        "income.png",
                        "#10B981",
                        m.Value.Amount,
                        m.Value.TransactionType,
                        m.Value.TransactionDate,
                        m.Value.Note,
                        true,
                        false,
                        false,
                        false,
                        m.Value.Timestamp
                    );
                    Incomes.Insert(0, newIncome);
                    MonthlyTotalIncome += m.Value.Amount;
                    FormattedMonthlyTotal = $"{MonthlyTotalIncome:N0} VND";
                }
            }
        });

        // Listen for server mode changes (Live API <-> Offline Mock)
        WeakReferenceMessenger.Default.Register<ServerModeChangedMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadIncomesAsync();
            });
        });
    }

    [RelayCommand]
    public async Task LoadIncomesAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            // Load list of incomes (TransactionType = 1)
            var transactions = await _transactionRepository.GetTransactionsAsync(transactionType: 1);
            Incomes.Clear();
            foreach (var t in transactions)
            {
                Incomes.Add(t);
            }

            // Load monthly total
            var now = DateTime.UtcNow;
            MonthlyTotalIncome = await _transactionRepository.GetMonthlyIncomeTotalAsync(now.Month, now.Year);
            FormattedMonthlyTotal = $"{MonthlyTotalIncome:N0} VND";

            // Load reference wallets and income categories for form pickers
            var walletsList = await _walletRepository.GetWalletsAsync();
            Wallets.Clear();
            foreach (var w in walletsList) Wallets.Add(w);
            if (Wallets.Count > 0 && SelectedWallet == null)
            {
                SelectedWallet = Wallets[0];
            }

            var catList = await _categoryRepository.GetCategoriesAsync(1); // 1 = Income
            IncomeCategories.Clear();
            foreach (var c in catList) IncomeCategories.Add(c);
            if (IncomeCategories.Count > 0 && SelectedCategory == null)
            {
                SelectedCategory = IncomeCategories[0];
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải dữ liệu thu nhập: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CreateIncomeAsync()
    {
        if (SelectedWallet == null)
        {
            ErrorMessage = "Vui lòng chọn ví nhận tiền.";
            return;
        }

        if (SelectedCategory == null)
        {
            ErrorMessage = "Vui lòng chọn danh mục thu nhập.";
            return;
        }

        if (!decimal.TryParse(AmountInput, out var amount) || amount <= 0)
        {
            ErrorMessage = "Số tiền thu nhập phải lớn hơn 0.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var req = new CreateIncomeModel(
                WalletId: SelectedWallet.Id,
                CategoryId: SelectedCategory.Id,
                Amount: amount,
                TransactionDate: TransactionDate,
                Note: string.IsNullOrWhiteSpace(Note) ? null : Note.Trim(),
                IsWindfall: IsWindfall
            );

            await _transactionRepository.CreateIncomeAsync(req);

            // Reset form inputs
            AmountInput = string.Empty;
            Note = string.Empty;
            IsWindfall = false;

            // Refresh list
            await LoadIncomesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi ghi nhận thu nhập: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteIncomeAsync(TransactionModel item)
    {
        if (item == null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _transactionRepository.DeleteIncomeAsync(item.Id);
            await LoadIncomesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xóa khoản thu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
