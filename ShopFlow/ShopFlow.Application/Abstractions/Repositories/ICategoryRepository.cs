using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Abstractions.Repositories;

/// <summary>
/// Repository interface for category operations
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Gets a category by its unique identifier
    /// </summary>
    /// <param name="id">The category identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The category if found, otherwise null</returns>
    Task<CatCategory?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a category by its slug
    /// </summary>
    /// <param name="slug">The category slug</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The category if found, otherwise null</returns>
    Task<CatCategory?> GetBySlugAsync(CategorySlug slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a category by its name
    /// </summary>
    /// <param name="name">The category name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The category if found, otherwise null</returns>
    Task<CatCategory?> GetByNameAsync(CategoryName name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all child categories of a parent category
    /// </summary>
    /// <param name="parentId">The parent category identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of child categories</returns>
    Task<IEnumerable<CatCategory>> GetChildrenAsync(long parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all root categories (categories without parent)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of root categories</returns>
    Task<IEnumerable<CatCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets categories with full hierarchy (parent and children)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of categories with hierarchy</returns>
    Task<IEnumerable<CatCategory>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds categories based on specification
    /// </summary>
    /// <param name="specification">The specification to apply</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of categories matching the specification</returns>
    Task<IEnumerable<CatCategory>> FindAsync(ISpecification<CatCategory> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category with the specified slug exists
    /// </summary>
    /// <param name="slug">The category slug</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if exists, otherwise false</returns>
    Task<bool> ExistsBySlugAsync(CategorySlug slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category with the specified name exists
    /// </summary>
    /// <param name="name">The category name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if exists, otherwise false</returns>
    Task<bool> ExistsByNameAsync(CategoryName name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category with the specified slug exists, excluding a specific category
    /// </summary>
    /// <param name="slug">The category slug</param>
    /// <param name="excludeId">The category ID to exclude from the check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if exists, otherwise false</returns>
    Task<bool> ExistsBySlugAsync(CategorySlug slug, long excludeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category with the specified name exists, excluding a specific category
    /// </summary>
    /// <param name="name">The category name</param>
    /// <param name="excludeId">The category ID to exclude from the check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if exists, otherwise false</returns>
    Task<bool> ExistsByNameAsync(CategoryName name, long excludeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new category
    /// </summary>
    /// <param name="category">The category to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added category</returns>
    Task<CatCategory> AddAsync(CatCategory category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="category">The category to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpdateAsync(CatCategory category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category
    /// </summary>
    /// <param name="category">The category to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task DeleteAsync(CatCategory category, CancellationToken cancellationToken = default);
}