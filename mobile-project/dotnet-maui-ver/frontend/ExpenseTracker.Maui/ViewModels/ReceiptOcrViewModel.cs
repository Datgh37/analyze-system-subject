using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Categories;
using ExpenseTracker.Maui.Models.Receipts;
using ExpenseTracker.Maui.Models.SmartUtilities;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Services.Api;
using Refit;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing supermarket/retail receipt AI Vision OCR parsing, mathematical audit, and ledger confirmation (UC-03.07/03.08)
public partial class ReceiptOcrViewModel : BaseViewModel
{
    private readonly IReceiptRepository _receiptRepo;
    private readonly IWalletRepository _walletRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly ISmartUtilityRepository _smartUtilityRepo;

    public ObservableCollection<WalletModel> Wallets { get; } = new();
    public ObservableCollection<CategoryModel> ExpenseCategories { get; } = new();
    public ObservableCollection<ParsedReceiptItemModel> Items { get; } = new();

    // Picked File info
    [ObservableProperty]
    private string? _selectedFileName;

    [ObservableProperty]
    private bool _hasParsedData;

    // Parsed Data fields
    [ObservableProperty]
    private string _merchantName = string.Empty;

    [ObservableProperty]
    private DateTime _invoiceDate = DateTime.Now;

    [ObservableProperty]
    private decimal _subTotal;

    [ObservableProperty]
    private decimal _taxAmount;

    [ObservableProperty]
    private decimal _discountAmount;

    [ObservableProperty]
    private decimal _totalAmount;

    [ObservableProperty]
    private decimal _calculatedTotal;

    [ObservableProperty]
    private decimal _discrepancyAmount;

    [ObservableProperty]
    private bool _isMathDiscrepancy;

    [ObservableProperty]
    private string? _auditWarningMessage;

    [ObservableProperty]
    private double _confidenceScore;

    // Selection fields for ledger commitment
    [ObservableProperty]
    private WalletModel? _selectedWallet;

    [ObservableProperty]
    private CategoryModel? _selectedCategory;

    [ObservableProperty]
    private bool _isEssential = true;

    [ObservableProperty]
    private bool _rememberRules = true;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private string? _statusMessage;

    // Voice quick input field (UC-03.11)
    [ObservableProperty]
    private string _voiceTranscriptInput = string.Empty;

    public ReceiptOcrViewModel(
        IReceiptRepository receiptRepo,
        IWalletRepository walletRepo,
        ICategoryRepository categoryRepo,
        ISmartUtilityRepository smartUtilityRepo)
    {
        _receiptRepo = receiptRepo;
        _walletRepo = walletRepo;
        _categoryRepo = categoryRepo;
        _smartUtilityRepo = smartUtilityRepo;
        Title = "Bóc Tách Hóa Đơn AI";
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

            var categories = await _categoryRepo.GetCategoriesAsync(2); // 2 = Expense
            ExpenseCategories.Clear();
            foreach (var c in categories) ExpenseCategories.Add(c);
            if (ExpenseCategories.Count > 0 && SelectedCategory == null)
            {
                // Prefer category named "Đi chợ", "Siêu thị", or first expense category
                SelectedCategory = ExpenseCategories.FirstOrDefault(c => 
                    c.Name.Contains("siêu thị", StringComparison.OrdinalIgnoreCase) || 
                    c.Name.Contains("chợ", StringComparison.OrdinalIgnoreCase) ||
                    c.Name.Contains("ăn uống", StringComparison.OrdinalIgnoreCase)) ?? ExpenseCategories[0];
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi nạp danh mục và ví: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task PickAndParseReceiptAsync()
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
                PickerTitle = "Chọn ảnh chụp hoặc PDF hóa đơn siêu thị/bán lẻ",
                FileTypes = customFileType
            });

            if (result == null) return;

            SelectedFileName = result.FileName;
            IsBusy = true;

            await using var stream = await result.OpenReadAsync();
            var streamPart = new StreamPart(stream, result.FileName, "application/octet-stream");

            var parsed = await _receiptRepo.ParseReceiptAsync(streamPart);

