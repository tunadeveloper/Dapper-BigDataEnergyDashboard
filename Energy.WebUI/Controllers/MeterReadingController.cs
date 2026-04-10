using Microsoft.AspNetCore.Mvc;
using Energy.WebUI.DTOs.MeterReadingDTOs;
using Newtonsoft.Json;

namespace Energy.WebUI.Controllers
{
    public class MeterReadingController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MeterReadingController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("EnergyApi");
            var overviewResponse = await client.GetAsync("api/MeterReadings/overview");
            var overview = new MeterReadingOverviewDto();

            if (overviewResponse.IsSuccessStatusCode)
            {
                var overviewJson = await overviewResponse.Content.ReadAsStringAsync();
                overview = JsonConvert.DeserializeObject<MeterReadingOverviewDto>(overviewJson) ?? new MeterReadingOverviewDto();
            }

            ViewBag.TodayReadingCount = overview.TodayReadingCount;
            ViewBag.TotalConsumption = overview.TotalConsumption;
            ViewBag.AverageVoltage = overview.AverageVoltage;
            ViewBag.AnomalyCount = overview.AnomalyCount;
            return View(overview.LatestReadings);
        }
    }
}
