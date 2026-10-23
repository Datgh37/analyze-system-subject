using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Models.RecurringIncomes;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing recurring income schedules, manual processing triggers, and status toggles (UC-02.07)
public partial class RecurringIncomesViewModel : BaseViewModel
{
    private readonly IRecurringIncomeApi _recurringIncomeApi;
    private readonly IWalletApi _walletApi;
    private readonly ICategoryApi _categoryApi;

    public ObservableCollection<RecurringIncomeModel> RecurringIncomes { get; } = new();
    public ObservableCollection<WalletModel> Wallets { get; } = new();
    public ObservableCollection<CategoryModel> IncomeCategories { get; } = new();

    public List<string> FrequencyOptions { get; } = new()
    {
        "Hàng ngày",
        "Hàng tuần",
        "Hàng tháng",
        "Hàng quý",
        "Hàng năm"
    };

    // Form inputs
    [ObservableProperty]
    private string _nameInput = string.Empty;

    [ObservableProperty]
    private string _amountInput = string.Empty;

    [ObservableProperty]
    private WalletModel? _selectedWallet;

    [ObservableProperty]
    private CategoryModel? _selectedCategory;

    [ObservableProperty]
    private int _selectedFrequencyIndex = 2; // Default: Hàng tháng (Frequency = 3)

    [ObservableProperty]
    private int _dayOfMonthInput = 1;

    [ObservableProperty]
    private bool _autoRecord = true;

    [ObservableProperty]
    private string? _statusMessage;

    public RecurringIncomesViewModel(
        IRecurringIncomeApi recurringIncomeApi,
        IWalletApi walletApi,
        ICategoryApi categoryApi)
    {
        _recurringIncomeApi = recurringIncomeApi;
        _walletApi = walletApi;
        _categoryApi = categoryApi;
        Title = "Khoản Thu Định Kỳ (UC-02.07)";
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var list = await _recurringIncomeApi.GetRecurringIncomesAsync();
            RecurringIncomes.Clear();
            foreach (var item in list)
            {
                RecurringIncomes.Add(item);
            }

            var wallets = await _walletApi.GetWalletsAsync();
            Wallets.Clear();
            foreach (var w in wallets) Wallets.Add(w);

            var categories = await _categoryApi.GetCategoriesAsync(1); // 1 = Income
            IncomeCategories.Clear();
            foreach (var c in categories) IncomeCategories.Add(c);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải dữ liệu thu định kỳ: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CreateRecurringIncomeAsync()
    {
        if (string.IsNullOrWhiteSpace(NameInput))
        {
            ErrorMessage = "Vui lòng nhập tên nguồn thu định kỳ.";
            return;
        }

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
            ErrorMessage = "Số tiền dự kiến phải lớn hơn 0.";
            return;
        }

        if (DayOfMonthInput < 1 || DayOfMonthInput > 31)
        {
            ErrorMessage = "Ngày nhận trong tháng phải từ 1 đến 31.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var frequency = SelectedFrequencyIndex + 1; // Maps 0..4 to Enum 1..5

            var request = new CreateRecurringIncomeModel(
                Name: NameInput.Trim(),
                ExpectedAmount: amount,
                WalletId: SelectedWallet.Id,
                CategoryId: SelectedCategory.Id,
                Frequency: frequency,
                DayOfMonth: DayOfMonthInput,
                AutoRecord: AutoRecord
            );

            await _recurringIncomeApi.CreateRecurringIncomeAsync(request);

            // Reset inputs
            NameInput = string.Empty;
            AmountInput = string.Empty;
            DayOfMonthInput = 1;
            AutoRecord = true;
            StatusMessage = "Đã tạo nguồn thu định kỳ thành công!";

            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tạo nguồn thu định kỳ: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ToggleStatusAsync(RecurringIncomeModel item)
    {
        if (item == null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _recurringIncomeApi.ToggleRecurringIncomeStatusAsync(item.Id);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi đổi trạng thái: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteRecurringIncomeAsync(RecurringIncomeModel item)
    {
        if (item == null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _recurringIncomeApi.DeleteRecurringIncomeAsync(item.Id);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xóa nguồn thu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ProcessDueIncomesAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var result = await _recurringIncomeApi.ProcessDueRecurringIncomesAsync(new ProcessRecurringIncomesModel(DateTime.UtcNow));
            StatusMessage = result.Message;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xử lý nguồn thu đến hạn: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
