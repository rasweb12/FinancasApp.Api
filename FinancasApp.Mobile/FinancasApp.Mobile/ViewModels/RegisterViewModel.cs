using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.Auth;
using FinancasApp.Mobile.Services.Navigation;
using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IApiService _api;
    private readonly IAuthService _auth;
    private readonly INavigationService _nav;
    private readonly ILogger<RegisterViewModel> _logger;

    [ObservableProperty] private string fullName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool hasError;

    public RegisterViewModel(
        IApiService api,
        IAuthService auth,
        INavigationService nav,
        ILogger<RegisterViewModel> logger)
    {
        _api = api;
        _auth = auth;
        _nav = nav;
        _logger = logger;
    }

    private bool CanRegister() =>
        !string.IsNullOrWhiteSpace(FullName) &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        Password == ConfirmPassword &&
        !IsBusy;

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private async Task RegisterAsync()
    {
        IsBusy = true;
        ErrorMessage = "";
        HasError = false;

        try
        {
            var response = await _api.RegisterAsync(new RegisterRequest
            {
                FullName = FullName,
                Email = Email,
                Password = Password
            });

            if (!string.IsNullOrEmpty(response?.Token))
            {
                await _auth.SaveTokenAsync(response.Token);
                await _nav.NavigateToAsync("//home");
            }
            else
            {
                ErrorMessage = "Erro ao criar conta. Tente novamente.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erro de conexão ou servidor indisponível.";
            HasError = true;
            _logger.LogError(ex, "Registro falhou");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task BackToLogin()
    {
        await _nav.GoBackAsync();
    }

    partial void OnFullNameChanged(string value) => RegisterCommand.NotifyCanExecuteChanged();
    partial void OnEmailChanged(string value) => RegisterCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string value) => RegisterCommand.NotifyCanExecuteChanged();
    partial void OnConfirmPasswordChanged(string value) => RegisterCommand.NotifyCanExecuteChanged();
}