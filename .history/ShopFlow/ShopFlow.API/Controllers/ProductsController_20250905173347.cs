using Microsoft.AspNetCore.Mvc;
using MediatR;
using Asp.Versioning;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Queries.Products;
using ShopFlow.Application.Contracts.Requests;

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
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.ProductType,
            request.ReturnDays,
            request.Language,
            request.Name,
            request.ShortDescription,
            request.Slug,
            request.MetaTitle,
            request.MetaDescription,
            request.CategoryIds
        );

        var result = await _mediator.Send(command, cancellationToken);
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
}
