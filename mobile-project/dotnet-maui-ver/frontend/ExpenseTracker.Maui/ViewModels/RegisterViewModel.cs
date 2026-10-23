using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Services.Auth;

namespace ExpenseTracker.Maui.ViewModels;

// View model managing new user account creation and password confirmation
public partial class RegisterViewModel : BaseViewModel
{
    private readonly IClientAuthService _authService;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string _baseCurrency = "VND";

    public RegisterViewModel(IClientAuthService authService)
    {
        _authService = authService;
        Title = "Create Account";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(FullName))
        {
            ErrorMessage = "Please enter your full name.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            ErrorMessage = "Please enter a valid email address.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters long.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return;
        }

        try
        {
            IsBusy = true;
            var (success, error) = await _authService.RegisterAsync(FullName, Email, Password, BaseCurrency);

            if (success)
            {
                await Shell.Current.GoToAsync("//WalletsPage");
            }
            else
            {
                ErrorMessage = error ?? "Registration failed. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private static async Task NavigateToLoginAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
