// Models/DTOs/TransactionDto.cs
using System;
using System.Text.Json.Serialization;

namespace FinancasApp.Mobile.Models.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; } = Guid.Empty;

    public Guid AccountId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public int? CategoryId { get; set; }
    public string Category { get; set; } = string.Empty;

    public string Type { get; set; } = "Expense"; // Default seguro
    public int? SubType { get; set; }

    // Tags como CSV (ex: "Supermercado,Alimentação")
    public string Tags { get; set; } = string.Empty;

    public int? InstallmentNumber { get; set; }
    public int? InstallmentTotal { get; set; }
    public Guid? TransactionGroupId { get; set; }

    public bool IsRecurring { get; set; } = false;

    // === Campos de sincronização (obrigatórios para o seu padrão) ===
    [JsonIgnore] // ← Evita enviar de volta ao servidor (opcional, mas recomendado)
    public bool IsNew { get; set; } = false;

    public bool IsDirty { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// Enum fora da classe para melhor reutilização
public enum TransactionType
{
    Income = 1,
    Expense = 2,
    Transfer = 3
}

// Extensão útil (opcional, mas recomendada)
public static class TransactionDtoExtensions
{
    public static TransactionType GetTypeEnum(this TransactionDto dto)
    {
        return dto.Type switch
        {
            "Income" => TransactionType.Income,
            "Expense" => TransactionType.Expense,
            "Transfer" => TransactionType.Transfer,
            _ => TransactionType.Expense
        };
    }

    public static void SetTypeEnum(this TransactionDto dto, TransactionType type)
    {
        dto.Type = type.ToString();
    }
}