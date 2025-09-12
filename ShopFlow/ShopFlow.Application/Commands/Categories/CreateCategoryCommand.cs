using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Commands.Categories;

/// <summary>
/// Command for creating a new category
/// </summary>
/// <param name="Name">The category name</param>
/// <param name="Slug">The category slug</param>
/// <param name="Description">The category description</param>
/// <param name="ParentId">The parent category ID (optional)</param>
/// <param name="SortOrder">The sort order</param>
/// <param name="ImageUrl">The category image URL (optional)</param>
/// <param name="IconUrl">The category icon URL (optional)</param>
/// <param name="IsActive">Whether the category is active</param>
public record CreateCategoryCommand(
    string Name,
    string? Slug,
    string? Description,
    long? ParentId,
    int SortOrder,
    string? ImageUrl,
    string? IconUrl,
    bool IsActive = true
) : IRequest<CategoryResponse>;