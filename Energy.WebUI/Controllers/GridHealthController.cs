using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class GridHealthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
