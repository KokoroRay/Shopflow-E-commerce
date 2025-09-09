using Microsoft.Extensions.DependencyInjection;
using ShopFlow.Infrastructure.Persistence;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShopFlow.Integration.Tests.TestFixtures;

/// <summary>
/// Base class for integration tests
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<ShopFlowWebApplicationFactory>, IDisposable
{
    protected readonly HttpClient HttpClient;
    protected readonly ShopFlowWebApplicationFactory Factory;
    protected readonly IServiceScope Scope;
    protected readonly ShopFlowDbContext DbContext;

    protected IntegrationTestBase(ShopFlowWebApplicationFactory factory)
    {
        Factory = factory;
        HttpClient = factory.CreateClient();
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<ShopFlowDbContext>();

        // Configure JSON options for HTTP client
        ConfigureJsonOptions();
    }

    /// <summary>
    /// Configure JSON serialization options for HTTP client
    /// </summary>
    private void ConfigureJsonOptions()
    {
        // This ensures consistent JSON serialization for tests
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    /// <summary>
    /// Clean database before each test
    /// </summary>
    protected async Task CleanDatabaseAsync()
    {
        // Remove all data to ensure test isolation
        DbContext.Users.RemoveRange(DbContext.Users);
        DbContext.Products.RemoveRange(DbContext.Products);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// POST request helper with JSON content
    /// </summary>
    protected async Task<HttpResponseMessage> PostJsonAsync<T>(string requestUri, T value)
    {
        return await HttpClient.PostAsJsonAsync(requestUri, value);
    }

    /// <summary>
    /// GET request helper
    /// </summary>
    protected async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        return await HttpClient.GetAsync(requestUri);
    }

    /// <summary>
    /// Deserialize response content to specified type
    /// </summary>
    protected async Task<T?> ReadResponseAsAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public void Dispose()
    {
        Scope?.Dispose();
        HttpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}
