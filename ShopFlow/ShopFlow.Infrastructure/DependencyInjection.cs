using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Abstractions.Messaging;
using ShopFlow.Application.Abstractions.Services;
using ShopFlow.Domain.Infrastructure;
using ShopFlow.Infrastructure.Persistence;
using ShopFlow.Infrastructure.Repositories;
using ShopFlow.Infrastructure.Services;
using ShopFlow.Infrastructure.Mappings;

namespace ShopFlow.Infrastructure;

/// <summary>
/// Extension methods for IServiceCollection to configure Infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Infrastructure layer services to the DI container
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="config">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default");
        services.AddDbContext<ShopFlowDbContext>(opt => opt.UseSqlServer(cs));

        // Add Memory Cache for password reset tokens
        services.AddMemoryCache();

        // Generic Repositories & UnitOfWork (keep for backward compatibility)
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Domain-specific Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        // Repositories
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenInMemoryRepository>();

        // Domain Event Publisher
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IPageRoutingService, PageRoutingService>();
        services.AddHttpContextAccessor();

        // Mappers
        services.AddScoped<IUserMapper, UserMapper>();
        services.AddScoped<IProductMapper, ProductMapper>();
        services.AddScoped<ICategoryMapper, CategoryMapper>();

        // Cache Service
        services.AddScoped<ICacheService, CacheService>();

        // Email and OTP Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IOtpService, OtpService>();

        // Domain Event Dispatcher
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        // Vietnamese Marketplace Services
        ConfigureVietnameseMarketplaceServices(services);

        return services;
    }

    /// <summary>
    /// Configures Vietnamese marketplace specific services
    /// </summary>
    /// <param name="services">The service collection</param>
    private static void ConfigureVietnameseMarketplaceServices(IServiceCollection services)
    {
        // Note: Authorization handlers are registered in API layer AuthenticationExtensions

        // Vietnamese Text Processing Services (Phase 3)
        // Multi-Currency Services (Phase 3)
        // Vietnamese Tax Compliance Services (Phase 3)
        // Product Approval Workflow Services (Phase 3)
        // Search and Analytics Services (Phase 4)

        // Caching for Vietnamese Marketplace Performance
        ConfigureVietnameseMarketplaceCaching(services);

        // Background Services for Vietnamese Marketplace (Phase 3)
    }

    /// <summary>
    /// Configures caching strategies for Vietnamese marketplace
    /// </summary>
    /// <param name="services">The service collection</param>
    private static void ConfigureVietnameseMarketplaceCaching(IServiceCollection services)
    {
        // Enhanced Memory Cache for Vietnamese marketplace
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024; // Limit cache size for performance
            options.CompactionPercentage = 0.25; // Remove 25% when limit reached
        });

        // Distributed Cache for multi-instance support (Phase 3)
        // Cache Key Generators for Vietnamese marketplace (Phase 3)
    }
}
