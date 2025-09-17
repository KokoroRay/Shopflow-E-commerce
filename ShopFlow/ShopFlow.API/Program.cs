using ShopFlow.Application;           // namespace của DI extension ở Application
using ShopFlow.Infrastructure;        // namespace của DI extension ở Infrastructure
using ShopFlow.API.Middlewares;       // nơi đặt GlobalExceptionHandler (nếu là middleware)
using ShopFlow.API.Extensions;        // Authentication extensions
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog first
    builder.ConfigureSerilog();

    Log.Information("Starting ShopFlow API");

    // 1) Configuration options
    builder.Services.AddConfigurationOptions(builder.Configuration);

    // 2) Đăng ký tầng App & Infra (không đụng EFModel/DbContext ở ngoài Infra)
    builder.Services.AddApplication();                      // Handlers, Validators, Mappers DTO↔Domain
    builder.Services.AddInfrastructure(builder.Configuration); // DbContext, Repositories, EFModel↔Domain, PasswordHasher

    // 3) Authentication & Authorization
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // 4) Web stuff
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // API Versioning
    builder.Services.AddApiVersioningConfiguration();
    builder.Services.AddSwaggerWithVersioning();

    // 5) Health checks
    builder.Services.AddHealthChecksConfiguration(builder.Configuration);

    // 6) Caching
    builder.Services.AddCachingConfiguration(builder.Configuration);

    // 7) Vietnamese Marketplace
    builder.Services.AddVietnameseMarketplace();

    // (tuỳ chọn) CORS mặc định
    builder.Services.AddCors(o =>
    {
        o.AddPolicy("Default", p => p
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
    });

    // (tuỳ chọn) ProblemDetails .NET 8/9
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // Request logging
    app.UseSerilogRequestLogging();

    // 3) Pipeline
    app.UseSwaggerWithVersioning();

    app.UseHttpsRedirection();
    app.UseCors("Default");

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Vietnamese Marketplace middleware (after auth, before exception handler)
    app.UseVietnameseMarketplace();

    // Global exception handler (middleware của bạn)
    app.UseMiddleware<GlobalExceptionHandler>();

    // 4) Endpoints
    app.MapControllers();

    // Health checks
    app.UseHealthChecksConfiguration();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>
/// Program class for integration tests
/// </summary>
internal partial class Program
{
    protected Program() { }
}