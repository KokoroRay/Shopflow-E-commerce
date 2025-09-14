using MediatR;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Queries.Products;

/// <summary>
/// Query to get paginated list of products with filtering and search - Vietnamese marketplace
/// </summary>
/// <param name="PageNumber">Page number (1-based)</param>
/// <param name="PageSize">Number of items per page</param>
/// <param name="SearchTerm">Search term for product name or description</param>
/// <param name="Status">Filter by product status</param>
/// <param name="ProductType">Filter by product type</param>
/// <param name="VendorId">Filter by vendor ID</param>
/// <param name="CategoryId">Filter by category ID</param>
/// <param name="SortBy">Sort field (Name, CreatedAt, UpdatedAt, Status)</param>
/// <param name="SortDirection">Sort direction (asc, desc)</param>
/// <param name="IncludeInactive">Include inactive products</param>
public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    ProductStatus? Status = null,
    byte? ProductType = null,
    long? VendorId = null,
    long? CategoryId = null,
    string SortBy = "CreatedAt",
    string SortDirection = "desc",
    bool IncludeInactive = false
) : IRequest<PagedProductResponse>;

/// <summary>
/// Response for paginated product list
/// </summary>
public class PagedProductResponse
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public IEnumerable<ProductResponse> Products { get; set; } = new List<ProductResponse>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}