using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.Abstractions.Mappings;

/// <summary>
/// Mapper interface for converting Category entities to response DTOs
/// </summary>
public interface ICategoryMapper
{
    /// <summary>
    /// Converts a Category entity to CategoryResponse
    /// </summary>
    /// <param name="category">The category entity to convert</param>
    /// <returns>CategoryResponse DTO</returns>
    CategoryResponse ToCategoryResponse(CatCategory category);

    /// <summary>
    /// Converts a collection of Category entities to CategoryResponse list
    /// </summary>
    /// <param name="categories">The category entities to convert</param>
    /// <returns>List of CategoryResponse DTOs</returns>
    List<CategoryResponse> ToCategoryResponseList(IEnumerable<CatCategory> categories);
}