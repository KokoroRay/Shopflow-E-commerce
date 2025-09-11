using Microsoft.AspNetCore.Authorization;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Abstractions.Security;

/// <summary>
/// Attribute to specify required roles for an action or controller
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : Attribute
{
    /// <summary>
    /// Required roles
    /// </summary>
    public string[] Roles { get; }

    /// <summary>
    /// Indicates if all roles are required (true) or any role is sufficient (false)
    /// </summary>
    public bool RequireAllRoles { get; set; } = false;

    /// <summary>
    /// Initializes a new instance with multiple roles
    /// </summary>
    /// <param name="roles">Required roles</param>
    public RequireRoleAttribute(params string[] roles)
    {
        Roles = roles ?? throw new ArgumentNullException(nameof(roles));
    }

    /// <summary>
    /// Initializes a new instance with a single role
    /// </summary>
    /// <param name="role">Required role</param>
    public RequireRoleAttribute(string role) : this(new[] { role })
    {
    }
}

/// <summary>
/// Attribute to specify required permissions for an action or controller
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute
{
    /// <summary>
    /// Required permissions
    /// </summary>
    public string[] Permissions { get; }

    /// <summary>
    /// Indicates if all permissions are required (true) or any permission is sufficient (false)
    /// </summary>
    public bool RequireAllPermissions { get; set; } = false;

    /// <summary>
    /// Initializes a new instance with multiple permissions
    /// </summary>
    /// <param name="permissions">Required permissions</param>
    public RequirePermissionAttribute(params string[] permissions)
    {
        Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
    }

    /// <summary>
    /// Initializes a new instance with a single permission
    /// </summary>
    /// <param name="permission">Required permission</param>
    public RequirePermissionAttribute(string permission) : this(new[] { permission })
    {
    }
}

/// <summary>
/// Predefined authorization attributes for common roles
/// </summary>
public static class RoleAttributes
{
    /// <summary>
    /// Require Admin role
    /// </summary>
    public class AdminOnlyAttribute : RequireRoleAttribute
    {
        public AdminOnlyAttribute() : base(RoleCode.ADMIN) { }
    }

    /// <summary>
    /// Require Moderator role
    /// </summary>
    public class ModeratorOnlyAttribute : RequireRoleAttribute
    {
        public ModeratorOnlyAttribute() : base(RoleCode.MODERATOR) { }
    }

    /// <summary>
    /// Require Customer role
    /// </summary>
    public class CustomerOnlyAttribute : RequireRoleAttribute
    {
        public CustomerOnlyAttribute() : base(RoleCode.CUSTOMER) { }
    }

    /// <summary>
    /// Require Staff roles (Admin, Moderator, Vendor Staff, Warehouse Staff)
    /// </summary>
    public class StaffOnlyAttribute : RequireRoleAttribute
    {
        public StaffOnlyAttribute() : base(RoleCode.StaffRoles) { }
    }

    /// <summary>
    /// Require Admin or Moderator role
    /// </summary>
    public class AdminOrModeratorAttribute : RequireRoleAttribute
    {
        public AdminOrModeratorAttribute() : base(RoleCode.ADMIN, RoleCode.MODERATOR) { }
    }

    /// <summary>
    /// Require Vendor Staff role
    /// </summary>
    public class VendorStaffOnlyAttribute : RequireRoleAttribute
    {
        public VendorStaffOnlyAttribute() : base(RoleCode.VENDOR_STAFF) { }
    }

    /// <summary>
    /// Require Warehouse Staff role
    /// </summary>
    public class WarehouseStaffOnlyAttribute : RequireRoleAttribute
    {
        public WarehouseStaffOnlyAttribute() : base(RoleCode.WAREHOUSE_STAFF) { }
    }
}
