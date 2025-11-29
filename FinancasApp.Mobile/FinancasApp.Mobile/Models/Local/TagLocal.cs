using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public class TagLocal : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Color { get; set; } // "#FFAA33"

        public bool IsDirty { get; set; } = false;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
