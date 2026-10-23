using ExpenseTracker.Maui.ViewModels;

namespace ExpenseTracker.Maui.Views;

// Code-behind for RegisterPage binding RegisterViewModel
public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
