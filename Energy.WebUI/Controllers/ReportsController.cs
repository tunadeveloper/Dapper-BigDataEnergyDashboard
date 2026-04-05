using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
