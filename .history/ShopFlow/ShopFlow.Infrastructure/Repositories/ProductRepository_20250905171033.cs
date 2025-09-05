using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Domain.Entities;
using ShopFlow.Infrastructure.Persistence;

namespace ShopFlow.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ShopFlowDbContext _context;
    private readonly IProductMapper _mapper;

    public ProductRepository(ShopFlowDbContext context, IProductMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CatProduct?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.cat_products.FindAsync(new object[] { id }, cancellationToken);
        return dataEntity != null ? _mapper.ToDomain(dataEntity) : null;
    }

    public async Task<CatProduct?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.cat_products
            .FirstOrDefaultAsync(p => p.sku == sku, cancellationToken);
        return dataEntity != null ? _mapper.ToDomain(dataEntity) : null;
    }

    public async Task<IEnumerable<CatProduct>> GetByCategoryAsync(long categoryId, CancellationToken cancellationToken = default)
    {
        var dataEntities = await _context.cat_products
            .Where(p => p.category_id == categoryId)
            .ToListAsync(cancellationToken);
        return dataEntities.Select(_mapper.ToDomain);
    }

    public async Task<IEnumerable<CatProduct>> FindAsync(ISpecification<CatProduct> specification, CancellationToken cancellationToken = default)
    {
        // This would require converting domain specification to EF specification
        // For now, return empty collection
        await Task.CompletedTask;
        return new List<CatProduct>();
    }

    public async Task<CatProduct> AddAsync(CatProduct product, CancellationToken cancellationToken = default)
    {
        var dataEntity = _mapper.ToData(product);
        var entry = await _context.cat_products.AddAsync(dataEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return _mapper.ToDomain(entry.Entity);
    }

    public async Task UpdateAsync(CatProduct product, CancellationToken cancellationToken = default)
    {
        var existingDataEntity = await _context.cat_products.FindAsync(new object[] { product.Id }, cancellationToken);
        if (existingDataEntity != null)
        {
            _mapper.UpdateData(product, existingDataEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(CatProduct product, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.cat_products.FindAsync(new object[] { product.Id }, cancellationToken);
        if (dataEntity != null)
        {
            _context.cat_products.Remove(dataEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
