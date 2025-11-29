using IdentityServer3.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancasApp.Api.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _service;
        public UsersController(IUserService service) { _service = service; }
        public IActionResult Index() => View();
    }
}
