using System.ComponentModel.DataAnnotations;

namespace ShopFlow.API.Configurations;

public class CacheOptions
{
    public const string SectionName = "Cache";

    public bool UseInMemoryCache { get; set; } = true;
    public bool UseRedisCache { get; set; } = false;
    public string? RedisConnectionString { get; set; }
    public int DefaultExpirationMinutes { get; set; } = 30;
    public string KeyPrefix { get; set; } = "ShopFlow:";
}
