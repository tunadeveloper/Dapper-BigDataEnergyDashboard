using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class LogisticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
