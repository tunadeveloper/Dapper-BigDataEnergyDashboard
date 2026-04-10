using Microsoft.AspNetCore.Mvc;
using Energy.WebUI.DTOs.MeterDTOs;
using Newtonsoft.Json;

namespace Energy.WebUI.Controllers
{
    public class MeterController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MeterController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("EnergyApi");
            var overviewResponse = await client.GetAsync("api/Meters/overview");
            var overview = new MeterOverviewDto();

            if (overviewResponse.IsSuccessStatusCode)
            {
                var overviewJson = await overviewResponse.Content.ReadAsStringAsync();
                overview = JsonConvert.DeserializeObject<MeterOverviewDto>(overviewJson) ?? new MeterOverviewDto();
            }

            ViewBag.TotalMeterCount = overview.TotalMeterCount;
            ViewBag.ActiveMeterCount = overview.ActiveMeterCount;
            ViewBag.PassiveMeterCount = overview.PassiveMeterCount;
            ViewBag.NewThisMonthCount = overview.NewThisMonthCount;
            ViewBag.ActiveRate = overview.ActiveRate;
            ViewBag.RegionDistributionJson = JsonConvert.SerializeObject(overview.RegionDistribution);
            ViewBag.TariffDistributionJson = JsonConvert.SerializeObject(overview.TariffDistribution);

            return View();
        }
    }
}
