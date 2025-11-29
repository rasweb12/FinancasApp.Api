using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile.Services.Navigation;

public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;

    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger;
    }

    public async Task NavigateToAsync(string route, bool animate = true)
    {
        try
        {
            _logger.LogInformation("Navigation → {Route}", route);
            await Shell.Current.GoToAsync(route, animate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao navegar para {Route}", route);
        }
    }

    public async Task GoBackAsync(bool animate = true)
    {
        try
        {
            _logger.LogInformation("Navigation ← GoBack");
            await Shell.Current.GoToAsync("..", animate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao voltar para a tela anterior");
        }
    }
}
