// ViewModels/NewTransactionViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Storage;
using FinancasApp.Mobile.Services.Sync;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinancasApp.Mobile.ViewModels;

public partial class NewTransactionViewModel : ObservableObject
{
    private readonly ILocalStorageService _local;
    private readonly ISyncService _sync; // ◄ CORRIGIDO: INTERFACE
    private readonly ILogger<NewTransactionViewModel> _logger;

    [ObservableProperty] private string[] transactionTypes = ["Despesa", "Receita"];
    [ObservableProperty] private string selectedType = "Despesa";

    [ObservableProperty] private decimal amount;
    [ObservableProperty] private DateTime date = DateTime.Today;
    [ObservableProperty] private string description = "";

    [ObservableProperty] private string[] categories = ["Alimentação", "Transporte", "Lazer", "Saúde", "Outros"];
    [ObservableProperty] private string selectedCategory = "Alimentação";

    [ObservableProperty] private ObservableCollection<TagItem> tags = new();
    [ObservableProperty] private bool isInstallment;
    [ObservableProperty] private int installments = 1;
    [ObservableProperty] private bool isBusy;

    public bool IsValid => Amount > 0 && !string.IsNullOrWhiteSpace(Description);

    public NewTransactionViewModel(
            ILocalStorageService local,
            ISyncService sync, // Interface
            ILogger<NewTransactionViewModel> logger)
    {
        _local = local;
        _sync = sync; // Agora compila!
        _logger = logger;
        LoadTags();
    }

    private void LoadTags()
    {
        Tags.Clear();
        Tags.Add(new TagItem { Name = "Urgente" });
        Tags.Add(new TagItem { Name = "Recorrente" });
        Tags.Add(new TagItem { Name = "Cartão" });
    }

    [RelayCommand]
    private void ToggleTag(TagItem tag)
    {
        tag.IsSelected = !tag.IsSelected;
    }

    [RelayCommand]
    private async Task AttachReceiptAsync()
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Selecione o comprovante",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
            await Shell.Current.DisplayAlert("Anexo", $"Comprovante: {result.FileName}", "OK");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!IsValid)
            return;

        IsBusy = true;

        try
        {
            // Mapeamento simples de categoria → ID (ajuste conforme seu back-end)
            var categoryMap = new Dictionary<string, int?>
            {
                { "Alimentação", 1 },
                { "Transporte", 2 },
                { "Lazer", 3 },
                { "Saúde", 4 },
                { "Outros", 5 }
            };

            var transaction = new TransactionLocal
            {
                Id = Guid.NewGuid(),
                AccountId = Guid.Empty, // TODO: permitir seleção de conta no futuro
                Description = Description.Trim(),
                Amount = SelectedType == "Despesa" ? -Amount : Amount,
                Date = Date,
                CategoryId = categoryMap.GetValueOrDefault(SelectedCategory),
                Category = SelectedCategory,

                // Type agora é string direta — sem enum!
                Type = SelectedType == "Despesa" ? "Expense" : "Income",

                InstallmentTotal = IsInstallment ? Installments : 1,
                InstallmentNumber = 1,
                TransactionGroupId = IsInstallment ? Guid.NewGuid() : null,

                // Tags como CSV
                Tags = string.Join(",", Tags.Where(t => t.IsSelected).Select(t => t.Name)),

                // Sync flags
                IsDirty = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _local.SaveTransactionAsync(transaction);
            await _sync.SyncAllAsync();

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar transação");
            await Shell.Current.DisplayAlert("Erro", "Não foi possível salvar a transação.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

// Classe auxiliar para tags
public partial class TagItem : ObservableObject
{
    public string Name { get; set; } = "";

    [ObservableProperty]
    private bool _isSelected;
}