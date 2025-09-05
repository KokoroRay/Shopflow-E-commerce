using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
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
        await _context.SaveChangesAsync(cancellationToken);
    }

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
