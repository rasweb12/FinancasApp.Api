using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.Auth;
using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IApiService _api;
    private readonly IAuthService _auth;
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
        ILogger<RegisterViewModel> logger)
    {
        _api = api;
        _auth = auth;
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
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            HasError = false;

            var response = await _api.RegisterAsync(new RegisterRequest
            {
                FullName = FullName.Trim(),
                Email = Email.Trim(),
                Password = Password
            });

            if (!string.IsNullOrWhiteSpace(response?.Token))
            {
                await _auth.SaveTokenAsync(response.Token);
                await Shell.Current.GoToAsync("///home");
            }
            else
            {
                ErrorMessage = "Erro ao criar conta.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erro de conexão.";
            HasError = true;
            _logger.LogError(ex, "Erro no registro");
        }
        finally
        {
            IsBusy = false;
            RegisterCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private Task BackToLogin() =>
        Shell.Current.GoToAsync("///login");

    partial void OnFullNameChanged(string _) => RegisterCommand.NotifyCanExecuteChanged();
    partial void OnEmailChanged(string _) => RegisterCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string _) => RegisterCommand.NotifyCanExecuteChanged();
    partial void OnConfirmPasswordChanged(string _) => RegisterCommand.NotifyCanExecuteChanged();
}
