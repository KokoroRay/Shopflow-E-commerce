using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShopFlow.Application.Abstractions.Services;
using System.Security.Claims;
using Asp.Versioning;

namespace ShopFlow.API.Controllers;

/// <summary>
/// Controller for role-based page routing and navigation
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class NavigationController : ControllerBase
{
    private readonly IPageRoutingService _pageRoutingService;
    private readonly IUserRoleService _userRoleService;

    /// <summary>
    /// Initializes a new instance of the NavigationController class
    /// </summary>
    /// <param name="pageRoutingService">Page routing service</param>
    /// <param name="userRoleService">User role service</param>
    public NavigationController(
        IPageRoutingService pageRoutingService,
        IUserRoleService userRoleService)
    {
        _pageRoutingService = pageRoutingService ?? throw new ArgumentNullException(nameof(pageRoutingService));
        _userRoleService = userRoleService ?? throw new ArgumentNullException(nameof(userRoleService));
    }

    /// <summary>
    /// Gets the default page URL for the current user based on their primary role
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Default page URL</returns>
    [HttpGet("default-page")]
    public async Task<IActionResult> GetDefaultPage(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var primaryRole = await _userRoleService.GetPrimaryRoleAsync(userId.Value, cancellationToken);
        if (string.IsNullOrEmpty(primaryRole))
        {
            return BadRequest("User has no assigned roles");
        }

        var defaultPage = _pageRoutingService.GetDefaultPageForRole(primaryRole);
        return Ok(new { DefaultPage = defaultPage, PrimaryRole = primaryRole });
    }

    /// <summary>
    /// Gets the dashboard configuration for the current user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dashboard configuration</returns>
    [HttpGet("dashboard-config")]
    public async Task<IActionResult> GetDashboardConfig(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var primaryRole = await _userRoleService.GetPrimaryRoleAsync(userId.Value, cancellationToken);
        if (string.IsNullOrEmpty(primaryRole))
        {
            return BadRequest("User has no assigned roles");
        }

        var dashboardConfig = _pageRoutingService.GetDashboardConfig(primaryRole);
        return Ok(dashboardConfig);
    }

    /// <summary>
    /// Gets allowed pages for the current user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of allowed pages</returns>
    [HttpGet("allowed-pages")]
    public async Task<IActionResult> GetAllowedPages(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var userRoles = await _userRoleService.GetUserRolesAsync(userId.Value, cancellationToken);
        var allAllowedPages = new HashSet<string>();

        foreach (var role in userRoles)
        {
            var rolePages = _pageRoutingService.GetAllowedPagesForRole(role);
            foreach (var page in rolePages)
            {
                allAllowedPages.Add(page);
            }
        }

        return Ok(new { AllowedPages = allAllowedPages.ToList() });
    }

    /// <summary>
    /// Checks if the current user can access a specific page
    /// </summary>
    /// <param name="pageUrl">Page URL to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access check result</returns>
    [HttpGet("can-access")]
    public async Task<IActionResult> CanAccessPage([FromQuery] string pageUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(pageUrl))
        {
            return BadRequest("Page URL is required");
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var userRoles = await _userRoleService.GetUserRolesAsync(userId.Value, cancellationToken);

        var canAccess = userRoles.Any(role => _pageRoutingService.CanRoleAccessPage(role, pageUrl));

        return Ok(new { CanAccess = canAccess, PageUrl = pageUrl });
    }

    /// <summary>
    /// Gets current user's roles and permissions
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User roles and permissions</returns>
    [HttpGet("user-context")]
    public async Task<IActionResult> GetUserContext(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var userRoles = await _userRoleService.GetUserRolesAsync(userId.Value, cancellationToken);
        var userPermissions = await _userRoleService.GetUserPermissionsAsync(userId.Value, cancellationToken);
        var primaryRole = await _userRoleService.GetPrimaryRoleAsync(userId.Value, cancellationToken);

        return Ok(new
        {
            UserId = userId.Value,
            Roles = userRoles,
            Permissions = userPermissions,
            PrimaryRole = primaryRole
        });
    }

    /// <summary>
    /// Gets the current user ID from JWT claims
    /// </summary>
    /// <returns>User ID or null if not found</returns>
    private long? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
