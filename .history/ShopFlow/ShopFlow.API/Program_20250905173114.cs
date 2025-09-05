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

// 1) Đăng ký tầng App & Infra (không đụng EFModel/DbContext ở ngoài Infra)
builder.Services.AddApplication();                      // Handlers, Validators, Mappers DTO↔Domain
builder.Services.AddInfrastructure(builder.Configuration); // DbContext, Repositories, EFModel↔Domain, PasswordHasher

// 2) Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// 3) Web stuff
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // dùng Swashbuckle để có UI

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

// 3) Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Default");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Global exception handler (middleware của bạn)
app.UseMiddleware<GlobalExceptionHandler>();

// 4) Endpoints
app.MapControllers();

// Health endpoints đơn giản (thay cho WeatherForecast demo)
app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
   .WithName("Health");
app.MapGet("/ready", () => Results.Ok(new { ready = true }))
   .WithName("Readiness");

app.Run();
