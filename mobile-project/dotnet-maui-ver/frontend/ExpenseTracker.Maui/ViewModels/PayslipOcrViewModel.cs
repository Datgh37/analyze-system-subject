using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Models.Payslips;
using ExpenseTracker.Maui.Models.RecurringIncomes;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Api;
using Refit;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing payslip image/PDF picking, AI Vision OCR parsing, and ledger confirmation (UC-02.08)
public partial class PayslipOcrViewModel : BaseViewModel
{
    private readonly IPayslipRepository _payslipRepo;
    private readonly IWalletRepository _walletRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IRecurringIncomeRepository _recurringIncomeRepo;

    public ObservableCollection<WalletModel> Wallets { get; } = new();
    public ObservableCollection<CategoryModel> IncomeCategories { get; } = new();
    public ObservableCollection<RecurringIncomeModel> RecurringIncomes { get; } = new();

    // Picked File info
    [ObservableProperty]
    private string? _selectedFileName;

    [ObservableProperty]
    private bool _hasParsedData;

    // Parsed Data fields
    [ObservableProperty]
    private string _employerName = string.Empty;

    [ObservableProperty]
    private string _payPeriod = string.Empty;

    [ObservableProperty]
    private decimal _grossSalary;

    [ObservableProperty]
    private decimal _netSalary;

    [ObservableProperty]
    private decimal _socialInsurance;

    [ObservableProperty]
    private decimal _healthInsurance;

    [ObservableProperty]
    private decimal _unemploymentInsurance;

    [ObservableProperty]
    private decimal _personalIncomeTax;

    [ObservableProperty]
    private decimal _allowances;

    [ObservableProperty]
    private decimal _bonus;

    [ObservableProperty]
    private decimal _otherDeductions;

    // Mathematical audit fields (UC-02.08 Step 6 & E2)
    [ObservableProperty]
    private bool _isMathAudited;

    [ObservableProperty]
    private bool _hasDiscrepancy;

    [ObservableProperty]
    private decimal _discrepancyAmount;

    // Selection fields
    [ObservableProperty]
    private WalletModel? _selectedWallet;

    [ObservableProperty]
    private CategoryModel? _selectedCategory;

    [ObservableProperty]
    private bool _updateRecurringIncome;

    [ObservableProperty]
    private RecurringIncomeModel? _selectedRecurringIncome;

    [ObservableProperty]
    private string? _statusMessage;

    public PayslipOcrViewModel(
        IPayslipRepository payslipRepo,
        IWalletRepository walletRepo,
        ICategoryRepository categoryRepo,
        IRecurringIncomeRepository recurringIncomeRepo)
    {
        _payslipRepo = payslipRepo;
        _walletRepo = walletRepo;
        _categoryRepo = categoryRepo;
        _recurringIncomeRepo = recurringIncomeRepo;
        Title = "Bóc Tách Phiếu Lương";
    }

    [RelayCommand]
    public async Task LoadReferenceDataAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var wallets = await _walletRepo.GetWalletsAsync();
            Wallets.Clear();
            foreach (var w in wallets) Wallets.Add(w);
            if (Wallets.Count > 0 && SelectedWallet == null)
            {
                SelectedWallet = Wallets[0];
            }

            var categories = await _categoryRepo.GetCategoriesAsync(1); // 1 = Income
            IncomeCategories.Clear();
            foreach (var c in categories) IncomeCategories.Add(c);
            if (IncomeCategories.Count > 0 && SelectedCategory == null)
            {
                // Prefer category named "Tiền lương" or first income category
                SelectedCategory = IncomeCategories.FirstOrDefault(c => c.Name.Contains("lương", StringComparison.OrdinalIgnoreCase)) ?? IncomeCategories[0];
            }

