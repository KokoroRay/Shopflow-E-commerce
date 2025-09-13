using System.Globalization;
using System.Text;
using System.Text.Json;

namespace ShopFlow.API.Middlewares;

/// <summary>
/// Middleware for Vietnamese marketplace request/response processing
/// Handles language detection, currency conversion, and market compliance
/// </summary>
public class VietnameseMarketplaceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<VietnameseMarketplaceMiddleware> _logger;
    private static readonly string[] SupportedLanguages = { "vi", "en" };
    private static readonly string[] SupportedCurrencies = { "VND", "USD" };

    public VietnameseMarketplaceMiddleware(RequestDelegate next, ILogger<VietnameseMarketplaceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip middleware for non-API routes
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var marketplaceContext = await ProcessMarketplaceHeaders(context);

        // Add marketplace context to HttpContext
        context.Items["MarketplaceContext"] = marketplaceContext;

        // Set culture for Vietnamese processing
        SetRequestCulture(marketplaceContext.Language);

        // Log marketplace request
        LogMarketplaceRequest(context, marketplaceContext);

        await _next(context);

        // Process response headers
        await ProcessMarketplaceResponse(context, marketplaceContext);
    }

    private async Task<MarketplaceContext> ProcessMarketplaceHeaders(HttpContext context)
    {
        var headers = context.Request.Headers;

        // Extract marketplace headers
        var language = ExtractLanguage(headers);
        var currency = ExtractCurrency(headers);
        var vendor = ExtractVendor(headers);
        var region = ExtractRegion(headers);
        var timezone = ExtractTimezone(headers);

        // Validate Vietnamese marketplace compliance
        ValidateMarketplaceCompliance(context, language, currency);

        return new MarketplaceContext
        {
            Language = language,
            Currency = currency,
            VendorId = vendor,
            Region = region,
            Timezone = timezone,
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    private string ExtractLanguage(IHeaderDictionary headers)
    {
        // Priority: X-Marketplace-Language -> Accept-Language -> Default
        if (headers.TryGetValue("X-Marketplace-Language", out var marketplaceLanguage))
        {
            var lang = marketplaceLanguage.ToString().ToLowerInvariant();
            if (SupportedLanguages.Contains(lang))
                return lang;
        }

        if (headers.TryGetValue("Accept-Language", out var acceptLanguage))
        {
            var languages = acceptLanguage.ToString().Split(',')
                .Select(l => l.Split(';')[0].Trim().ToLowerInvariant())
                .Where(l => SupportedLanguages.Contains(l))
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(languages))
                return languages;
        }

        return "vi"; // Default to Vietnamese
    }

    private string ExtractCurrency(IHeaderDictionary headers)
    {
        // Priority: X-Marketplace-Currency -> X-Currency -> Default
        if (headers.TryGetValue("X-Marketplace-Currency", out var marketplaceCurrency))
        {
            var currency = marketplaceCurrency.ToString().ToUpperInvariant();
            if (SupportedCurrencies.Contains(currency))
                return currency;
        }

        if (headers.TryGetValue("X-Currency", out var currencyHeader))
        {
            var currency = currencyHeader.ToString().ToUpperInvariant();
            if (SupportedCurrencies.Contains(currency))
                return currency;
        }

        return "VND"; // Default to Vietnamese Dong
    }

    private long? ExtractVendor(IHeaderDictionary headers)
    {
        if (headers.TryGetValue("X-Marketplace-Vendor-Id", out var vendorId))
        {
            if (long.TryParse(vendorId.ToString(), out var id))
                return id;
        }

        return null;
    }

    private string ExtractRegion(IHeaderDictionary headers)
    {
        if (headers.TryGetValue("X-Marketplace-Region", out var region))
        {
            return region.ToString();
        }

        return "VN"; // Default to Vietnam
    }

    private string ExtractTimezone(IHeaderDictionary headers)
    {
        if (headers.TryGetValue("X-Marketplace-Timezone", out var timezone))
        {
            return timezone.ToString();
        }

        return "Asia/Ho_Chi_Minh"; // Default to Vietnam timezone
    }

    private void ValidateMarketplaceCompliance(HttpContext context, string language, string currency)
    {
        var violations = new List<string>();

        // Vietnamese marketplace must support Vietnamese language
        if (!SupportedLanguages.Contains(language))
        {
            violations.Add($"Unsupported language: {language}. Supported: {string.Join(", ", SupportedLanguages)}");
        }

        // Currency compliance for Vietnamese market
        if (!SupportedCurrencies.Contains(currency))
        {
            violations.Add($"Unsupported currency: {currency}. Supported: {string.Join(", ", SupportedCurrencies)}");
        }

        // Check for required tax compliance headers for Vietnamese market
        if (context.Request.Method != "GET" && !context.Request.Headers.ContainsKey("X-Tax-Compliance"))
        {
            // Warning only - not blocking
            _logger.LogWarning("Missing X-Tax-Compliance header for Vietnamese marketplace tax compliance");
        }

        if (violations.Any())
        {
            var errorMessage = $"Vietnamese marketplace compliance violations: {string.Join("; ", violations)}";
            _logger.LogWarning(errorMessage);

            // Add compliance warning header instead of throwing exception
            context.Response.Headers.Add("X-Marketplace-Compliance-Warning", errorMessage);
        }
    }

    private void SetRequestCulture(string language)
    {
        try
        {
            var cultureInfo = language switch
            {
                "vi" => new CultureInfo("vi-VN"),
                "en" => new CultureInfo("en-US"),
                _ => new CultureInfo("vi-VN")
            };

            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set culture for language: {Language}", language);
        }
    }

    private void LogMarketplaceRequest(HttpContext context, MarketplaceContext marketplaceContext)
    {
        _logger.LogInformation(
            "Vietnamese Marketplace Request: {Method} {Path} | Language: {Language} | Currency: {Currency} | Vendor: {VendorId} | Region: {Region} | RequestId: {RequestId}",
            context.Request.Method,
            context.Request.Path,
            marketplaceContext.Language,
            marketplaceContext.Currency,
            marketplaceContext.VendorId?.ToString() ?? "N/A",
            marketplaceContext.Region,
            marketplaceContext.RequestId
        );
    }

    private async Task ProcessMarketplaceResponse(HttpContext context, MarketplaceContext marketplaceContext)
    {
        // Add marketplace response headers
        context.Response.Headers.Add("X-Marketplace-Language", marketplaceContext.Language);
        context.Response.Headers.Add("X-Marketplace-Currency", marketplaceContext.Currency);
        context.Response.Headers.Add("X-Marketplace-Region", marketplaceContext.Region);
        context.Response.Headers.Add("X-Marketplace-Request-Id", marketplaceContext.RequestId);
        context.Response.Headers.Add("X-Marketplace-Timestamp", marketplaceContext.Timestamp.ToString("O"));

        // Add Vietnamese marketplace compliance headers
        context.Response.Headers.Add("X-Vietnamese-Marketplace", "enabled");
        context.Response.Headers.Add("X-Tax-Jurisdiction", "Vietnam");
        context.Response.Headers.Add("X-Currency-Exchange-Rate", GetExchangeRateInfo(marketplaceContext.Currency));

        // Log response
        _logger.LogInformation(
            "Vietnamese Marketplace Response: {StatusCode} | RequestId: {RequestId} | Language: {Language} | Currency: {Currency}",
            context.Response.StatusCode,
            marketplaceContext.RequestId,
            marketplaceContext.Language,
            marketplaceContext.Currency
        );
    }

    private string GetExchangeRateInfo(string currency)
    {
        // In production, this would fetch real exchange rates
        return currency switch
        {
            "VND" => "1.0000",
            "USD" => "0.00004", // Approximate VND to USD rate (1 USD = ~25,000 VND)
            _ => "1.0000"
        };
    }
}

