// Views/Auth/LoginPage.xaml.cs
using FinancasApp.Mobile.ViewModels;

namespace FinancasApp.Mobile.Views.Auth
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}