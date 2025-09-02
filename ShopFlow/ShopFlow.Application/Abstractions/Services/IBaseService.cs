using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.Abstractions.Services;

public interface IBaseService<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
}
