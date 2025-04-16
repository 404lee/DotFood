using Microsoft.AspNetCore.Mvc;

namespace DotFood.Controllers
{
    public class VendorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
