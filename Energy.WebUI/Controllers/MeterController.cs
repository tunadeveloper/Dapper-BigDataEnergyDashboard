using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class MeterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
