using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Categories;

/// <summary>
/// Query for getting a category by slug
/// </summary>
/// <param name="Slug">The category slug</param>
public record GetCategoryBySlugQuery(string Slug) : IRequest<CategoryResponse?>;