using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ShopFlow.Application.Commands.Skus;
using ShopFlow.Application.Queries.Skus;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.Contracts.Response;
using System.ComponentModel.DataAnnotations;

namespace ShopFlow.API.Controllers;

/// <summary>
/// SKU management controller for Vietnamese marketplace
/// Handles CRUD operations, inventory tracking, and marketplace compliance
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/skus")]
[Authorize]
public class SkusController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SkusController> _logger;

    public SkusController(IMediator mediator, ILogger<SkusController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of SKUs with filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetSkusResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    public async Task<ActionResult<GetSkusResponse>> GetSkus(
        [FromQuery] long? productId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDirection = "asc",
        [FromQuery] bool includeRelated = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSkusQuery
        {
            ProductId = productId,
            IsActive = isActive,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = Math.Min(pageSize, 100),
            SortBy = sortBy,
            SortDirection = sortDirection,
            IncludeRelated = includeRelated
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get SKU by ID
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GetSkuResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<GetSkuResponse>> GetSku(
        long id,
        [FromQuery] bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSkuByIdQuery(id)
        {
            IncludeRelated = includeRelated
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get SKU by code
    /// </summary>
    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(GetSkuResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<GetSkuResponse>> GetSkuByCode(
        string code,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSkuByCodeQuery(code);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create new SKU
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateSkuResponse), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<ActionResult<CreateSkuResponse>> CreateSku(
        [FromBody] CreateSkuRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateSkuCommand
        {
            Code = request.Code,
            Barcode = request.Barcode,
            BarcodeType = request.BarcodeType,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CompareAtPrice = request.CompareAtPrice,
            CostPrice = request.CostPrice,
            IsActive = request.IsActive,
            ProductId = request.ProductId,
            LengthMm = request.Dimensions.LengthMm,
            WidthMm = request.Dimensions.WidthMm,
            HeightMm = request.Dimensions.HeightMm,
            WeightGrams = request.Weight.Grams
        };

        // Add option values
        foreach (var optionValue in request.OptionValues)
        {
            command.OptionValues.Add(new CreateSkuOptionValueCommand
            {
                AttributeId = optionValue.AttributeId,
                OptionId = optionValue.OptionId
            });
        }

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetSku), new { id = result.Sku.Id }, result);
    }

    /// <summary>
    /// Update existing SKU
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(UpdateSkuResponse), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<UpdateSkuResponse>> UpdateSku(
        long id,
        [FromBody] UpdateSkuRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateSkuCommand
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CompareAtPrice = request.CompareAtPrice,
            CostPrice = request.CostPrice,
            IsActive = request.IsActive
        };

        if (request.Dimensions != null)
        {
            command.LengthMm = request.Dimensions.LengthMm;
            command.WidthMm = request.Dimensions.WidthMm;
            command.HeightMm = request.Dimensions.HeightMm;
        }

        if (request.Weight != null)
        {
            command.WeightGrams = request.Weight.Grams;
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete SKU if no dependencies exist
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<ActionResult> DeleteSku(
        long id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteSkuCommand
        {
            Id = id
        };

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get inventory summary for SKU across all warehouses
    /// </summary>
    [HttpGet("{id:long}/inventory")]
    [ProducesResponseType(typeof(ShopFlow.Application.Queries.Skus.SkuInventoryResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<ShopFlow.Application.Queries.Skus.SkuInventoryResponse>> GetSkuInventory(
        long id,
        [FromQuery] long? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSkuInventoryQuery(id)
        {
            WarehouseId = warehouseId
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Bulk update SKU status (activate/deactivate)
    /// </summary>
    [HttpPatch("bulk-status")]
    [ProducesResponseType(typeof(BulkSkuOperationResponse), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<ActionResult<BulkSkuOperationResponse>> BulkUpdateSkuStatus(
        [FromBody] BulkUpdateSkuStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new BulkUpdateSkuStatusCommand
        {
            IsActive = request.IsActive
        };

        // Add SKU IDs to collection
        foreach (var skuId in request.SkuIds)
        {
            command.SkuIds.Add(skuId);
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}