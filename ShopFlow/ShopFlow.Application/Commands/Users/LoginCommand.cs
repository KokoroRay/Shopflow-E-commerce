using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Commands.Users;

/// <summary>
/// Command to authenticate a user
/// </summary>
public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResponse>;
