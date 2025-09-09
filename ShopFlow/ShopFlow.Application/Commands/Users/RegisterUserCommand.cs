using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Commands.Users;

/// <summary>
/// Command to register a new user
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string? Phone = null,
    string? FullName = null,
    string? Gender = null,
    DateTime? DateOfBirth = null
) : IRequest<UserResponse>;
