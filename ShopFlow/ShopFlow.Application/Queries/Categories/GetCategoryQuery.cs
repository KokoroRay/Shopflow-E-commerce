using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Categories;

/// <summary>
/// Query for getting a category by ID
/// </summary>
/// <param name="Id">The category ID</param>
public record GetCategoryQuery(long Id) : IRequest<CategoryResponse?>;