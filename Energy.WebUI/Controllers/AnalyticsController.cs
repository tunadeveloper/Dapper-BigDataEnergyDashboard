using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class AnalyticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
