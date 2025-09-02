using Microsoft.AspNetCore.Mvc;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.UseCases.Products;

namespace ShopFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICreateProductUseCase _createProductUseCase;
    private readonly IGetProductUseCase _getProductUseCase;

    public ProductsController(
        ICreateProductUseCase createProductUseCase,
        IGetProductUseCase getProductUseCase)
    {
        _createProductUseCase = createProductUseCase;
        _getProductUseCase = getProductUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createProductUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the product" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getProductUseCase.ExecuteAsync(id, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the product" });
        }
    }
}
