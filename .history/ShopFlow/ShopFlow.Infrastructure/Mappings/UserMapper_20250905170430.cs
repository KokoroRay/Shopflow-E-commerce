using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Infrastructure.Mappings;

public class UserMapper : IUserMapper
{
    private readonly IPasswordHasher _passwordHasher;

    public UserMapper(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public CoreUser ToDomain(core_user dataEntity)
    {
        if (dataEntity == null) throw new ArgumentNullException(nameof(dataEntity));

        var email = new Email(dataEntity.email ?? throw new InvalidOperationException("Email cannot be null"));
        PhoneNumber? phone = !string.IsNullOrEmpty(dataEntity.phone) ? new PhoneNumber(dataEntity.phone) : null;
        
        // Create CoreUser using constructor
        var user = new CoreUser(email, dataEntity.password_hash ?? "", phone);
        
        // Set additional properties
        SetPrivateProperty(user, nameof(CoreUser.Id), dataEntity.Id);
        SetPrivateProperty(user, nameof(CoreUser.Status), (UserStatus)dataEntity.status);
        SetPrivateProperty(user, nameof(CoreUser.EmailVerified), dataEntity.email_verified);
        SetPrivateProperty(user, nameof(CoreUser.CreatedAt), dataEntity.created_at);
        SetPrivateProperty(user, nameof(CoreUser.UpdatedAt), dataEntity.updated_at);

        return user;
    }

    public core_user ToData(CoreUser domainEntity)
    {
        if (domainEntity == null) throw new ArgumentNullException(nameof(domainEntity));

        return new core_user
        {
            Id = domainEntity.Id,
            email = domainEntity.Email.Value,
            phone = domainEntity.Phone?.Value,
            password_hash = domainEntity.PasswordHash,
            status = (byte)domainEntity.Status,
            email_verified = domainEntity.EmailVerified,
            created_at = domainEntity.CreatedAt,
            updated_at = domainEntity.UpdatedAt
        };
    }

    public void UpdateData(CoreUser domainEntity, core_user dataEntity)
    {
        if (domainEntity == null) throw new ArgumentNullException(nameof(domainEntity));
        if (dataEntity == null) throw new ArgumentNullException(nameof(dataEntity));

        dataEntity.email = domainEntity.Email.Value;
        dataEntity.phone = domainEntity.Phone?.Value;
        dataEntity.password_hash = domainEntity.PasswordHash;
        dataEntity.status = (byte)domainEntity.Status;
        dataEntity.email_verified = domainEntity.EmailVerified;
        dataEntity.updated_at = domainEntity.UpdatedAt;
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            // Use backing field if property is private set
            var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(obj, value);
        }
    }
    {
        _userRepository = userRepository;
        _customerProfileRepository = customerProfileRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
    }

    public async Task<UserResponse> MapToUserResponseAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} not found");
        }

        var customerProfile = await _customerProfileRepository.FindAsync(p => p.user_id == userId, cancellationToken);
        var userRoles = await _userRoleRepository.FindAsync(ur => ur.user_id == userId, cancellationToken);
        
        var roleIds = userRoles.Select(ur => ur.role_id).ToList();
        var roles = await _roleRepository.FindAsync(r => roleIds.Contains(r.id), cancellationToken);

        return new UserResponse
        {
            Id = user.id,
            Email = user.email,
            Phone = user.phone,
            EmailVerified = user.email_verified,
            Status = user.status,
            CreatedAt = user.created_at,
            UpdatedAt = user.updated_at,
            FullName = customerProfile.FirstOrDefault()?.full_name,
            Gender = customerProfile.FirstOrDefault()?.gender,
            DateOfBirth = null, // Add if you have this field in your database
            Roles = roles.Select(r => r.code).ToList()
        };
    }
}
