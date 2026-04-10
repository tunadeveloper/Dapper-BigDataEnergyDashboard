using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class MeterReadingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
