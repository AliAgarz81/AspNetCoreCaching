using System.Collections.Concurrent;
using AspNetCoreCaching.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AspNetCoreCaching.Services;

public class CacheServices : ICacheService
{
    private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();
    private readonly IDistributedCache _distributedCache;

    public CacheServices(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        string? cachedValue = await _distributedCache.GetStringAsync(
            key, 
            cancellationToken);

        if (cachedValue is null)
        {
            return null;
        }

        T? value = JsonConvert.DeserializeObject<T>(cachedValue);
        return value;
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        string cacheValue = JsonConvert.SerializeObject(value);
        await _distributedCache.SetStringAsync(key, cacheValue, cancellationToken);
        CacheKeys.TryAdd(key, false);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
        CacheKeys.TryRemove(key, out bool _);
    }

    public async Task RemovePyPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
       /* foreach (string key in CacheKeys.Keys)
        {
            if (key.StartsWith(prefixKey))
            {
                await RemoveAsync(key, cancellationToken);
            }
        }*/

       IEnumerable<Task> tasks = CacheKeys
           .Keys
           .Where(k => k.StartsWith(prefixKey))
           .Select(k => RemoveAsync(k, cancellationToken));
       await Task.WhenAll(tasks);
    }
}