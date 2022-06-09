namespace Hawf.Utils;

public interface ApiCacheBase
{
    public Task Set<T>(string key, T? value, TimeSpan? lifeTime = null);
    public Task<T?> Get<T>(string key);
    public Task<bool> Exists(string key);
    public Task Unset(string key);
}