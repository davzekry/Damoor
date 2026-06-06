using Microsoft.Extensions.Caching.Distributed;
using Damoor.Infrastructure.Interfaces;
using System.Text.Json;

namespace Damoor.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        return data is null ? default : JsonSerializer.Deserialize<T>(data, _opts);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(value, _opts), options);
    }

    public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var cached = await GetAsync<T>(key);
        if (cached is not null) return cached;
        var result = await factory();
        await SetAsync(key, result, expiry);
        return result;
    }
}