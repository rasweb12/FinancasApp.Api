// DialogService.cs
using Microsoft.Maui.Controls;
using FinancasApp.Mobile.Services.Dialog;


namespace FinancasApp.Mobile.Services;


public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task<bool> ShowConfirmAsync(string title, string message, string accept = "Sim", string cancel = "Não");
}


public class DialogService : IDialogService
{
    public Task ShowAlertAsync(string title, string message, string cancel = "OK") =>
    Application.Current.MainPage.DisplayAlert(title, message, cancel);


    public Task<bool> ShowConfirmAsync(string title, string message, string accept = "Sim", string cancel = "Não") =>
    Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
}