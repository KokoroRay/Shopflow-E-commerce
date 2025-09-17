using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using PersistenceEntities = ShopFlow.Infrastructure.Persistence.Entities;

namespace ShopFlow.Infrastructure.Mappings;

public class UserMapper : IDomainPersistenceMapper<CoreUser, PersistenceEntities.CoreUser>, IUserMapper
{
    public CoreUser ToDomain(PersistenceEntities.CoreUser dataEntity)
    {
        if (dataEntity == null) return null;

        var domain = new CoreUser(
            Email.FromString(dataEntity.Email),
            dataEntity.PasswordHash,
            dataEntity.Phone != null ? PhoneNumber.FromString(dataEntity.Phone) : null
        );

        // Use reflection to set private properties
        SetPrivateProperty(domain, "Id", dataEntity.Id);
        SetPrivateProperty(domain, "Status", (UserStatus)dataEntity.Status);
        SetPrivateProperty(domain, "CreatedAt", dataEntity.CreatedAt);
        SetPrivateProperty(domain, "UpdatedAt", dataEntity.UpdatedAt);

        return domain;
    }

    public PersistenceEntities.CoreUser ToPersistence(CoreUser domainEntity)
    {
        if (domainEntity == null) return null;

        return new PersistenceEntities.CoreUser
        {
            Id = domainEntity.Id,
            Email = domainEntity.Email.Value,
            PasswordHash = domainEntity.PasswordHash,
            Phone = domainEntity.Phone?.Value,
            Status = (byte)domainEntity.Status,
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt
        };
    }

    public void UpdatePersistence(CoreUser domainEntity, PersistenceEntities.CoreUser persistenceEntity)
    {
        if (domainEntity == null || persistenceEntity == null) return;

        persistenceEntity.Email = domainEntity.Email.Value;
        persistenceEntity.PasswordHash = domainEntity.PasswordHash;
        persistenceEntity.Phone = domainEntity.Phone?.Value;
        persistenceEntity.Status = (byte)domainEntity.Status;
        persistenceEntity.UpdatedAt = domainEntity.UpdatedAt;
    }

    public async Task<UserResponse> MapToUserResponseAsync(long userId, CancellationToken cancellationToken = default)
    {
        // This is a placeholder implementation
        return new UserResponse { Id = userId, Email = "test@example.com" };
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value, null);
        }
    }
}

