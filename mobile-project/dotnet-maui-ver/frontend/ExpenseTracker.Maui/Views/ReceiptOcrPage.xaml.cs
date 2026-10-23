using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class ReceiptOcrPage : ContentPage
{
    private readonly ReceiptOcrViewModel _viewModel;

    public ReceiptOcrPage(ReceiptOcrViewModel viewModel)
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
