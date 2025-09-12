using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Categories;

/// <summary>
/// Query for getting all root categories (categories without parent)
/// </summary>
public record GetRootCategoriesQuery : IRequest<IEnumerable<CategoryResponse>>;