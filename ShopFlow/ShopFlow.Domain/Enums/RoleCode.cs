namespace ShopFlow.Domain.Enums;

/// <summary>
/// Predefined role codes for the system
/// </summary>
public static class RoleCode
{
    /// <summary>
    /// System administrator with full access
    /// </summary>
    public const string ADMIN = "ADMIN";

    /// <summary>
    /// Content moderator with review and moderation permissions
    /// </summary>
    public const string MODERATOR = "MODERATOR";

    /// <summary>
    /// Regular customer user
    /// </summary>
    public const string CUSTOMER = "CUSTOMER";

    /// <summary>
    /// Vendor staff managing vendor products and orders
    /// </summary>
    public const string VENDOR_STAFF = "VENDOR_STAFF";

    /// <summary>
    /// Warehouse staff managing inventory and logistics
    /// </summary>
    public const string WAREHOUSE_STAFF = "WAREHOUSE_STAFF";

    /// <summary>
    /// Guest user with limited access (for future use)
    /// </summary>
    public const string GUEST = "GUEST";

    /// <summary>
    /// Gets all valid role codes
    /// </summary>
    public static readonly string[] AllRoles =
    {
        ADMIN,
        MODERATOR,
        CUSTOMER,
        VENDOR_STAFF,
        WAREHOUSE_STAFF,
        GUEST
    };

    /// <summary>
    /// Gets staff role codes (roles with management permissions)
    /// </summary>
    public static readonly string[] StaffRoles =
    {
        ADMIN,
        MODERATOR,
        VENDOR_STAFF,
        WAREHOUSE_STAFF
    };

    /// <summary>
    /// Gets customer role codes
    /// </summary>
    public static readonly string[] CustomerRoles =
    {
        CUSTOMER,
        GUEST
    };
}
