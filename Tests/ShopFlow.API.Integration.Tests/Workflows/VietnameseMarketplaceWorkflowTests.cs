using FluentAssertions;
using ShopFlow.API.Integration.Tests.TestFixtures;
using ShopFlow.Application.Contracts.Response;
using System.Net;
using System.Net.Http.Json;

namespace ShopFlow.API.Integration.Tests.Workflows;

[Trait("Category", "Integration")]
[Trait("Layer", "API")]
[Trait("Component", "VietnameseMarketplaceWorkflows")]
public class VietnameseMarketplaceWorkflowTests : VietnameseMarketplaceIntegrationTestBase
{
    public VietnameseMarketplaceWorkflowTests(VietnameseMarketplaceWebApplicationFactory factory)
        : base(factory)
    {
    }

    #region Complete Vietnamese Product Lifecycle Workflow

    [Fact]
    public async Task CompleteProductLifecycle_FromCreationToApproval_ShouldSucceed()
    {
        // Phase 1: Vendor creates Vietnamese product
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var createRequest = CreateVietnameseProductRequest("Áo dài truyền thống Huế", 2500000);
        createRequest["description"] = "Áo dài được may từ lụa tằm cao cấp, thêu tay tinh xảo theo phong cách hoàng gia Huế";
        createRequest["tags"] = new[] { "áo dài", "truyền thống", "huế", "lụa tằm", "thêu tay" };

        var createResponse = await Client.PostAsync("/api/products", CreateJsonContent(createRequest));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdProduct = await DeserializeResponse<ProductResponse>(createResponse);
        createdProduct.Should().NotBeNull();
        var productId = createdProduct!.Id;

        // Phase 2: Verify product is in pending status
        var getResponse = await Client.GetAsync($"/api/products/{productId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var product = await DeserializeResponse<ProductResponse>(getResponse);
        product!.Status.Should().Be(0); // Pending approval
        product.Name.Should().Be("Áo dài truyền thống Huế");
        product.Price.Amount.Should().Be(2500000);
        product.Price.Currency.Should().Be("VND");

        // Phase 3: Admin views pending products
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var pendingResponse = await Client.GetAsync("/api/products/pending-approval");
        pendingResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var pendingProducts = await DeserializeResponse<ProductSummaryResponse[]>(pendingResponse);
        pendingProducts.Should().NotBeNull();
        pendingProducts!.Should().Contain(p => p.Id == productId);

        // Phase 4: Admin approves the product
        var approvalRequest = CreateApprovalRequest(productId, true, "Sản phẩm áo dài chất lượng cao, phù hợp với thị trường Việt Nam. Thiết kế truyền thống đẹp mắt.");

        var approvalResponse = await Client.PutAsync("/api/products/approval", CreateJsonContent(approvalRequest));
        approvalResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Phase 5: Verify product is now approved and publicly visible
        SetAuthenticationHeader(102, "vendor2@shopflow.vn", "Vendor"); // Different vendor
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var publicResponse = await Client.GetAsync($"/api/products/{productId}");
        publicResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var approvedProduct = await DeserializeResponse<ProductResponse>(publicResponse);
        approvedProduct!.Status.Should().Be(1); // Approved
        approvedProduct.ApprovalNotes.Should().Contain("Sản phẩm áo dài chất lượng cao");

        // Phase 6: Verify product appears in Vietnamese search results
        var searchResponse = await Client.GetAsync("/api/products/search?term=áo dài&languageCode=vi&page=1&pageSize=10");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var searchResults = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(searchResponse);
        searchResults!.Data.Should().Contain(p => p.Id == productId);
    }

    #endregion

    #region Multi-Language Product Management Workflow

    [Fact]
    public async Task MultiLanguageProduct_CreateVietnameseAndEnglish_ShouldSucceed()
    {
        // Phase 1: Create Vietnamese version
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var vietnameseRequest = CreateVietnameseProductRequest("Phở bò Hà Nội", 65000);
        vietnameseRequest["description"] = "Phở bò truyền thống Hà Nội với nước dùng niêu từ xương bò 24 tiếng";

        var vietnameseResponse = await Client.PostAsync("/api/products", CreateJsonContent(vietnameseRequest));
        vietnameseResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var vietnameseProduct = await DeserializeResponse<ProductResponse>(vietnameseResponse);
        var productId = vietnameseProduct!.Id;

        // Phase 2: Create English version of the same product
        SetVietnameseMarketplaceHeaders("en", "USD", "VN");

        var englishRequest = CreateVietnameseProductRequest("Traditional Hanoi Beef Pho", 2.75m, "USD");
        englishRequest["description"] = "Authentic Hanoi beef pho with 24-hour slow-cooked bone broth";
        englishRequest["parentProductId"] = productId; // Link to Vietnamese version

        var englishResponse = await Client.PostAsync("/api/products", CreateJsonContent(englishRequest));
        englishResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Phase 3: Verify Vietnamese language search returns Vietnamese version
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var viSearchResponse = await Client.GetAsync("/api/products/language/vi?page=1&pageSize=10");
        viSearchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var viResults = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(viSearchResponse);
        viResults!.Data.Should().Contain(p => p.Name.Contains("Phở bò Hà Nội"));

        // Phase 4: Verify English language search returns English version
        SetVietnameseMarketplaceHeaders("en", "USD", "VN");

        var enSearchResponse = await Client.GetAsync("/api/products/language/en?page=1&pageSize=10");
        enSearchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var enResults = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(enSearchResponse);
        enResults!.Data.Should().Contain(p => p.Name.Contains("Traditional Hanoi Beef Pho"));
    }

    #endregion

    #region Vietnamese Currency Conversion Workflow

    [Fact]
    public async Task CurrencyConversion_VndToUsd_ShouldWorkCorrectly()
    {
        // Phase 1: Get product price in VND
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var vndResponse = await Client.GetAsync("/api/products/price-range?currency=VND&minPrice=50000&maxPrice=1000000&page=1&pageSize=5");
        vndResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var vndProducts = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(vndResponse);
        vndProducts!.Data.Should().NotBeEmpty();

        var sampleProduct = vndProducts.Data.First();
        var vndPrice = sampleProduct.Price.Amount;

        // Phase 2: Get same product in USD
        SetVietnameseMarketplaceHeaders("en", "USD", "VN");

        var productResponse = await Client.GetAsync($"/api/products/{sampleProduct.Id}");
        productResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var productInUsd = await DeserializeResponse<ProductResponse>(productResponse);
        productInUsd!.Price.Currency.Should().Be("USD");

        // Phase 3: Verify conversion is reasonable (approximate VND/USD rate)
        var expectedUsdPrice = vndPrice / 24000; // Approximate conversion rate
        var actualUsdPrice = productInUsd.Price.Amount;

        Math.Abs(actualUsdPrice - expectedUsdPrice).Should().BeLessThan(expectedUsdPrice * 0.1m); // Within 10%
    }

    #endregion

    #region Vietnamese Vendor Authorization Workflow

    [Fact]
    public async Task VendorAuthorization_AccessControl_ShouldWorkCorrectly()
    {
        // Phase 1: Vendor 1 creates a product
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var createRequest = CreateVietnameseProductRequest("Sản phẩm riêng của vendor 1", 150000);
        var createResponse = await Client.PostAsync("/api/products", CreateJsonContent(createRequest));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdProduct = await DeserializeResponse<ProductResponse>(createResponse);
        var productId = createdProduct!.Id;

        // Phase 2: Vendor 1 can access their own products
        var ownProductsResponse = await Client.GetAsync("/api/products/vendor/101");
        ownProductsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var ownProducts = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(ownProductsResponse);
        ownProducts!.Data.Should().Contain(p => p.Id == productId);

        // Phase 3: Vendor 2 cannot access Vendor 1's products
        SetAuthenticationHeader(102, "vendor2@shopflow.vn", "Vendor");

        var unauthorizedResponse = await Client.GetAsync("/api/products/vendor/101");
        unauthorizedResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Phase 4: Vendor 2 cannot update Vendor 1's product
        var updateRequest = CreateVietnameseProductRequest("Unauthorized update", 200000);
        var updateResponse = await Client.PutAsync($"/api/products/{productId}", CreateJsonContent(updateRequest));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Phase 5: Admin can access all vendor products
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");

        var adminAccessResponse = await Client.GetAsync("/api/products/vendor/101");
        adminAccessResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var adminAccessProducts = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(adminAccessResponse);
        adminAccessProducts!.Data.Should().Contain(p => p.Id == productId);
    }

    #endregion

    #region Vietnamese Tax Compliance Workflow

    [Fact]
    public async Task TaxCompliance_VietnameseVat_ShouldCalculateCorrectly()
    {
        // Phase 1: Admin generates tax report
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var taxReportResponse = await Client.GetAsync($"/api/products/tax-report?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&format=json");
        taxReportResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var taxReportContent = await taxReportResponse.Content.ReadAsStringAsync();
        taxReportContent.Should().NotBeEmpty();

        // Phase 2: Verify VAT rates for Vietnamese products
        var vatProductsResponse = await Client.GetAsync("/api/products/vat-rate/0.10"); // 10% Vietnamese VAT
        vatProductsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var vatContent = await vatProductsResponse.Content.ReadAsStringAsync();
        vatContent.Should().Contain("VAT");

        // Phase 3: Regular vendor cannot access tax reports
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");

        var unauthorizedTaxResponse = await Client.GetAsync($"/api/products/tax-report?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        unauthorizedTaxResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Vietnamese Bulk Operations Workflow

    [Fact]
    public async Task BulkOperations_Vietnamese_ShouldProcessCorrectly()
    {
        // Phase 1: Admin performs bulk approval of Vietnamese products
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Get pending products
        var pendingResponse = await Client.GetAsync("/api/products/pending-approval");
        pendingResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var pendingProducts = await DeserializeResponse<ProductSummaryResponse[]>(pendingResponse);

        if (pendingProducts != null && pendingProducts.Any())
        {
            var productIds = pendingProducts.Take(3).Select(p => p.Id).ToArray();

            // Phase 2: Bulk approve multiple products
            var bulkRequest = CreateBulkUpdateRequest(productIds, 1, "Bulk approval for Vietnamese marketplace products - meeting quality standards");

            var bulkResponse = await Client.PutAsync("/api/products/bulk-update-status", CreateJsonContent(bulkRequest));
            bulkResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Phase 3: Verify all products are now approved
            foreach (var productId in productIds)
            {
                var verifyResponse = await Client.GetAsync($"/api/products/{productId}");
                verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

                var product = await DeserializeResponse<ProductResponse>(verifyResponse);
                product!.Status.Should().Be(1); // Approved
            }
        }
    }

    #endregion

    #region Vietnamese Search and Discovery Workflow

    [Fact]
    public async Task SearchAndDiscovery_Vietnamese_ShouldWorkCorrectly()
    {
        // Phase 1: Search for Vietnamese products by category
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var categorySearchResponse = await Client.GetAsync("/api/products/search?term=thời trang&languageCode=vi&page=1&pageSize=10");
        categorySearchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var categoryResults = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(categorySearchResponse);
        categoryResults!.Data.Should().NotBeEmpty();

        // Phase 2: Search with Vietnamese diacritics
        var diacriticSearchResponse = await Client.GetAsync("/api/products/search?term=áo dài&languageCode=vi&page=1&pageSize=10");
        diacriticSearchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var diacriticResults = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(diacriticSearchResponse);
        diacriticResults!.Data.Should().NotBeEmpty();

        // Phase 3: Search without diacritics should still find results
        var noDiacriticSearchResponse = await Client.GetAsync("/api/products/search?term=ao dai&languageCode=vi&page=1&pageSize=10");
        noDiacriticSearchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var noDiacriticResults = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(noDiacriticSearchResponse);
        noDiacriticResults!.Data.Should().NotBeEmpty();

        // Phase 4: Featured products should include Vietnamese items
        var featuredResponse = await Client.GetAsync("/api/products/featured");
        featuredResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var featuredContent = await featuredResponse.Content.ReadAsStringAsync();
        featuredContent.Should().NotBeEmpty();
    }

    #endregion

    #region Vietnamese Marketplace Headers Workflow

    [Fact]
    public async Task MarketplaceHeaders_VietnameseLocalization_ShouldWorkCorrectly()
    {
        // Phase 1: Request with Vietnamese locale
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var vietnameseResponse = await Client.GetAsync("/api/products/vendor/101");
        vietnameseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var vietnameseContent = await vietnameseResponse.Content.ReadAsStringAsync();
        vietnameseContent.Should().NotBeEmpty();

        // Phase 2: Request with English locale but VN region
        SetVietnameseMarketplaceHeaders("en", "USD", "VN");

        var englishResponse = await Client.GetAsync("/api/products/vendor/101");
        englishResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var englishContent = await englishResponse.Content.ReadAsStringAsync();
        englishContent.Should().NotBeEmpty();

        // Phase 3: Verify currency consistency
        var vndPriceResponse = await Client.GetAsync("/api/products/price-range?currency=VND&minPrice=50000&maxPrice=1000000");
        vndPriceResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var usdPriceResponse = await Client.GetAsync("/api/products/price-range?currency=USD&minPrice=2&maxPrice=50");
        usdPriceResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Both should return results in their respective currencies
        var vndContent = await vndPriceResponse.Content.ReadAsStringAsync();
        var usdContent = await usdPriceResponse.Content.ReadAsStringAsync();

        vndContent.Should().Contain("VND");
        usdContent.Should().Contain("USD");
    }

    #endregion

    #region Error Recovery Workflow

    [Fact]
    public async Task ErrorRecovery_VietnameseWorkflow_ShouldHandleGracefully()
    {
        // Phase 1: Attempt invalid operation
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var invalidRequest = CreateVietnameseProductRequest("", -100); // Invalid data
        var errorResponse = await Client.PostAsync("/api/products", CreateJsonContent(invalidRequest));
        errorResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Phase 2: Correct the request and retry
        var validRequest = CreateVietnameseProductRequest("Sản phẩm hợp lệ", 100000);
        var successResponse = await Client.PostAsync("/api/products", CreateJsonContent(validRequest));
        successResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Phase 3: Verify the valid product was created
        var createdProduct = await DeserializeResponse<ProductResponse>(successResponse);
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be("Sản phẩm hợp lệ");

        // Phase 4: Verify error handling didn't affect subsequent operations
        var listResponse = await Client.GetAsync("/api/products/vendor/101");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(listResponse);
        products!.Data.Should().Contain(p => p.Name == "Sản phẩm hợp lệ");
    }

    #endregion
}