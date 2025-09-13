using ShopFlow.API.Middlewares;

namespace ShopFlow.API.Extensions;

/// <summary>
/// Extension methods for registering Vietnamese marketplace middleware
/// </summary>
public static class VietnameseMarketplaceExtensions
{
    /// <summary>
    /// Adds Vietnamese marketplace services to the service collection
    /// </summary>
    public static IServiceCollection AddVietnameseMarketplace(this IServiceCollection services)
    {
        // Register currency exchange service
        services.AddSingleton<CurrencyExchangeService>();

        // Add marketplace-specific services
        services.AddScoped<IMarketplaceHeaderValidator, MarketplaceHeaderValidator>();
        services.AddScoped<IVietnameseTextProcessor, VietnameseTextProcessor>();

        return services;
    }

    /// <summary>
    /// Configures Vietnamese marketplace middleware in the request pipeline
    /// </summary>
    public static IApplicationBuilder UseVietnameseMarketplace(this IApplicationBuilder app)
    {
        // Order matters: Marketplace middleware should come before currency conversion
        app.UseMiddleware<VietnameseMarketplaceMiddleware>();
        app.UseMiddleware<CurrencyConversionMiddleware>();

        return app;
    }
}

/// <summary>
/// Interface for marketplace header validation
/// </summary>
public interface IMarketplaceHeaderValidator
{
    bool ValidateHeaders(IHeaderDictionary headers);
    MarketplaceValidationResult ValidateMarketplaceCompliance(IHeaderDictionary headers);
}

/// <summary>
/// Interface for Vietnamese text processing
/// </summary>
public interface IVietnameseTextProcessor
{
    string NormalizeVietnameseText(string text);
    string ConvertToUrlSlug(string vietnameseText);
    bool IsVietnameseText(string text);
}

/// <summary>
/// Marketplace header validator implementation
/// </summary>
public class MarketplaceHeaderValidator : IMarketplaceHeaderValidator
{
    private readonly ILogger<MarketplaceHeaderValidator> _logger;
    private static readonly string[] RequiredHeaders =
    {
        "X-Marketplace-Language",
        "X-Marketplace-Currency"
    };

    public MarketplaceHeaderValidator(ILogger<MarketplaceHeaderValidator> logger)
    {
        _logger = logger;
    }

    public bool ValidateHeaders(IHeaderDictionary headers)
    {
        var missingHeaders = RequiredHeaders.Where(header => !headers.ContainsKey(header)).ToList();

        if (missingHeaders.Any())
        {
            _logger.LogWarning("Missing required marketplace headers: {Headers}", string.Join(", ", missingHeaders));
            return false;
        }

        return true;
    }

    public MarketplaceValidationResult ValidateMarketplaceCompliance(IHeaderDictionary headers)
    {
        var result = new MarketplaceValidationResult { IsValid = true };
        var issues = new List<string>();

        // Validate language header
        if (headers.TryGetValue("X-Marketplace-Language", out var languageHeader))
        {
            var language = languageHeader.ToString().ToLowerInvariant();
            if (language != "vi" && language != "en")
            {
                issues.Add($"Unsupported language: {language}. Vietnamese marketplace supports 'vi' and 'en' only.");
            }
        }
        else
        {
            issues.Add("Missing X-Marketplace-Language header for Vietnamese marketplace.");
        }

        // Validate currency header
        if (headers.TryGetValue("X-Marketplace-Currency", out var currencyHeader))
        {
            var currency = currencyHeader.ToString().ToUpperInvariant();
            if (currency != "VND" && currency != "USD")
            {
                issues.Add($"Unsupported currency: {currency}. Vietnamese marketplace supports 'VND' and 'USD' only.");
            }
        }
        else
        {
            issues.Add("Missing X-Marketplace-Currency header for Vietnamese marketplace.");
        }

        // Validate region compliance
        if (headers.TryGetValue("X-Marketplace-Region", out var regionHeader))
        {
            var region = regionHeader.ToString().ToUpperInvariant();
            if (region != "VN")
            {
                _logger.LogWarning("Non-Vietnamese region detected: {Region}. Some features may be limited.", region);
            }
        }

        // Check for tax compliance (warning only)
        if (!headers.ContainsKey("X-Tax-Compliance"))
        {
            issues.Add("Recommended: Include X-Tax-Compliance header for Vietnamese tax regulations.");
        }

        if (issues.Any())
        {
            result.IsValid = false;
            result.Issues = issues;
        }

        return result;
    }
}

