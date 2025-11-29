using CommunityToolkit.Mvvm.ComponentModel;

namespace FinancasApp.Mobile.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value) =>
        OnPropertyChanged(nameof(IsNotBusy));
}