using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public class AccountLocal : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public AccountType AccountType { get; set; } = AccountType.Checking;

        public string Currency { get; set; } = "BRL";

        public decimal Balance { get; set; }
        public decimal InitialBalance { get; set; }

        public DateTime LastBalanceUpdate { get; set; } = DateTime.UtcNow;
    }

    public enum AccountType
    {
        Checking = 1,
        Savings = 2,
        Wallet = 3,
        Investment = 4,
        Credit = 5
    }
}
