using Energy.WebAPI.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Energy.WebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration["Redis:Configuration"] ?? "localhost:6379";
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfig));
            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            return services;
        }
    }
}
