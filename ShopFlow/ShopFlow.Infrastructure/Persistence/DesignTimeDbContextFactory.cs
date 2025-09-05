using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ShopFlow.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ShopFlowDbContext>
{
    public ShopFlowDbContext CreateDbContext(string[] args)
    {
        var current = Directory.GetCurrentDirectory();
        var apiDir = Path.GetFullPath(Path.Combine(current, "..", "ShopFlow.API"));
        var basePath = File.Exists(Path.Combine(apiDir, "appsettings.json")) ? apiDir : current;

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddUserSecrets<DesignTimeDbContextFactory>(optional: true)
            .Build();

        var cs = config.GetConnectionString("Default")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:Default.");

        var options = new DbContextOptionsBuilder<ShopFlowDbContext>()
            .UseSqlServer(cs)
            .Options;

        return new ShopFlowDbContext(options);
    }
}