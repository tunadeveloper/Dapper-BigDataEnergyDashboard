using System.Diagnostics;
using Energy.WebAPI.Hubs;
using Energy.WebAPI.Redis;
using Microsoft.AspNetCore.SignalR;

namespace Energy.WebAPI.Services
{
    public class RedisPulseBroadcastService : BackgroundService
    {
        private readonly IRedisCacheService _cache;
        private readonly IHubContext<EnergyHub> _hubContext;

        public RedisPulseBroadcastService(IRedisCacheService cache, IHubContext<EnergyHub> hubContext)
        {
            _cache = cache;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var durationMs = 0L;
                var isHealthy = true;
                try
                {
                    var watch = Stopwatch.StartNew();
                    var key = $"redis:pulse:{now:yyyyMMddHHmmss}";
                    _cache.SetData(key, now.ToString("O"), TimeSpan.FromSeconds(10));
                    _cache.GetData<string>(key);
                    watch.Stop();
                    durationMs = watch.ElapsedMilliseconds;
                }
                catch
                {
                    isHealthy = false;
                }

                await _hubContext.Clients.All.SendAsync("ReceiveRedisPullStatus", new
                {
                    LastPullAtUtc = now,
                    PullDurationMs = durationMs,
                    IsHealthy = isHealthy
                }, stoppingToken);

                await timer.WaitForNextTickAsync(stoppingToken);
            }
        }
    }
}
