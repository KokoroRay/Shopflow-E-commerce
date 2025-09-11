using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Abstractions.Services;

namespace ShopFlow.API.Filters;

/// <summary>
/// Authorization filter for role-based and permission-based access control
/// </summary>
public class RolePermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly IUserRoleService _userRoleService;

    /// <summary>
    /// Initializes a new instance of the RolePermissionAuthorizationFilter class
    /// </summary>
    /// <param name="userRoleService">User role service</param>
    public RolePermissionAuthorizationFilter(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService ?? throw new ArgumentNullException(nameof(userRoleService));
    }

    /// <summary>
    /// Called early in the filter pipeline to confirm request is authorized
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    /// <returns>Task</returns>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Check if action allows anonymous access
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
        {
            return;
        }

        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check role requirements
        var roleRequirements = context.ActionDescriptor.EndpointMetadata
            .OfType<RequireRoleAttribute>()
            .ToList();

        if (roleRequirements.Any())
        {
            var hasRequiredRole = await CheckRoleRequirements(userId, roleRequirements);
            if (!hasRequiredRole)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        // Check permission requirements
        var permissionRequirements = context.ActionDescriptor.EndpointMetadata
            .OfType<RequirePermissionAttribute>()
            .ToList();

        if (permissionRequirements.Any())
        {
            var hasRequiredPermission = await CheckPermissionRequirements(userId, permissionRequirements);
            if (!hasRequiredPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

    /// <summary>
    /// Checks if user meets role requirements
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleRequirements">Role requirements</param>
    /// <returns>True if requirements are met</returns>
    private async Task<bool> CheckRoleRequirements(long userId, IEnumerable<RequireRoleAttribute> roleRequirements)
    {
        var userRoles = (await _userRoleService.GetUserRolesAsync(userId)).ToList();

        foreach (var requirement in roleRequirements)
        {
            bool requirementMet;

            if (requirement.RequireAllRoles)
            {
                // User must have ALL required roles
                requirementMet = requirement.Roles.All(role => userRoles.Contains(role));
            }
            else
            {
                // User must have ANY of the required roles
                requirementMet = requirement.Roles.Any(role => userRoles.Contains(role));
            }

            if (!requirementMet)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if user meets permission requirements
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="permissionRequirements">Permission requirements</param>
    /// <returns>True if requirements are met</returns>
    private async Task<bool> CheckPermissionRequirements(long userId, IEnumerable<RequirePermissionAttribute> permissionRequirements)
    {
        var userPermissions = (await _userRoleService.GetUserPermissionsAsync(userId)).ToList();

        foreach (var requirement in permissionRequirements)
        {
            bool requirementMet;

            if (requirement.RequireAllPermissions)
            {
                // User must have ALL required permissions
                requirementMet = requirement.Permissions.All(permission => userPermissions.Contains(permission));
            }
            else
            {
                // User must have ANY of the required permissions
                requirementMet = requirement.Permissions.Any(permission => userPermissions.Contains(permission));
            }

            if (!requirementMet)
            {
                return false;
            }
        }

        return true;
    }
}
