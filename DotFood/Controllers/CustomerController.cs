using Microsoft.AspNetCore.Mvc;

namespace DotFood.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
