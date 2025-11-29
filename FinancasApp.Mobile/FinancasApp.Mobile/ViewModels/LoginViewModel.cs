using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Services.Auth;
using FinancasApp.Mobile.Services.Navigation;
using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly INavigationService _nav;
    private readonly ILogger<LoginViewModel> _logger;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool hasError;
    [ObservableProperty] private bool isBiometricAvailable = true;

    public LoginViewModel(
        IAuthService auth,
        INavigationService nav,
        ILogger<LoginViewModel> logger)
    {
        _auth = auth;
        _nav = nav;
        _logger = logger;
    }

    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !IsBusy;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        IsBusy = true;
        ErrorMessage = "";
        HasError = false;

        try
        {
            var success = await _auth.LoginAsync(Email, Password);
            if (success)
            {
                await _nav.NavigateToAsync("//home"); // 💡 Navegação via service
            }
            else
            {
                ErrorMessage = "E-mail ou senha inválidos";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erro de conexão";
            HasError = true;
            _logger.LogError(ex, "Login falhou");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task BiometricLoginAsync()
    {
        // Implementar biometria aqui depois
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        await _nav.NavigateToAsync("register");
    }

    partial void OnEmailChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
}
