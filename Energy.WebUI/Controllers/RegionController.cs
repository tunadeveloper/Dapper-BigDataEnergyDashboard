using Microsoft.AspNetCore.Mvc;

namespace Energy.WebUI.Controllers
{
    public class RegionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