/// <summary>
/// Context information for Vietnamese marketplace processing
/// </summary>
public class MarketplaceContext
{
    public string Language { get; set; } = "vi";
    public string Currency { get; set; } = "VND";
    public long? VendorId { get; set; }
    public string Region { get; set; } = "VN";
    public string Timezone { get; set; } = "Asia/Ho_Chi_Minh";
    public string RequestId { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}

/// <summary>
/// Extension methods for marketplace context
/// </summary>
public static class MarketplaceContextExtensions
{
    public static MarketplaceContext? GetMarketplaceContext(this HttpContext context)
    {
        return context.Items["MarketplaceContext"] as MarketplaceContext;
    }

    public static bool IsVietnameseMarketplace(this HttpContext context)
    {
        var marketplaceContext = context.GetMarketplaceContext();
        return marketplaceContext?.Language == "vi" && marketplaceContext?.Region == "VN";
    }

    public static string GetMarketplaceCurrency(this HttpContext context)
    {
        var marketplaceContext = context.GetMarketplaceContext();
        return marketplaceContext?.Currency ?? "VND";
    }

    public static string GetMarketplaceLanguage(this HttpContext context)
    {
        var marketplaceContext = context.GetMarketplaceContext();
        return marketplaceContext?.Language ?? "vi";
    }
}