using Microsoft.Extensions.DependencyInjection;
using ShopFlow.Infrastructure.Persistence;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShopFlow.API.Integration.Tests.TestFixtures;

/// <summary>
/// Base class for Vietnamese marketplace integration tests
/// Provides common setup and utilities for API testing
/// </summary>
public abstract class VietnameseMarketplaceIntegrationTestBase : IClassFixture<VietnameseMarketplaceWebApplicationFactory>
{
    protected readonly VietnameseMarketplaceWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected readonly ShopFlowDbContext DbContext;

    protected VietnameseMarketplaceIntegrationTestBase(VietnameseMarketplaceWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<ShopFlowDbContext>();
    }

    /// <summary>
    /// Creates a JSON HTTP content for API requests
    /// </summary>
    protected static StringContent CreateJsonContent(object data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Deserializes HTTP response content to specified type
    /// </summary>
    protected static async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });
    }

    /// <summary>
    /// Creates JWT token for test authentication with Vietnamese marketplace claims
    /// </summary>
    protected string CreateTestJwtToken(long userId, string email, string role = "Vendor", bool isVietnameseMarketplace = true)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-256-bit-secret-key-for-testing-vietnamese-marketplace"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role),
            new("VendorId", userId.ToString()),
            new("IsVietnameseMarketplace", isVietnameseMarketplace.ToString()),
            new("MarketplaceRegion", "VN"),
            new("SupportedCurrencies", "VND,USD"),
            new("DefaultLanguage", "vi")
        };

        var token = new JwtSecurityToken(
            issuer: "ShopFlow.VietnameseMarketplace",
            audience: "ShopFlow.API",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Sets authentication header with Vietnamese marketplace JWT token
    /// </summary>
    protected void SetAuthenticationHeader(long userId, string email, string role = "Vendor")
    {
        var token = CreateTestJwtToken(userId, email, role);
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Clears authentication header
    /// </summary>
    protected void ClearAuthenticationHeader()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Sets Vietnamese marketplace specific headers
    /// </summary>
    protected void SetVietnameseMarketplaceHeaders(string language = "vi", string currency = "VND", string region = "VN")
    {
        Client.DefaultRequestHeaders.Add("Accept-Language", language);
        Client.DefaultRequestHeaders.Add("X-Currency", currency);
        Client.DefaultRequestHeaders.Add("X-Region", region);
        Client.DefaultRequestHeaders.Add("X-Marketplace", "vietnamese");
    }

    /// <summary>
    /// Resets the database to clean state for Vietnamese marketplace testing
    /// </summary>
    protected async Task ResetDatabaseAsync()
    {
        DbContext.Products.RemoveRange(DbContext.Products);
        DbContext.Categories.RemoveRange(DbContext.Categories);
        DbContext.Users.RemoveRange(DbContext.Users);
        await DbContext.SaveChangesAsync();

        // Re-seed with fresh test data
        var factoryType = typeof(VietnameseMarketplaceWebApplicationFactory);
        var seedMethod = factoryType.GetMethod("SeedVietnameseMarketplaceTestData",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        seedMethod?.Invoke(null, new object[] { DbContext });
    }

    /// <summary>
    /// Gets product count for specific vendor
    /// </summary>
    protected int GetProductCountByVendor(long vendorId)
    {
        return DbContext.Products.Count(p => p.VendorId == vendorId);
    }

    /// <summary>
    /// Gets products by status
    /// </summary>
    protected List<Domain.Entities.CatProduct> GetProductsByStatus(byte status)
    {
        return DbContext.Products.Where(p => p.Status == status).ToList();
    }

    /// <summary>
    /// Creates test product data for Vietnamese marketplace
    /// </summary>
    protected static object CreateVietnameseProductRequest(string name, decimal price, string currency = "VND")
    {
        return new
        {
            Name = name,
            Description = $"Mô tả cho {name}",
            Price = new { Amount = price, Currency = currency },
            CategoryId = 1,
            Tags = new[] { "việt nam", "truyền thống" }
        };
    }

    /// <summary>
    /// Creates test approval request for Vietnamese marketplace
    /// </summary>
    protected static object CreateApprovalRequest(long productId, bool isApproved, string reason)
    {
        return new
        {
            ProductId = productId,
            IsApproved = isApproved,
            Reason = reason
        };
    }

    /// <summary>
    /// Creates test bulk update request for Vietnamese marketplace
    /// </summary>
    protected static object CreateBulkUpdateRequest(long[] productIds, byte newStatus, string reason)
    {
        return new
        {
            ProductIds = productIds,
            NewStatus = newStatus,
            Reason = reason
        };
    }
}