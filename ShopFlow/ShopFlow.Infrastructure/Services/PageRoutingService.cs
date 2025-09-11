using ShopFlow.Application.Abstractions.Services;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Infrastructure.Services;

/// <summary>
/// Implementation of page routing service for role-based navigation
/// </summary>
public class PageRoutingService : IPageRoutingService
{
    /// <summary>
    /// Gets the default page URL based on user's primary role
    /// </summary>
    /// <param name="primaryRole">Primary role code</param>
    /// <returns>Default page URL</returns>
    public string GetDefaultPageForRole(string primaryRole)
    {
        return primaryRole switch
        {
            RoleCode.ADMIN => "/admin/dashboard",
            RoleCode.MODERATOR => "/moderator/dashboard",
            RoleCode.VENDOR_STAFF => "/vendor/dashboard",
            RoleCode.WAREHOUSE_STAFF => "/warehouse/dashboard",
            RoleCode.CUSTOMER => "/customer/dashboard",
            RoleCode.GUEST => "/guest/catalog",
            _ => "/customer/dashboard" // Default fallback
        };
    }

    /// <summary>
    /// Gets allowed pages for a specific role
    /// </summary>
    /// <param name="roleCode">Role code</param>
    /// <returns>List of allowed page URLs</returns>
    public IEnumerable<string> GetAllowedPagesForRole(string roleCode)
    {
        return roleCode switch
        {
            RoleCode.ADMIN => GetAdminPages(),
            RoleCode.MODERATOR => GetModeratorPages(),
            RoleCode.VENDOR_STAFF => GetVendorStaffPages(),
            RoleCode.WAREHOUSE_STAFF => GetWarehouseStaffPages(),
            RoleCode.CUSTOMER => GetCustomerPages(),
            RoleCode.GUEST => GetGuestPages(),
            _ => GetGuestPages() // Default fallback
        };
    }

