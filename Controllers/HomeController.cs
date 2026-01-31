using Microsoft.AspNetCore.Mvc;

namespace StokApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}