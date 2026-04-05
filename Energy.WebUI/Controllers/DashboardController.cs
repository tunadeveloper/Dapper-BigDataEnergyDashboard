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
            var responseMessage = await client.GetAsync("api/MeterReadings/with-region");
            if (responseMessage.IsSuccessStatusCode)
            {
                var json = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultMeterReadingWithRegionDTO>>(json) ?? new List<ResultMeterReadingWithRegionDTO>();
                return View(values.Take(5).ToList());
            }
            return View();
        }
    }
}
