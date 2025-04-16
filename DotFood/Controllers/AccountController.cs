using Microsoft.AspNetCore.Mvc;

namespace DotFood.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
