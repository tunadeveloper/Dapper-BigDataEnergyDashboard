using Microsoft.AspNetCore.Mvc;
using Energy.WebUI.DTOs.RegionDTOs;
using Newtonsoft.Json;

namespace Energy.WebUI.Controllers
{
    public class RegionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegionController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("EnergyApi");
            var overviewResponse = await client.GetAsync("api/Regions/overview");
            var overview = new RegionOverviewDto();

            if (overviewResponse.IsSuccessStatusCode)
            {
                var overviewJson = await overviewResponse.Content.ReadAsStringAsync();
                overview = JsonConvert.DeserializeObject<RegionOverviewDto>(overviewJson) ?? new RegionOverviewDto();
            }

            ViewBag.TotalRegionCount = overview.TotalRegionCount;
            ViewBag.TopConsumptionRegionName = overview.TopConsumptionRegionName;
            ViewBag.TopConsumptionValue = overview.TopConsumptionValue;
            ViewBag.TopMeterRegionName = overview.TopMeterRegionName;
            ViewBag.TopMeterRegionCount = overview.TopMeterRegionCount;
            ViewBag.AverageVoltage = overview.AverageVoltage;
            return View(overview.Regions);
        }
    }
}