            MerchantName = parsed.MerchantName;
            InvoiceDate = parsed.InvoiceDate;
            SubTotal = parsed.SubTotal;
            TaxAmount = parsed.TaxAmount;
            DiscountAmount = parsed.DiscountAmount;
            TotalAmount = parsed.TotalAmount;
            CalculatedTotal = parsed.CalculatedTotal;
            DiscrepancyAmount = parsed.DiscrepancyAmount;
            IsMathDiscrepancy = parsed.IsMathDiscrepancy;
            AuditWarningMessage = parsed.AuditWarningMessage;
            ConfidenceScore = parsed.ConfidenceScore;

            Items.Clear();
            foreach (var item in parsed.Items)
            {
                Items.Add(item);
            }

            Note = $"Bóc tách hóa đơn tự động từ {MerchantName}";
            HasParsedData = true;

            if (IsMathDiscrepancy)
            {
                StatusMessage = $"⚠️ Cảnh báo kiểm toán số học: Phát hiện chênh lệch {DiscrepancyAmount:N0} đ. Vui lòng kiểm tra lại bảng món hàng.";
            }
            else
            {
                StatusMessage = $"✅ Kiểm toán số học hoàn hảo (Δ ≤ 1,000 đ). Độ tin cậy AI: {ConfidenceScore * 100:N0}%.";
            }
        }
        catch (ApiException apiEx)
        {
            ErrorMessage = $"Lỗi API bóc tách hóa đơn: {apiEx.Content ?? apiEx.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xử lý hóa đơn: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ParseVoiceExpenseAsync()
    {
        if (string.IsNullOrWhiteSpace(VoiceTranscriptInput))
        {
            ErrorMessage = "Vui lòng nhập câu khẩu lệnh chi tiêu.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var result = await _smartUtilityRepo.ParseVoiceExpenseAsync(new(VoiceTranscriptInput));
            TotalAmount = result.Amount;
            Note = result.Note;

            if (result.SuggestedWalletId.HasValue)
            {
                var matchedWallet = Wallets.FirstOrDefault(w => w.Id == result.SuggestedWalletId.Value);
                if (matchedWallet != null) SelectedWallet = matchedWallet;
            }

            if (result.SuggestedCategoryId.HasValue)
            {
                var matchedCategory = ExpenseCategories.FirstOrDefault(c => c.Id == result.SuggestedCategoryId.Value);
                if (matchedCategory != null) SelectedCategory = matchedCategory;
            }

            HasParsedData = true;
            StatusMessage = $"🎤 Bóc tách giọng nói thành công: {result.Amount:N0} đ | Ví: {result.SuggestedWalletName ?? "Chưa rõ"} | Danh mục: {result.SuggestedCategoryName ?? "Chưa rõ"}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi bóc tách khẩu lệnh: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ConfirmReceiptAsync()
    {
        if (SelectedWallet == null)
        {
            ErrorMessage = "Vui lòng chọn ví thanh toán nguồn.";
            return;
        }

        if (SelectedCategory == null)
        {
            ErrorMessage = "Vui lòng chọn danh mục chi tiêu chính.";
            return;
        }

        if (TotalAmount <= 0)
        {
            ErrorMessage = "Số tiền thanh toán phải lớn hơn 0 đ.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var itemPayloads = Items.Select(i => new CreateReceiptItemModel(
                i.ItemName,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice,
                i.SuggestedCategoryId
            )).ToList();

            var request = new ConfirmReceiptModel(
                SelectedWallet.Id,
                SelectedCategory.Id,
                TotalAmount,
                InvoiceDate,
                MerchantName,
                Note,
                IsEssential,
                itemPayloads,
                RememberRules
            );

            var createdTransaction = await _receiptRepo.ConfirmReceiptAsync(request);

            StatusMessage = $"🎉 Hóa đơn {MerchantName} ({createdTransaction.Amount:N0} đ) đã được ghi nhận vào sổ cái thành công!";
            
            // Clear current parsed data to allow new upload
            HasParsedData = false;
            Items.Clear();
            SelectedFileName = null;
            MerchantName = string.Empty;
            TotalAmount = 0;
        }
        catch (ApiException apiEx)
        {
            ErrorMessage = $"Lỗi xác nhận hóa đơn: {apiEx.Content ?? apiEx.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi ghi nhận hóa đơn: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
