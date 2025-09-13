using Microsoft.AspNetCore.Authorization;

namespace ShopFlow.API.Authorization;

/// <summary>
/// Authorization requirement for Vietnamese marketplace vendor access
/// Ensures vendors can only access their own products
/// </summary>
public class VendorResourceRequirement : IAuthorizationRequirement
{
    public string ResourceType { get; }

    public VendorResourceRequirement(string resourceType)
    {
        ResourceType = resourceType;
    }
}

/// <summary>
/// Authorization handler for vendor resource access in Vietnamese marketplace
/// </summary>
public class VendorResourceHandler : AuthorizationHandler<VendorResourceRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        VendorResourceRequirement requirement)
    {
        // Check if user is Admin (can access all resources)
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Check if user is Vendor and has vendor_id claim
        if (context.User.IsInRole("Vendor"))
        {
            var vendorIdClaim = context.User.FindFirst("vendor_id");
            if (vendorIdClaim != null && !string.IsNullOrEmpty(vendorIdClaim.Value))
            {
                // For products, check if the resource belongs to this vendor
                // This will be enhanced in Phase 3 with actual resource validation
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        // Requirement not met
        context.Fail();
        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization requirement for Vietnamese marketplace admin approval operations
/// </summary>
public class ApprovalAuthorityRequirement : IAuthorizationRequirement
{
    public string OperationType { get; }

    public ApprovalAuthorityRequirement(string operationType)
    {
        OperationType = operationType;
    }
}

/// <summary>
/// Authorization handler for admin approval operations
/// </summary>
public class ApprovalAuthorityHandler : AuthorizationHandler<ApprovalAuthorityRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ApprovalAuthorityRequirement requirement)
    {
        // Only admins can approve/reject products
        if (context.User.IsInRole("Admin"))
        {
            var permissionClaim = context.User.FindFirst("permission");
            if (permissionClaim?.Value?.Contains("product.approve") == true)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        context.Fail();
        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization requirement for Vietnamese tax compliance operations
/// </summary>
public class TaxComplianceRequirement : IAuthorizationRequirement
{
    public string Operation { get; }

    public TaxComplianceRequirement(string operation)
    {
        Operation = operation;
    }
}

/// <summary>
/// Authorization handler for tax compliance operations
/// </summary>
public class TaxComplianceHandler : AuthorizationHandler<TaxComplianceRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TaxComplianceRequirement requirement)
    {
        // Admin or Accountant roles can access tax operations
        if (context.User.IsInRole("Admin") || context.User.IsInRole("Accountant"))
        {
            var permissionClaim = context.User.FindFirst("permission");
            if (permissionClaim?.Value?.Contains("tax.report") == true)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        context.Fail();
        return Task.CompletedTask;
    }
}