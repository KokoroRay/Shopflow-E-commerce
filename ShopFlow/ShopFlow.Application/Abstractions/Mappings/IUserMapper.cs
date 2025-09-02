using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Abstractions.Mappings;

public interface IUserMapper
{
    Task<UserResponse> MapToUserResponseAsync(long userId, CancellationToken cancellationToken = default);
}


