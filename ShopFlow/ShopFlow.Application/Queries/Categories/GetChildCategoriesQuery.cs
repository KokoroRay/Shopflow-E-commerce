using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Categories;

/// <summary>
/// Query for getting child categories of a parent
/// </summary>
/// <param name="ParentId">The parent category ID</param>
public record GetChildCategoriesQuery(long ParentId) : IRequest<IEnumerable<CategoryResponse>>;