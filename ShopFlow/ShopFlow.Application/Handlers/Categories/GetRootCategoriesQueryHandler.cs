using MediatR;
using ShopFlow.Application.Queries.Categories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Handlers.Categories;

/// <summary>
/// Handler for getting root categories
/// </summary>
public class GetRootCategoriesQueryHandler : IRequestHandler<GetRootCategoriesQuery, IEnumerable<CategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetRootCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryResponse>> Handle(GetRootCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetRootCategoriesAsync(cancellationToken);

        return categories.Select(category => new CategoryResponse
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
        }).OrderBy(c => c.SortOrder).ThenBy(c => c.Name);
    }
}