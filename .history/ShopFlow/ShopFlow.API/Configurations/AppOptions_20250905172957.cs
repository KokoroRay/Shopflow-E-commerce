using System.ComponentModel.DataAnnotations;

namespace ShopFlow.API.Configurations;

public class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";

    [Required]
    public string Default { get; set; } = string.Empty;
}

public class CorsOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public string[] AllowedMethods { get; set; } = { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
    public string[] AllowedHeaders { get; set; } = { "*" };
    public bool AllowCredentials { get; set; } = true;
}

public class SwaggerOptions
{
    public const string SectionName = "Swagger";

    public bool Enabled { get; set; } = true;
    public string Title { get; set; } = "ShopFlow API";
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = "ShopFlow E-commerce API";
}
