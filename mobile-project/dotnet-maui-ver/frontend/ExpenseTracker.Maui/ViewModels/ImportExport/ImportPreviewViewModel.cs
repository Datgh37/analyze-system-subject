using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.ImportExport;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Repositories;
using ExpenseTracker.Maui.Repositories.ImportExport;

namespace ExpenseTracker.Maui.ViewModels.ImportExport;

// ViewModel managing Human-In-The-Loop (HITL) Financial Data Import Preview and Bulk Commit (UC-07.06)
public partial class ImportPreviewViewModel : BaseViewModel
{
    private readonly IDataImportRepository _importRepo;
    private readonly IWalletRepository _walletRepo;

    public ObservableCollection<ParsedImportRowModel> Rows { get; } = new();
    public ObservableCollection<WalletModel> Wallets { get; } = new();

    [ObservableProperty]
    private WalletModel? _selectedWallet;

    [ObservableProperty]
    private string _sourceType = "StructuredExcel";

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _validCount;

    [ObservableProperty]
    private decimal _totalAmount;

    [ObservableProperty]
    private string _summaryMessage = string.Empty;

    public ImportPreviewViewModel(IDataImportRepository importRepo, IWalletRepository walletRepo)
    {
        _importRepo = importRepo;
        _walletRepo = walletRepo;
        Title = "Xem trước nhập khẩu dữ liệu";
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var wallets = await _walletRepo.GetWalletsAsync();
            Wallets.Clear();
            foreach (var w in wallets)
            {
                Wallets.Add(w);
            }
            SelectedWallet = Wallets.FirstOrDefault();
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

    [RelayCommand]
    public async Task LoadSpreadsheetPreviewAsync((Stream stream, string fileName) fileInfo)
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var preview = await _importRepo.PreviewSpreadsheetAsync(fileInfo.stream, fileInfo.fileName);
            PopulatePreviewData(preview);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi đọc tệp bảng tính: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task LoadTextPreviewAsync(string rawText)
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var preview = await _importRepo.PreviewUnstructuredTextAsync(rawText);
            PopulatePreviewData(preview);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi bóc tách ghi chú: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void PopulatePreviewData(ImportPreviewResultModel preview)
    {
        SourceType = preview.SourceType;
        TotalCount = preview.TotalRows;
        ValidCount = preview.ValidRows;
        TotalAmount = preview.TotalAmount;
        SummaryMessage = preview.SummaryMessage;

        Rows.Clear();
        foreach (var r in preview.Rows)
        {
            Rows.Add(r);
        }
    }

    [RelayCommand]
    public async Task CommitBulkAsync()
    {
        if (SelectedWallet == null)
        {
            ErrorMessage = "Vui lòng chọn ví đích để lưu giao dịch.";
            return;
        }

        var selected = Rows.Where(r => r.IsSelected && r.IsValid).ToList();
        if (selected.Count == 0)
        {
            ErrorMessage = "Không có dòng giao dịch hợp lệ nào được chọn để nhập.";
            return;
        }

        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var req = new BulkCommitImportRequestModel(SelectedWallet.Id, selected);
            var result = await _importRepo.CommitBulkImportAsync(req);
            SummaryMessage = result.Message;

            // Remove committed rows
            var committedIndices = new HashSet<int>(selected.Select(s => s.RowIndex));
            var remaining = Rows.Where(r => !committedIndices.Contains(r.RowIndex)).ToList();
            Rows.Clear();
            foreach (var rem in remaining)
            {
                Rows.Add(rem);
            }

            TotalCount = Rows.Count;
            ValidCount = Rows.Count(r => r.IsValid);
            TotalAmount = Rows.Where(r => r.IsSelected && r.IsValid).Sum(r => r.Amount);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi lưu giao dịch hàng loạt: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
