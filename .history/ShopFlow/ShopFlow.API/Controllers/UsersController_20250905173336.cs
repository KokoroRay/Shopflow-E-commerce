using Microsoft.AspNetCore.Mvc;
using MediatR;
using Asp.Versioning;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Queries.Users;
using ShopFlow.Application.Contracts.Requests;

namespace ShopFlow.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.Password,
            request.Phone,
            request.FullName,
            request.Gender,
            request.DateOfBirth
        );

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(long id, CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }
}
