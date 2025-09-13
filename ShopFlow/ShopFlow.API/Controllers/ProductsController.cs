using Microsoft.AspNetCore.Mvc;
using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Queries.Products;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "ProductManagement")] // Vietnamese marketplace: enhanced policy
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(long id, CancellationToken cancellationToken)
    {
        var query = new GetProductQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
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
        // Note: Will implement GetProductsByVendorQuery in Phase 6
        return Ok(new { message = "Vietnamese marketplace: Get products by vendor - to be implemented in Phase 6" });
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
        // Note: Will implement GetProductsByLanguageQuery in Phase 6
        return Ok(new { message = $"Vietnamese marketplace: Get products in {languageCode} - to be implemented in Phase 6" });
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
        // Note: Will implement SearchProductsQuery in Phase 6
        return Ok(new { message = $"Vietnamese marketplace: Search '{searchTerm}' in {language} - to be implemented in Phase 6" });
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
        // Note: Will implement GetProductsByPriceRangeQuery in Phase 6
        return Ok(new { message = $"Vietnamese marketplace: Get products {minPrice}-{maxPrice} {currencyCode} - to be implemented in Phase 6" });
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
        // Note: Will implement GetProductsPendingApprovalQuery in Phase 6
        return Ok(new { message = "Vietnamese marketplace: Get products pending approval - to be implemented in Phase 6" });
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
        // Note: Will implement ApproveRejectProductCommand in Phase 7
        return Ok(new { message = $"Vietnamese marketplace: Update approval for product {id} - to be implemented in Phase 7" });
    }

    /// <summary>
    /// Get product variants for Vietnamese marketplace size/color options
    /// </summary>
    [HttpGet("{id}/variants")]
    public async Task<IActionResult> GetProductVariants(
        long id,
        CancellationToken cancellationToken = default)
    {
        // Note: Will implement GetProductVariantsQuery in Phase 6
        return Ok(new { message = $"Vietnamese marketplace: Get variants for product {id} - to be implemented in Phase 6" });
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
        // Note: Will implement GetFeaturedProductsQuery in Phase 6
        return Ok(new { message = $"Vietnamese marketplace: Get {count} featured products in {language} - to be implemented in Phase 6" });
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
        // Note: Will implement BulkUpdateProductsCommand in Phase 7
        return Ok(new { message = $"Vietnamese marketplace: Bulk update {request.ProductIds.Count} products - to be implemented in Phase 7" });
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
        // Note: Will implement GetProductsByVatRateQuery in Phase 6
        return Ok(new { message = $"Vietnamese marketplace: Get products with {vatRate}% VAT - to be implemented in Phase 6" });
    }

    /// <summary>
    /// Export products for Vietnamese tax reporting
    /// </summary>
    [HttpGet("tax/export")]
    [Authorize(Policy = "TaxCompliance")]
    public async Task<IActionResult> ExportTaxReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string format = "excel",
        CancellationToken cancellationToken = default)
    {
        // Note: Will implement ExportTaxReportQuery in Phase 6
        return Ok(new { message = $"Vietnamese marketplace: Export tax report {format} - to be implemented in Phase 6" });
    }

    #endregion
}
