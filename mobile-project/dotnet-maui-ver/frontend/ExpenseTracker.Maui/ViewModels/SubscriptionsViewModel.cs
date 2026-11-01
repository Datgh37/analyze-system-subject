using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.SmartUtilities;
using ExpenseTracker.Maui.Repositories;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel quản lý danh mục thuê bao định kỳ, kiểm toán bẫy chi tiêu và đếm ngược gia hạn (UC-03.09)
public partial class SubscriptionsViewModel : BaseViewModel
{
    private readonly ISubscriptionRepository _repository;

    [ObservableProperty]
    private ObservableCollection<SubscriptionModel> _subscriptions = new();

    [ObservableProperty]
    private ObservableCollection<UpcomingRenewalModel> _upcomingRenewals = new();

    [ObservableProperty]
    private SubscriptionAuditReportModel? _auditReport;

    [ObservableProperty]
    private string _selectedTab = "MANAGED"; // "MANAGED", "AUDIT", "UPCOMING"

    [ObservableProperty]
    private string _feedbackMessage = string.Empty;

    public SubscriptionsViewModel(ISubscriptionRepository repository)
    {
        _repository = repository;
        Title = "Quản Lý Thuê Bao & Bẫy Chi Tiêu";
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
            FeedbackMessage = $"Lỗi khi tải dữ liệu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadDataInternalAsync()
    {
        var subsTask = _repository.GetSubscriptionsAsync();
        var upcomingTask = _repository.GetUpcomingRenewalsAsync(7);
        var auditTask = _repository.AuditSubscriptionsAsync();

        await Task.WhenAll(subsTask, upcomingTask, auditTask);

        var list = await subsTask;
        var upcoming = await upcomingTask;
        AuditReport = await auditTask;

        Subscriptions.Clear();
        foreach (var item in list)
        {
            Subscriptions.Add(item);
        }

        UpcomingRenewals.Clear();
        foreach (var item in upcoming)
        {
            UpcomingRenewals.Add(item);
        }
    }

    [RelayCommand]
    public async Task CreateSubscriptionAsync(CreateSubscriptionRequest request)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            FeedbackMessage = string.Empty;

            await _repository.CreateSubscriptionAsync(request);
            FeedbackMessage = "Thêm thuê bao mới thành công!";
            await LoadDataInternalAsync();
        }
        catch (Exception ex)
        {
            FeedbackMessage = $"Lỗi tạo thuê bao: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ArchiveSubscriptionAsync(Guid id)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            FeedbackMessage = string.Empty;

            await _repository.ArchiveSubscriptionAsync(id);
            FeedbackMessage = "Đã hủy theo dõi gói thuê bao thành công!";
            await LoadDataInternalAsync();
        }
        catch (Exception ex)
        {
            FeedbackMessage = $"Lỗi hủy thuê bao: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ConvertCandidateAsync(SubscriptionCandidateModel candidate)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            FeedbackMessage = string.Empty;

            var req = new CreateSubscriptionRequest
            {
                Name = candidate.ServiceName,
                Amount = candidate.SuggestedAmount,
                BillingCycle = candidate.SuggestedBillingCycle,
                NextBillingDate = DateTime.UtcNow.AddMonths(1),
                CategoryId = candidate.CategoryId,
                Status = 1,
                AutoRenew = true
            };

            await _repository.ConvertCandidateAsync(req);
            FeedbackMessage = $"Đã chuyển '{candidate.ServiceName}' vào danh mục quản lý chính thức thành công!";
            await LoadDataInternalAsync();
        }
        catch (Exception ex)
        {
            FeedbackMessage = $"Lỗi chuyển đổi: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void SelectTab(string tab)
    {
        SelectedTab = tab;
    }
}
