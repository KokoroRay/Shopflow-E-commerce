using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopFlow.Infrastructure.Persistence;

namespace ShopFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default");
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(cs, x => x.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Đăng ký repositories/services implement từ Infrastructure...
        // services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
