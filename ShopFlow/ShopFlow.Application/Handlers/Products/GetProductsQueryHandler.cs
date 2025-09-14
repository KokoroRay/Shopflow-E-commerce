using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Queries.Products;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Handlers.Products;

/// <summary>
/// Handler for GetProductsQuery - Vietnamese marketplace product listing with pagination and filtering
/// </summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedProductResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting products - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
            request.PageNumber, request.PageSize, request.SearchTerm);

        try
        {
            // Validate pagination parameters
            if (request.PageNumber < 1)
            {
                return new PagedProductResponse
                {
                    Success = false,
                    Message = "Page number must be greater than 0"
                };
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return new PagedProductResponse
                {
                    Success = false,
                    Message = "Page size must be between 1 and 100"
                };
            }

            // Calculate pagination
            var skip = (request.PageNumber - 1) * request.PageSize;
            var take = request.PageSize;

            // Determine status filter
            byte? statusFilter = null;
            if (request.Status.HasValue)
            {
                statusFilter = (byte)request.Status.Value;
            }
            else if (!request.IncludeInactive)
            {
                statusFilter = (byte)ProductStatus.Active;
            }

            // Get products with repository pagination and basic filtering
            var (products, totalCount) = await _productRepository.GetPaginatedAsync(
                skip: skip,
                take: take,
                vendorId: request.VendorId,
                categoryId: request.CategoryId,
                status: statusFilter,
                searchTerm: request.SearchTerm,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var productList = products.ToList();

            // Apply additional filtering that's not in repository yet
            var filteredProducts = productList.AsQueryable();

            // Filter by product type (if not handled by repository)
            if (request.ProductType.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.ProductType == request.ProductType.Value);
            }

            // Apply sorting
            filteredProducts = ApplySorting(filteredProducts, request.SortBy, request.SortDirection);

            var pagedProducts = filteredProducts.ToList();

            // Calculate pagination info
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var hasPreviousPage = request.PageNumber > 1;
            var hasNextPage = request.PageNumber < totalPages;

            // Map to response
            var productResponses = pagedProducts.Select(p => new ProductResponse
            {
                Success = true,
                Id = p.Id,
                Name = p.Name.Value,
                Slug = p.Slug.Value,
                Status = (byte)p.Status,
                ProductType = p.ProductType,
                ReturnDays = p.ReturnDays,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                VendorId = 1, // Placeholder - will be implemented in Phase 3
                PrimaryLanguage = "vi"
            }).ToList();

            _logger.LogInformation("Retrieved {Count} products out of {TotalCount} total",
                productResponses.Count, totalCount);

            return new PagedProductResponse
            {
                Success = true,
                Products = productResponses,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return new PagedProductResponse
            {
                Success = false,
                Message = "An error occurred while retrieving products"
            };
        }
    }

    private static IQueryable<Domain.Entities.CatProduct> ApplySorting(
        IQueryable<Domain.Entities.CatProduct> query,
        string sortBy,
        string sortDirection)
    {
        var isDescending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToUpperInvariant() switch
        {
            "NAME" => isDescending 
                ? query.OrderByDescending(p => p.Name.Value)
                : query.OrderBy(p => p.Name.Value),
            "STATUS" => isDescending
                ? query.OrderByDescending(p => p.Status)
                : query.OrderBy(p => p.Status),
            "PRODUCTTYPE" => isDescending
                ? query.OrderByDescending(p => p.ProductType)
                : query.OrderBy(p => p.ProductType),
            "UPDATEDAT" => isDescending
                ? query.OrderByDescending(p => p.UpdatedAt)
                : query.OrderBy(p => p.UpdatedAt),
            "CREATEDAT" or _ => isDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt)
        };
    }
}