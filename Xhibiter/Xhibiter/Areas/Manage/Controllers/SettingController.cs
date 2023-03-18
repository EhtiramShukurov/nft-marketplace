using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Xhibiter.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]

    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
