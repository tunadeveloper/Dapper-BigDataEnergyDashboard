using Energy.WebUI.DTOs.DashboardDTOs;
using Energy.WebUI.DTOs.MeterReadingDTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Energy.WebUI.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("EnergyApi");
            var end = DateTime.UtcNow;
            var start = new DateTime(end.Year, end.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var statsUrl = $"api/Dashboard/stats?start={Uri.EscapeDataString(start.ToString("o"))}&end={Uri.EscapeDataString(end.ToString("o"))}";
            var statsResponse = await client.GetAsync(statsUrl);
            if (statsResponse.IsSuccessStatusCode)
            {
                var statsJson = await statsResponse.Content.ReadAsStringAsync();
                var stats = JsonConvert.DeserializeObject<EnergyDashboardDto>(statsJson) ?? new EnergyDashboardDto();
                ViewBag.TotalConsumption = stats.TotalConsumption;
                ViewBag.ActiveMeterCount = stats.ActiveMeterCount;
                ViewBag.AverageVoltage = stats.AverageVoltage;
            }
            var responseMessage = await client.GetAsync("api/MeterReadings/with-region");
            if (responseMessage.IsSuccessStatusCode)
            {
                var json = await responseMessage.Content.ReadAsStringAsync();
                ViewBag.MonthlyLoadJson = json;
                var values = JsonConvert.DeserializeObject<List<ResultMeterReadingWithRegionDTO>>(json) ?? new List<ResultMeterReadingWithRegionDTO>();
                return View(values.Take(5).ToList());
            }
            return View();
        }
    }
}
