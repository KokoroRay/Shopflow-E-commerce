using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Infrastructure.Persistence;
using ShopFlow.Application.Abstractions.Mappings;
using PersistenceEntities = ShopFlow.Infrastructure.Persistence.Entities;
using System.Linq.Expressions;

namespace ShopFlow.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Category entity
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ShopFlowDbContext _context;
    private readonly IDomainPersistenceMapper<CatCategory, PersistenceEntities.CatCategory> _categoryMapper;

    /// <summary>
    /// Initializes a new instance of the CategoryRepository class
    /// </summary>
    /// <param name="context">Database context</param>
    public CategoryRepository(ShopFlowDbContext context, IDomainPersistenceMapper<CatCategory, PersistenceEntities.CatCategory> categoryMapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _categoryMapper = categoryMapper;
    }

    /// <inheritdoc />
    public async Task<CatCategory?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatCategories
            .Include(c => c.Translations)
            .Include(c => c.Parent)
            .Include(c => c.InverseParent)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return _categoryMapper.ToDomain(dataEntity);
    }

    /// <inheritdoc />
    public async Task<CatCategory?> GetBySlugAsync(CategorySlug slug, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatCategories
            .Include(c => c.Translations)
            .Include(c => c.Parent)
            .Include(c => c.InverseParent)
            .FirstOrDefaultAsync(c => c.Slug == slug.Value, cancellationToken);
        return _categoryMapper.ToDomain(dataEntity);
    }

    /// <inheritdoc />
    public async Task<CatCategory?> GetByNameAsync(CategoryName name, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatCategories
            .Include(c => c.Translations)
            .Include(c => c.Parent)
            .Include(c => c.InverseParent)
            .FirstOrDefaultAsync(c => c.Name == name.Value, cancellationToken);
        return _categoryMapper.ToDomain(dataEntity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> GetChildrenAsync(long parentId, CancellationToken cancellationToken = default)
    {
        var dataEntities = await _context.CatCategories
            .Include(c => c.Translations)
            .Where(c => c.ParentId == parentId && c.IsActive)
            .OrderBy(c => c.Sort)
            .ToListAsync(cancellationToken);
        return dataEntities.Select(de => _categoryMapper.ToDomain(de)).ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var dataEntities = await _context.CatCategories
            .Include(c => c.Translations)
            .Include(c => c.InverseParent.Where(child => child.IsActive))
            .Where(c => c.ParentId == null && c.IsActive)
            .OrderBy(c => c.Sort)
            .ToListAsync(cancellationToken);
        return dataEntities.Select(de => _categoryMapper.ToDomain(de)).ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dataEntities = await _context.CatCategories
            .Include(c => c.Translations)
            .Include(c => c.Parent)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Sort)
            .ToListAsync(cancellationToken);
        return dataEntities.Select(de => _categoryMapper.ToDomain(de)).ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatCategory>> GetHierarchyAsync(CancellationToken cancellationToken = default)
    {
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
        return await _context.CatCategories
            .AnyAsync(c => c.Slug == slug.Value && c.IsActive, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsBySlugAsync(CategorySlug slug, long excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.CatCategories
            .AnyAsync(c => c.Slug == slug.Value && c.Id != excludeId && c.IsActive, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(CategoryName name, CancellationToken cancellationToken = default)
    {
        return await _context.CatCategories
            .AnyAsync(c => c.Name == name.Value && c.IsActive, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(CategoryName name, long excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.CatCategories
            .AnyAsync(c => c.Name == name.Value && c.Id != excludeId && c.IsActive, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasChildrenAsync(long categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.CatCategories
            .AnyAsync(c => c.ParentId == categoryId && c.IsActive, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CatCategory> AddAsync(CatCategory category, CancellationToken cancellationToken = default)
    {
        var dataEntity = _categoryMapper.ToPersistence(category);
        await _context.CatCategories.AddAsync(dataEntity, cancellationToken);
        return _categoryMapper.ToDomain(dataEntity);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(CatCategory category, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatCategories.FindAsync(new object[] { category.Id }, cancellationToken);
        if (dataEntity == null)
        {
            throw new InvalidOperationException($"Category with ID {category.Id} not found for update.");
        }
        _categoryMapper.UpdatePersistence(category, dataEntity);
        _context.CatCategories.Update(dataEntity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CatCategory category, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CatCategories.FindAsync(new object[] { category.Id }, cancellationToken);
        if (dataEntity == null)
        {
            return;
        }
        _context.CatCategories.Remove(dataEntity);
    }
}
