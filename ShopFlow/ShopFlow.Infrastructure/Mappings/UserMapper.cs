using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;
using ShopFlow.Application.Abstractions.Mappings;

namespace ShopFlow.Infrastructure.Mappings;

public class UserMapper : IUserMapper
{
    private readonly IRepository<core_user> _userRepository;
    private readonly IRepository<role_customer_profile> _customerProfileRepository;
    private readonly IRepository<core_user_role> _userRoleRepository;
    private readonly IRepository<core_role> _roleRepository;

    public UserMapper(
        IRepository<core_user> userRepository,
        IRepository<role_customer_profile> customerProfileRepository,
        IRepository<core_user_role> userRoleRepository,
        IRepository<core_role> roleRepository)
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
