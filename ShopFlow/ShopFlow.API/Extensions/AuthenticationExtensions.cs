using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ShopFlow.API.Configurations;
using ShopFlow.API.Authorization;

namespace ShopFlow.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JWT options
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        if (jwtOptions == null || string.IsNullOrEmpty(jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("JWT configuration is invalid or missing");
        }

        // Add authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Set to true in production
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new { error = "Invalid token" });
                    return context.Response.WriteAsync(result);
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new { error = "Unauthorized" });
                    return context.Response.WriteAsync(result);
                }
            };
        });

        // Add authorization with Vietnamese marketplace policies
        services.AddAuthorization(options =>
        {
            // Basic role policies
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
            options.AddPolicy("VendorOnly", policy => policy.RequireRole("Vendor"));

            // Vietnamese marketplace specific policies
            options.AddPolicy("VendorOrAdmin", policy =>
                policy.RequireRole("Vendor", "Admin"));

            options.AddPolicy("ProductManagement", policy =>
                policy.RequireRole("Vendor", "Admin")
                      .RequireClaim("permission", "product.manage"));

            options.AddPolicy("ProductApproval", policy =>
                policy.RequireRole("Admin")
                      .RequireClaim("permission", "product.approve"));

            options.AddPolicy("VendorProductAccess", policy =>
                policy.RequireRole("Vendor")
                      .RequireClaim("vendor_id")); // Must have vendor_id claim

            options.AddPolicy("TaxReporting", policy =>
                policy.RequireRole("Admin", "Accountant")
                      .RequireClaim("permission", "tax.report"));

            options.AddPolicy("BulkOperations", policy =>
                policy.RequireRole("Admin")
                      .RequireClaim("permission", "bulk.operations"));

            // Multi-language content management
            options.AddPolicy("ContentManagement", policy =>
                policy.RequireRole("Admin", "ContentManager")
                      .RequireClaim("permission", "content.manage"));

            // Vietnamese marketplace custom authorization requirements
            options.AddPolicy("VendorResourceAccess", policy =>
                policy.Requirements.Add(new VendorResourceRequirement("product")));

            options.AddPolicy("ApprovalAuthority", policy =>
                policy.Requirements.Add(new ApprovalAuthorityRequirement("product.approval")));

            options.AddPolicy("TaxCompliance", policy =>
                policy.Requirements.Add(new TaxComplianceRequirement("tax.operations")));
        });

        // Register custom authorization handlers
        services.AddScoped<IAuthorizationHandler, VendorResourceHandler>();
        services.AddScoped<IAuthorizationHandler, ApprovalAuthorityHandler>();
        services.AddScoped<IAuthorizationHandler, TaxComplianceHandler>();

        return services;
    }
}
