using FinancasApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancasApp.Api.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _service;
        public TransactionsController(ITransactionService service) { _service = service; }
        public IActionResult Index() => View();
    }
}
