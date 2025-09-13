using FluentAssertions;
using ShopFlow.API.Integration.Tests.TestFixtures;
using ShopFlow.Application.Contracts.Response;
using System.Net;
using System.Net.Http.Json;

namespace ShopFlow.API.Integration.Tests.Controllers;

[Trait("Category", "Integration")]
[Trait("Layer", "API")]
[Trait("Component", "ProductsController")]
public class VietnameseMarketplaceProductsControllerTests : VietnameseMarketplaceIntegrationTestBase
{
    public VietnameseMarketplaceProductsControllerTests(VietnameseMarketplaceWebApplicationFactory factory)
        : base(factory)
    {
    }

    #region Vietnamese Product Creation Tests

    [Fact]
    public async Task CreateProduct_ValidVietnameseProduct_ShouldReturnCreated()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var request = CreateVietnameseProductRequest("Áo dài lụa tằm", 1250000);

        // Act
        var response = await Client.PostAsync("/api/products", CreateJsonContent(request));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Áo dài lụa tằm");
        content.Should().Contain("1250000");
        content.Should().Contain("VND");
    }

    [Fact]
    public async Task CreateProduct_EnglishProductInVietnameseMarketplace_ShouldReturnCreated()
    {
        // Arrange
        SetAuthenticationHeader(102, "vendor2@shopflow.vn");
        SetVietnameseMarketplaceHeaders("en", "USD", "VN");

        var request = CreateVietnameseProductRequest("Vietnamese Silk Dress", 52.50m, "USD");

        // Act
        var response = await Client.PostAsync("/api/products", CreateJsonContent(request));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Vietnamese Silk Dress");
        content.Should().Contain("52.50");
        content.Should().Contain("USD");
    }

    [Fact]
    public async Task CreateProduct_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthenticationHeader();
        var request = CreateVietnameseProductRequest("Unauthorized Product", 100000);

        // Act
        var response = await Client.PostAsync("/api/products", CreateJsonContent(request));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Vietnamese Product Retrieval Tests

    [Fact]
    public async Task GetProductsByVendor_ValidVendorId_ShouldReturnVendorProducts()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");
        const long vendorId = 101;

        // Act
        var response = await Client.GetAsync($"/api/products/vendor/{vendorId}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(response);
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        result.Data.Should().AllSatisfy(p => p.VendorId.Should().Be(vendorId));
        result.Data.Should().Contain(p => p.Name.Contains("Áo dài"));
    }

    [Fact]
    public async Task GetProductsByLanguage_VietnameseLanguage_ShouldReturnVietnameseProducts()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/language/vi?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(response);
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        result.Data.Should().Contain(p => p.Name.Contains("Áo dài") || p.Name.Contains("Phở"));
    }

    [Fact]
    public async Task GetProductsByLanguage_EnglishLanguage_ShouldReturnEnglishProducts()
    {
        // Arrange
        SetAuthenticationHeader(103, "vendor3@shopflow.vn");
        SetVietnameseMarketplaceHeaders("en", "USD", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/language/en?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(response);
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        result.Data.Should().Contain(p => p.Name.Contains("Vietnamese"));
    }

    [Fact]
    public async Task SearchProducts_VietnameseSearchTerm_ShouldReturnRelevantProducts()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/search?term=áo dài&languageCode=vi&page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(response);
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        result.Data.Should().Contain(p => p.Name.Contains("Áo dài"));
    }

    [Fact]
    public async Task GetProductsByPriceRange_VndCurrency_ShouldReturnProductsInRange()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/price-range?currency=VND&minPrice=50000&maxPrice=1000000&page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(response);
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        result.Data.Should().AllSatisfy(p =>
        {
            p.Price.Currency.Should().Be("VND");
            p.Price.Amount.Should().BeInRange(50000, 1000000);
        });
    }

    #endregion

    #region Vietnamese Product Approval Workflow Tests

    [Fact]
    public async Task GetProductsPendingApproval_AsAdmin_ShouldReturnPendingProducts()
    {
        // Arrange
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/pending-approval");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await DeserializeResponse<ProductSummaryResponse[]>(response);
        result.Should().NotBeNull();
        result!.Should().NotBeEmpty();
        result.Should().Contain(p => p.Name.Contains("Vietnamese Traditional Dress"));
    }

    [Fact]
    public async Task UpdateProductApproval_ApproveProduct_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var approvalRequest = CreateApprovalRequest(3, true, "Sản phẩm chất lượng tốt, phù hợp với thị trường Việt Nam");

        // Act
        var response = await Client.PutAsync("/api/products/approval", CreateJsonContent(approvalRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("approved");

        // Verify product status updated in database
        var product = DbContext.Products.First(p => p.Id == 3);
        product.Status.Should().Be(1); // Approved
    }

    [Fact]
    public async Task UpdateProductApproval_RejectProduct_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var rejectionRequest = CreateApprovalRequest(3, false, "Không đạt tiêu chuẩn chất lượng cho thị trường Việt Nam");

        // Act
        var response = await Client.PutAsync("/api/products/approval", CreateJsonContent(rejectionRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("rejected");

        // Verify product status updated in database
        var product = DbContext.Products.First(p => p.Id == 3);
        product.Status.Should().Be(2); // Rejected
    }

    [Fact]
    public async Task UpdateProductApproval_AsVendor_ShouldReturnForbidden()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn", "Vendor");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var approvalRequest = CreateApprovalRequest(3, true, "Vendor trying to approve");

        // Act
        var response = await Client.PutAsync("/api/products/approval", CreateJsonContent(approvalRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Vietnamese Bulk Operations Tests

    [Fact]
    public async Task BulkUpdateProductStatuses_AsAdmin_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var bulkRequest = CreateBulkUpdateRequest(new long[] { 1, 2 }, 1, "Bulk approval cho sản phẩm Việt Nam");

        // Act
        var response = await Client.PutAsync("/api/products/bulk-update-status", CreateJsonContent(bulkRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Bulk update");

        // Verify products updated in database
        var products = DbContext.Products.Where(p => new[] { 1L, 2L }.Contains(p.Id)).ToList();
        products.Should().AllSatisfy(p => p.Status.Should().Be(1));
    }

    #endregion

    #region Vietnamese VAT and Tax Tests

    [Fact]
    public async Task GetProductsByVatRate_VietnameseVatRate_ShouldReturnProductsWithVat()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vat-rate/0.10"); // 10% VAT

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("VAT rate");
    }

    [Fact]
    public async Task ExportTaxReport_VietnameseTaxCompliance_ShouldReturnReport()
    {
        // Arrange
        SetAuthenticationHeader(999, "admin@shopflow.vn", "Admin");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"/api/products/tax-report?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&format=json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("tax report");
    }

    #endregion

    #region Multi-Currency Tests

    [Theory]
    [InlineData("VND", 50000, 1000000)]
    [InlineData("USD", 2, 50)]
    public async Task GetProductsByPriceRange_MultiCurrency_ShouldReturnCorrectProducts(
        string currency, decimal minPrice, decimal maxPrice)
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", currency, "VN");

        // Act
        var response = await Client.GetAsync($"/api/products/price-range?currency={currency}&minPrice={minPrice}&maxPrice={maxPrice}&page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await DeserializeResponse<PagedResponse<ProductSummaryResponse>>(response);
        result.Should().NotBeNull();

        if (result!.Data.Any())
        {
            result.Data.Should().AllSatisfy(p =>
            {
                p.Price.Currency.Should().Be(currency);
                p.Price.Amount.Should().BeInRange(minPrice, maxPrice);
            });
        }
    }

    #endregion

    #region Authentication and Authorization Tests

    [Fact]
    public async Task GetProductsByVendor_DifferentVendor_ShouldReturnForbidden()
    {
        // Arrange - Vendor 101 trying to access Vendor 102's products
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/vendor/102?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProductVariants_ValidProduct_ShouldReturnVariants()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/1/variants");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("variants");
    }

    [Fact]
    public async Task GetFeaturedProducts_VietnameseMarketplace_ShouldReturnFeaturedProducts()
    {
        // Arrange
        SetAuthenticationHeader(101, "vendor1@shopflow.vn");
        SetVietnameseMarketplaceHeaders("vi", "VND", "VN");

        // Act
        var response = await Client.GetAsync("/api/products/featured");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("featured");
    }

    #endregion
}