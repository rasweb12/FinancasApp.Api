using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Storage;
using FinancasApp.Mobile.Services.Sync;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinancasApp.Mobile.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly ILocalStorageService _local;
    private readonly ISyncService _sync;
    private readonly ILogger<CategoriesViewModel> _logger;

    [ObservableProperty] private ObservableCollection<CategoryLocal> categories = new();
    [ObservableProperty] private bool isBusy;

    public CategoriesViewModel(
        ILocalStorageService local,
        ISyncService sync,
        ILogger<CategoriesViewModel> logger)
    {
        _local = local;
        _sync = sync;
        _logger = logger;
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        IsBusy = true;
        try
        {
            var list = await _local.GetCategoriesAsync() ?? new List<CategoryLocal>();
            Categories.Clear();
            foreach (var c in list.OrderBy(c => c.Name))
                Categories.Add(c);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar categorias");
            await Shell.Current.DisplayAlert("Erro", "Falha ao carregar categorias", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddCategoryAsync()
    {
        IsBusy = true;
        try
        {
            var name = await Shell.Current.DisplayPromptAsync("Nova Categoria", "Nome da categoria:", placeholder: "Ex: Alimentação");
            if (string.IsNullOrWhiteSpace(name)) return;

            var typeString = await Shell.Current.DisplayActionSheet("Tipo", "Cancelar", null, "Despesa", "Receita");
            if (typeString == "Cancelar" || typeString is null) return;

            var type = typeString == "Receita" ? CategoryType.Income : CategoryType.Expense;

            var iconOptions = new[] { "food.png", "transport.png", "shopping.png", "home.png", "entertainment.png", "health.png", "other.png" };
            var icon = await Shell.Current.DisplayActionSheet("Ícone", "Cancelar", null, iconOptions);
            if (icon == "Cancelar" || icon is null) icon = "other.png";

            var category = new CategoryLocal
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Type = type,
                Icon = icon,
                IsDirty = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _local.SaveCategoryAsync(category);
            await LoadCategoriesAsync();

            // ◄ SYNC IMEDIATO (dentro do try principal)
            await _sync.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao adicionar categoria ou sincronizar");
            await Shell.Current.DisplayAlert("Erro", "Categoria salva localmente. Sync falhou — tente novamente mais tarde.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteCategoryAsync(CategoryLocal category)
    {
        if (category is null) return;

        var confirm = await Shell.Current.DisplayAlert(
            "Excluir categoria",
            $"Tem certeza que deseja remover \"{category.Name}\"?\nIsso pode afetar transações existentes.",
            "Sim, remover", "Não");

        if (!confirm) return;

        IsBusy = true;
        try
        {
            category.IsDeleted = true;
            category.IsDirty = true;
            category.UpdatedAt = DateTime.UtcNow;

            await _local.SaveCategoryAsync(category);
            await LoadCategoriesAsync();
            await _sync.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir categoria");
            await Shell.Current.DisplayAlert("Erro", "Falha ao excluir categoria", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditCategoryAsync(CategoryLocal category)
    {
        await Shell.Current.DisplayAlert("Editar", "Funcionalidade em desenvolvimento", "OK");
    }
}