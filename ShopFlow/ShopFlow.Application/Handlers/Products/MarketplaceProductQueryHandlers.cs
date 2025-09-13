using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Queries.Products;

namespace ShopFlow.Application.Handlers.Products;

/// <summary>
/// Handler for GetProductsByVendorQuery - Vietnamese marketplace multi-vendor support
/// </summary>
public class GetProductsByVendorQueryHandler : IRequestHandler<GetProductsByVendorQuery, PagedResponse<ProductSummaryResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsByVendorQueryHandler> _logger;

    public GetProductsByVendorQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductsByVendorQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductSummaryResponse>> Handle(
        GetProductsByVendorQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting products for vendor {VendorId}, page {Page}",
            request.VendorId, request.Page);

        try
        {
            var skip = (request.Page - 1) * request.PageSize;

            // Get paginated results from repository
            var (products, totalCount) = await _productRepository.GetPaginatedAsync(
                skip: skip,
                take: request.PageSize,
                vendorId: request.VendorId,
                categoryId: null,
                status: request.Status,
                searchTerm: null,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            // Map to response DTOs
            var productResponses = products.Select(p => new ProductSummaryResponse
            {
                Id = p.Id,
                Name = p.Name.Value, // Using value object
                Slug = p.Slug.Value, // Using value object
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                // Note: Additional properties will be mapped in Phase 3
                VendorId = request.VendorId, // Temporary until vendor relationship is implemented
                Language = request.Language,
                CurrencyCode = "VND", // Default currency
                BasePrice = 0 // Will be implemented with pricing in Phase 3
            });

            return new PagedResponse<ProductSummaryResponse>
            {
                Items = productResponses,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for vendor {VendorId}", request.VendorId);
            throw;
        }
    }
}

/// <summary>
/// Handler for GetProductsByLanguageQuery - Vietnamese marketplace i18n support
/// </summary>
public class GetProductsByLanguageQueryHandler : IRequestHandler<GetProductsByLanguageQuery, PagedResponse<ProductSummaryResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsByLanguageQueryHandler> _logger;

    public GetProductsByLanguageQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductsByLanguageQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductSummaryResponse>> Handle(
        GetProductsByLanguageQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting products for language {Language}, page {Page}",
            request.LanguageCode, request.Page);

        try
        {
            var skip = (request.Page - 1) * request.PageSize;

            // Get products by language from repository
            var products = await _productRepository.GetByLanguageAsync(
                languageCode: request.LanguageCode,
                skip: skip,
                take: request.PageSize,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            // Get total count for pagination
            var totalCount = await _productRepository.GetTotalCountAsync(cancellationToken).ConfigureAwait(false);

            // Map to response DTOs
            var productResponses = products.Select(p => new ProductSummaryResponse
            {
                Id = p.Id,
                Name = p.Name.Value,
                Slug = p.Slug.Value,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Language = request.LanguageCode,
                CurrencyCode = "VND",
                BasePrice = 0
            });

            return new PagedResponse<ProductSummaryResponse>
            {
                Items = productResponses,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for language {Language}", request.LanguageCode);
            throw;
        }
    }
}

/// <summary>
/// Handler for GetProductsByPriceRangeQuery - Vietnamese marketplace multi-currency support
/// </summary>
public class GetProductsByPriceRangeQueryHandler : IRequestHandler<GetProductsByPriceRangeQuery, PagedResponse<ProductSummaryResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsByPriceRangeQueryHandler> _logger;

    public GetProductsByPriceRangeQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductsByPriceRangeQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductSummaryResponse>> Handle(
        GetProductsByPriceRangeQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting products in price range {Min}-{Max} {Currency}",
            request.MinPrice, request.MaxPrice, request.CurrencyCode);

        try
        {
            // Get products by price range from repository
            var products = await _productRepository.GetByPriceRangeAsync(
                currencyCode: request.CurrencyCode,
                minPrice: request.MinPrice,
                maxPrice: request.MaxPrice,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            // Apply pagination
            var skip = (request.Page - 1) * request.PageSize;
            var paginatedProducts = products.Skip(skip).Take(request.PageSize);
            var totalCount = products.Count();

            // Map to response DTOs
            var productResponses = paginatedProducts.Select(p => new ProductSummaryResponse
            {
                Id = p.Id,
                Name = p.Name.Value,
                Slug = p.Slug.Value,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Language = request.Language,
                CurrencyCode = request.CurrencyCode,
                BasePrice = 0 // Will be implemented with pricing in Phase 3
            });

            return new PagedResponse<ProductSummaryResponse>
            {
                Items = productResponses,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products by price range {Min}-{Max} {Currency}",
                request.MinPrice, request.MaxPrice, request.CurrencyCode);
            throw;
        }
    }
}

/// <summary>
/// Handler for GetProductsForApprovalQuery - Vietnamese marketplace approval workflow
/// </summary>
public class GetProductsForApprovalQueryHandler : IRequestHandler<GetProductsForApprovalQuery, PagedResponse<ProductSummaryResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsForApprovalQueryHandler> _logger;

    public GetProductsForApprovalQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductsForApprovalQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductSummaryResponse>> Handle(
        GetProductsForApprovalQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting products pending approval, page {Page}", request.Page);

        try
        {
            // Get products pending approval
            var pendingProducts = await _productRepository.GetPendingApprovalAsync(cancellationToken).ConfigureAwait(false);

            // Filter by vendor if specified
            if (request.VendorId.HasValue)
            {
                // Note: Vendor filtering will be implemented in Phase 3
                _logger.LogInformation("Filtering by vendor {VendorId}", request.VendorId);
            }

            // Apply pagination
            var skip = (request.Page - 1) * request.PageSize;
            var paginatedProducts = pendingProducts.Skip(skip).Take(request.PageSize);
            var totalCount = pendingProducts.Count();

            // Map to response DTOs
            var productResponses = paginatedProducts.Select(p => new ProductSummaryResponse
            {
                Id = p.Id,
                Name = p.Name.Value,
                Slug = p.Slug.Value,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Language = request.Language,
                CurrencyCode = "VND",
                BasePrice = 0
            });

            return new PagedResponse<ProductSummaryResponse>
            {
                Items = productResponses,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for approval");
            throw;
        }
    }
}