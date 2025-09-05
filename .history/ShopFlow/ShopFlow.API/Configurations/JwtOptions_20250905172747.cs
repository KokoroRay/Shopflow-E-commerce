using System.ComponentModel.DataAnnotations;

namespace ShopFlow.API.Configurations;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ExpirationInMinutes { get; set; } = 60;

    [Range(1, int.MaxValue)]
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}
