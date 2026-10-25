using ExpenseTracker.Maui.ViewModels.ImportExport;

namespace ExpenseTracker.Maui.Views.ImportExport;

public partial class ImportPreviewPage : ContentPage
{
    private readonly ImportPreviewViewModel _vm;

    public ImportPreviewPage(ImportPreviewViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeCommand.ExecuteAsync(null);
    }
}
