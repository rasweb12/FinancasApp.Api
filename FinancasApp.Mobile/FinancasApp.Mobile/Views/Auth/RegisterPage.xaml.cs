using FinancasApp.Mobile.ViewModels;

namespace FinancasApp.Mobile.Views.Auth;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = new RegisterViewModel();
    }
}