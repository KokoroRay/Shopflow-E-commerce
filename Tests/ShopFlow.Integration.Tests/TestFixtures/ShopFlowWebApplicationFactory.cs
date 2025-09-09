using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShopFlow.Infrastructure.Persistence;

namespace ShopFlow.Integration.Tests.TestFixtures;

/// <summary>
/// Custom WebApplicationFactory for integration tests
/// </summary>
public class ShopFlowWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove all Entity Framework related registrations
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.Namespace != null &&
                           (d.ServiceType.Namespace.Contains("Microsoft.EntityFrameworkCore") ||
                            d.ServiceType == typeof(ShopFlowDbContext) ||
                            d.ServiceType == typeof(DbContextOptions<ShopFlowDbContext>)))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database for testing
            services.AddDbContext<ShopFlowDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
        });
    }
}
