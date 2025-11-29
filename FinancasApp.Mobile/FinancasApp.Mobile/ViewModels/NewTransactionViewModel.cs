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
    private readonly SyncService _sync;
    private readonly ILogger<NewTransactionViewModel> _logger;

    [ObservableProperty] private string[] transactionTypes = ["Despesa", "Receita"];
    [ObservableProperty] private string selectedType = "Despesa";
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private DateTime date = DateTime.Today;
    [ObservableProperty] private string description = "";
    [ObservableProperty] private string[] categories = ["Alimentação", "Transporte", "Lazer", "Saúde", "Outros"];
    [ObservableProperty] private string selectedCategory = "Alimentação";
    [ObservableProperty] private ObservableCollection<TagItem> tags;
    [ObservableProperty] private bool isInstallment;
    [ObservableProperty] private int installments = 1;
    [ObservableProperty] private bool isBusy;

    public bool IsValid => Amount > 0 && !string.IsNullOrWhiteSpace(Description);

    public NewTransactionViewModel(ILocalStorageService local, SyncService sync, ILogger<NewTransactionViewModel> logger)
    {
        _local = local;
        _sync = sync;
        _logger = logger;
        LoadTags();
    }

    private void LoadTags()
    {
        Tags = new()
        {
            new TagItem { Name = "Urgente", IsSelected = false },
            new TagItem { Name = "Recorrente", IsSelected = false },
            new TagItem { Name = "Cartão", IsSelected = false }
        };
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
            await Application.Current!.MainPage!.DisplayAlert("Anexo", $"Comprovante: {result.FileName}", "OK");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        IsBusy = true;
        try
        {
            // Mapeamento manual de categorias para IDs (ajuste conforme seu back-end)
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
                AccountId = Guid.Empty, // TODO: preencher conta selecionada
                Description = Description,
                Amount = Amount * (SelectedType == "Despesa" ? -1 : 1),
                Date = Date,
                CategoryId = categoryMap.ContainsKey(SelectedCategory) ? categoryMap[SelectedCategory] : null,

                // Tipo
                Type = SelectedType == "Despesa" ?
                    ((int)TransactionType.Expense).ToString() :
                    ((int)TransactionType.Income).ToString(),

                // Parcelamento
                InstallmentTotal = IsInstallment ? Installments : 1,
                InstallmentNumber = 1,
                TransactionGroupId = IsInstallment ? Guid.NewGuid() : null,

                // Tags CSV
                Tags = string.Join(",", Tags.Where(t => t.IsSelected).Select(t => t.Name)),

                // Defaults importantes
                IsNew = true,
                IsDirty = true,
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
            await Application.Current!.MainPage!.DisplayAlert("Erro", "Falha ao salvar", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

}

// CORRIGIDO: Remover herança duplicada
[ObservableObject]
public partial class TagItem
{
    public string Name { get; set; } = "";
    [ObservableProperty] private bool isSelected;
}