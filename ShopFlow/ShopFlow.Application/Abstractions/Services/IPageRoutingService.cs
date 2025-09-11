using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Abstractions.Services;

/// <summary>
/// Service for determining role-based page routing after login
/// </summary>
public interface IPageRoutingService
{
    /// <summary>
    /// Gets the default page URL based on user's primary role
    /// </summary>
    /// <param name="primaryRole">Primary role code</param>
    /// <returns>Default page URL</returns>
    string GetDefaultPageForRole(string primaryRole);

    /// <summary>
    /// Gets allowed pages for a specific role
    /// </summary>
    /// <param name="roleCode">Role code</param>
    /// <returns>List of allowed page URLs</returns>
    IEnumerable<string> GetAllowedPagesForRole(string roleCode);

    /// <summary>
    /// Checks if a role can access a specific page
    /// </summary>
    /// <param name="roleCode">Role code</param>
    /// <param name="pageUrl">Page URL</param>
    /// <returns>True if role can access the page</returns>
    bool CanRoleAccessPage(string roleCode, string pageUrl);

    /// <summary>
    /// Gets the dashboard configuration for a role
    /// </summary>
    /// <param name="primaryRole">Primary role code</param>
    /// <returns>Dashboard configuration</returns>
    DashboardConfig GetDashboardConfig(string primaryRole);
}

/// <summary>
/// Dashboard configuration for different roles
/// </summary>
public class DashboardConfig
{
    /// <summary>
    /// Dashboard title
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Welcome message
    /// </summary>
    public string WelcomeMessage { get; init; } = string.Empty;

    /// <summary>
    /// Available menu items
    /// </summary>
    public IEnumerable<MenuItem> MenuItems { get; init; } = new List<MenuItem>();

    /// <summary>
    /// Quick action buttons
    /// </summary>
    public IEnumerable<QuickAction> QuickActions { get; init; } = new List<QuickAction>();
}

/// <summary>
/// Menu item configuration
/// </summary>
public class MenuItem
{
    /// <summary>
    /// Menu item name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Menu item URL
    /// </summary>
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// Icon class
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Required permissions to view this menu item
    /// </summary>
    public IEnumerable<string> RequiredPermissions { get; init; } = new List<string>();
}

/// <summary>
/// Quick action configuration
/// </summary>
public class QuickAction
{
    /// <summary>
    /// Action name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Action URL
    /// </summary>
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// Action description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Icon class
    /// </summary>
    public string? Icon { get; init; }
}
