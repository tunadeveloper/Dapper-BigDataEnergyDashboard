using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class LoadForecastController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
