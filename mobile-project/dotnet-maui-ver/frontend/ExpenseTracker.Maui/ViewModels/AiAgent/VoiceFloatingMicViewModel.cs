using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.AiAgent;
using ExpenseTracker.Maui.Repositories.AiAgent;

namespace ExpenseTracker.Maui.ViewModels.AiAgent;

// ViewModel managing Voice Assistant Floating Mic and Intent Navigation (UC-03.11 Enhancement)
public partial class VoiceFloatingMicViewModel : BaseViewModel
{
    private readonly IVoiceAgentRepository _voiceRepo;

    [ObservableProperty]
    private bool _isListening;

    [ObservableProperty]
    private string _transcript = string.Empty;

    [ObservableProperty]
    private string _feedbackMessage = "Chạm vào micro để nói yêu cầu...";

    [ObservableProperty]
    private VoiceIntentResultModel? _lastResult;

    public VoiceFloatingMicViewModel(IVoiceAgentRepository voiceRepo)
    {
        _voiceRepo = voiceRepo;
        Title = "Trợ lý Giọng nói Thông minh";
    }

    [RelayCommand]
    public void StartListening()
    {
        IsListening = true;
        Transcript = string.Empty;
        FeedbackMessage = "Đang lắng nghe... (Hãy nói: 'Tiêu 50k đi chợ' hoặc 'Tạo danh mục Nuôi mèo')";
    }

    [RelayCommand]
    public void StopListening()
    {
        IsListening = false;
        FeedbackMessage = "Đang xử lý ý định...";
    }

    [RelayCommand]
    public async Task ProcessTranscriptAsync(string transcriptText)
    {
        if (string.IsNullOrWhiteSpace(transcriptText))
        {
            FeedbackMessage = "Không nhận diện được giọng nói. Vui lòng thử lại.";
            return;
        }

        Transcript = transcriptText;
        IsListening = false;
        IsBusy = true;
        ErrorMessage = null;
        FeedbackMessage = "AI đang phân tích ý định...";

        try
        {
            var req = new VoiceIntentCommandRequest(Transcript);
            LastResult = await _voiceRepo.ParseIntentAsync(req);
            FeedbackMessage = LastResult.ConfirmationMessage;

            // Trigger autonomous Shell navigation if Route is specified
            if (!string.IsNullOrWhiteSpace(LastResult.TargetRoute))
            {
                await ExecuteNavigationIntentAsync(LastResult);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xử lý yêu cầu giọng nói: {ex.Message}";
            FeedbackMessage = "Không thể hoàn thành yêu cầu. Vui lòng thử lại.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task ExecuteNavigationIntentAsync(VoiceIntentResultModel result)
    {
        if (string.IsNullOrWhiteSpace(result.TargetRoute))
            return;

        try
        {
            if (result.TargetRoute == "CreateCategoryPage" && result.PrefillData.TryGetValue("categoryName", out var catName))
            {
                await Shell.Current.GoToAsync($"CreateCategoryPage?name={Uri.EscapeDataString(catName)}&type=Expense");
            }
            else if (result.TargetRoute == "AddTransactionPage")
            {
                string amt = result.ExtractedAmount.HasValue ? result.ExtractedAmount.Value.ToString("G29") : "0";
                string cat = result.SuggestedCategory ?? "Chi tiêu";
                string type = result.ActionType;
                await Shell.Current.GoToAsync($"AddTransactionPage?amount={amt}&category={Uri.EscapeDataString(cat)}&type={type}");
            }
            else if (result.TargetRoute == "AnalyticsReportPage")
            {
                await Shell.Current.GoToAsync("//AnalyticsReportPage");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[VoiceMic] Navigation failed: {ex.Message}");
        }
    }
}
