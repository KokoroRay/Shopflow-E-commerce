using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Services;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Infrastructure.Services;

/// <summary>
/// Implementation of user role service
/// </summary>
public class UserRoleService : IUserRoleService
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the UserRoleService class
    /// </summary>
    /// <param name="userRepository">User repository</param>
    public UserRoleService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Gets user roles by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of role codes</returns>
    public async Task<IEnumerable<string>> GetUserRolesAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);

        if (user == null)
        {
            return Enumerable.Empty<string>();
        }

        return user.UserRoles.Select(ur => ur.Role.Code).ToList();
    }

    /// <summary>
    /// Gets user permissions by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of permission codes</returns>
    public async Task<IEnumerable<string>> GetUserPermissionsAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);

        if (user == null)
        {
            return Enumerable.Empty<string>();
        }

        var permissions = new HashSet<string>();

        foreach (var userRole in user.UserRoles)
        {
            foreach (var permission in userRole.Role.Permissions)
            {
                permissions.Add(permission.Code);
            }
        }

        return permissions;
    }

    /// <summary>
    /// Checks if user has specific role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleCode">Role code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has role</returns>
    public async Task<bool> HasRoleAsync(long userId, string roleCode, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        return user?.HasRole(roleCode) ?? false;
    }

    /// <summary>
    /// Checks if user has specific permission
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="permissionCode">Permission code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has permission</returns>
    public async Task<bool> HasPermissionAsync(long userId, string permissionCode, CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken).ConfigureAwait(false);
        return permissions.Contains(permissionCode);
    }

    /// <summary>
    /// Assigns role to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleCode">Role code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task AssignRoleAsync(long userId, string roleCode, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        if (user.HasRole(roleCode))
        {
            return; // User already has this role
        }

        // Note: This would require role repository and proper implementation
        // For now, this is just the interface structure
        throw new NotImplementedException("Role assignment requires role repository implementation");
    }

    /// <summary>
    /// Removes role from user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleCode">Role code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task RemoveRoleAsync(long userId, string roleCode, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        // Note: This would require proper implementation to remove role
        throw new NotImplementedException("Role removal requires proper repository implementation");
    }

    /// <summary>
    /// Gets user's primary role (for role-based page routing)
    /// Priority: ADMIN > MODERATOR > VENDOR_STAFF > WAREHOUSE_STAFF > CUSTOMER
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Primary role code or null</returns>
    public async Task<string?> GetPrimaryRoleAsync(long userId, CancellationToken cancellationToken = default)
    {
        var roles = await GetUserRolesAsync(userId, cancellationToken).ConfigureAwait(false);
        var roleList = roles.ToList();

        if (!roleList.Any())
        {
            return null;
        }

        // Define role priority order (highest to lowest)
        var rolePriority = new[]
        {
            RoleCode.ADMIN,
            RoleCode.MODERATOR,
            RoleCode.VENDOR_STAFF,
            RoleCode.WAREHOUSE_STAFF,
            RoleCode.CUSTOMER,
            RoleCode.GUEST
        };

        // Return the highest priority role the user has
        foreach (var priorityRole in rolePriority)
        {
            if (roleList.Contains(priorityRole))
            {
                return priorityRole;
            }
        }

        return roleList.First(); // Fallback to first role if none match priority list
    }
}
