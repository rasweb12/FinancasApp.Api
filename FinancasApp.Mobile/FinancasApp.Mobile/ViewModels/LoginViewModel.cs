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

    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !IsBusy;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        HasError = false;

        try
        {
            var result = await _auth.LoginAsync(Email.Trim(), Password);

            if (result.Success)
            {
                // Esta é a navegação 100% garantida no .NET MAUI
                await Shell.Current.GoToAsync("//home");
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

    [RelayCommand]
    private async Task GoToRegister()
    {
        await Shell.Current.GoToAsync("register");
    }

    partial void OnEmailChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
}