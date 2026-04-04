using Energy.WebAPI.DTOs.DashboardDTOs;
using Energy.WebAPI.Hubs;
using Energy.WebAPI.Redis;
using Energy.WebAPI.Repositories.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Energy.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly IHubContext<EnergyHub> _hubContext;

        public DashboardController(
            IDashboardRepository repository,
            ICacheService cacheService,
            IHubContext<EnergyHub> hubContext)
        {
            _repository = repository;
            _cacheService = cacheService;
            _hubContext = hubContext;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            string cacheKey = $"dashboard_{start:yyyyMMdd}_{end:yyyyMMdd}";
            var cachedData = _cacheService.GetData<EnergyDashboardDto>(cacheKey);

            if (cachedData != null) return Ok(cachedData);

            var stats = await _repository.GetDashboardStatsAsync(start, end);
            _cacheService.SetData(cacheKey, stats, TimeSpan.FromMinutes(5));

            return Ok(stats);
        }

        [HttpPost("simulate-anomaly")]
        public async Task<IActionResult> SimulateAnomaly([FromBody] AnomalousMeterDto anomaly)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveAnomaly", anomaly);
            return Ok(new { Message = "Anomaly broadcasted to all clients" });
        }
    }
}
