using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Infrastructure.Persistence;

namespace ShopFlow.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Category entity
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ShopFlowDbContext _context;

    /// <summary>
    /// Initializes a new instance of the CategoryRepository class
    /// </summary>
    /// <param name="context">Database context</param>
    public CategoryRepository(ShopFlowDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<CatCategory?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Translations)
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CatCategory?> GetBySlugAsync(CategorySlug slug, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Translations)
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Slug.Value == slug.Value, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CatCategory?> GetByNameAsync(CategoryName name, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Translations)
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Name.Value == name.Value, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> GetChildrenAsync(long parentId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Translations)
            .Where(c => c.ParentId == parentId && c.Status != CategoryStatus.Deleted)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Translations)
            .Include(c => c.Children.Where(child => child.Status != CategoryStatus.Deleted))
            .Where(c => c.ParentId == null && c.Status != CategoryStatus.Deleted)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Translations)
            .Include(c => c.Parent)
            .Where(c => c.Status != CategoryStatus.Deleted)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> GetHierarchyAsync(CancellationToken cancellationToken = default)
    {
        // Simply return root categories - the hierarchy will be built by the mapping layer
        return await GetRootCategoriesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default)
    {
        return await GetHierarchyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsBySlugAsync(CategorySlug slug, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AnyAsync(c => c.Slug.Value == slug.Value && c.Status != CategoryStatus.Deleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsBySlugAsync(CategorySlug slug, long excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AnyAsync(c => c.Slug.Value == slug.Value && c.Id != excludeId && c.Status != CategoryStatus.Deleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(CategoryName name, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AnyAsync(c => c.Name.Value == name.Value && c.Status != CategoryStatus.Deleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(CategoryName name, long excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AnyAsync(c => c.Name.Value == name.Value && c.Id != excludeId && c.Status != CategoryStatus.Deleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasChildrenAsync(long categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AnyAsync(c => c.ParentId == categoryId && c.Status != CategoryStatus.Deleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CatCategory> AddAsync(CatCategory category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
        return category;
    }

    /// <inheritdoc />
    public Task UpdateAsync(CatCategory category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(CatCategory category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Remove(category);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> FindAsync(ISpecification<CatCategory> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    private IQueryable<CatCategory> ApplySpecification(ISpecification<CatCategory> spec)
    {
        var query = _context.Categories.AsQueryable();

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