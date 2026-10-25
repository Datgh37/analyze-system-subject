using ExpenseTracker.Maui.ViewModels.AiAgent;

namespace ExpenseTracker.Maui.Views.AiAgent;

public partial class VoiceMicModalPage : ContentPage
{
    private readonly VoiceFloatingMicViewModel _vm;

    public VoiceMicModalPage(VoiceFloatingMicViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
