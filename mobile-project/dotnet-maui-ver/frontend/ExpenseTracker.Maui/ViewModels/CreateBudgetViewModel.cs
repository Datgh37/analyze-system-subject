using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Budgets;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.ViewModels;

public class CategoryAllocationInputItem : ObservableObject
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryIcon { get; set; }
    public int? BudgetRuleMapping { get; set; }

    private decimal _allocatedAmount;
    public decimal AllocatedAmount
    {
        get => _allocatedAmount;
        set => SetProperty(ref _allocatedAmount, value);
    }
}

// ViewModel managing creation of budget with 50/30/20, ZBB, or Kakeibo methodologies (UC-05.01)
public partial class CreateBudgetViewModel : BaseViewModel
{
    private readonly IBudgetApi _budgetApi;
    private readonly ICategoryApi _categoryApi;

    public ObservableCollection<CategoryAllocationInputItem> CategoryInputs { get; } = new();

    [ObservableProperty]
    private string _budgetName = $"Ngân sách Tháng {DateTime.Now.Month}/{DateTime.Now.Year}";

    [ObservableProperty]
    private decimal _totalAmount = 10_000_000m;

    [ObservableProperty]
    private int _periodType = 1; // 1 = Monthly, 2 = Weekly

    [ObservableProperty]
    private DateTime _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

    [ObservableProperty]
    private DateTime _endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

    [ObservableProperty]
    private int _method = 1; // 1 = 50/30/20, 2 = ZBB, 3 = Kakeibo

    public List<string> PeriodOptions { get; } = new()
    {
        "Hàng tháng (Monthly)",
        "Hàng tuần (Weekly)"
    };

    [ObservableProperty]
    private int _selectedPeriodIndex = 0;

    public List<string> MethodOptions { get; } = new()
    {
        "Quy tắc 50/30/20 (Needs / Wants / Savings)",
        "Ngân sách số không ZBB (Zero-Based Budgeting)",
        "Sổ Kakeibo Nhật Bản (4 Cột trụ)"
    };

    [ObservableProperty]
    private int _selectedMethodIndex = 0;

    [ObservableProperty]
    private decimal _totalAllocatedSum;

    [ObservableProperty]
    private decimal _remainingToAllocate;

    [ObservableProperty]
    private string? _statusMessage;

    public CreateBudgetViewModel(
        IBudgetApi budgetApi,
        ICategoryApi categoryApi)
    {
        _budgetApi = budgetApi;
        _categoryApi = categoryApi;
        Title = "Lập Kế Hoạch Ngân Sách Mới (UC-05.01)";
    }

    [RelayCommand]
    public async Task LoadCategoriesAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var categories = await _categoryApi.GetCategoriesAsync(2); // 2 = Expense
            CategoryInputs.Clear();

            foreach (var c in categories)
            {
                var item = new CategoryAllocationInputItem
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    CategoryIcon = c.IconKey,
                    BudgetRuleMapping = c.BudgetRuleMapping
                };
                item.PropertyChanged += (s, e) => RecalculateAllocationTotals();
                CategoryInputs.Add(item);
            }

            ApplyMethodDistribution();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi nạp danh mục: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedPeriodIndexChanged(int value)
    {
        PeriodType = value + 1; // 0 => 1 (Monthly), 1 => 2 (Weekly)
        if (PeriodType == 2) // Weekly
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            StartDate = today.AddDays(-1 * diff).Date;
            EndDate = StartDate.AddDays(6).Date;
        }
        else // Monthly
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
    }

    partial void OnSelectedMethodIndexChanged(int value)
    {
        Method = value + 1; // 0 => 1 (50/30/20), 1 => 2 (ZBB), 2 => 3 (Kakeibo)
    }

    partial void OnMethodChanged(int value)
    {
        ApplyMethodDistribution();
    }

    partial void OnTotalAmountChanged(decimal value)
    {
        ApplyMethodDistribution();
    }

    private void ApplyMethodDistribution()
    {
        if (CategoryInputs.Count == 0 || TotalAmount <= 0) return;

        if (Method == 1) // 50/30/20 Rule
        {
            var needsCats = CategoryInputs.Where(c => c.BudgetRuleMapping == 1).ToList();
            var wantsCats = CategoryInputs.Where(c => c.BudgetRuleMapping == 2).ToList();

            decimal needsAmount = TotalAmount * 0.50m;
            decimal wantsAmount = TotalAmount * 0.30m;

            if (needsCats.Count > 0)
            {
                decimal perCat = Math.Round(needsAmount / needsCats.Count, 0);
                foreach (var c in needsCats) c.AllocatedAmount = perCat;
            }

            if (wantsCats.Count > 0)
            {
                decimal perCat = Math.Round(wantsAmount / wantsCats.Count, 0);
                foreach (var c in wantsCats) c.AllocatedAmount = perCat;
            }
        }

        RecalculateAllocationTotals();
    }

    private void RecalculateAllocationTotals()
    {
        TotalAllocatedSum = CategoryInputs.Sum(c => c.AllocatedAmount);
        RemainingToAllocate = TotalAmount - TotalAllocatedSum;
    }

    [RelayCommand]
    public async Task CreateBudgetAsync()
    {
        if (string.IsNullOrWhiteSpace(BudgetName))
        {
            ErrorMessage = "Vui lòng nhập tên kế hoạch ngân sách.";
            return;
        }

        if (TotalAmount <= 0)
        {
            ErrorMessage = "Tổng số tiền ngân sách phải lớn hơn 0 đ.";
            return;
        }

        // Method 2 (ZBB) check: sum must equal total
        if (Method == 2 && RemainingToAllocate != 0)
        {
            ErrorMessage = $"Phương pháp Ngân sách số không (ZBB) yêu cầu tổng phân bổ ({TotalAllocatedSum:N0} đ) phải bằng chính xác tổng ngân sách ({TotalAmount:N0} đ). Chênh lệch: {RemainingToAllocate:N0} đ.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var allocations = CategoryInputs
                .Where(c => c.AllocatedAmount > 0)
                .Select(c => new CreateBudgetAllocationModel(c.CategoryId, c.AllocatedAmount, 80m))
                .ToList();

            var request = new CreateBudgetModel(
                BudgetName,
                PeriodType,
                StartDate,
                EndDate,
                TotalAmount,
                Method,
                allocations
            );

            await _budgetApi.CreateBudgetAsync(request);

            StatusMessage = "🎉 Lập kế hoạch ngân sách thành công!";
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi lập ngân sách: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
