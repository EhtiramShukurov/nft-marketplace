using Microsoft.AspNetCore.Mvc;

namespace Xhibiter.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("NotFound");
                case 400:
                    return View("BadRequest");
                default:
                    return View("Error");
            }
        }

        [Route("/Error/500")]
        public IActionResult InternalServerError()
        {
            return View("Error");
        }
    }

}
