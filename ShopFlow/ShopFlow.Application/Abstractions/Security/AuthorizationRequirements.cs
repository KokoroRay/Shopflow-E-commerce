using Microsoft.AspNetCore.Authorization;

namespace ShopFlow.Application.Abstractions.Security;

/// <summary>
/// Authorization requirement for role-based access control
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Required roles for authorization
    /// </summary>
    public string[] RequiredRoles { get; }

    /// <summary>
    /// Indicates if the user must have ALL roles (true) or ANY role (false)
    /// </summary>
    public bool RequireAllRoles { get; }

    /// <summary>
    /// Initializes a new instance of the RoleRequirement class
    /// </summary>
    /// <param name="requiredRoles">Required roles</param>
    /// <param name="requireAllRoles">Require all roles or any role</param>
    public RoleRequirement(string[] requiredRoles, bool requireAllRoles = false)
    {
        RequiredRoles = requiredRoles ?? throw new ArgumentNullException(nameof(requiredRoles));
        RequireAllRoles = requireAllRoles;
    }

    /// <summary>
    /// Initializes a new instance of the RoleRequirement class with a single role
    /// </summary>
    /// <param name="requiredRole">Required role</param>
    public RoleRequirement(string requiredRole) : this(new[] { requiredRole }, false)
    {
    }
}

/// <summary>
/// Authorization requirement for permission-based access control
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Required permissions for authorization
    /// </summary>
    public string[] RequiredPermissions { get; }

    /// <summary>
    /// Indicates if the user must have ALL permissions (true) or ANY permission (false)
    /// </summary>
    public bool RequireAllPermissions { get; }

    /// <summary>
    /// Initializes a new instance of the PermissionRequirement class
    /// </summary>
    /// <param name="requiredPermissions">Required permissions</param>
    /// <param name="requireAllPermissions">Require all permissions or any permission</param>
    public PermissionRequirement(string[] requiredPermissions, bool requireAllPermissions = false)
    {
        RequiredPermissions = requiredPermissions ?? throw new ArgumentNullException(nameof(requiredPermissions));
        RequireAllPermissions = requireAllPermissions;
    }

    /// <summary>
    /// Initializes a new instance of the PermissionRequirement class with a single permission
    /// </summary>
    /// <param name="requiredPermission">Required permission</param>
    public PermissionRequirement(string requiredPermission) : this(new[] { requiredPermission }, false)
    {
    }
}
