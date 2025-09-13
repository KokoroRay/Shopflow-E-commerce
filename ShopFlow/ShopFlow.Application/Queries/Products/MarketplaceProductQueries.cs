using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Products;

/// <summary>
/// Query to get products by vendor for Vietnamese marketplace multi-vendor support
/// </summary>
/// <param name="VendorId">ID of the vendor</param>
/// <param name="Page">Page number for pagination</param>
/// <param name="PageSize">Number of items per page</param>
/// <param name="Status">Optional status filter</param>
/// <param name="Language">Language for i18n content</param>
public record GetProductsByVendorQuery(
    long VendorId,
    int Page = 1,
    int PageSize = 20,
    byte? Status = null,
    string Language = "vi"
) : IRequest<PagedResponse<ProductSummaryResponse>>;

/// <summary>
/// Query to get products by language for Vietnamese marketplace i18n support
/// </summary>
/// <param name="LanguageCode">Language code (vi/en)</param>
/// <param name="Page">Page number for pagination</param>
/// <param name="PageSize">Number of items per page</param>
/// <param name="IncludeInactive">Whether to include inactive products</param>
public record GetProductsByLanguageQuery(
    string LanguageCode,
    int Page = 1,
    int PageSize = 20,
    bool IncludeInactive = false
) : IRequest<PagedResponse<ProductSummaryResponse>>;

/// <summary>
/// Query to get products by price range for Vietnamese marketplace multi-currency support
/// </summary>
/// <param name="CurrencyCode">Currency code (VND/USD)</param>
/// <param name="MinPrice">Minimum price in specified currency</param>
/// <param name="MaxPrice">Maximum price in specified currency</param>
/// <param name="Page">Page number for pagination</param>
/// <param name="PageSize">Number of items per page</param>
/// <param name="Language">Language for i18n content</param>
public record GetProductsByPriceRangeQuery(
    string CurrencyCode,
    decimal MinPrice,
    decimal MaxPrice,
    int Page = 1,
    int PageSize = 20,
    string Language = "vi"
) : IRequest<PagedResponse<ProductSummaryResponse>>;

/// <summary>
/// Query to get products pending approval for Vietnamese marketplace workflow
/// </summary>
/// <param name="Page">Page number for pagination</param>
/// <param name="PageSize">Number of items per page</param>
/// <param name="VendorId">Optional vendor filter for admin review</param>
/// <param name="Language">Language for i18n content</param>
public record GetProductsForApprovalQuery(
    int Page = 1,
    int PageSize = 20,
    long? VendorId = null,
    string Language = "vi"
) : IRequest<PagedResponse<ProductSummaryResponse>>;

/// <summary>
/// Query to search products with Vietnamese text support
/// </summary>
/// <param name="SearchTerm">Search term with Vietnamese character support</param>
/// <param name="Language">Primary language for search</param>
/// <param name="Page">Page number for pagination</param>
/// <param name="PageSize">Number of items per page</param>
/// <param name="CategoryIds">Optional category filter</param>
/// <param name="VendorIds">Optional vendor filter</param>
/// <param name="CurrencyCode">Optional currency filter</param>
/// <param name="MinPrice">Optional minimum price filter</param>
/// <param name="MaxPrice">Optional maximum price filter</param>
public record SearchProductsQuery(
    string SearchTerm,
    string Language = "vi",
    int Page = 1,
    int PageSize = 20,
    IReadOnlyCollection<long>? CategoryIds = null,
    IReadOnlyCollection<long>? VendorIds = null,
    string? CurrencyCode = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null
) : IRequest<PagedResponse<ProductSummaryResponse>>;

/// <summary>
/// Query to get featured products for Vietnamese marketplace homepage
/// </summary>
/// <param name="Language">Language for i18n content</param>
/// <param name="Count">Number of featured products to return</param>
/// <param name="CategoryId">Optional category filter</param>
public record GetFeaturedProductsQuery(
    string Language = "vi",
    int Count = 10,
    long? CategoryId = null
) : IRequest<IEnumerable<ProductSummaryResponse>>;

/// <summary>
/// Query to get product variants for Vietnamese marketplace size/color options
/// </summary>
/// <param name="ProductId">ID of the product</param>
/// <param name="Language">Language for variant descriptions</param>
public record GetProductVariantsQuery(
    long ProductId,
    string Language = "vi"
) : IRequest<IEnumerable<ProductVariantResponse>>;

/// <summary>
/// Query to get products by VAT rate for Vietnamese tax compliance
/// </summary>
/// <param name="VatRate">VAT rate (e.g., 0.10 for 10%)</param>
/// <param name="Page">Page number for pagination</param>
/// <param name="PageSize">Number of items per page</param>
/// <param name="FromDate">Optional date range start</param>
/// <param name="ToDate">Optional date range end</param>
public record GetProductsByVatRateQuery(
    decimal VatRate,
    int Page = 1,
    int PageSize = 20,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<PagedResponse<ProductSummaryResponse>>;