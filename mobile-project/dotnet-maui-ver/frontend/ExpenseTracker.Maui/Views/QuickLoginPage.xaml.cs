using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class QuickLoginPage : ContentPage
{
    private readonly QuickLoginViewModel _viewModel;

    public QuickLoginPage(QuickLoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}