            var recurring = await _recurringIncomeRepo.GetRecurringIncomesAsync();
            RecurringIncomes.Clear();
            foreach (var r in recurring) RecurringIncomes.Add(r);
            if (RecurringIncomes.Count > 0 && SelectedRecurringIncome == null)
            {
                SelectedRecurringIncome = RecurringIncomes[0];
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi nạp dữ liệu tham chiếu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task PickAndParsePayslipAsync()
    {
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.image", "com.adobe.pdf" } },
                { DevicePlatform.Android, new[] { "image/*", "application/pdf" } },
                { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".pdf" } },
                { DevicePlatform.Tizen, new[] { "*/*" } },
                { DevicePlatform.macOS, new[] { "public.image", "com.adobe.pdf" } },
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Chọn tệp hình ảnh hoặc PDF phiếu lương",
                FileTypes = customFileType
            });

            if (result == null) return;

            SelectedFileName = result.FileName;
            IsBusy = true;

            await using var stream = await result.OpenReadAsync();
            var streamPart = new StreamPart(stream, result.FileName, "application/octet-stream");

            var parsed = await _payslipRepo.ParsePayslipAsync(streamPart);

            EmployerName = parsed.EmployerName;
            PayPeriod = parsed.PayPeriod;
            GrossSalary = parsed.GrossSalary;
            NetSalary = parsed.NetSalary;
            SocialInsurance = parsed.SocialInsurance;
            HealthInsurance = parsed.HealthInsurance;
            UnemploymentInsurance = parsed.UnemploymentInsurance;
            PersonalIncomeTax = parsed.PersonalIncomeTax;
            Allowances = parsed.Allowances;
            Bonus = parsed.Bonus;
            OtherDeductions = parsed.OtherDeductions;
            IsMathAudited = parsed.IsMathAudited;
            HasDiscrepancy = parsed.HasDiscrepancy;
            DiscrepancyAmount = parsed.DiscrepancyAmount;
            HasParsedData = true;

            if (HasDiscrepancy)
            {
                ErrorMessage = $"⚠️ Cảnh báo sai lệch số học: Chênh lệch {DiscrepancyAmount:N0} VND giữa Net và công thức khấu trừ. Vui lòng kiểm tra lại.";
            }
            else
            {
                StatusMessage = "✅ Đã bóc tách thành công và xác thực toàn vẹn công thức tiền lương (Δ <= 1,000 VND).";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi bóc tách phiếu lương: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ConfirmPayslipAsync()
    {
        if (SelectedWallet == null)
        {
            ErrorMessage = "Vui lòng chọn ví nhận tiền lương.";
            return;
        }

        if (SelectedCategory == null)
        {
            ErrorMessage = "Vui lòng chọn danh mục thu nhập.";
            return;
        }

        if (NetSalary <= 0)
        {
            ErrorMessage = "Số tiền lương thực nhận (Net) phải lớn hơn 0.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var request = new ConfirmPayslipModel(
                WalletId: SelectedWallet.Id,
                CategoryId: SelectedCategory.Id,
                EmployerName: EmployerName,
                PayPeriod: PayPeriod,
                GrossSalary: GrossSalary,
                NetSalary: NetSalary,
                SocialInsurance: SocialInsurance,
                HealthInsurance: HealthInsurance,
                UnemploymentInsurance: UnemploymentInsurance,
                PersonalIncomeTax: PersonalIncomeTax,
                Allowances: Allowances,
                Bonus: Bonus,
                OtherDeductions: OtherDeductions,
                TransactionDate: DateTime.UtcNow,
                Note: $"Lương kỳ {PayPeriod} - {EmployerName}",
                UpdateRecurringIncomeId: (UpdateRecurringIncome && SelectedRecurringIncome != null) ? SelectedRecurringIncome.Id : null
            );

            await _payslipRepo.ConfirmPayslipAsync(request);

            StatusMessage = $"🎉 Ghi nhận thành công thu nhập lương {NetSalary:N0} VND vào ví {SelectedWallet.Name}!";
            HasParsedData = false;
            SelectedFileName = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xác nhận lưu phiếu lương: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
