using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.Abstractions.Services;

/// <summary>
/// Service for managing user roles and permissions
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Gets user roles by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of role codes</returns>
    Task<IEnumerable<string>> GetUserRolesAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user permissions by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of permission codes</returns>
    Task<IEnumerable<string>> GetUserPermissionsAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if user has specific role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleCode">Role code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has role</returns>
    Task<bool> HasRoleAsync(long userId, string roleCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if user has specific permission
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="permissionCode">Permission code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has permission</returns>
    Task<bool> HasPermissionAsync(long userId, string permissionCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns role to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleCode">Role code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task AssignRoleAsync(long userId, string roleCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes role from user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleCode">Role code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task RemoveRoleAsync(long userId, string roleCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user's primary role (for role-based page routing)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Primary role code or null</returns>
    Task<string?> GetPrimaryRoleAsync(long userId, CancellationToken cancellationToken = default);
}
