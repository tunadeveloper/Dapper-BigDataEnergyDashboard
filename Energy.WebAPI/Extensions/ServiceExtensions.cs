using Energy.WebAPI.Redis;
using StackExchange.Redis;

namespace Energy.WebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration["Redis:Configuration"];
            services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(redisConfig));
            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            return services;
        }
    }
}
