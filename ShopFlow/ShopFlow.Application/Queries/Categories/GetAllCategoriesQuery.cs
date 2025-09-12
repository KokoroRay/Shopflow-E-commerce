using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Categories;

/// <summary>
/// Query for getting all categories with pagination and filtering
/// </summary>
/// <param name="Page">Page number (default: 1)</param>
/// <param name="PageSize">Page size (default: 10)</param>
/// <param name="SearchTerm">Search term for name filtering</param>
/// <param name="ParentId">Filter by parent category ID</param>
/// <param name="IsActive">Filter by active status</param>
/// <param name="SortBy">Sort field (name, createdAt, sortOrder)</param>
/// <param name="SortDirection">Sort direction (asc, desc)</param>
public record GetAllCategoriesQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    long? ParentId = null,
    bool? IsActive = null,
    string SortBy = "sortOrder",
    string SortDirection = "asc"
) : IRequest<CategoryListResponse>;