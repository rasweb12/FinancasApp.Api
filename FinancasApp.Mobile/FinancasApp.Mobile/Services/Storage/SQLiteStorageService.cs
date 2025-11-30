// Services/Storage/SQLiteStorageService.cs
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.LocalDatabase;

namespace FinancasApp.Mobile.Services.Storage;

public class SQLiteStorageService : ILocalStorageService
{
    private readonly IBaseRepository<AccountLocal> _accountRepo;
    private readonly IBaseRepository<TransactionLocal> _transactionRepo;
    private readonly IBaseRepository<CreditCardLocal> _creditCardRepo;
    private readonly IBaseRepository<InvoiceLocal> _invoiceRepo;

    public SQLiteStorageService(
        IBaseRepository<AccountLocal> accountRepo,
        IBaseRepository<TransactionLocal> transactionRepo,
        IBaseRepository<CreditCardLocal> creditCardRepo,
        IBaseRepository<InvoiceLocal> invoiceRepo)
    {
        _accountRepo = accountRepo;
        _transactionRepo = transactionRepo;
        _creditCardRepo = creditCardRepo;
        _invoiceRepo = invoiceRepo;
    }

    // === ACCOUNTS ===
    public async Task<AccountLocal?> GetAccountByIdAsync(Guid id) => await _accountRepo.GetByIdAsync(id);
    public async Task<List<AccountLocal>> GetAllAccountsAsync() => await _accountRepo.GetAllAsync();
    public async Task<int> SaveAccountAsync(AccountLocal account)
    {
        if (account.Id == Guid.Empty)
        {
            account.Id = Guid.NewGuid();
            return await _accountRepo.InsertAsync(account);
        }
        return await _accountRepo.UpdateAsync(account);
    }

    // === TRANSACTIONS ===
    public async Task<TransactionLocal?> GetTransactionByIdAsync(Guid id) => await _transactionRepo.GetByIdAsync(id);
    public async Task<List<TransactionLocal>> GetAllTransactionsAsync() => await _transactionRepo.GetAllAsync();
    public async Task<int> SaveTransactionAsync(TransactionLocal transaction)
    {
        if (transaction.Id == Guid.Empty)
        {
            transaction.Id = Guid.NewGuid();
            return await _transactionRepo.InsertAsync(transaction);
        }
        return await _transactionRepo.UpdateAsync(transaction);
    }

    // === CREDIT CARDS ===
    public async Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id) => await _creditCardRepo.GetByIdAsync(id);
    public async Task<List<CreditCardLocal>> GetAllCreditCardsAsync() => await _creditCardRepo.GetAllAsync();
    public async Task<int> SaveCreditCardAsync(CreditCardLocal card)
    {
        if (card.Id == Guid.Empty)
        {
            card.Id = Guid.NewGuid();
            return await _creditCardRepo.InsertAsync(card);
        }
        return await _creditCardRepo.UpdateAsync(card);
    }

    // === INVOICES ===
    public async Task<InvoiceLocal?> GetInvoiceByIdAsync(Guid id) => await _invoiceRepo.GetByIdAsync(id);
    public async Task<List<InvoiceLocal>> GetAllInvoicesAsync() => await _invoiceRepo.GetAllAsync();
    public async Task<List<InvoiceLocal>> GetPendingInvoicesAsync()
    {
        if (_invoiceRepo is InvoiceRepository invoiceRepo)
            return await invoiceRepo.GetPendingAsync();
        return await _invoiceRepo.GetAllAsync();
    }

    public async Task<int> SaveInvoiceAsync(InvoiceLocal invoice)
    {
        // ... seu código
        if (invoice.Id == Guid.Empty)
            return await _invoiceRepo.InsertAsync(invoice);
        else
            return await _invoiceRepo.UpdateAsync(invoice);
    }
}