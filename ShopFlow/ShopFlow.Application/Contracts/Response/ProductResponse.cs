using System.Collections.ObjectModel;

namespace ShopFlow.Application.Contracts.Response;

/// <summary>
/// Comprehensive product response for Vietnamese marketplace
/// Includes multi-vendor, multi-currency, and multi-language support
/// </summary>
public class ProductResponse
{
    /// <summary>Product unique identifier</summary>
    public long Id { get; set; }

    /// <summary>Product name in primary language</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Brief product description</summary>
    public string? ShortDescription { get; set; }

    /// <summary>Detailed product description</summary>
    public string? LongDescription { get; set; }

    /// <summary>SEO-friendly URL slug</summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>Product type identifier</summary>
    public byte ProductType { get; set; }

    /// <summary>Product status identifier</summary>
    public byte Status { get; set; }

    /// <summary>Return period in days</summary>
    public int? ReturnDays { get; set; }

    /// <summary>Product creation timestamp</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Last update timestamp</summary>
    public DateTime? UpdatedAt { get; set; }

    // Multi-Vendor Support
    /// <summary>Vendor unique identifier</summary>
    public long VendorId { get; set; }

    /// <summary>Vendor summary information</summary>
    public VendorSummaryResponse? Vendor { get; set; }

    // Multi-Language Support (Vietnamese Marketplace)
    /// <summary>Primary language code (vi or en)</summary>
    public string PrimaryLanguage { get; set; } = string.Empty;

    /// <summary>Secondary language content for i18n</summary>
    public ProductI18NContentResponse? SecondaryLanguageContent { get; set; }

    // SEO & Marketing
    /// <summary>SEO meta title</summary>
    public string? MetaTitle { get; set; }

    /// <summary>SEO meta description</summary>
    public string? MetaDescription { get; set; }

    /// <summary>Product tags for search and categorization</summary>
    public IReadOnlyCollection<string> Tags { get; init; } = new List<string>();

    // Vietnamese Tax Compliance
    /// <summary>Vietnamese VAT rate (0-1)</summary>
    public decimal? VatRate { get; set; }

    /// <summary>Whether VAT is included in pricing</summary>
    public bool IsVatIncluded { get; set; }

    /// <summary>Vietnamese tax classification code</summary>
    public string? TaxCode { get; set; }

    // Marketplace Workflow
    /// <summary>Internal notes for admin review</summary>
    public string? AdminNotes { get; set; }

    /// <summary>Whether product is under review</summary>
    public bool IsUnderReview { get; set; }

    /// <summary>Review request timestamp</summary>
    public DateTime? ReviewRequestedAt { get; set; }

    /// <summary>Approval timestamp</summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>Approver identifier</summary>
    public string? ApprovedBy { get; set; }

    // Related Data
    /// <summary>Product categories</summary>
    public IReadOnlyCollection<CategoryResponse> Categories { get; init; } = new List<CategoryResponse>();

    /// <summary>Multi-currency pricing information</summary>
    public IReadOnlyCollection<ProductPricingResponse> Pricing { get; init; } = new List<ProductPricingResponse>();

    /// <summary>Product variants (size, color, etc.)</summary>
    public IReadOnlyCollection<ProductVariantResponse> Variants { get; init; } = new List<ProductVariantResponse>();

    /// <summary>Product SKUs</summary>
    public IReadOnlyCollection<SkuResponse> Skus { get; init; } = new List<SkuResponse>();
}

/// <summary>
/// Summary product response for listings and search results
/// Optimized for performance with essential marketplace information
/// </summary>
public class ProductSummaryResponse
{
    // Essential Information
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string Slug { get; set; } = string.Empty;
    public byte ProductType { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAt { get; set; }

    // Vendor Information
    public long VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public bool IsVendorVerified { get; set; }

    // Pricing (showing primary currency)
    public ProductPricingResponse? MainPricing { get; set; }

    // Key Metrics
    public int ReviewCount { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalSold { get; set; }
    public bool HasVariants { get; set; }
    public bool InStock { get; set; }

    // SEO
    public string? MetaTitle { get; set; }
    public List<string> Tags { get; set; } = new();

    // Primary Category
    public CategoryResponse? PrimaryCategory { get; set; }
}

/// <summary>
/// Vendor summary for product responses
/// </summary>
public class VendorSummaryResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; }
    public decimal Rating { get; set; }
    public int TotalProducts { get; set; }
    public string? LogoUrl { get; set; }
    public string? BusinessLicense { get; set; }
    public DateTime JoinedAt { get; set; }
}

/// <summary>
/// Internationalization content response
/// </summary>
public class ProductI18NContentResponse
{
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? LongDescription { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

/// <summary>
/// Multi-currency pricing response for Vietnamese marketplace
/// </summary>
public class ProductPricingResponse
{
    public long Id { get; set; }
    public string CurrencyCode { get; set; } = string.Empty; // "VND" or "USD"
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime? SaleStartDate { get; set; }
    public DateTime? SaleEndDate { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Computed Properties
    public bool IsOnSale => SalePrice.HasValue &&
                           SaleStartDate <= DateTime.UtcNow &&
                           (SaleEndDate == null || SaleEndDate >= DateTime.UtcNow);

    public decimal EffectivePrice => IsOnSale ? SalePrice!.Value : BasePrice;

    public decimal? DiscountPercentage => SalePrice.HasValue ?
        Math.Round((BasePrice - SalePrice.Value) / BasePrice * 100, 2) : null;
}

/// <summary>
/// Product variant response for size/color combinations
/// </summary>
public class ProductVariantResponse
{
    public long Id { get; set; }
    public string VariantType { get; set; } = string.Empty; // "Size", "Color", etc.
    public string VariantValue { get; set; } = string.Empty; // "L", "Red", etc.
    public string? VariantSku { get; set; }
    public decimal? PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
    public long? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Computed Properties
    public bool InStock => StockQuantity > 0;
    public string DisplayName => $"{VariantType}: {VariantValue}";
}

/// <summary>
/// Enhanced SKU response with marketplace information
/// </summary>
public class SkuResponse
{
    public long Id { get; set; }
    public string SkuCode { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public bool IsActive { get; set; }
    public string? OptionsJson { get; set; }
    public int StockQuantity { get; set; }
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Computed Properties
    public bool InStock => StockQuantity > 0;
}
