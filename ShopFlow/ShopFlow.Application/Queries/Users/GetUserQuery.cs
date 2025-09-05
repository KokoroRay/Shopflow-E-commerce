using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Users;

public record GetUserQuery(long UserId) : IRequest<UserResponse?>;
