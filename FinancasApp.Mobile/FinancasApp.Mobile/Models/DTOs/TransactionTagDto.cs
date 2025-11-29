using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancasApp.Mobile.Models.DTOs;

public class TransactionTagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; } // opcional
    public string? Color { get; set; } // Hex ex: "#FF4500"
}

