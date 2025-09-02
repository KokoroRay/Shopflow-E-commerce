using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Infrastructure.Persistence;

namespace ShopFlow.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ShopFlowDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ShopFlowDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(long id, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbSet.AsQueryable();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // Cannot assume property name; fallback to FindAsync when no predicate available.
        // For includes, we attempt key equality via EF metadata
        var key = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
        if (key != null && key.Properties.Count == 1)
        {
            var keyProperty = key.Properties[0].PropertyInfo;
            if (keyProperty != null)
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var propertyAccess = Expression.Property(parameter, keyProperty);
                var idConstant = Expression.Convert(Expression.Constant(id), keyProperty.PropertyType);
                var equal = Expression.Equal(propertyAccess, idConstant);
                var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);
                return await query.FirstOrDefaultAsync(lambda);
            }
        }
        // Fallback without includes
        return await _dbSet.FindAsync(new object[] { id });
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbSet.AsQueryable();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.Where(predicate).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return await Task.FromResult(entity);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }

    public virtual async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        var key = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
        if (key != null && key.Properties.Count == 1)
        {
            var keyProperty = key.Properties[0].PropertyInfo;
            if (keyProperty != null)
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var propertyAccess = Expression.Property(parameter, keyProperty);
                var idConstant = Expression.Convert(Expression.Constant(id), keyProperty.PropertyType);
                var equal = Expression.Equal(propertyAccess, idConstant);
                var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);
                return await _dbSet.AnyAsync(lambda, cancellationToken);
            }
        }
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        return entity != null;
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }
}
