using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Services.Navigation;

namespace FinancasApp.Mobile.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly INavigationService _nav;

    public RegisterViewModel(INavigationService nav)
    {
        _nav = nav;
    }

    [RelayCommand]
    private async Task BackToLogin()
    {
        await _nav.GoBackAsync();
    }
}
