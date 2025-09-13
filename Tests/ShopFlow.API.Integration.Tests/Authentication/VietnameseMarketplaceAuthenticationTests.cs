using FluentAssertions;
using ShopFlow.API.Integration.Tests.TestFixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShopFlow.API.Integration.Tests.Authentication;

[Trait("Category", "Integration")]
[Trait("Layer", "API")]
[Trait("Component", "Authentication")]
public class VietnameseMarketplaceAuthenticationTests : VietnameseMarketplaceIntegrationTestBase
{
    public VietnameseMarketplaceAuthenticationTests(VietnameseMarketplaceWebApplicationFactory factory)
        : base(factory)
    {
    }

    #region JWT Authentication Tests

    [Fact]
    public async Task AccessProtectedEndpoint_WithValidJwt_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AccessProtectedEndpoint_WithoutJwt_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthenticationHeader();
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AccessProtectedEndpoint_WithExpiredJwt_ShouldReturnUnauthorized()
    {
        // Arrange
        var expiredToken = CreateJwtToken(101, "vendor1@shopflow.vn", "Vendor", DateTime.UtcNow.AddHours(-1));
        Client.DefaultRequestHeaders.Authorization = new("Bearer", expiredToken);
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AccessProtectedEndpoint_WithInvalidJwt_ShouldReturnUnauthorized()
    {
        // Arrange
        Client.DefaultRequestHeaders.Authorization = new("Bearer", "invalid.jwt.token");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Vietnamese Marketplace Authorization Policies Tests

    [Fact]
    public async Task VendorResourceAccess_OwnerAccess_ShouldAllow()
    {
        // Arrange - Vendor accessing their own resources
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VendorResourceAccess_NonOwnerAccess_ShouldDeny()
    {
        // Arrange - Vendor trying to access another vendor's resources
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/102");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ApprovalAuthority_AdminAccess_ShouldAllow()
    {
        // Arrange - Admin accessing approval functions
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var approvalRequest = CreateApprovalRequest(1, true, "Admin approval");

        // Act
        var response = await Client.PutAsync("/api/products/approval", CreateJsonContent(approvalRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ApprovalAuthority_VendorAccess_ShouldDeny()
    {
        // Arrange - Vendor trying to access approval functions
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var approvalRequest = CreateApprovalRequest(1, true, "Vendor approval attempt");

        // Act
        var response = await Client.PutAsync("/api/products/approval", CreateJsonContent(approvalRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task TaxCompliance_AdminAccess_ShouldAllow()
    {
        // Arrange - Admin accessing tax compliance functions
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"/api/products/tax-report?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task TaxCompliance_VendorAccess_ShouldDeny()
    {
        // Arrange - Vendor trying to access tax compliance functions
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"/api/products/tax-report?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Vietnamese Marketplace Claims Tests

    [Fact]
    public async Task MarketplaceRegionClaim_ValidRegion_ShouldAllow()
    {
        // Arrange - Vietnamese marketplace region claim
        var token = CreateJwtToken(101, "vendor1@shopflow.vn", "Vendor", extraClaims: new[]
        {
            ("MarketplaceRegion", "VN"),
            ("SupportedCurrencies", "VND,USD")
        });
        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MarketplaceRegionClaim_InvalidRegion_ShouldDeny()
    {
        // Arrange - Non-Vietnamese marketplace region claim
        var token = CreateJwtToken(101, "vendor1@shopflow.vn", "Vendor", extraClaims: new[]
        {
            ("MarketplaceRegion", "US"),
            ("SupportedCurrencies", "USD")
        });
        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SupportedCurrenciesClaim_ValidCurrency_ShouldAllow()
    {
        // Arrange - Valid currency in supported currencies claim
        var token = CreateJwtToken(101, "vendor1@shopflow.vn", "Vendor", extraClaims: new[]
        {
            ("MarketplaceRegion", "VN"),
            ("SupportedCurrencies", "VND,USD")
        });
        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/price-range?currency=VND&minPrice=50000&maxPrice=1000000");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SupportedCurrenciesClaim_InvalidCurrency_ShouldDeny()
    {
        // Arrange - Invalid currency not in supported currencies claim
        var token = CreateJwtToken(101, "vendor1@shopflow.vn", "Vendor", extraClaims: new[]
        {
            ("MarketplaceRegion", "VN"),
            ("SupportedCurrencies", "VND")
        });
        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        SetVietnameseMarketplaceHeaders("vi", "EUR", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/price-range?currency=EUR&minPrice=10&maxPrice=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Cross-Origin Resource Sharing (CORS) Tests

    [Fact]
    public async Task PreflightRequest_VietnameseMarketplaceDomain_ShouldAllow()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/products");
        request.Headers.Add("Origin", "https://shopflow.vn");
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "Authorization,Content-Type,Accept-Language,Accept-Currency");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("Access-Control-Allow-Origin");
    }

    [Fact]
    public async Task CorsRequest_InvalidOrigin_ShouldDeny()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/products");
        request.Headers.Add("Origin", "https://malicious-site.com");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.Headers.Should().NotContainKey("Access-Control-Allow-Origin");
    }

    #endregion

    #region Vietnamese Marketplace Headers Validation Tests

    [Fact]
    public async Task MissingMarketplaceHeaders_ShouldReturnBadRequest()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        // Not setting Vietnamese marketplace headers

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidLanguageHeader_ShouldReturnBadRequest()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        Client.DefaultRequestHeaders.Add("Accept-Language", "invalid-lang");
        Client.DefaultRequestHeaders.Add("Accept-Currency", "VND");
        Client.DefaultRequestHeaders.Add("X-Marketplace-Region", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidCurrencyHeader_ShouldReturnBadRequest()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        Client.DefaultRequestHeaders.Add("Accept-Language", "vi");
        Client.DefaultRequestHeaders.Add("Accept-Currency", "INVALID");
        Client.DefaultRequestHeaders.Add("X-Marketplace-Region", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidMarketplaceRegionHeader_ShouldReturnBadRequest()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        Client.DefaultRequestHeaders.Add("Accept-Language", "vi");
        Client.DefaultRequestHeaders.Add("Accept-Currency", "VND");
        Client.DefaultRequestHeaders.Add("X-Marketplace-Region", "INVALID");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/101");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Rate Limiting Tests

    [Fact]
    public async Task RapidRequests_ShouldTriggerRateLimit()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var tasks = new List<Task<HttpResponseMessage>>();

        // Act - Send 50 rapid requests
        for (int i = 0; i < 50; i++)
        {
            tasks.Add(Client.GetAsync("/api/products/vendor/101"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert - At least some requests should be rate limited
        responses.Should().Contain(r => r.StatusCode == HttpStatusCode.TooManyRequests);
    }

    #endregion

    #region Session Management Tests

    [Fact]
    public async Task ConcurrentSessions_SameUser_ShouldAllowBoth()
    {
        // Arrange
        var token1 = CreateJwtToken(101, "vendor1@shopflow.vn", "Vendor");
        var token2 = CreateJwtToken(101, "vendor1@shopflow.vn", "Vendor");

        var client1 = Factory.CreateClient();
        var client2 = Factory.CreateClient();

        client1.DefaultRequestHeaders.Authorization = new("Bearer", token1);
        client2.DefaultRequestHeaders.Authorization = new("Bearer", token2);

        SetVietnameseMarketplaceHeaders(client1, "vi", "VND", "VN");
        SetVietnameseMarketplaceHeaders(client2, "vi", "VND", "VN");

        // Act
        var response1 = await client1.GetAsync("/api/products/vendor/101");
        var response2 = await client2.GetAsync("/api/products/vendor/101");

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Helper Methods

    private void SetVietnameseMarketplaceHeaders(HttpClient client, string language, string currency, string region)
    {
        client.DefaultRequestHeaders.Add("Accept-Language", language);
        client.DefaultRequestHeaders.Add("Accept-Currency", currency);
        client.DefaultRequestHeaders.Add("X-Marketplace-Region", region);
    }

    #endregion
}