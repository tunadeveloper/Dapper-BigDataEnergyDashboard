using Energy.WebAPI.Context;
using Energy.WebAPI.Repositories.Dashboard;
using Energy.WebAPI.Repositories.MeterReadings;
using Energy.WebAPI.Repositories.Meters;
using Energy.WebAPI.Repositories.Regions;
using Energy.WebAPI.Services;
using Energy.WebAPI.Redis;
using StackExchange.Redis;

namespace Energy.WebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("SignalRClient", policy =>
                {
                    policy
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddSignalR();
            services.AddOpenApi();
            services.AddScoped<DapperContext>();
            services.AddRedisCache(configuration);
            services.AddScoped<IMeterService, MeterService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IMeterReadingService, MeterReadingService>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddHostedService<RedisPulseBroadcastService>();
            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration["Redis:Configuration"];
            services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(redisConfig));
            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            return services;
        }
    }
}
