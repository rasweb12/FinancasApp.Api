using FinancasApp.Mobile.ViewModels;

namespace FinancasApp.Mobile.Views.Reports;

public partial class ReportsPage : ContentPage
{
    private readonly ReportsViewModel _vm;
    public ReportsPage(ReportsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadDataCommand.ExecuteAsync(null);
    }
}