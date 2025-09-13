using System.Security.Claims;

namespace ShopFlow.API.Extensions;

/// <summary>
/// Extension methods for working with user context from HTTP context
/// </summary>
public static class UserContextExtensions
{
    /// <summary>
    /// Gets the current user ID from the authenticated context
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>User ID if authenticated, null otherwise</returns>
    public static long? GetCurrentUserId(this HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
            return null;

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    /// <summary>
    /// Gets the current user ID from the authenticated context, throwing if not authenticated
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>User ID</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user is not authenticated</exception>
    public static long GetRequiredUserId(this HttpContext context)
    {
        var userId = GetCurrentUserId(context);
        return userId ?? throw new UnauthorizedAccessException("User must be authenticated to perform this action");
    }

    /// <summary>
    /// Gets the current user's email from the authenticated context
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>User email if authenticated, null otherwise</returns>
    public static string? GetCurrentUserEmail(this HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
            return null;

        return context.User.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets the current user's roles from the authenticated context
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>List of user roles</returns>
    public static IEnumerable<string> GetCurrentUserRoles(this HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
            return Enumerable.Empty<string>();

        return context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="role">Role to check</param>
    /// <returns>True if user has the role, false otherwise</returns>
    public static bool HasRole(this HttpContext context, string role)
    {
        return context.User?.IsInRole(role) == true;
    }

    /// <summary>
    /// Checks if the current user is an administrator
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>True if user is admin, false otherwise</returns>
    public static bool IsAdmin(this HttpContext context)
    {
        return HasRole(context, "Admin") || HasRole(context, "Administrator");
    }
}