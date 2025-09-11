namespace ShopFlow.Domain.Enums;

/// <summary>
/// Predefined permission codes for the system
/// </summary>
public static class PermissionCode
{
    // Admin Permissions
    public const string ADMIN_FULL_ACCESS = "ADMIN_FULL_ACCESS";
    public const string MANAGE_USERS = "MANAGE_USERS";
    public const string MANAGE_ROLES = "MANAGE_ROLES";
    public const string MANAGE_PERMISSIONS = "MANAGE_PERMISSIONS";
    public const string SYSTEM_SETTINGS = "SYSTEM_SETTINGS";

    // Product Management
    public const string VIEW_PRODUCTS = "VIEW_PRODUCTS";
    public const string CREATE_PRODUCTS = "CREATE_PRODUCTS";
    public const string EDIT_PRODUCTS = "EDIT_PRODUCTS";
    public const string DELETE_PRODUCTS = "DELETE_PRODUCTS";
    public const string APPROVE_PRODUCTS = "APPROVE_PRODUCTS";

    // Category Management
    public const string MANAGE_CATEGORIES = "MANAGE_CATEGORIES";

    // Order Management
    public const string VIEW_ORDERS = "VIEW_ORDERS";
    public const string CREATE_ORDERS = "CREATE_ORDERS";
    public const string EDIT_ORDERS = "EDIT_ORDERS";
    public const string CANCEL_ORDERS = "CANCEL_ORDERS";
    public const string PROCESS_ORDERS = "PROCESS_ORDERS";
    public const string MANAGE_PAYMENTS = "MANAGE_PAYMENTS";

    // Inventory Management
    public const string VIEW_INVENTORY = "VIEW_INVENTORY";
    public const string MANAGE_INVENTORY = "MANAGE_INVENTORY";
    public const string UPDATE_STOCK = "UPDATE_STOCK";
    public const string MANAGE_WAREHOUSES = "MANAGE_WAREHOUSES";

    // Customer Management
    public const string VIEW_CUSTOMERS = "VIEW_CUSTOMERS";
    public const string MANAGE_CUSTOMER_SUPPORT = "MANAGE_CUSTOMER_SUPPORT";
    public const string VIEW_CUSTOMER_ORDERS = "VIEW_CUSTOMER_ORDERS";

    // Vendor Management
    public const string VIEW_VENDORS = "VIEW_VENDORS";
    public const string MANAGE_VENDORS = "MANAGE_VENDORS";
    public const string APPROVE_VENDOR_APPLICATIONS = "APPROVE_VENDOR_APPLICATIONS";

    // Content Moderation
    public const string MODERATE_REVIEWS = "MODERATE_REVIEWS";
    public const string MODERATE_CONTENT = "MODERATE_CONTENT";
    public const string MANAGE_REPORTS = "MANAGE_REPORTS";

    // Customer Permissions
    public const string PLACE_ORDERS = "PLACE_ORDERS";
    public const string VIEW_OWN_ORDERS = "VIEW_OWN_ORDERS";
    public const string MANAGE_PROFILE = "MANAGE_PROFILE";
    public const string WRITE_REVIEWS = "WRITE_REVIEWS";
    public const string MANAGE_CART = "MANAGE_CART";
    public const string MANAGE_WISHLIST = "MANAGE_WISHLIST";

    // Reports and Analytics
    public const string VIEW_REPORTS = "VIEW_REPORTS";
    public const string VIEW_ANALYTICS = "VIEW_ANALYTICS";
    public const string EXPORT_DATA = "EXPORT_DATA";

    /// <summary>
    /// Gets all admin permissions
    /// </summary>
    public static readonly string[] AdminPermissions =
    {
        ADMIN_FULL_ACCESS,
        MANAGE_USERS,
        MANAGE_ROLES,
        MANAGE_PERMISSIONS,
        SYSTEM_SETTINGS,
        VIEW_PRODUCTS,
        CREATE_PRODUCTS,
        EDIT_PRODUCTS,
        DELETE_PRODUCTS,
        APPROVE_PRODUCTS,
        MANAGE_CATEGORIES,
        VIEW_ORDERS,
        CREATE_ORDERS,
        EDIT_ORDERS,
        CANCEL_ORDERS,
        PROCESS_ORDERS,
        MANAGE_PAYMENTS,
        VIEW_INVENTORY,
        MANAGE_INVENTORY,
        UPDATE_STOCK,
        MANAGE_WAREHOUSES,
        VIEW_CUSTOMERS,
        MANAGE_CUSTOMER_SUPPORT,
        VIEW_CUSTOMER_ORDERS,
        VIEW_VENDORS,
        MANAGE_VENDORS,
        APPROVE_VENDOR_APPLICATIONS,
        MODERATE_REVIEWS,
        MODERATE_CONTENT,
        MANAGE_REPORTS,
        VIEW_REPORTS,
        VIEW_ANALYTICS,
        EXPORT_DATA
    };

    /// <summary>
    /// Gets moderator permissions
    /// </summary>
    public static readonly string[] ModeratorPermissions =
    {
        VIEW_PRODUCTS,
        APPROVE_PRODUCTS,
        VIEW_ORDERS,
        VIEW_CUSTOMERS,
        MANAGE_CUSTOMER_SUPPORT,
        VIEW_CUSTOMER_ORDERS,
        MODERATE_REVIEWS,
        MODERATE_CONTENT,
        MANAGE_REPORTS,
        VIEW_REPORTS
    };

    /// <summary>
    /// Gets customer permissions
    /// </summary>
    public static readonly string[] CustomerPermissions =
    {
        VIEW_PRODUCTS,
        PLACE_ORDERS,
        VIEW_OWN_ORDERS,
        MANAGE_PROFILE,
        WRITE_REVIEWS,
        MANAGE_CART,
        MANAGE_WISHLIST
    };

    /// <summary>
    /// Gets vendor staff permissions
    /// </summary>
    public static readonly string[] VendorStaffPermissions =
    {
        VIEW_PRODUCTS,
        CREATE_PRODUCTS,
        EDIT_PRODUCTS,
        VIEW_ORDERS,
        PROCESS_ORDERS,
        VIEW_INVENTORY,
        UPDATE_STOCK,
        VIEW_CUSTOMERS,
        VIEW_CUSTOMER_ORDERS,
        VIEW_REPORTS
    };

    /// <summary>
    /// Gets warehouse staff permissions
    /// </summary>
    public static readonly string[] WarehouseStaffPermissions =
    {
        VIEW_PRODUCTS,
        VIEW_ORDERS,
        PROCESS_ORDERS,
        VIEW_INVENTORY,
        MANAGE_INVENTORY,
        UPDATE_STOCK,
        MANAGE_WAREHOUSES
    };
}
