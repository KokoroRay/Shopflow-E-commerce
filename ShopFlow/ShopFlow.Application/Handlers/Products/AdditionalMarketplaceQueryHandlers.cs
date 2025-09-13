using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Queries.Products;

namespace ShopFlow.Application.Handlers.Products;

/// <summary>
/// Handler for SearchProductsQuery - Vietnamese marketplace search with text support
/// </summary>
public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PagedResponse<ProductSummaryResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SearchProductsQueryHandler> _logger;

    public SearchProductsQueryHandler(
        IProductRepository productRepository,
        ILogger<SearchProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductSummaryResponse>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching products with term '{SearchTerm}' in {Language}",
            request.SearchTerm, request.Language);

        try
        {
            // Search products with Vietnamese text support
            var products = await _productRepository.SearchAsync(
                searchTerm: request.SearchTerm,
                languageCode: request.Language,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            // Apply additional filters if specified
            if (request.CategoryIds?.Any() == true)
            {
                // Note: Category filtering will be enhanced in Phase 3
                _logger.LogInformation("Filtering by categories: {Categories}",
                    string.Join(", ", request.CategoryIds));
            }

            if (request.VendorIds?.Any() == true)
            {
                // Note: Vendor filtering will be enhanced in Phase 3
                _logger.LogInformation("Filtering by vendors: {Vendors}",
                    string.Join(", ", request.VendorIds));
            }

            // Apply price filtering if specified
            if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
            {
                _logger.LogInformation("Applying price filter: {Min}-{Max} {Currency}",
                    request.MinPrice, request.MaxPrice, request.CurrencyCode);
            }

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
                CurrencyCode = request.CurrencyCode ?? "VND",
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
            _logger.LogError(ex, "Error searching products with term '{SearchTerm}'", request.SearchTerm);
            throw;
        }
    }
}

/// <summary>
/// Handler for GetFeaturedProductsQuery - Vietnamese marketplace homepage
/// </summary>
public class GetFeaturedProductsQueryHandler : IRequestHandler<GetFeaturedProductsQuery, IEnumerable<ProductSummaryResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetFeaturedProductsQueryHandler> _logger;

    public GetFeaturedProductsQueryHandler(
        IProductRepository productRepository,
        ILogger<GetFeaturedProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductSummaryResponse>> Handle(
        GetFeaturedProductsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting {Count} featured products in {Language}",
            request.Count, request.Language);

        try
        {
            // Get featured products
            var products = await _productRepository.GetFeaturedProductsAsync(cancellationToken).ConfigureAwait(false);

            // Apply category filter if specified
            if (request.CategoryId.HasValue)
            {
                // Note: Category filtering will be enhanced in Phase 3
                _logger.LogInformation("Filtering featured products by category {CategoryId}", request.CategoryId);
            }

            // Take only requested count
            var featuredProducts = products.Take(request.Count);

            // Map to response DTOs
            var productResponses = featuredProducts.Select(p => new ProductSummaryResponse
            {
                Id = p.Id,
                Name = p.Name.Value,
                Slug = p.Slug.Value,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Language = request.Language,
                CurrencyCode = "VND",
                BasePrice = 0,
                IsFeatured = true // Mark as featured
            });

            return productResponses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured products");
            throw;
        }
    }
}

/// <summary>
/// Handler for GetProductVariantsQuery - Vietnamese marketplace variants
/// </summary>
public class GetProductVariantsQueryHandler : IRequestHandler<GetProductVariantsQuery, IEnumerable<ProductVariantResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductVariantsQueryHandler> _logger;

    public GetProductVariantsQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductVariantsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductVariantResponse>> Handle(
        GetProductVariantsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting variants for product {ProductId} in {Language}",
            request.ProductId, request.Language);

        try
        {
            // Get product to check if it exists
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).ConfigureAwait(false);

            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found", request.ProductId);
                return Enumerable.Empty<ProductVariantResponse>();
            }

            // Get products with variants (placeholder implementation)
            var productsWithVariants = await _productRepository.GetProductsWithVariantsAsync(cancellationToken).ConfigureAwait(false);

            // Check if our product has variants
            var hasVariants = productsWithVariants.Any(p => p.Id == request.ProductId);

            if (!hasVariants)
            {
                _logger.LogInformation("Product {ProductId} has no variants", request.ProductId);
                return Enumerable.Empty<ProductVariantResponse>();
            }

            // Note: In Phase 3, this will return actual variant data from variant tables
            // For now, returning placeholder variants
            var variantResponses = new List<ProductVariantResponse>
            {
                new ProductVariantResponse
                {
                    Id = 1,
                    ProductId = request.ProductId,
                    VariantType = "Size",
                    VariantValue = "L",
                    Sku = $"SKU-{request.ProductId}-L",
                    PriceAdjustment = 0,
                    IsAvailable = true,
                    Language = request.Language
                },
                new ProductVariantResponse
                {
                    Id = 2,
                    ProductId = request.ProductId,
                    VariantType = "Color",
                    VariantValue = "Red",
                    Sku = $"SKU-{request.ProductId}-RED",
                    PriceAdjustment = 5000, // 5k VND extra for red color
                    IsAvailable = true,
                    Language = request.Language
                }
            };

            return variantResponses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting variants for product {ProductId}", request.ProductId);
            throw;
        }
    }
}

/// <summary>
/// Handler for GetProductsByVatRateQuery - Vietnamese tax compliance
/// </summary>
public class GetProductsByVatRateQueryHandler : IRequestHandler<GetProductsByVatRateQuery, PagedResponse<ProductSummaryResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsByVatRateQueryHandler> _logger;

    public GetProductsByVatRateQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductsByVatRateQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductSummaryResponse>> Handle(
        GetProductsByVatRateQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting products with VAT rate {VatRate}%, page {Page}",
            request.VatRate * 100, request.Page);

        try
        {
            // Get products by VAT rate
            var products = await _productRepository.GetByVatRateAsync(
                vatRate: request.VatRate,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            // Apply date filtering if specified
            if (request.FromDate.HasValue || request.ToDate.HasValue)
            {
                _logger.LogInformation("Applying date filter: {From} to {To}",
                    request.FromDate, request.ToDate);

                // Note: Date filtering will be enhanced in Phase 3
                if (request.FromDate.HasValue)
                {
                    products = products.Where(p => p.CreatedAt >= request.FromDate.Value);
                }

                if (request.ToDate.HasValue)
                {
                    products = products.Where(p => p.CreatedAt <= request.ToDate.Value);
                }
            }

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
                Language = "vi",
                CurrencyCode = "VND",
                BasePrice = 0,
                VatRate = request.VatRate
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
            _logger.LogError(ex, "Error getting products by VAT rate {VatRate}", request.VatRate);
            throw;
        }
    }
}