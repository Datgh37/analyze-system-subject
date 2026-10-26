using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.Behavioral;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing Behavioral Nudge Feed & Thaler Choice Architecture (UC-09.01 -> UC-09.04)
public partial class NudgeFeedViewModel : BaseViewModel
{
    private readonly INudgeRepository _repository;

    [ObservableProperty]
    private FinancialNudgeModel? _selectedNudge;

    [ObservableProperty]
    private XaiRationaleModel? _activeXaiRationale;

    [ObservableProperty]
    private DynamicRebalanceProposalModel? _activeRebalanceProposal;

    [ObservableProperty]
    private bool _isXaiModalVisible;

    [ObservableProperty]
    private string? _statusMessage;

    public ObservableCollection<FinancialNudgeModel> Nudges { get; } = new();

    public NudgeFeedViewModel(INudgeRepository repository)
    {
        _repository = repository;
        Title = "Cú Hích Thaler";
    }

    [RelayCommand]
    public async Task LoadNudgesAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var items = await _repository.GetActiveNudgesAsync();
            Nudges.Clear();
            foreach (var item in items)
            {
                Nudges.Add(item);
            }

            if (Nudges.Count == 0)
            {
                StatusMessage = "Tuyệt vời! Hiện tại bạn đang duy trì kỷ luật tài chính tốt, không có cảnh báo nào.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải danh sách Cú hích tài chính: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ViewXaiRationaleAsync(FinancialNudgeModel nudge)
    {
        if (nudge == null) return;

        SelectedNudge = nudge;

        if (!string.IsNullOrEmpty(nudge.RationaleExplanationJson))
        {
            try
            {
                ActiveXaiRationale = JsonSerializer.Deserialize<XaiRationaleModel>(nudge.RationaleExplanationJson);
                IsXaiModalVisible = true;
                return;
            }
            catch
            {
                // Fallback to fetch from server
            }
        }

        try
        {
            IsBusy = true;
            ActiveXaiRationale = await _repository.GetXaiRationaleAsync(nudge.Id);
            IsXaiModalVisible = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải phân tích minh bạch XAI: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void CloseXaiModal()
    {
        IsXaiModalVisible = false;
        ActiveXaiRationale = null;
    }

    [RelayCommand]
    public async Task ApproveNudgeAsync(FinancialNudgeModel nudge)
    {
        if (nudge == null || IsBusy) return;

        try
        {
            IsBusy = true;
            bool ok = await _repository.ApproveNudgeAsync(nudge.Id);
            if (ok)
            {
                Nudges.Remove(nudge);
                StatusMessage = "Đã áp dụng đề xuất tái cân đối ngân sách thành công!";
            }
            else
            {
                ErrorMessage = "Không thể áp dụng đề xuất.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi phê duyệt cú hích: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DismissNudgeAsync(FinancialNudgeModel nudge)
    {
        if (nudge == null || IsBusy) return;

        try
        {
            IsBusy = true;
            bool ok = await _repository.DismissNudgeAsync(nudge.Id);
            if (ok)
            {
                Nudges.Remove(nudge);
            }
            else
            {
                ErrorMessage = "Không thể bỏ qua cú hích.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi bỏ qua cú hích: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