    /// <summary>
    /// Checks if a role can access a specific page
    /// </summary>
    /// <param name="roleCode">Role code</param>
    /// <param name="pageUrl">Page URL</param>
    /// <returns>True if role can access the page</returns>
    public bool CanRoleAccessPage(string roleCode, string pageUrl)
    {
        var allowedPages = GetAllowedPagesForRole(roleCode);
        return allowedPages.Any(page => pageUrl.StartsWith(page, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the dashboard configuration for a role
    /// </summary>
    /// <param name="primaryRole">Primary role code</param>
    /// <returns>Dashboard configuration</returns>
    public DashboardConfig GetDashboardConfig(string primaryRole)
    {
        return primaryRole switch
        {
            RoleCode.ADMIN => GetAdminDashboardConfig(),
            RoleCode.MODERATOR => GetModeratorDashboardConfig(),
            RoleCode.VENDOR_STAFF => GetVendorStaffDashboardConfig(),
            RoleCode.WAREHOUSE_STAFF => GetWarehouseStaffDashboardConfig(),
            RoleCode.CUSTOMER => GetCustomerDashboardConfig(),
            _ => GetCustomerDashboardConfig() // Default fallback
        };
    }

    #region Private Helper Methods

    private static IEnumerable<string> GetAdminPages()
    {
        return new[]
        {
            "/admin",
            "/admin/dashboard",
            "/admin/users",
            "/admin/roles",
            "/admin/permissions",
            "/admin/products",
            "/admin/orders",
            "/admin/vendors",
            "/admin/warehouses",
            "/admin/reports",
            "/admin/settings",
            "/products",
            "/orders",
            "/customers",
            "/reports"
        };
    }

    private static IEnumerable<string> GetModeratorPages()
    {
        return new[]
        {
            "/moderator",
            "/moderator/dashboard",
            "/moderator/reviews",
            "/moderator/reports",
            "/moderator/content",
            "/products",
            "/orders",
            "/customers"
        };
    }

    private static IEnumerable<string> GetVendorStaffPages()
    {
        return new[]
        {
            "/vendor",
            "/vendor/dashboard",
            "/vendor/products",
            "/vendor/orders",
            "/vendor/inventory",
            "/vendor/customers",
            "/vendor/reports",
            "/products",
            "/orders"
        };
    }

    private static IEnumerable<string> GetWarehouseStaffPages()
    {
        return new[]
        {
            "/warehouse",
            "/warehouse/dashboard",
            "/warehouse/inventory",
            "/warehouse/orders",
            "/warehouse/shipments",
            "/warehouse/reports",
            "/orders",
            "/inventory"
        };
    }

    private static IEnumerable<string> GetCustomerPages()
    {
        return new[]
        {
            "/customer",
            "/customer/dashboard",
            "/customer/orders",
            "/customer/profile",
            "/customer/wishlist",
            "/customer/cart",
            "/customer/reviews",
            "/products",
            "/catalog",
            "/cart",
            "/checkout"
        };
    }

    private static IEnumerable<string> GetGuestPages()
    {
        return new[]
        {
            "/guest",
            "/guest/catalog",
            "/products",
            "/catalog",
            "/login",
            "/register"
        };
    }

    private static DashboardConfig GetAdminDashboardConfig()
    {
        return new DashboardConfig
        {
            Title = "Admin Dashboard",
            WelcomeMessage = "Welcome to ShopFlow Admin Panel",
            MenuItems = new[]
            {
                new MenuItem { Name = "Users", Url = "/admin/users", Icon = "users", RequiredPermissions = new[] { PermissionCode.MANAGE_USERS } },
                new MenuItem { Name = "Products", Url = "/admin/products", Icon = "package", RequiredPermissions = new[] { PermissionCode.VIEW_PRODUCTS } },
                new MenuItem { Name = "Orders", Url = "/admin/orders", Icon = "shopping-cart", RequiredPermissions = new[] { PermissionCode.VIEW_ORDERS } },
                new MenuItem { Name = "Vendors", Url = "/admin/vendors", Icon = "store", RequiredPermissions = new[] { PermissionCode.MANAGE_VENDORS } },
                new MenuItem { Name = "Reports", Url = "/admin/reports", Icon = "bar-chart", RequiredPermissions = new[] { PermissionCode.VIEW_REPORTS } },
                new MenuItem { Name = "Settings", Url = "/admin/settings", Icon = "settings", RequiredPermissions = new[] { PermissionCode.SYSTEM_SETTINGS } }
            },
            QuickActions = new[]
            {
                new QuickAction { Name = "Add User", Url = "/admin/users/create", Description = "Create a new user account", Icon = "user-plus" },
                new QuickAction { Name = "Add Product", Url = "/admin/products/create", Description = "Add a new product", Icon = "plus" },
                new QuickAction { Name = "View Reports", Url = "/admin/reports", Description = "View system reports", Icon = "file-text" }
            }
        };
    }

    private static DashboardConfig GetModeratorDashboardConfig()
    {
        return new DashboardConfig
        {
            Title = "Moderator Dashboard",
            WelcomeMessage = "Content Moderation Center",
            MenuItems = new[]
            {
                new MenuItem { Name = "Reviews", Url = "/moderator/reviews", Icon = "star", RequiredPermissions = new[] { PermissionCode.MODERATE_REVIEWS } },
                new MenuItem { Name = "Reports", Url = "/moderator/reports", Icon = "flag", RequiredPermissions = new[] { PermissionCode.MANAGE_REPORTS } },
                new MenuItem { Name = "Content", Url = "/moderator/content", Icon = "edit", RequiredPermissions = new[] { PermissionCode.MODERATE_CONTENT } },
                new MenuItem { Name = "Products", Url = "/products", Icon = "package", RequiredPermissions = new[] { PermissionCode.VIEW_PRODUCTS } }
            },
            QuickActions = new[]
            {
                new QuickAction { Name = "Review Queue", Url = "/moderator/reviews/pending", Description = "Check pending reviews", Icon = "clock" },
                new QuickAction { Name = "Report Queue", Url = "/moderator/reports/pending", Description = "Handle user reports", Icon = "alert-circle" }
            }
        };
    }

    private static DashboardConfig GetVendorStaffDashboardConfig()
    {
        return new DashboardConfig
        {
            Title = "Vendor Dashboard",
            WelcomeMessage = "Vendor Management Center",
            MenuItems = new[]
            {
                new MenuItem { Name = "Products", Url = "/vendor/products", Icon = "package", RequiredPermissions = new[] { PermissionCode.VIEW_PRODUCTS } },
                new MenuItem { Name = "Orders", Url = "/vendor/orders", Icon = "shopping-cart", RequiredPermissions = new[] { PermissionCode.VIEW_ORDERS } },
                new MenuItem { Name = "Inventory", Url = "/vendor/inventory", Icon = "archive", RequiredPermissions = new[] { PermissionCode.VIEW_INVENTORY } },
                new MenuItem { Name = "Reports", Url = "/vendor/reports", Icon = "bar-chart", RequiredPermissions = new[] { PermissionCode.VIEW_REPORTS } }
            },
            QuickActions = new[]
            {
                new QuickAction { Name = "Add Product", Url = "/vendor/products/create", Description = "Add a new product", Icon = "plus" },
                new QuickAction { Name = "Process Orders", Url = "/vendor/orders/pending", Description = "Process pending orders", Icon = "clock" },
                new QuickAction { Name = "Update Inventory", Url = "/vendor/inventory", Description = "Update stock levels", Icon = "edit" }
            }
        };
    }

    private static DashboardConfig GetWarehouseStaffDashboardConfig()
    {
        return new DashboardConfig
        {
            Title = "Warehouse Dashboard",
            WelcomeMessage = "Warehouse Operations Center",
            MenuItems = new[]
            {
                new MenuItem { Name = "Inventory", Url = "/warehouse/inventory", Icon = "archive", RequiredPermissions = new[] { PermissionCode.MANAGE_INVENTORY } },
                new MenuItem { Name = "Orders", Url = "/warehouse/orders", Icon = "shopping-cart", RequiredPermissions = new[] { PermissionCode.PROCESS_ORDERS } },
                new MenuItem { Name = "Shipments", Url = "/warehouse/shipments", Icon = "truck", RequiredPermissions = new[] { PermissionCode.PROCESS_ORDERS } },
                new MenuItem { Name = "Reports", Url = "/warehouse/reports", Icon = "bar-chart", RequiredPermissions = new[] { PermissionCode.VIEW_REPORTS } }
            },
            QuickActions = new[]
            {
                new QuickAction { Name = "Process Shipments", Url = "/warehouse/shipments/pending", Description = "Process pending shipments", Icon = "truck" },
                new QuickAction { Name = "Stock Check", Url = "/warehouse/inventory/check", Description = "Perform stock check", Icon = "check-circle" },
                new QuickAction { Name = "Receive Stock", Url = "/warehouse/inventory/receive", Description = "Receive new stock", Icon = "inbox" }
            }
        };
    }

    private static DashboardConfig GetCustomerDashboardConfig()
    {
        return new DashboardConfig
        {
            Title = "My Account",
            WelcomeMessage = "Welcome to ShopFlow",
            MenuItems = new[]
            {
                new MenuItem { Name = "Orders", Url = "/customer/orders", Icon = "shopping-bag", RequiredPermissions = new[] { PermissionCode.VIEW_OWN_ORDERS } },
                new MenuItem { Name = "Profile", Url = "/customer/profile", Icon = "user", RequiredPermissions = new[] { PermissionCode.MANAGE_PROFILE } },
                new MenuItem { Name = "Wishlist", Url = "/customer/wishlist", Icon = "heart", RequiredPermissions = new[] { PermissionCode.MANAGE_WISHLIST } },
                new MenuItem { Name = "Reviews", Url = "/customer/reviews", Icon = "star", RequiredPermissions = new[] { PermissionCode.WRITE_REVIEWS } }
            },
            QuickActions = new[]
            {
                new QuickAction { Name = "Browse Products", Url = "/products", Description = "Explore our product catalog", Icon = "search" },
                new QuickAction { Name = "Track Orders", Url = "/customer/orders", Description = "Track your recent orders", Icon = "map-pin" },
                new QuickAction { Name = "View Cart", Url = "/cart", Description = "Review items in your cart", Icon = "shopping-cart" }
            }
        };
    }

    #endregion
}
