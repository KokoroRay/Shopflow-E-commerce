using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Infrastructure.Mappings;

/// <summary>
/// Mapper for Category entities to response DTOs
/// </summary>
public static class CategoryMapper
{
    /// <summary>
    /// Maps a CatCategory entity to CategoryResponse DTO
    /// </summary>
    /// <param name="category">The category entity to map</param>
    /// <returns>CategoryResponse DTO</returns>
    public static CategoryResponse ToResponse(CatCategory category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name.Value,
            Description = category.Description,
            Slug = category.Slug.Value,
            ParentId = category.ParentId,
            Status = (byte)category.Status,
            SortOrder = category.SortOrder,
            ImageUrl = category.ImageUrl,
            IconUrl = category.IconUrl,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    /// <summary>
    /// Maps a CatCategory entity to CategoryHierarchyResponse DTO with children
    /// </summary>
    /// <param name="category">The category entity to map</param>
    /// <returns>CategoryHierarchyResponse DTO</returns>
    public static CategoryHierarchyResponse ToHierarchyResponse(CatCategory category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        return new CategoryHierarchyResponse
        {
            Id = category.Id,
            Name = category.Name.Value,
            Description = category.Description,
            Slug = category.Slug.Value,
            ParentId = category.ParentId,
            Status = (byte)category.Status,
            SortOrder = category.SortOrder,
            ImageUrl = category.ImageUrl,
            IconUrl = category.IconUrl,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            Children = category.Children?
                .Where(c => c.Status != Domain.Enums.CategoryStatus.Deleted)
                .OrderBy(c => c.SortOrder)
                .Select(ToHierarchyResponse)
                .ToList() ?? new List<CategoryHierarchyResponse>()
        };
    }

    /// <summary>
    /// Maps a collection of CatCategory entities to CategoryListResponse DTO
    /// </summary>
    /// <param name="categories">The categories to map</param>
    /// <param name="totalCount">Total count for pagination</param>
    /// <param name="pageNumber">Current page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>CategoryListResponse DTO</returns>
    public static CategoryListResponse ToListResponse(
        IEnumerable<CatCategory> categories,
        int totalCount,
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (categories == null)
            throw new ArgumentNullException(nameof(categories));

        return new CategoryListResponse
        {
            Categories = categories.Select(ToResponse).ToList(),
            TotalCount = totalCount,
            Page = pageNumber,
            PageSize = pageSize
        };
    }
}