/// <summary>
/// Vietnamese text processor implementation
/// </summary>
public class VietnameseTextProcessor : IVietnameseTextProcessor
{
    private readonly ILogger<VietnameseTextProcessor> _logger;

    // Vietnamese character mappings for URL slug generation
    private static readonly Dictionary<char, string> VietnameseCharMap = new()
    {
        ['à'] = "a",
        ['á'] = "a",
        ['ạ'] = "a",
        ['ả'] = "a",
        ['ã'] = "a",
        ['â'] = "a",
        ['ầ'] = "a",
        ['ấ'] = "a",
        ['ậ'] = "a",
        ['ẩ'] = "a",
        ['ẫ'] = "a",
        ['ă'] = "a",
        ['ằ'] = "a",
        ['ắ'] = "a",
        ['ặ'] = "a",
        ['ẳ'] = "a",
        ['ẵ'] = "a",
        ['è'] = "e",
        ['é'] = "e",
        ['ẹ'] = "e",
        ['ẻ'] = "e",
        ['ẽ'] = "e",
        ['ê'] = "e",
        ['ề'] = "e",
        ['ế'] = "e",
        ['ệ'] = "e",
        ['ể'] = "e",
        ['ễ'] = "e",
        ['ì'] = "i",
        ['í'] = "i",
        ['ị'] = "i",
        ['ỉ'] = "i",
        ['ĩ'] = "i",
        ['ò'] = "o",
        ['ó'] = "o",
        ['ọ'] = "o",
        ['ỏ'] = "o",
        ['õ'] = "o",
        ['ô'] = "o",
        ['ồ'] = "o",
        ['ố'] = "o",
        ['ộ'] = "o",
        ['ổ'] = "o",
        ['ỗ'] = "o",
        ['ơ'] = "o",
        ['ờ'] = "o",
        ['ớ'] = "o",
        ['ợ'] = "o",
        ['ở'] = "o",
        ['ỡ'] = "o",
        ['ù'] = "u",
        ['ú'] = "u",
        ['ụ'] = "u",
        ['ủ'] = "u",
        ['ũ'] = "u",
        ['ư'] = "u",
        ['ừ'] = "u",
        ['ứ'] = "u",
        ['ự'] = "u",
        ['ử'] = "u",
        ['ữ'] = "u",
        ['ỳ'] = "y",
        ['ý'] = "y",
        ['ỵ'] = "y",
        ['ỷ'] = "y",
        ['ỹ'] = "y",
        ['đ'] = "d"
    };

    public VietnameseTextProcessor(ILogger<VietnameseTextProcessor> logger)
    {
        _logger = logger;
    }

    public string NormalizeVietnameseText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        try
        {
            // Normalize to NFD (decomposed form) then remove diacritics
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var result = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (VietnameseCharMap.TryGetValue(char.ToLowerInvariant(c), out var replacement))
                {
                    result.Append(char.IsUpper(c) ? replacement.ToUpperInvariant() : replacement);
                }
                else if (char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to normalize Vietnamese text: {Text}", text);
            return text;
        }
    }

    public string ConvertToUrlSlug(string vietnameseText)
    {
        if (string.IsNullOrWhiteSpace(vietnameseText))
            return string.Empty;

        try
        {
            // First normalize Vietnamese characters
            var normalized = NormalizeVietnameseText(vietnameseText);

            // Convert to lowercase and replace spaces with hyphens
            var slug = normalized
                .ToLowerInvariant()
                .Trim()
                .Replace(" ", "-", StringComparison.OrdinalIgnoreCase)
                .Replace("_", "-", StringComparison.OrdinalIgnoreCase);

            // Remove invalid characters (keep only letters, numbers, and hyphens)
            var cleanSlug = new StringBuilder();
            foreach (var c in slug)
            {
                if (char.IsLetterOrDigit(c) || c == '-')
                {
                    cleanSlug.Append(c);
                }
            }

            // Remove multiple consecutive hyphens
            var result = System.Text.RegularExpressions.Regex.Replace(cleanSlug.ToString(), @"-+", "-");

            // Remove leading/trailing hyphens
            result = result.Trim('-');

            return string.IsNullOrEmpty(result) ? "product" : result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to convert Vietnamese text to URL slug: {Text}", vietnameseText);
            return "product";
        }
    }

    public bool IsVietnameseText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        // Check if text contains Vietnamese characters
        return text.Any(c => VietnameseCharMap.ContainsKey(char.ToLowerInvariant(c)) || c == 'đ' || c == 'Đ');
    }
}

/// <summary>
/// Result of marketplace validation
/// </summary>
public class MarketplaceValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}