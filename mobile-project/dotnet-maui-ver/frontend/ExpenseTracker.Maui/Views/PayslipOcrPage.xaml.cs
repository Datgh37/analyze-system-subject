using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class PayslipOcrPage : ContentPage
{
    private readonly PayslipOcrViewModel _viewModel;

    public PayslipOcrPage(PayslipOcrViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadReferenceDataCommand.ExecuteAsync(null);
    }
}
