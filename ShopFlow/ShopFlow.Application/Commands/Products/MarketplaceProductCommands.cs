using MediatR;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Commands.Products;

/// <summary>
/// Command to update product status in Vietnamese marketplace
/// </summary>
/// <param name="ProductId">ID of the product to update</param>
/// <param name="NewStatus">New status to set</param>
/// <param name="AdminNotes">Admin notes about the status change</param>
/// <param name="NotifyVendor">Whether to notify the vendor about status change</param>
public record UpdateProductStatusCommand(
    long ProductId,
    ProductStatus NewStatus,
    string? AdminNotes = null,
    bool NotifyVendor = true
) : IRequest<ProductResponse>;

/// <summary>
/// Command to approve or reject product in Vietnamese marketplace
/// </summary>
/// <param name="ProductId">ID of the product to approve/reject</param>
/// <param name="IsApproved">Whether the product is approved</param>
/// <param name="AdminNotes">Admin notes about the decision</param>
/// <param name="RejectionReason">Reason for rejection if not approved</param>
/// <param name="AdminId">ID of the admin making the decision</param>
public record ApproveRejectProductCommand(
    long ProductId,
    bool IsApproved,
    string? AdminNotes = null,
    string? RejectionReason = null,
    long? AdminId = null
) : IRequest<ProductResponse>;

/// <summary>
/// Command to bulk update multiple products in Vietnamese marketplace
/// </summary>
/// <param name="ProductIds">Collection of product IDs to update</param>
/// <param name="NewStatus">New status to apply to all products</param>
/// <param name="AdminNotes">Admin notes about the bulk operation</param>
/// <param name="AdminId">ID of the admin performing bulk operation</param>
/// <param name="NotifyVendors">Whether to notify affected vendors</param>
public record BulkUpdateProductsCommand(
    IReadOnlyCollection<long> ProductIds,
    ProductStatus NewStatus,
    string? AdminNotes = null,
    long? AdminId = null,
    bool NotifyVendors = true
) : IRequest<BulkUpdateProductsResponse>;

/// <summary>
/// Command to update product pricing for Vietnamese marketplace multi-currency support
/// </summary>
/// <param name="ProductId">ID of the product</param>
/// <param name="Pricing">New pricing information</param>
/// <param name="VendorId">ID of the vendor (for authorization)</param>
/// <param name="EffectiveDate">When the new pricing takes effect</param>
public record UpdateProductPricingCommand(
    long ProductId,
    IReadOnlyCollection<UpdateProductPricingRequest> Pricing,
    long VendorId,
    DateTime? EffectiveDate = null
) : IRequest<ProductResponse>;

/// <summary>
/// Command to add or update product variants in Vietnamese marketplace
/// </summary>
/// <param name="ProductId">ID of the product</param>
/// <param name="Variants">Collection of variants to add/update</param>
/// <param name="VendorId">ID of the vendor (for authorization)</param>
/// <param name="ReplaceExisting">Whether to replace all existing variants</param>
public record UpdateProductVariantsCommand(
    long ProductId,
    IReadOnlyCollection<UpdateProductVariantRequest> Variants,
    long VendorId,
    bool ReplaceExisting = false
) : IRequest<ProductResponse>;

/// <summary>
/// Command to update product i18n content for Vietnamese marketplace
/// </summary>
/// <param name="ProductId">ID of the product</param>
/// <param name="LanguageCode">Language code (vi/en)</param>
/// <param name="Content">Content in the specified language</param>
/// <param name="VendorId">ID of the vendor (for authorization)</param>
public record UpdateProductI18nContentCommand(
    long ProductId,
    string LanguageCode,
    UpdateProductI18nContentRequest Content,
    long VendorId
) : IRequest<ProductResponse>;

// Supporting request DTOs

/// <summary>
/// Request for updating product pricing
/// </summary>
/// <param name="CurrencyCode">Currency code (VND/USD)</param>
/// <param name="BasePrice">Base price in the currency</param>
/// <param name="SalePrice">Sale price (optional)</param>
/// <param name="SaleStartDate">Sale start date</param>
/// <param name="SaleEndDate">Sale end date</param>
/// <param name="MinQuantity">Minimum quantity for this price</param>
/// <param name="MaxQuantity">Maximum quantity for this price</param>
public record UpdateProductPricingRequest(
    string CurrencyCode,
    decimal BasePrice,
    decimal? SalePrice = null,
    DateTime? SaleStartDate = null,
    DateTime? SaleEndDate = null,
    int? MinQuantity = null,
    int? MaxQuantity = null
);

/// <summary>
/// Request for updating product variants
/// </summary>
/// <param name="Id">Variant ID (0 for new variants)</param>
/// <param name="VariantType">Type of variant (Size, Color, etc.)</param>
/// <param name="VariantValue">Specific value (L, Red, etc.)</param>
/// <param name="Sku">Unique SKU for this variant</param>
/// <param name="PriceAdjustment">Price adjustment from base price</param>
/// <param name="StockQuantity">Stock quantity for this variant</param>
/// <param name="IsActive">Whether this variant is active</param>
public record UpdateProductVariantRequest(
    long Id,
    string VariantType,
    string VariantValue,
    string Sku,
    decimal PriceAdjustment = 0,
    int StockQuantity = 0,
    bool IsActive = true
);

/// <summary>
/// Request for updating product i18n content
/// </summary>
/// <param name="Name">Product name in the language</param>
/// <param name="ShortDescription">Short description in the language</param>
/// <param name="LongDescription">Long description in the language</param>
/// <param name="MetaTitle">SEO meta title in the language</param>
/// <param name="MetaDescription">SEO meta description in the language</param>
/// <param name="Tags">Tags in the language</param>
public record UpdateProductI18nContentRequest(
    string Name,
    string? ShortDescription = null,
    string? LongDescription = null,
    string? MetaTitle = null,
    string? MetaDescription = null,
    IReadOnlyCollection<string>? Tags = null
);

/// <summary>
/// Response for bulk update operations
/// </summary>
public record BulkUpdateProductsResponse
{
    /// <summary>Number of products successfully updated</summary>
    public int UpdatedCount { get; init; }

    /// <summary>Number of products that failed to update</summary>
    public int FailedCount { get; init; }

    /// <summary>Collection of product IDs that were successfully updated</summary>
    public IReadOnlyCollection<long> UpdatedProductIds { get; init; } = new List<long>();

    /// <summary>Collection of product IDs that failed to update</summary>
    public IReadOnlyCollection<long> FailedProductIds { get; init; } = new List<long>();

    /// <summary>Error messages for failed updates</summary>
    public IReadOnlyCollection<string> ErrorMessages { get; init; } = new List<string>();

    /// <summary>Total number of products in the operation</summary>
    public int TotalCount => UpdatedCount + FailedCount;

    /// <summary>Success rate percentage</summary>
    public decimal SuccessRate => TotalCount > 0 ? (decimal)UpdatedCount / TotalCount * 100 : 0;
}