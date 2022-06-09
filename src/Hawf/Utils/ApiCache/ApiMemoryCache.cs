using System.Collections.Concurrent;

namespace Hawf.Utils;

public class ApiMemoryCache : ApiCacheBase
{
    private ConcurrentDictionary<string, CacheObject> _cache;

    public ApiMemoryCache()
    {
        Reset();
    }

    private void Reset()
    {
        if (_cache == null)
            _cache = new ConcurrentDictionary<string, CacheObject>();
        _cache.Clear();
    }

    private async Task CheckExpiration(string key)
    {
        if (_cache.TryGetValue(key, out CacheObject? value))
        {
            if (value.LifeTime == null)
                return;
            
            if (DateTime.Now - value.Registered > value.LifeTime)
                await Unset(key);
        }
    }

    public Task Set<T>(string key, T? value, TimeSpan? lifeTime = null)
    {
        var cacheObj = new CacheObject
        {
            Key = key,
            Value = value,
            Registered = DateTime.Now,
            LifeTime = lifeTime
        };

        var existing = _cache.GetOrAdd(key, cacheObj);
        _cache.TryUpdate(key, cacheObj, existing);
        return Task.CompletedTask;
    }

    public async Task<T?> Get<T>(string key)
    {
        await CheckExpiration(key);

        if (_cache.TryGetValue(key, out CacheObject cacheObj))
            return (T?) cacheObj.Value;
        throw new InvalidOperationException("The cache key does not exist or has expired.");
    }

    public async Task<bool> Exists(string key)
    {
        await CheckExpiration(key);
        return _cache.ContainsKey(key);
    }

    public Task Unset(string key)
    {
        _cache.Remove(key, out _);
        return Task.CompletedTask;
    }
}