using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Abstractions.Repositories;

/// <summary>
/// Repository interface for product operations with Vietnamese marketplace support
/// Includes multi-vendor, multi-language, and multi-currency capabilities
/// </summary>
public interface IProductRepository
{
    // Basic CRUD Operations
    /// <summary>Gets a product by its unique identifier</summary>
    Task<CatProduct?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>Gets a product by SKU code</summary>
    Task<CatProduct?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>Gets products by category</summary>
    Task<IEnumerable<CatProduct>> GetByCategoryAsync(long categoryId, CancellationToken cancellationToken = default);

    

    /// <summary>Adds a new product</summary>
    Task<CatProduct> AddAsync(CatProduct product, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing product</summary>
    Task UpdateAsync(CatProduct product, CancellationToken cancellationToken = default);

    /// <summary>Deletes a product</summary>
    Task DeleteAsync(CatProduct product, CancellationToken cancellationToken = default);

    // Vietnamese Marketplace Specific Operations

    /// <summary>Gets a product by slug (SEO-friendly URL)</summary>
    Task<CatProduct?> GetBySlugAsync(ProductSlug slug, CancellationToken cancellationToken = default);

    /// <summary>Checks if a product slug already exists (for uniqueness validation)</summary>
    Task<bool> ExistsBySlugAsync(ProductSlug slug, CancellationToken cancellationToken = default);

    /// <summary>Checks if a product name exists for a specific vendor</summary>
    Task<bool> ExistsByNameAndVendorAsync(ProductName name, long vendorId, CancellationToken cancellationToken = default);

    // Multi-Vendor Operations

    /// <summary>Gets all products for a specific vendor with pagination</summary>
    Task<IEnumerable<CatProduct>> GetByVendorAsync(long vendorId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>Gets active products for a vendor (published and approved)</summary>
    Task<IEnumerable<CatProduct>> GetActiveProductsByVendorAsync(long vendorId, CancellationToken cancellationToken = default);

    /// <summary>Gets product count for a vendor</summary>
    Task<int> GetProductCountByVendorAsync(long vendorId, CancellationToken cancellationToken = default);

    // Multi-Language Support

    /// <summary>Gets products by language code (vi or en)</summary>
    Task<IEnumerable<CatProduct>> GetByLanguageAsync(string languageCode, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>Gets products that have content in specific language</summary>
    Task<IEnumerable<CatProduct>> GetWithI18nContentAsync(string languageCode, CancellationToken cancellationToken = default);

    // Pricing and Currency Operations

    /// <summary>Gets products with pricing in specific currency (VND or USD)</summary>
    Task<IEnumerable<CatProduct>> GetByCurrencyAsync(string currencyCode, CancellationToken cancellationToken = default);

    /// <summary>Gets products within price range for specific currency</summary>
    Task<IEnumerable<CatProduct>> GetByPriceRangeAsync(string currencyCode, decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);

    /// <summary>Gets products currently on sale</summary>
    Task<IEnumerable<CatProduct>> GetOnSaleProductsAsync(string? currencyCode = null, CancellationToken cancellationToken = default);

    // Variant and Stock Operations

    /// <summary>Gets products with variants (size, color, etc.)</summary>
    Task<IEnumerable<CatProduct>> GetProductsWithVariantsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets products with specific variant type and value</summary>
    Task<IEnumerable<CatProduct>> GetByVariantAsync(string variantType, string variantValue, CancellationToken cancellationToken = default);

    /// <summary>Gets products that are in stock</summary>
    Task<IEnumerable<CatProduct>> GetInStockProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets products with low stock (below threshold)</summary>
    Task<IEnumerable<CatProduct>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);

    // Approval Workflow Operations

    /// <summary>Gets products pending approval</summary>
    Task<IEnumerable<CatProduct>> GetPendingApprovalAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets products under review</summary>
    Task<IEnumerable<CatProduct>> GetUnderReviewAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets approved products</summary>
    Task<IEnumerable<CatProduct>> GetApprovedProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets rejected products</summary>
    Task<IEnumerable<CatProduct>> GetRejectedProductsAsync(CancellationToken cancellationToken = default);

    // Search and Analytics

    /// <summary>Searches products by name, description, or tags</summary>
    Task<IEnumerable<CatProduct>> SearchAsync(string searchTerm, string? languageCode = null, CancellationToken cancellationToken = default);

    /// <summary>Gets products by tags</summary>
    Task<IEnumerable<CatProduct>> GetByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);

    /// <summary>Gets top-selling products</summary>
    Task<IEnumerable<CatProduct>> GetTopSellingAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>Gets recently added products</summary>
    Task<IEnumerable<CatProduct>> GetRecentlyAddedAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>Gets featured products</summary>
    Task<IEnumerable<CatProduct>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default);

    // Vietnamese Tax and Compliance

    /// <summary>Gets products with specific VAT rate</summary>
    Task<IEnumerable<CatProduct>> GetByVatRateAsync(decimal vatRate, CancellationToken cancellationToken = default);

    /// <summary>Gets products with tax codes (for Vietnamese compliance)</summary>
    Task<IEnumerable<CatProduct>> GetWithTaxCodeAsync(string? taxCode = null, CancellationToken cancellationToken = default);

    // Bulk Operations

    /// <summary>Bulk updates product status</summary>
    Task<int> BulkUpdateStatusAsync(IEnumerable<long> productIds, byte newStatus, CancellationToken cancellationToken = default);

    /// <summary>Bulk updates vendor products</summary>
    Task<int> BulkUpdateVendorProductsAsync(long vendorId, byte newStatus, CancellationToken cancellationToken = default);

    // Pagination and Counting

    /// <summary>Gets total product count</summary>
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets product count by status</summary>
    Task<int> GetCountByStatusAsync(byte status, CancellationToken cancellationToken = default);

    /// <summary>Gets paginated products with optional filtering</summary>
    Task<(IEnumerable<CatProduct> Products, int TotalCount)> GetPaginatedAsync(
        int skip = 0,
        int take = 50,
        long? vendorId = null,
        long? categoryId = null,
        byte? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}
