using Microsoft.AspNetCore.Mvc;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.UseCases.Users;

namespace ShopFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ICreateUserUseCase _createUserUseCase;
    private readonly IGetUserUseCase _getUserUseCase;

    public UsersController(
        ICreateUserUseCase createUserUseCase,
        IGetUserUseCase getUserUseCase)
    {
        _createUserUseCase = createUserUseCase;
        _getUserUseCase = getUserUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createUserUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the user" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getUserUseCase.ExecuteAsync(id, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the user" });
        }
    }
}
