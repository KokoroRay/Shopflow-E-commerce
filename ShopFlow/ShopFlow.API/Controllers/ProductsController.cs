using Microsoft.AspNetCore.Mvc;
using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Queries.Products;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Enums;
using ShopFlow.API.Extensions;

namespace ShopFlow.API.Controllers;

/// <summary>
/// Vietnamese marketplace products controller with multi-vendor support
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
internal class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the ProductsController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    public ProductsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Creates a new product in the Vietnamese marketplace
    /// </summary>
    /// <param name="request">The create product request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created product</returns>
    [HttpPost]
    [Authorize(Policy = "ProductManagement")] // Vietnamese marketplace: enhanced policy
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Note: In Phase 3, this will use request parameters directly
        // For now, using simplified version with hardcoded Vietnamese marketplace defaults
        var command = new CreateProductCommand(
            Name: request.Name,
            ShortDescription: request.ShortDescription,
            LongDescription: null,
            Slug: request.Slug,
            ProductType: request.ProductType,
            ReturnDays: request.ReturnDays,
            CategoryIds: request.CategoryIds,
            VendorId: 1, // Will get from authenticated user in Phase 3
            PrimaryLanguage: request.Language ?? "vi",
            SecondaryLanguageContent: null, // Will be implemented in Phase 3
            Pricing: new List<CreateProductPricingRequest>
            {
                new CreateProductPricingRequest(
                    CurrencyCode: "VND",
                    BasePrice: 100000, // Default 100k VND, will be from request in Phase 3
                    SalePrice: null,
                    SaleStartDate: null,
                    SaleEndDate: null,
                    MinQuantity: null,
                    MaxQuantity: null
                )
            },
            Variants: null, // Will be implemented in Phase 3
            MetaTitle: request.MetaTitle,
            MetaDescription: request.MetaDescription,
            Tags: null, // Will be implemented in Phase 3
            VatRate: 0.10m, // 10% Vietnamese VAT
            IsVatIncluded: true,
            TaxCode: null,
            AdminNotes: null,
            RequestImmediateReview: false
        );

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The product details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(long id, CancellationToken cancellationToken)
    {
        var query = new GetProductQuery(id);
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Updates an existing product in the Vietnamese marketplace
    /// </summary>
    /// <param name="id">Product ID to update</param>
    /// <param name="request">The edit product request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated product</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "ProductManagement")]
    public async Task<IActionResult> UpdateProduct(long id, [FromBody] EditProductRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // TODO: Get vendor ID from authenticated user claims
        var vendorId = 1L; // Placeholder for authentication system

        var command = new EditProductCommand(
            ProductId: id,
            Name: request.Name,
            ShortDescription: request.ShortDescription,
            LongDescription: request.LongDescription,
            ProductType: request.ProductType,
            ReturnDays: request.ReturnDays,
            VendorId: vendorId
        );

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a product (set to discontinued status)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Delete operation result</returns>
    [HttpDelete("{id:long}")]
    [Authorize(Policy = "VendorResourcePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteProduct(long id, CancellationToken cancellationToken = default)
    {
        // TODO: Extract vendor ID from claims in real implementation
        var vendorId = HttpContext.GetCurrentUserId();

        var command = new DeleteProductCommand(
            ProductId: id,
            VendorId: vendorId,
            AdminNotes: $"Product deleted by vendor {vendorId}",
            NotifyVendor: false // Don't notify vendor when they delete their own product
        );

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get paginated list of products with filtering and search
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Search term for product name</param>
    /// <param name="status">Filter by product status</param>
    /// <param name="productType">Filter by product type</param>
    /// <param name="vendorId">Filter by vendor ID</param>
    /// <param name="categoryId">Filter by category ID</param>
    /// <param name="sortBy">Sort field (Name, CreatedAt, UpdatedAt, Status)</param>
    /// <param name="sortDirection">Sort direction (asc, desc)</param>
    /// <param name="includeInactive">Include inactive products</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [AllowAnonymous] // Public endpoint for product browsing
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(PagedProductResponse))]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null,
        [FromQuery] byte? productType = null,
        [FromQuery] long? vendorId = null,
        [FromQuery] long? categoryId = null,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] string sortDirection = "desc",
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        ProductStatus? productStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProductStatus>(status, true, out var parsedStatus))
        {
            productStatus = parsedStatus;
        }

        var query = new GetProductsQuery(
            PageNumber: pageNumber,
            PageSize: pageSize,
            SearchTerm: searchTerm,
            Status: productStatus,
            ProductType: productType,
            VendorId: vendorId,
            CategoryId: categoryId,
            SortBy: sortBy,
            SortDirection: sortDirection,
            IncludeInactive: includeInactive
        );

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    #region Vietnamese Marketplace Endpoints

    /// <summary>
    /// Get products by vendor with pagination - Vietnamese marketplace multi-vendor support
    /// </summary>
    [HttpGet("vendor/{vendorId}")]
    [Authorize(Policy = "VendorResourceAccess")]
    public async Task<IActionResult> GetProductsByVendor(
        long vendorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        byte? statusByte = null;
        if (byte.TryParse(status, out var parsedStatus))
        {
            statusByte = parsedStatus;
        }

        var query = new GetProductsByVendorQuery(
            VendorId: vendorId,
            Page: page,
            PageSize: pageSize,
            Status: statusByte,
            Language: "vi"
        );

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Get products by language - Vietnamese marketplace i18n support
    /// </summary>
    [HttpGet("language/{languageCode}")]
    public async Task<IActionResult> GetProductsByLanguage(
        string languageCode,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(languageCode);

        var query = new GetProductsByLanguageQuery(
            LanguageCode: languageCode,
            Page: page,
            PageSize: pageSize,
            IncludeInactive: false
        );

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Search products with Vietnamese text support
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string searchTerm,
        [FromQuery] string? language = "vi",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(searchTerm);

        var query = new SearchProductsQuery(
            SearchTerm: searchTerm,
            Language: language ?? "vi",
            Page: page,
            PageSize: pageSize
        );

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Get products by price range in specified currency (VND/USD)
    /// </summary>
    [HttpGet("price-range")]
    public async Task<IActionResult> GetProductsByPriceRange(
        [FromQuery] string currencyCode = "VND",
        [FromQuery] decimal minPrice = 0,
        [FromQuery] decimal maxPrice = 1000000,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(currencyCode);

        var query = new GetProductsByPriceRangeQuery(
            CurrencyCode: currencyCode,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            Page: page,
            PageSize: pageSize,
            Language: "vi"
        );

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Admin endpoint: Get products pending approval
    /// </summary>
    [HttpGet("pending-approval")]
    [Authorize(Policy = "ApprovalAuthority")]
    public async Task<IActionResult> GetProductsPendingApproval(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsForApprovalQuery(
            Page: page,
            PageSize: pageSize,
            VendorId: null,
            Language: "vi"
        );

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Admin endpoint: Approve or reject product
    /// </summary>
    [HttpPut("{id}/approval")]
    [Authorize(Policy = "ApprovalAuthority")]
    public async Task<IActionResult> UpdateProductApproval(
        long id,
        [FromBody] ProductApprovalRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var command = new ApproveRejectProductCommand(
            ProductId: id,
            IsApproved: request.IsApproved,
            AdminNotes: request.AdminNotes,
            RejectionReason: request.RejectionReason,
            AdminId: HttpContext.GetRequiredUserId()
        );

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Get product variants for Vietnamese marketplace size/color options
    /// </summary>
    [HttpGet("{id}/variants")]
    public async Task<IActionResult> GetProductVariants(
        long id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductVariantsQuery(
            ProductId: id,
            Language: "vi"
        );

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Get featured products for Vietnamese marketplace homepage
    /// </summary>
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedProducts(
        [FromQuery] string? language = "vi",
        [FromQuery] int count = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFeaturedProductsQuery(
            Language: language ?? "vi",
            Count: count,
            CategoryId: null
        );

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Bulk update product statuses - Admin operation for Vietnamese marketplace
    /// </summary>
    [HttpPut("bulk-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkUpdateProductStatuses(
        [FromBody] BulkUpdateStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var command = new BulkUpdateProductsCommand(
            ProductIds: request.ProductIds,
            NewStatus: (ProductStatus)request.NewStatus,
            AdminNotes: request.AdminNotes,
            AdminId: HttpContext.GetRequiredUserId(),
            NotifyVendors: true
        );

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    #endregion

    #region Vietnamese Tax and Compliance Endpoints

    /// <summary>
    /// Get products by VAT rate for Vietnamese tax compliance
    /// </summary>
    [HttpGet("tax/vat/{vatRate}")]
    [Authorize(Policy = "TaxCompliance")]
    public async Task<IActionResult> GetProductsByVatRate(
        decimal vatRate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsByVatRateQuery(
            VatRate: vatRate,
            Page: page,
            PageSize: pageSize,
            FromDate: null,
            ToDate: null
        );

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Export products for Vietnamese tax reporting
    /// </summary>
    [HttpGet("tax/export")]
    [Authorize(Policy = "TaxCompliance")]
    public Task<IActionResult> ExportTaxReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string format = "excel",
        CancellationToken cancellationToken = default)
    {
        // Note: Export functionality will be implemented in Phase 4
        return Task.FromResult<IActionResult>(Ok(new
        {
            message = $"Vietnamese marketplace: Export tax report {format} from {fromDate} to {toDate} - to be implemented in Phase 4"
        }));
    }

    #endregion
}
