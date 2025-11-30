// Views/Auth/RegisterPage.xaml.cs
using FinancasApp.Mobile.ViewModels;
using FinancasApp.Mobile.Services.Auth;
using FinancasApp.Mobile.Services.Navigation;
using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile.Views.Auth;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(
        RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}