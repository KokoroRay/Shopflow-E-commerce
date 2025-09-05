using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using ShopFlow.Infrastructure.Persistence;

namespace ShopFlow.API.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        // Database health check
        var connectionString = configuration.GetConnectionString("Default");
        if (!string.IsNullOrEmpty(connectionString))
        {
            healthChecksBuilder.AddDbContextCheck<ShopFlowDbContext>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "database", "sql" });
        }

        // Application health check
        healthChecksBuilder.AddCheck("self", () => HealthCheckResult.Healthy("Application is running"), tags: new[] { "self" });

        // Memory health check
        healthChecksBuilder.AddCheck("memory", () =>
        {
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var memoryMB = allocated / 1024 / 1024;
            
            return memoryMB < 200 
                ? HealthCheckResult.Healthy($"Memory usage: {memoryMB} MB")
                : HealthCheckResult.Unhealthy($"Memory usage is too high: {memoryMB} MB");
        }, tags: new[] { "memory" });

        // Add health checks UI
        services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(15); // Configures the UI to poll for healthchecks updates every 15 seconds
            options.MaximumHistoryEntriesPerEndpoint(60); // Maximum history entries per endpoint that will be served by the UI api
            options.SetApiMaxActiveRequests(1); // Api maximum active requests concurrency

            options.AddHealthCheckEndpoint("ShopFlow API", "/health");
        }).AddInMemoryStorage();

        return services;
    }

    public static WebApplication UseHealthChecksConfiguration(this WebApplication app)
    {
        // Health checks endpoints
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Health checks UI
        if (app.Environment.IsDevelopment())
        {
            app.MapHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui"; // Health checks UI path
                options.ApiPath = "/health-ui-api"; // Health checks UI API path
            });
        }

        return app;
    }
}
