using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using ShopFlow.API.Configurations;

namespace ShopFlow.API.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        var loggingOptions = builder.Configuration.GetSection(LoggingOptions.SectionName).Get<LoggingOptions>() ?? new LoggingOptions();

        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(Enum.Parse<LogEventLevel>(loggingOptions.Serilog.MinimumLevel))
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "ShopFlow.API")
            .Enrich.WithExceptionDetails();

        // Console sink
        if (loggingOptions.Serilog.EnableConsole)
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
        }

        // File sink
        if (loggingOptions.Serilog.EnableFile)
        {
            loggerConfiguration.WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: loggingOptions.Serilog.FilePathTemplate,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30);
        }

        // Seq sink (if enabled)
        if (loggingOptions.Serilog.EnableSeq && !string.IsNullOrEmpty(loggingOptions.Serilog.SeqServerUrl))
        {
            loggerConfiguration.WriteTo.Seq(loggingOptions.Serilog.SeqServerUrl);
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }

    public static WebApplication UseSerilogRequestLogging(this WebApplication app)
    {
        var loggingOptions = app.Configuration.GetSection(LoggingOptions.SectionName).Get<LoggingOptions>() ?? new LoggingOptions();

        if (loggingOptions.Serilog.EnableRequestLogging)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.GetLevel = (httpContext, elapsed, ex) => ex != null
                    ? LogEventLevel.Error
                    : httpContext.Response.StatusCode > 499
                        ? LogEventLevel.Error
                        : LogEventLevel.Information;

                if (loggingOptions.Serilog.EnrichFromRequest)
                {
                    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault());
                        
                        if (httpContext.User.Identity?.IsAuthenticated == true)
                        {
                            diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
                            diagnosticContext.Set("UserEmail", httpContext.User.FindFirst("email")?.Value);
                        }
                    };
                }
            });
        }

        return app;
    }
}
