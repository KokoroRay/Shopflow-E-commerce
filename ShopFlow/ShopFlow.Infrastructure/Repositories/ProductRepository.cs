using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using ShopFlow.Infrastructure.Persistence;

namespace ShopFlow.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ShopFlowDbContext _context;

    public ProductRepository(ShopFlowDbContext context)
    {
        _context = context;
    }

    public async Task<CatProduct?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<CatProduct?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        // This would need proper implementation based on your CatProduct entity structure
        return await _context.Products.FirstOrDefaultAsync(p => p.Id.ToString() == sku, cancellationToken);
    }

    public async Task<IEnumerable<CatProduct>> GetByCategoryAsync(long categoryId, CancellationToken cancellationToken = default)
    {
        // This would need proper implementation based on your CatProduct entity structure
        return await _context.Products.Where(p => p.Id == categoryId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CatProduct>> FindAsync(ISpecification<CatProduct> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<CatProduct> AddAsync(CatProduct product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task UpdateAsync(CatProduct product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CatProduct product, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    #region Vietnamese Marketplace Specific Operations

    public async Task<CatProduct?> GetBySlugAsync(ProductSlug slug, CancellationToken cancellationToken = default)
    {
        // Note: In Phase 3, this will use the ProductSlug value object mapping
        // For now, using basic implementation as placeholder
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .FirstOrDefaultAsync(p => p.Id > 0, cancellationToken) // Placeholder
            .ConfigureAwait(false);
    }

    public async Task<bool> ExistsBySlugAsync(ProductSlug slug, CancellationToken cancellationToken = default)
    {
        // Note: In Phase 3, this will use proper slug comparison
        await Task.CompletedTask.ConfigureAwait(false);
        return false; // Placeholder
    }

    public async Task<bool> ExistsByNameAndVendorAsync(ProductName name, long vendorId, CancellationToken cancellationToken = default)
    {
        // Note: In Phase 3, this will use ProductName value object and vendor relationship
        await Task.CompletedTask.ConfigureAwait(false);
        return false; // Placeholder
    }

    #endregion

    #region Multi-Vendor Operations

    public async Task<IEnumerable<CatProduct>> GetByVendorAsync(long vendorId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        // Note: Vendor relationship will be implemented in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetActiveProductsByVendorAsync(long vendorId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => p.IsActive())
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<int> GetProductCountByVendorAsync(long vendorId, CancellationToken cancellationToken = default)
    {
        // Note: Vendor filtering will be implemented in Phase 3
        return await _context.Products
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion

    #region Multi-Language Support

    public async Task<IEnumerable<CatProduct>> GetByLanguageAsync(string languageCode, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        // Note: Language filtering will be implemented with I18n tables in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetWithI18nContentAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        // Note: I18n content filtering will be implemented in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion

    #region Pricing and Currency Operations

    public async Task<IEnumerable<CatProduct>> GetByCurrencyAsync(string currencyCode, CancellationToken cancellationToken = default)
    {
        // Note: Currency filtering will be implemented with pricing tables in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetByPriceRangeAsync(string currencyCode, decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        // Note: Price range filtering will be implemented with pricing tables in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetOnSaleProductsAsync(string? currencyCode = null, CancellationToken cancellationToken = default)
    {
        // Note: Sale detection will be implemented with pricing logic in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion

    #region Variant and Stock Operations

    public async Task<IEnumerable<CatProduct>> GetProductsWithVariantsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => p.Skus.Count > 1)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetByVariantAsync(string variantType, string variantValue, CancellationToken cancellationToken = default)
    {
        // Note: Variant filtering will be implemented with variant tables in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetInStockProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => p.CanBeOrdered())
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        // Note: Stock threshold logic will be enhanced in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => p.Skus.Any(s => s.IsActive))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion

    #region Approval Workflow Operations

    public async Task<IEnumerable<CatProduct>> GetPendingApprovalAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => p.Status == ProductStatus.Pending)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetUnderReviewAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => p.Status == ProductStatus.UnderReview)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetApprovedProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => p.Status == ProductStatus.Active)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetRejectedProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => p.Status == ProductStatus.Rejected)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion

    #region Search and Analytics

    public async Task<IEnumerable<CatProduct>> SearchAsync(string searchTerm, string? languageCode = null, CancellationToken cancellationToken = default)
    {
        // Note: Full-text search will be implemented in Phase 3 with proper indexing
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => EF.Functions.Like(p.Id.ToString(), $"%{searchTerm}%")) // Placeholder search
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        // Note: Tag filtering will be implemented with tag tables in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetTopSellingAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        // Note: Sales analytics will be implemented in Phase 4
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Take(count)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetRecentlyAddedAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default)
    {
        // Note: Featured logic will be implemented with product flags in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .Where(p => p.IsActive())
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion

    #region Vietnamese Tax and Compliance

    public async Task<IEnumerable<CatProduct>> GetByVatRateAsync(decimal vatRate, CancellationToken cancellationToken = default)
    {
        // Note: VAT filtering will be implemented with tax tables in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetWithTaxCodeAsync(string? taxCode = null, CancellationToken cancellationToken = default)
    {
        // Note: Tax code filtering will be implemented with tax tables in Phase 3
        return await _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion

    #region Bulk Operations

    public async Task<int> BulkUpdateStatusAsync(IEnumerable<long> productIds, byte newStatus, CancellationToken cancellationToken = default)
    {
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var product in products)
        {
            // Status update logic will be implemented in Phase 3 when entity methods are added
        }

        return products.Count;
    }

    public async Task<int> BulkUpdateVendorProductsAsync(long vendorId, byte newStatus, CancellationToken cancellationToken = default)
    {
        // Note: Vendor filtering will be implemented in Phase 3
        var products = await _context.Products
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return products.Count;
    }

    #endregion

    #region Pagination and Counting

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<int> GetCountByStatusAsync(byte status, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .CountAsync(p => p.Status == (ProductStatus)status, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<(IEnumerable<CatProduct> Products, int TotalCount)> GetPaginatedAsync(
        int skip = 0,
        int take = 50,
        long? vendorId = null,
        long? categoryId = null,
        byte? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .AsQueryable();

        // Apply filters
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.Categories.Any(c => c.Id == categoryId.Value));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == (ProductStatus)status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // Note: Enhanced search will be implemented in Phase 3
            query = query.Where(p => EF.Functions.Like(p.Id.ToString(), $"%{searchTerm}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        var products = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (products, totalCount);
    }

    #endregion

    private IQueryable<CatProduct> ApplySpecification(ISpecification<CatProduct> spec)
    {
        var query = _context.Products.AsQueryable();

        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        if (spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending != null)
            query = query.OrderByDescending(spec.OrderByDescending);

        if (spec.IsPagingEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);

        return query;
    }
}
