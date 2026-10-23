using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

public partial class CreateSavingsGoalPage : ContentPage
{
    public CreateSavingsGoalPage(CreateSavingsGoalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
