using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Contracts.Requests;

namespace ShopFlow.Application.UseCases.Users;

public interface ICreateUserUseCase
{
    Task<UserResponse> ExecuteAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
}
