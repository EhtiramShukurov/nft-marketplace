using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Xhibiter.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]

    public class ActivityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
