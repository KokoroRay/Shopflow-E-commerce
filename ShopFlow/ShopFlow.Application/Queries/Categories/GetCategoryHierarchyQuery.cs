using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Categories;

/// <summary>
/// Query for getting category hierarchy tree
/// </summary>
public record GetCategoryHierarchyQuery : IRequest<IEnumerable<CategoryHierarchyResponse>>;