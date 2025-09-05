using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ShopFlow.API.Configurations;
using ShopFlow.Application.Abstractions.Services;

namespace ShopFlow.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache? _memoryCache;
    private readonly IDistributedCache? _distributedCache;
    private readonly CacheOptions _options;
    private readonly bool _useDistributedCache;

    public CacheService(
        IOptions<CacheOptions> options,
        IMemoryCache? memoryCache = null,
        IDistributedCache? distributedCache = null)
    {
        _options = options.Value;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _useDistributedCache = _options.UseRedisCache && _distributedCache != null;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var cacheKey = GetCacheKey(key);

        if (_useDistributedCache)
        {
            var cachedValue = await _distributedCache!.GetStringAsync(cacheKey, cancellationToken);
            if (cachedValue != null)
            {
                return JsonSerializer.Deserialize<T>(cachedValue);
            }
        }
        else if (_memoryCache != null)
        {
            return _memoryCache.Get<T>(cacheKey);
        }

        return null;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var cacheKey = GetCacheKey(key);
        var expirationTime = expiration ?? TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);

        if (_useDistributedCache)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            };
            await _distributedCache!.SetStringAsync(cacheKey, serializedValue, options, cancellationToken);
        }
        else if (_memoryCache != null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            };
            _memoryCache.Set(cacheKey, value, options);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(key);

        if (_useDistributedCache)
        {
            await _distributedCache!.RemoveAsync(cacheKey, cancellationToken);
        }
        else if (_memoryCache != null)
        {
            _memoryCache.Remove(cacheKey);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Note: This is a simplified implementation
        // For Redis, you might want to use Redis-specific commands for pattern matching
        // For MemoryCache, this would require keeping track of keys which is complex
        
        if (_useDistributedCache)
        {
            // This would require Redis-specific implementation
            // For now, we'll leave it as a placeholder
            await Task.CompletedTask;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await GetAsync<object>(key, cancellationToken);
        return value != null;
    }

    private string GetCacheKey(string key)
    {
        return $"{_options.KeyPrefix}{key}";
    }
}
