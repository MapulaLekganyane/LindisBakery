using Microsoft.AspNetCore.Mvc;

namespace LindisBakery.Controllers
{
    public class HomeController : Controller
    {
        // Run Home page
        public IActionResult Index()
        {
            return View();
        }

        // RUn About Us 
        public IActionResult About()
        {
            return View();
        }

        // RUn Contacts Page
        public IActionResult Contacts()
        {
            return View();
        }
    }
}
