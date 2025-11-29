using FinancasApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancasApp.Api.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountService _service;
        public AccountsController(IAccountService service) { _service = service; }
        public IActionResult Index() => View();
    }
}
