// Models/DTOs/AccountDto.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancasApp.Api.DTOs;

public class AccountDto : ISyncableDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    // usar enum no app — no JSON vira int automaticamente
    public int AccountType { get; set; }
    public string Currency { get; set; } = "BRL";
    public decimal Balance { get; set; }
    public decimal InitialBalance { get; set; }
    public bool IsNew { get; set; }
    public bool IsDirty { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

