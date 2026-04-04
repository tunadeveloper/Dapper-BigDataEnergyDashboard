using System.Text.Json;
using StackExchange.Redis;

namespace Energy.WebAPI.Redis
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _database.StringGet(key);
            if (!value.HasValue)
                return default!;
            return JsonSerializer.Deserialize<T>(value.ToString()!)!;
        }

        public void RemoveData(string key)
        {
            _database.KeyDelete(key);
        }

        public void SetData<T>(string key, T value, TimeSpan expirationTime)
        {
            var json = JsonSerializer.Serialize(value);
            _database.StringSet(key, json, expirationTime);
        }
    }
}
