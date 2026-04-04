namespace Energy.WebAPI.Redis
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        void SetData<T>(string key, T value, TimeSpan expirationTime);
        void RemoveData(string key);
    }
}
