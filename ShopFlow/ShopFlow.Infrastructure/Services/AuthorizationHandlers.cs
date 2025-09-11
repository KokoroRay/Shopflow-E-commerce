using Microsoft.AspNetCore.Authorization;
using ShopFlow.Application.Abstractions.Security;
using System.Security.Claims;

namespace ShopFlow.Infrastructure.Services;

/// <summary>
/// Authorization handler for role-based access control
/// </summary>
public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    /// <summary>
    /// Handles the authorization requirement
    /// </summary>
    /// <param name="context">Authorization context</param>
    /// <param name="requirement">Role requirement</param>
    /// <returns>Task</returns>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        var userRoles = context.User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (!userRoles.Any())
        {
            return Task.CompletedTask;
        }

        bool hasRequiredAccess;

        if (requirement.RequireAllRoles)
        {
            // User must have ALL required roles
            hasRequiredAccess = requirement.RequiredRoles.All(role => userRoles.Contains(role));
        }
        else
        {
            // User must have ANY of the required roles
            hasRequiredAccess = requirement.RequiredRoles.Any(role => userRoles.Contains(role));
        }

        if (hasRequiredAccess)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for permission-based access control
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>
    /// Handles the authorization requirement
    /// </summary>
    /// <param name="context">Authorization context</param>
    /// <param name="requirement">Permission requirement</param>
    /// <returns>Task</returns>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        var userPermissions = context.User.FindAll("permission")
            .Select(c => c.Value)
            .ToList();

        if (!userPermissions.Any())
        {
            return Task.CompletedTask;
        }

        bool hasRequiredAccess;

        if (requirement.RequireAllPermissions)
        {
            // User must have ALL required permissions
            hasRequiredAccess = requirement.RequiredPermissions.All(permission => userPermissions.Contains(permission));
        }
        else
        {
            // User must have ANY of the required permissions
            hasRequiredAccess = requirement.RequiredPermissions.Any(permission => userPermissions.Contains(permission));
        }

        if (hasRequiredAccess)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
