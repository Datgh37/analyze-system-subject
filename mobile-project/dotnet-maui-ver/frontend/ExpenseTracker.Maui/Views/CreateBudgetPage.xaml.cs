using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class CreateBudgetPage : ContentPage
{
    private readonly CreateBudgetViewModel _viewModel;

    public CreateBudgetPage(CreateBudgetViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCategoriesAsync();
    }
}
