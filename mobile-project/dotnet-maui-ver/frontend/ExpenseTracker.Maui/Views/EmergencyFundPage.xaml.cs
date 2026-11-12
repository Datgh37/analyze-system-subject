namespace ExpenseTracker.Maui.Views;

using ExpenseTracker.Maui.ViewModels;

public partial class EmergencyFundPage : ContentPage
{
    private readonly EmergencyFundViewModel _viewModel;

    public EmergencyFundPage(EmergencyFundViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}

