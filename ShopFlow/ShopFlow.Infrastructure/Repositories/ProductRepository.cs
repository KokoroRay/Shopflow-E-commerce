using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using ShopFlow.Infrastructure.Persistence;
using ShopFlow.Application.Abstractions.Mappings;
using PersistenceEntities = ShopFlow.Infrastructure.Persistence.Entities;
using System.Linq;

namespace ShopFlow.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ShopFlowDbContext _context;
    private readonly IDomainPersistenceMapper<CatProduct, PersistenceEntities.CatProduct> _productMapper;

    public ProductRepository(ShopFlowDbContext context, IDomainPersistenceMapper<CatProduct, PersistenceEntities.CatProduct> productMapper)
    {
        _context = context;
        _productMapper = productMapper;
    }

    public async Task<CatProduct?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatProducts.FindAsync(new object[] { id }, cancellationToken);
        return _productMapper.ToDomain(dataEntity);
    }

    public async Task<CatProduct?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatProducts.FirstOrDefaultAsync(p => p.Skus.Any(s => s.SkuCode == sku), cancellationToken);
        return _productMapper.ToDomain(dataEntity);
    }

    public async Task<IEnumerable<CatProduct>> GetByCategoryAsync(long categoryId, CancellationToken cancellationToken = default)
    {
        var dataEntities = await _context.CatProducts.Where(p => p.Categories.Any(c => c.Id == categoryId)).ToListAsync(cancellationToken);
        return dataEntities.Select(de => _productMapper.ToDomain(de)).ToList();
    }

    public async Task<CatProduct> AddAsync(CatProduct product, CancellationToken cancellationToken = default)
    {
        var dataEntity = _productMapper.ToPersistence(product);
        await _context.CatProducts.AddAsync(dataEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return _productMapper.ToDomain(dataEntity);
    }

    public async Task UpdateAsync(CatProduct product, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatProducts.FindAsync(new object[] { product.Id }, cancellationToken);
        if (dataEntity == null)
        {
            throw new InvalidOperationException($"Product with ID {product.Id} not found for update.");
        }
        _productMapper.UpdatePersistence(product, dataEntity);
        _context.CatProducts.Update(dataEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CatProduct product, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatProducts.FindAsync(new object[] { product.Id }, cancellationToken);
        if (dataEntity == null)
        {
            return;
        }
        _context.CatProducts.Remove(dataEntity);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<CatProduct?> GetBySlugAsync(ProductSlug slug, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatProducts
            .Include(p => p.Categories)
            .Include(p => p.Skus)
            .FirstOrDefaultAsync(p => p.Slug == slug.Value, cancellationToken)
            .ConfigureAwait(false);
        return _productMapper.ToDomain(dataEntity);
    }

    public async Task<bool> ExistsBySlugAsync(ProductSlug slug, CancellationToken cancellationToken = default)
    {
        return await _context.CatProducts.AnyAsync(p => p.Slug == slug.Value, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsByNameAndVendorAsync(ProductName name, long vendorId, CancellationToken cancellationToken = default)
    {
        return await _context.CatProducts.AnyAsync(p => p.Name == name.Value, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatProduct>> GetByVendorAsync(long vendorId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetActiveProductsByVendorAsync(long vendorId, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<int> GetProductCountByVendorAsync(long vendorId, CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<IEnumerable<CatProduct>> GetByLanguageAsync(string languageCode, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetWithI18nContentAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetByCurrencyAsync(string currencyCode, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetByPriceRangeAsync(string currencyCode, decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetOnSaleProductsAsync(string? currencyCode = null, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetProductsWithVariantsAsync(CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetByVariantAsync(string variantType, string variantValue, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetInStockProductsAsync(CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetPendingApprovalAsync(CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetUnderReviewAsync(CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetApprovedProductsAsync(CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetRejectedProductsAsync(CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> SearchAsync(string searchTerm, string? languageCode = null, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetTopSellingAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetRecentlyAddedAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetByVatRateAsync(decimal vatRate, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<IEnumerable<CatProduct>> GetWithTaxCodeAsync(string? taxCode = null, CancellationToken cancellationToken = default)
    {
        return new List<CatProduct>();
    }

    public async Task<int> BulkUpdateStatusAsync(IEnumerable<long> productIds, byte newStatus, CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<int> BulkUpdateVendorProductsAsync(long vendorId, byte newStatus, CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<int> GetCountByStatusAsync(byte status, CancellationToken cancellationToken = default)
    {
        return 0;
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
        return (new List<CatProduct>(), 0);
    }
}

