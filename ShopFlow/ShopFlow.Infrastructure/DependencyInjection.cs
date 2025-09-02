using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Infrastructure.Persistence;
using ShopFlow.Infrastructure.Repositories;
using ShopFlow.Infrastructure.Services;
using ShopFlow.Application.Abstractions.Mappings; // nếu cần mapper interfaces
using ShopFlow.Infrastructure.Mappings;

namespace ShopFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default");
        services.AddDbContext<ShopFlowDbContext>(opt => opt.UseSqlServer(cs));

        // Repositories & UnitOfWork
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Mappers (nếu còn sử dụng)
        services.AddScoped<IUserMapper, UserMapper>();
        services.AddScoped<IProductMapper, ProductMapper>();

        return services;
    }
}
