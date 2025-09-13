using MediatR;
using ShopFlow.Application.Contracts.Response;
using System.Collections.ObjectModel;

namespace ShopFlow.Application.Commands.Products;

/// <summary>
/// Command for creating a new product in the Vietnamese marketplace with multi-vendor support
/// </summary>
/// <param name="Name">Product name in primary language</param>
/// <param name="ShortDescription">Brief product description</param>
/// <param name="LongDescription">Detailed product description</param>
/// <param name="Slug">SEO-friendly URL slug</param>
/// <param name="ProductType">Type of product (physical, digital, etc.)</param>
/// <param name="ReturnDays">Number of days for returns (Vietnamese marketplace: 15-30 days)</param>
/// <param name="CategoryIds">Collection of category IDs this product belongs to</param>
/// <param name="VendorId">ID of the vendor selling this product</param>
/// <param name="PrimaryLanguage">Primary language code (vi or en)</param>
/// <param name="SecondaryLanguageContent">Content in secondary language for i18n</param>
/// <param name="Pricing">Multi-currency pricing information (VND/USD)</param>
/// <param name="Variants">Product variants like size, color combinations</param>
/// <param name="MetaTitle">SEO meta title</param>
/// <param name="MetaDescription">SEO meta description</param>
/// <param name="Tags">Product tags for search and categorization</param>
/// <param name="VatRate">Vietnamese VAT rate (usually 10%)</param>
/// <param name="IsVatIncluded">Whether VAT is included in pricing</param>
/// <param name="TaxCode">Vietnamese tax classification code</param>
/// <param name="AdminNotes">Internal notes for admin review</param>
/// <param name="RequestImmediateReview">Whether to prioritize this product for review</param>
public record CreateProductCommand(
    // Basic Product Information
    string Name,
    string? ShortDescription,
    string? LongDescription,
    string Slug,
    byte ProductType,
    int? ReturnDays,
    IReadOnlyCollection<long> CategoryIds,

    // Multi-Vendor Support
    long VendorId,

    // Multi-Language Support (Vietnamese Marketplace)
    string PrimaryLanguage, // "vi" or "en"
    CreateProductI18NContent? SecondaryLanguageContent,

    // Multi-Currency Pricing (VND/USD)
    IReadOnlyCollection<CreateProductPricingRequest> Pricing,

    // Product Variants (Size/Color)
    IReadOnlyCollection<CreateProductVariantRequest>? Variants,

    // SEO & Marketing
    string? MetaTitle,
    string? MetaDescription,
    IReadOnlyCollection<string>? Tags,

    // Vietnamese Tax Compliance
    decimal? VatRate,
    bool IsVatIncluded,
    string? TaxCode,

    // Marketplace Workflow
    string? AdminNotes,
    bool RequestImmediateReview
) : IRequest<ProductResponse>;

/// <summary>
/// Internationalization content for secondary language support
/// </summary>
/// <param name="Name">Product name in secondary language</param>
/// <param name="ShortDescription">Brief description in secondary language</param>
/// <param name="LongDescription">Detailed description in secondary language</param>
/// <param name="MetaTitle">SEO meta title in secondary language</param>
/// <param name="MetaDescription">SEO meta description in secondary language</param>
public record CreateProductI18NContent(
    string Name,
    string? ShortDescription,
    string? LongDescription,
    string? MetaTitle,
    string? MetaDescription
);

/// <summary>
/// Multi-currency pricing request for Vietnamese marketplace
/// </summary>
/// <param name="CurrencyCode">Currency code (VND or USD)</param>
/// <param name="BasePrice">Regular selling price</param>
/// <param name="SalePrice">Discounted price if on sale</param>
/// <param name="SaleStartDate">Sale start date</param>
/// <param name="SaleEndDate">Sale end date</param>
/// <param name="MinQuantity">Minimum quantity for bulk pricing</param>
/// <param name="MaxQuantity">Maximum quantity allowed per order</param>
public record CreateProductPricingRequest(
    string CurrencyCode, // "VND" or "USD"
    decimal BasePrice,
    decimal? SalePrice,
    DateTime? SaleStartDate,
    DateTime? SaleEndDate,
    int? MinQuantity,
    int? MaxQuantity
);

/// <summary>
/// Product variant request for size/color combinations
/// </summary>
/// <param name="VariantType">Type of variant (Size, Color, etc.)</param>
/// <param name="VariantValue">Specific value (L, Red, etc.)</param>
/// <param name="VariantSku">Unique SKU for this variant</param>
/// <param name="PriceAdjustment">Price adjustment from base price</param>
/// <param name="StockQuantity">Available stock for this variant</param>
/// <param name="WarehouseId">Warehouse storing this variant</param>
/// <param name="IsDefault">Whether this is the default variant</param>
public record CreateProductVariantRequest(
    string VariantType, // "Size", "Color", etc.
    string VariantValue, // "L", "Red", etc.
    string? VariantSku,
    decimal? PriceAdjustment,
    int StockQuantity,
    long? WarehouseId,
    bool IsDefault
);
