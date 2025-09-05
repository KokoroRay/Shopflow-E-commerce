using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task<CatProduct?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<CatProduct?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<CatProduct>> GetByCategoryAsync(long categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CatProduct>> FindAsync(ISpecification<CatProduct> specification, CancellationToken cancellationToken = default);
    Task<CatProduct> AddAsync(CatProduct product, CancellationToken cancellationToken = default);
    Task UpdateAsync(CatProduct product, CancellationToken cancellationToken = default);
    Task DeleteAsync(CatProduct product, CancellationToken cancellationToken = default);
}
