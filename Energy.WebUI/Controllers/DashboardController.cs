using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
