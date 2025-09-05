using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Infrastructure.Mappings;

public class UserMapper : IUserMapper
{
    public CoreUser ToDomain(CoreUser dataEntity)
    {
        return dataEntity ?? throw new ArgumentNullException(nameof(dataEntity));
    }

    public CoreUser ToData(CoreUser domainEntity)
    {
        return domainEntity ?? throw new ArgumentNullException(nameof(domainEntity));
    }

    public void UpdateData(CoreUser domainEntity, CoreUser dataEntity)
    {
        // Update logic here
    }

    public async Task<UserResponse> MapToUserResponseAsync(long userId, CancellationToken cancellationToken = default)
    {
        // Temporary implementation
        return new UserResponse
        {
            Id = userId,
            Email = "sample@example.com",
            Roles = new List<string>()
        };
    }
}
