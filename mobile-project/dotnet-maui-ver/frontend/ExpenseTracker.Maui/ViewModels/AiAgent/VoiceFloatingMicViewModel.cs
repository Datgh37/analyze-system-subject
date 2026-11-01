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
    private string _directTextInput = string.Empty;

    [ObservableProperty]
    private bool _hasMicrophonePermission = true;

    [ObservableProperty]
    private VoiceIntentResultModel? _lastResult;

    public VoiceFloatingMicViewModel(IVoiceAgentRepository voiceRepo)
    {
        _voiceRepo = voiceRepo;
        Title = "Trợ lý Giọng nói Thông minh";
    }

    [RelayCommand]
    public async Task StartListeningAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
            }

            if (status != PermissionStatus.Granted)
            {
                HasMicrophonePermission = false;
                FeedbackMessage = "Chưa được cấp quyền Microphone. Vui lòng cấp quyền hoặc nhập khẩu lệnh bằng văn bản bên dưới.";
                return;
            }

            HasMicrophonePermission = true;
            IsListening = true;
            Transcript = string.Empty;
            FeedbackMessage = "Đang lắng nghe... (Hãy nói: 'Chi 50k ăn trưa', 'Trả nợ 2 triệu', 'Quỹ khẩn cấp'...)";
        }
        catch
        {
            // Fallback for mock/test harness where Native Permissions API is not initialized
            HasMicrophonePermission = true;
            IsListening = true;
            Transcript = string.Empty;
            FeedbackMessage = "Đang lắng nghe... (Hãy nói hoặc gõ lệnh bên dưới)";
        }
    }

    public void StartListening()
    {
        _ = StartListeningAsync();
    }

    [RelayCommand]
    public void StopListening()
    {
        IsListening = false;
        FeedbackMessage = "Đang xử lý ý định...";
    }

    [RelayCommand]
    public async Task SendDirectTextInputAsync()
    {
        if (string.IsNullOrWhiteSpace(DirectTextInput))
            return;

        string input = DirectTextInput.Trim();
        DirectTextInput = string.Empty;
        await ProcessTranscriptAsync(input);
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

            // Optional Native Text-To-Speech audio feedback
            _ = SpeakFeedbackAsync(LastResult.ConfirmationMessage);

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

    private static async Task SpeakFeedbackAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        try
        {
            await TextToSpeech.Default.SpeakAsync(message, new SpeechOptions
            {
                Pitch = 1.0f,
                Volume = 0.8f
            });
        }
        catch
        {
            // Ignore if TTS hardware/service is unavailable on device or in test runners
        }
    }

    public async Task ExecuteNavigationIntentAsync(VoiceIntentResultModel result)
    {
        if (string.IsNullOrWhiteSpace(result.TargetRoute) || Shell.Current == null)
            return;

        try
        {
            string amt = result.ExtractedAmount.HasValue ? result.ExtractedAmount.Value.ToString("G29") : "0";

            if (result.TargetRoute == "ExpensesPage" || result.TargetRoute == "AddTransactionPage")
            {
                string note = result.PrefillData.TryGetValue("note", out var n) && !string.IsNullOrWhiteSpace(n)
                    ? n
                    : (result.SuggestedCategory ?? "Chi tiêu");
                await Shell.Current.GoToAsync($"///ExpensesPage?amount={amt}&note={Uri.EscapeDataString(note)}");
            }
            else if (result.TargetRoute == "DebtTrackerPage" || result.TargetRoute == "DebtsPage")
            {
                await Shell.Current.GoToAsync($"///DebtTrackerPage?amount={amt}");
            }
            else if (result.TargetRoute == "EmergencyFundPage")
            {
                await Shell.Current.GoToAsync("///EmergencyFundPage");
            }
            else if (result.TargetRoute == "SubscriptionsPage")
            {
                await Shell.Current.GoToAsync("///SubscriptionsPage");
            }
            else if (result.TargetRoute == "ForecastingPage")
            {
                await Shell.Current.GoToAsync($"///ForecastingPage?amount={amt}");
            }
            else if (result.TargetRoute == "ReportsPage" || result.TargetRoute == "AnalyticsReportPage")
            {
                await Shell.Current.GoToAsync("///ReportsPage");
            }
            else if (result.TargetRoute == "BudgetsPage" || result.TargetRoute == "BudgetHubPage")
            {
                await Shell.Current.GoToAsync("///BudgetsPage");
            }
            else if (result.TargetRoute == "CreateCategoryPage" || result.TargetRoute == "CategoriesPage")
            {
                await Shell.Current.GoToAsync("///CategoriesPage");
            }
            else if (result.TargetRoute == "WalletsPage")
            {
                await Shell.Current.GoToAsync("///WalletsPage");
            }
            else if (result.TargetRoute == "ReceiptOcrPage")
            {
                await Shell.Current.GoToAsync("///ReceiptOcrPage");
            }
            else
            {
                await Shell.Current.GoToAsync($"///{result.TargetRoute}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[VoiceMic] Navigation failed: {ex.Message}");
        }
    }
}
