using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Reports;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel quản lý báo cáo và phân tích Giá Trị Tài Sản Ròng Đa Chiều (UC-07.02)
public partial class NetWorthViewModel : BaseViewModel
{
    private readonly INetWorthRepository _repository;

    [ObservableProperty]
    private NetWorthBreakdownModel? _breakdown;

    [ObservableProperty]
    private ObservableCollection<NetWorthHistoryItemModel> _history = new();

    [ObservableProperty]
    private string _snapshotNotes = string.Empty;

    [ObservableProperty]
    private string _feedbackMessage = string.Empty;

    public NetWorthViewModel(INetWorthRepository repository)
    {
        _repository = repository;
        Title = "Tài Sản Ròng (Net Worth)";
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            FeedbackMessage = string.Empty;
            await LoadDataInternalAsync();
        }
        catch (Exception ex)
        {
            FeedbackMessage = $"Lỗi khi tải tài sản ròng: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadDataInternalAsync()
    {
        var currentTask = _repository.GetCurrentNetWorthAsync();
        var historyTask = _repository.GetNetWorthHistoryAsync(12);

        await Task.WhenAll(currentTask, historyTask);

        Breakdown = await currentTask;
        var list = await historyTask;

        History.Clear();
        foreach (var item in list)
        {
            History.Add(item);
        }
    }

    [RelayCommand]
    public async Task RecordSnapshotAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            FeedbackMessage = string.Empty;

            var req = new RecordNetWorthSnapshotRequest
            {
                Notes = string.IsNullOrWhiteSpace(SnapshotNotes) ? null : SnapshotNotes.Trim()
            };

            await _repository.RecordSnapshotAsync(req);
            SnapshotNotes = string.Empty;
            FeedbackMessage = "Đã lưu lại điểm ảnh tài sản ròng thành công!";
            await LoadDataInternalAsync();
        }
        catch (Exception ex)
        {
            FeedbackMessage = $"Lỗi khi lưu điểm ảnh: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
