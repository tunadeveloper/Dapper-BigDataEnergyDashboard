using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class AlertsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
