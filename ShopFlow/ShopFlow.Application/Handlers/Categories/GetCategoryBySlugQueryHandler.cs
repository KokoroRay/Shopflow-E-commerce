using MediatR;
using ShopFlow.Application.Queries.Categories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Handlers.Categories;

/// <summary>
/// Handler for getting a category by slug
/// </summary>
public class GetCategoryBySlugQueryHandler : IRequestHandler<GetCategoryBySlugQuery, CategoryResponse?>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryBySlugQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponse?> Handle(GetCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        var categorySlug = new CategorySlug(request.Slug);
        var category = await _categoryRepository.GetBySlugAsync(categorySlug, cancellationToken);
        if (category == null)
        {
            return null;
        }

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name.Value,
            Slug = category.Slug.Value,
            Description = category.Description,
            ParentId = category.ParentId,
            SortOrder = category.SortOrder,
            ImageUrl = category.ImageUrl,
            IconUrl = category.IconUrl,
            Status = (byte)category.Status,
            IsActive = category.Status == CategoryStatus.Active,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}