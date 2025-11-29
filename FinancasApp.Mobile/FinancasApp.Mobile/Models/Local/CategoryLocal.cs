using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancasApp.Mobile.Models.Local;

public class CategoryLocal
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; } // exemplo: "ic_food", "ic_transport"

    public string? Color { get; set; } // ex: "#0088FF"

    public bool IsIncome { get; set; } // true = categoria de receitas, false = despesas

}

