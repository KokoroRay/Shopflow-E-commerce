using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Commands.Users;

public record CreateUserCommand(
    string Email,
    string Password,
    string? Phone,
    string FullName,
    string? Gender,
    DateTime? DateOfBirth
) : IRequest<UserResponse>;
