using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Infrastructure.Persistence;

namespace ShopFlow.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    /// <summary>
    /// Database context
    /// </summary>
    protected readonly ShopFlowDbContext Context;

    /// <summary>
    /// Entity DbSet
    /// </summary>
    protected readonly DbSet<T> DbSet;

    /// <summary>
    /// Initializes a new instance of the Repository class
    /// </summary>
    /// <param name="context">Database context</param>
    public Repository(ShopFlowDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<T?> GetByIdAsync(long id, params Expression<Func<T, object>>[] includes)
    {
        ArgumentNullException.ThrowIfNull(includes);

        var query = DbSet.AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // Cannot assume property name; fallback to FindAsync when no predicate available.
        // For includes, we attempt key equality via EF metadata
        var key = Context.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
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
                return await query.FirstOrDefaultAsync(lambda).ConfigureAwait(false);
            }
        }
        // Fallback without includes
        return await DbSet.FindAsync(new object[] { id }).ConfigureAwait(false);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        ArgumentNullException.ThrowIfNull(includes);

        var query = DbSet.AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.Where(predicate).ToListAsync().ConfigureAwait(false);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var result = await DbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return result.Entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        return entities;
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        return await Task.FromResult(entity).ConfigureAwait(false);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public virtual async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }

    public virtual async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        var key = Context.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
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
                return await DbSet.AnyAsync(lambda, cancellationToken).ConfigureAwait(false);
            }
        }
        var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
        return entity != null;
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
    }
}
