using Microsoft.AspNetCore.Mvc;

namespace Xhibiter.Controllers
{
    public class PagesController : Controller
    {
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult Terms()
        {
            return View();
        }
        public IActionResult Newsletter()
        {
            return View();
        }
    }
}
