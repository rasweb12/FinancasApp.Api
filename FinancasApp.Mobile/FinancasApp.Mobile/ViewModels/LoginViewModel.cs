using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Services.Auth;
using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly ILogger<LoginViewModel> _logger;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool hasError;

    public LoginViewModel(IAuthService auth, ILogger<LoginViewModel> logger)
    {
        _auth = auth;
        _logger = logger;
    }

    // Verificar se o login pode ser feito
    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !IsBusy;

    // Método de login com o [RelayCommand]
    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            HasError = false;

            _logger.LogInformation("Tentando login para {Email}", Email);

            var result = await _auth.LoginAsync(Email.Trim(), Password);

            if (result.Success)
            {
                // Navegação correta com Shell (destino absoluto)
                await Shell.Current.GoToAsync("///home");
            }
            else
            {
                ErrorMessage = result.Message ?? "E-mail ou senha inválidos";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erro de conexão. Tente novamente.";
            HasError = true;
            _logger.LogError(ex, "Falha no login");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Comando para ir para a tela de registro
    [RelayCommand]
    private async Task GoToRegister()
    {
        await Shell.Current.GoToAsync("///register");
    }

    // Comando de login via biometria (ainda não implementado)
    [RelayCommand]
    private async Task BiometricLogin()
    {
        await Application.Current!.MainPage!.DisplayAlert(
            "Biometria",
            "Biometria ainda não implementada",
            "OK");
    }
}
