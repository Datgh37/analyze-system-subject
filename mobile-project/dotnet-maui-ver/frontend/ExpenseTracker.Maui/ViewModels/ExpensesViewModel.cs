using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Models.Transactions;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Api;

using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Maui.Services.Realtime.Messages;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing personal expense tracking, timeline ledger, and real-time auto-categorization (Clean MVVM Architecture)
public partial class ExpensesViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAutoRuleRepository _autoRuleRepository;

    public ObservableCollection<TransactionModel> Expenses { get; } = new();
    public ObservableCollection<WalletModel> Wallets { get; } = new();
    public ObservableCollection<CategoryModel> Categories { get; } = new();

    [ObservableProperty]
    private decimal _monthlyTotalExpense;

    [ObservableProperty]
    private decimal _newAmount;

    [ObservableProperty]
    private WalletModel? _selectedWallet;

    [ObservableProperty]
    private CategoryModel? _selectedCategory;

    [ObservableProperty]
    private DateTime _transactionDate = DateTime.Today;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private bool _isEssential = true;

    [ObservableProperty]
    private string? _autoMatchedHint;

    public ExpensesViewModel(
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        ICategoryRepository categoryRepository,
        IAutoRuleRepository autoRuleRepository)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _categoryRepository = categoryRepository;
        _autoRuleRepository = autoRuleRepository;
        Title = "Sổ Giao Dịch Chi Tiêu";

        // Listen for realtime transaction created events (Instant 0ms timeline prepend)
        WeakReferenceMessenger.Default.Register<TransactionCreatedMessage>(this, (r, m) =>
        {
            if (m.Value.TransactionType == 2) // Expense
            {
                var existing = Expenses.FirstOrDefault(e => e.Id == m.Value.TransactionId);
                if (existing == null)
                {
                    var newExpense = new TransactionModel(
                        m.Value.TransactionId,
                        Guid.Empty,
                        m.Value.WalletId,
                        m.Value.WalletName,
                        m.Value.CategoryId,
                        m.Value.CategoryName,
                        "receipt.png",
                        "#EF4444",
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
                    Expenses.Insert(0, newExpense);
                    MonthlyTotalExpense += m.Value.Amount;
                }
            }
        });

        // Listen for server mode changes (Live API <-> Offline Mock)
        WeakReferenceMessenger.Default.Register<ServerModeChangedMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadDataAsync();
            });
        });
    }


    // Real-time auto-categorization trigger when user types in transaction note (UC-04.07)
    async partial void OnNoteChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 2)
        {
            AutoMatchedHint = null;
            return;
        }

        try
        {
            var match = await _autoRuleRepository.MatchCategoryAsync(value.Trim());
            if (match.CategoryId.HasValue)
            {
                var matchedCat = Categories.FirstOrDefault(c => c.Id == match.CategoryId.Value);
                if (matchedCat != null)
                {
                    SelectedCategory = matchedCat;
                    AutoMatchedHint = $"⚡ Tự động gán danh mục '{matchedCat.Name}' theo từ khóa '{match.MatchedPattern}'";
                }
            }
            else
            {
                AutoMatchedHint = null;
            }
        }
        catch
        {
            // Silently ignore matching errors in live input
        }
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            // Load user wallets
            var wallets = await _walletRepository.GetWalletsAsync();
            Wallets.Clear();
            foreach (var w in wallets)
            {
                Wallets.Add(w);
            }
            if (Wallets.Count > 0 && SelectedWallet == null)
            {
                SelectedWallet = Wallets[0];
            }

            // Load expense categories (type = 2)
            var categories = await _categoryRepository.GetCategoriesAsync(2);
            Categories.Clear();
            foreach (var c in categories)
            {
                Categories.Add(c);
            }
            if (Categories.Count > 0 && SelectedCategory == null)
            {
                SelectedCategory = Categories[0];
            }

            // Load expense transactions (transactionType = 2)
            var transactions = await _transactionRepository.GetTransactionsAsync(transactionType: 2);
            Expenses.Clear();
            foreach (var t in transactions)
            {
                Expenses.Add(t);
            }

            // Load monthly total
            MonthlyTotalExpense = await _transactionRepository.GetMonthlyExpenseTotalAsync(DateTime.UtcNow.Month, DateTime.UtcNow.Year);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải dữ liệu chi tiêu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CreateExpenseAsync()
    {
        if (NewAmount <= 0)
        {
            ErrorMessage = "Số tiền chi tiêu phải lớn hơn 0.";
            return;
        }

        if (SelectedWallet == null)
        {
            ErrorMessage = "Vui lòng chọn ví thanh toán.";
            return;
        }

        if (SelectedCategory == null)
        {
            ErrorMessage = "Vui lòng chọn danh mục chi tiêu.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var req = new CreateExpenseModel(
                WalletId: SelectedWallet.Id,
                CategoryId: SelectedCategory.Id,
                Amount: NewAmount,
                TransactionDate: TransactionDate,
                Note: string.IsNullOrWhiteSpace(Note) ? null : Note.Trim(),
                IsEssential: IsEssential
            );

            await _transactionRepository.CreateExpenseAsync(req);

            NewAmount = 0;
            Note = string.Empty;
            AutoMatchedHint = null;

            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi ghi nhận chi tiêu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteExpenseAsync(TransactionModel expense)
    {
        if (expense == null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _transactionRepository.DeleteExpenseAsync(expense.Id);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xóa khoản chi tiêu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ObservableProperty]
    private string? _autoFillBannerMessage;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null || query.Count == 0) return;

        bool hasAutoFill = false;
        string detectedBank = string.Empty;

        if (query.TryGetValue("amount", out var amountObj))
        {
            if (decimal.TryParse(amountObj?.ToString(), out decimal parsedAmount) && parsedAmount > 0)
            {
                NewAmount = parsedAmount;
                hasAutoFill = true;
            }
        }

        if (query.TryGetValue("note", out var noteObj))
        {
            var rawNote = noteObj?.ToString();
            if (!string.IsNullOrWhiteSpace(rawNote))
            {
                Note = Uri.UnescapeDataString(rawNote);
                hasAutoFill = true;
            }
        }

        if (query.TryGetValue("bank", out var bankObj))
        {
            detectedBank = Uri.UnescapeDataString(bankObj?.ToString() ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(detectedBank))
            {
                var matched = Wallets.FirstOrDefault(w => 
                    w.Name.Contains(detectedBank, StringComparison.OrdinalIgnoreCase) ||
                    detectedBank.Contains(w.Name, StringComparison.OrdinalIgnoreCase));

                if (matched != null)
                {
                    SelectedWallet = matched;
                }
                hasAutoFill = true;
            }
        }

        if (hasAutoFill)
        {
            AutoFillBannerMessage = $"⚡ Đã tự động điền giao dịch {NewAmount:N0} đ từ {(!string.IsNullOrEmpty(detectedBank) ? detectedBank : "Ngân hàng")}. Vui lòng kiểm tra và lưu!";
        }
    }

    [RelayCommand]
    public void SimulateBankDeduction(string bankType)
    {
        var (bankName, amount, note) = bankType switch
        {
            "acb" => ("Ngân hàng ACB", 350000m, "Thanh toán siêu thị Emart Gò Vấp"),
            "vcb" => ("Vietcombank", 500000m, "Chuyển tiền tiền điện nước EVN"),
            _ => ("Ví MoMo", 85000m, "Bán lẻ đồ uống Circle K")
        };

        var query = new Dictionary<string, object>
        {
            { "amount", amount },
            { "bank", bankName },
            { "note", note }
        };

        ApplyQueryAttributes(query);
    }
}

