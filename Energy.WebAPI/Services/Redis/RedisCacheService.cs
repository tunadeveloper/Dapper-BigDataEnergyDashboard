using StackExchange.Redis;

namespace Energy.WebAPI.Services.Redis
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;

        public RedisCacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _database = redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveData(string key)
        {
            throw new NotImplementedException();
        }

        public void SetData<T>(string key, T value, TimeSpan expirationTime)
        {
            throw new NotImplementedException();
        }
    }
}
