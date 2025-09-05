using Microsoft.Extensions.Caching.Memory;
using ShopFlow.API.Configurations;

namespace ShopFlow.API.Extensions;

public static class CacheExtensions
{
    public static IServiceCollection AddCachingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheOptions = configuration.GetSection(CacheOptions.SectionName).Get<CacheOptions>() ?? new CacheOptions();

        // Always add in-memory cache
        services.AddMemoryCache();

        // Add Redis cache if configured
        if (cacheOptions.UseRedisCache && !string.IsNullOrEmpty(cacheOptions.RedisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheOptions.RedisConnectionString;
            });
        }

        return services;
    }
}
