using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.UseCases.Users;

public interface IGetUserUseCase
{
    Task<UserResponse?> ExecuteAsync(long id, CancellationToken cancellationToken = default);
}
