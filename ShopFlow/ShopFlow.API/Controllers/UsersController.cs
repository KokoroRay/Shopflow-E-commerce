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
        if (request == null)
            return BadRequest("Request cannot be null");

        var command = new CreateUserCommand(
            request.Email,
            request.Password,
            request.Phone,
            request.FullName,
            request.Gender,
            request.DateOfBirth
        );

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user response</returns>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest("Request cannot be null");

        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.Phone,
            request.FullName,
            request.Gender,
            request.DateOfBirth
        );

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(long id, CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(id);
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Initiates a password reset process by sending OTP to user's email
    /// </summary>
    /// <param name="request">Forgot password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Forgot password response</returns>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest("Request cannot be null");

        var command = new ForgotPasswordCommand(request.Email);
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Ok(new ShopFlow.Application.Contracts.Response.ForgotPasswordResponse
        {
            Success = result.Success,
            Message = result.Message,
            ExpiresInMinutes = result.ExpiresInMinutes
        });
    }

    /// <summary>
    /// Resets user password using OTP verification
    /// </summary>
    /// <param name="request">Reset password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Reset password response</returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest("Request cannot be null");

        var command = new ResetPasswordCommand(
            request.Email,
            request.OtpCode,
            request.NewPassword);

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Ok(new ShopFlow.Application.Contracts.Response.ResetPasswordResponse
        {
            Success = result.Success,
            Message = result.Message
        });
    }
}
