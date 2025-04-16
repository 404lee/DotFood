using Microsoft.AspNetCore.Mvc;

namespace DotFood.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
