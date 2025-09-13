using System.Text.Json;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.API.Middlewares;

/// <summary>
/// Middleware for Vietnamese marketplace currency conversion
/// Handles automatic conversion between VND and USD for Vietnamese market
/// </summary>
public class CurrencyConversionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CurrencyConversionMiddleware> _logger;
    private readonly CurrencyExchangeService _exchangeService;

    public CurrencyConversionMiddleware(
        RequestDelegate next,
        ILogger<CurrencyConversionMiddleware> logger,
        CurrencyExchangeService exchangeService)
    {
        _next = next;
        _logger = logger;
        _exchangeService = exchangeService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip for non-API routes or non-GET requests
        if (!context.Request.Path.StartsWithSegments("/api") || context.Request.Method != "GET")
        {
            await _next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;

        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context);

        // Only process successful responses with JSON content
        if (context.Response.StatusCode == 200 &&
            context.Response.ContentType?.Contains("application/json") == true)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            if (!string.IsNullOrEmpty(responseBody))
            {
                var convertedBody = await ConvertCurrencyInResponse(context, responseBody);
                var convertedBytes = System.Text.Encoding.UTF8.GetBytes(convertedBody);

                context.Response.ContentLength = convertedBytes.Length;
                await originalBodyStream.WriteAsync(convertedBytes);
            }
        }
        else
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }

        context.Response.Body = originalBodyStream;
    }

    private async Task<string> ConvertCurrencyInResponse(HttpContext context, string responseBody)
    {
        try
        {
            var marketplaceContext = context.GetMarketplaceContext();
            var targetCurrency = marketplaceContext?.Currency ?? "VND";

            // Skip conversion if already in target currency or if target is VND (base currency)
            if (targetCurrency == "VND")
            {
                return responseBody;
            }

            var jsonDocument = JsonDocument.Parse(responseBody);
            var convertedJson = await ConvertJsonCurrency(jsonDocument.RootElement, targetCurrency);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            return JsonSerializer.Serialize(convertedJson, options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to convert currency in response");
            return responseBody; // Return original response if conversion fails
        }
    }

    private async Task<object> ConvertJsonCurrency(JsonElement element, string targetCurrency)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var convertedObject = new Dictionary<string, object>();

                foreach (var property in element.EnumerateObject())
                {
                    if (IsCurrencyProperty(property.Name))
                    {
                        convertedObject[property.Name] = await ConvertCurrencyValue(property.Value, targetCurrency);
                    }
                    else
                    {
                        convertedObject[property.Name] = await ConvertJsonCurrency(property.Value, targetCurrency);
                    }
                }
                return convertedObject;

            case JsonValueKind.Array:
                var convertedArray = new List<object>();
                foreach (var item in element.EnumerateArray())
                {
                    convertedArray.Add(await ConvertJsonCurrency(item, targetCurrency));
                }
                return convertedArray;

            case JsonValueKind.String:
                return element.GetString() ?? string.Empty;

            case JsonValueKind.Number:
                return element.TryGetDecimal(out var decimalValue) ? decimalValue : element.GetDouble();

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            case JsonValueKind.Null:
                return null!;

            default:
                return element.ToString();
        }
    }

    private async Task<object> ConvertCurrencyValue(JsonElement currencyElement, string targetCurrency)
    {
        try
        {
            // Handle currency object with amount and currency properties
            if (currencyElement.ValueKind == JsonValueKind.Object)
            {
                var currencyObject = new Dictionary<string, object>();

                foreach (var property in currencyElement.EnumerateObject())
                {
                    if (property.Name.Equals("amount", StringComparison.OrdinalIgnoreCase) &&
                        property.Value.TryGetDecimal(out var amount))
                    {
                        var convertedAmount = await _exchangeService.ConvertCurrency(amount, "VND", targetCurrency);
                        currencyObject[property.Name] = convertedAmount;
                    }
                    else if (property.Name.Equals("currency", StringComparison.OrdinalIgnoreCase))
                    {
                        currencyObject[property.Name] = targetCurrency;
                    }
                    else
                    {
                        currencyObject[property.Name] = property.Value.ValueKind switch
                        {
                            JsonValueKind.String => property.Value.GetString() ?? string.Empty,
                            JsonValueKind.Number => property.Value.TryGetDecimal(out var decVal) ? decVal : property.Value.GetDouble(),
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.Null => null!,
                            _ => property.Value.ToString()
                        };
                    }
                }
                return currencyObject;
            }

            // Handle simple decimal/number currency values
            if (currencyElement.TryGetDecimal(out var simpleAmount))
            {
                return await _exchangeService.ConvertCurrency(simpleAmount, "VND", targetCurrency);
            }

            return currencyElement.ValueKind switch
            {
                JsonValueKind.String => currencyElement.GetString() ?? string.Empty,
                JsonValueKind.Number => currencyElement.TryGetDecimal(out var decVal) ? decVal : currencyElement.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                _ => currencyElement.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to convert currency value");
            return currencyElement.ToString();
        }
    }

    private static bool IsCurrencyProperty(string propertyName)
    {
        var currencyProperties = new[]
        {
            "price", "cost", "amount", "total", "subtotal", "tax", "discount", "fee",
            "unitPrice", "totalPrice", "originalPrice", "salePrice", "finalPrice",
            "minimumPrice", "maximumPrice", "averagePrice", "basePrice", "netPrice",
            "grossPrice", "taxAmount", "shippingCost", "handlingFee", "serviceFee",
            "money", "value", "balance", "credit", "debit", "revenue", "profit"
        };

        return currencyProperties.Any(cp =>
            propertyName.Equals(cp, StringComparison.OrdinalIgnoreCase) ||
            propertyName.EndsWith("Price", StringComparison.OrdinalIgnoreCase) ||
            propertyName.EndsWith("Cost", StringComparison.OrdinalIgnoreCase) ||
            propertyName.EndsWith("Amount", StringComparison.OrdinalIgnoreCase) ||
            propertyName.EndsWith("Fee", StringComparison.OrdinalIgnoreCase) ||
            propertyName.EndsWith("Total", StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Service for currency exchange rate management
/// Provides exchange rates for Vietnamese marketplace
/// </summary>
public class CurrencyExchangeService
{
    private readonly ILogger<CurrencyExchangeService> _logger;
    private readonly Dictionary<string, decimal> _exchangeRates;

    public CurrencyExchangeService(ILogger<CurrencyExchangeService> logger)
    {
        _logger = logger;

        // In production, these would be fetched from a real exchange rate API
        _exchangeRates = new Dictionary<string, decimal>
        {
            ["VND_USD"] = 0.00004m, // 1 VND = 0.00004 USD (approximate)
            ["USD_VND"] = 25000m,   // 1 USD = 25,000 VND (approximate)
            ["VND_VND"] = 1.0m,
            ["USD_USD"] = 1.0m
        };
    }

    public async Task<decimal> ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return amount;
        }

        var rateKey = $"{fromCurrency.ToUpperInvariant()}_{toCurrency.ToUpperInvariant()}";

        if (_exchangeRates.TryGetValue(rateKey, out var rate))
        {
            var convertedAmount = amount * rate;

            _logger.LogDebug(
                "Currency conversion: {Amount} {FromCurrency} -> {ConvertedAmount} {ToCurrency} (Rate: {Rate})",
                amount, fromCurrency, convertedAmount, toCurrency, rate);

            return Math.Round(convertedAmount, GetCurrencyDecimals(toCurrency));
        }

        _logger.LogWarning("Exchange rate not found for {FromCurrency} to {ToCurrency}", fromCurrency, toCurrency);
        return amount; // Return original amount if rate not found
    }

    public async Task<decimal> GetExchangeRate(string fromCurrency, string toCurrency)
    {
        var rateKey = $"{fromCurrency.ToUpperInvariant()}_{toCurrency.ToUpperInvariant()}";
        return _exchangeRates.GetValueOrDefault(rateKey, 1.0m);
    }

    private static int GetCurrencyDecimals(string currency)
    {
        return currency.ToUpperInvariant() switch
        {
            "VND" => 0, // Vietnamese Dong has no decimal places
            "USD" => 2, // US Dollar has 2 decimal places
            _ => 2
        };
    }

    public void UpdateExchangeRate(string fromCurrency, string toCurrency, decimal rate)
    {
        var rateKey = $"{fromCurrency.ToUpperInvariant()}_{toCurrency.ToUpperInvariant()}";
        _exchangeRates[rateKey] = rate;

        _logger.LogInformation(
            "Updated exchange rate: {FromCurrency} to {ToCurrency} = {Rate}",
            fromCurrency, toCurrency, rate);
    }
}