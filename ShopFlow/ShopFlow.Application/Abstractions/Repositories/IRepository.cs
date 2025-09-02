using System.Linq.Expressions;

namespace ShopFlow.Application.Abstractions.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    
    Task<T?> GetByIdAsync(long id, params Expression<Func<T, object>>[] includes);
    
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
