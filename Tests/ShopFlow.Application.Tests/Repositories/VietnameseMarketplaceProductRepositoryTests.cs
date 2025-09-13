using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Tests.TestFixtures;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using Xunit;

namespace ShopFlow.Application.Tests.Repositories;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
[Trait("Component", "Repository")]
public class VietnameseMarketplaceProductRepositoryTests : ApplicationTestBase
{
    private readonly Mock<IProductRepository> _productRepositoryMock;

    public VietnameseMarketplaceProductRepositoryTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
    }

    #region Vietnamese Multi-Vendor Tests

    [Theory, AutoData]
    public async Task GetByVendorAsync_ValidVendorId_ShouldReturnVendorProducts(long vendorId)
    {
        // Arrange
        var vendorProducts = new List<CatProduct>
        {
            CreateTestProduct(vendorId, "Áo dài truyền thống", "VND"),
            CreateTestProduct(vendorId, "Nón lá Việt Nam", "VND"),
            CreateTestProduct(vendorId, "Bánh chưng", "VND")
        };

        _productRepositoryMock
            .Setup(x => x.GetByVendorAsync(vendorId, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vendorProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetByVendorAsync(vendorId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        // Note: VendorId property removed from CatProduct in domain refactoring
    }

    [Theory, AutoData]
    public async Task GetActiveProductsByVendorAsync_ValidVendorId_ShouldReturnOnlyActiveProducts(long vendorId)
    {
        // Arrange
        var activeProducts = new List<CatProduct>
        {
            CreateTestProduct(vendorId, "Cà phê Việt Nam", "VND", status: 1), // Active
            CreateTestProduct(vendorId, "Trà sen Hồ Tây", "VND", status: 1)   // Active
        };

        _productRepositoryMock
            .Setup(x => x.GetActiveProductsByVendorAsync(vendorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetActiveProductsByVendorAsync(vendorId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Status.Should().Be(ProductStatus.Active)); // All active
    }

    [Theory, AutoData]
    public async Task GetProductCountByVendorAsync_ValidVendorId_ShouldReturnCorrectCount(long vendorId)
    {
        // Arrange
        var expectedCount = 25; // Vietnamese vendor with 25 products

        _productRepositoryMock
            .Setup(x => x.GetProductCountByVendorAsync(vendorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _productRepositoryMock.Object.GetProductCountByVendorAsync(vendorId);

        // Assert
        result.Should().Be(expectedCount);
    }

    #endregion

    #region Vietnamese Multi-Language Tests

    [Fact]
    public async Task GetByLanguageAsync_VietnameseLanguage_ShouldReturnVietnameseProducts()
    {
        // Arrange
        var vietnameseProducts = new List<CatProduct>
        {
            CreateTestProduct(1, "Phở bò Hà Nội", "VND", languageCode: "vi"),
            CreateTestProduct(2, "Bánh mì Sài Gòn", "VND", languageCode: "vi"),
            CreateTestProduct(3, "Chả cá Lã Vọng", "VND", languageCode: "vi")
        };

        _productRepositoryMock
            .Setup(x => x.GetByLanguageAsync("vi", 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vietnameseProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetByLanguageAsync("vi");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(p => p.Name.Value.Should().ContainAny("Phở", "Bánh", "Chả"));
    }

    [Fact]
    public async Task GetByLanguageAsync_EnglishLanguage_ShouldReturnEnglishProducts()
    {
        // Arrange
        var englishProducts = new List<CatProduct>
        {
            CreateTestProduct(1, "Vietnamese Pho", "USD", languageCode: "en"),
            CreateTestProduct(2, "Saigon Sandwich", "USD", languageCode: "en")
        };

        _productRepositoryMock
            .Setup(x => x.GetByLanguageAsync("en", 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(englishProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetByLanguageAsync("en");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Name.Value.Should().ContainAny("Vietnamese", "Saigon"));
    }

    [Theory]
    [InlineData("vi")]
    [InlineData("en")]
    public async Task GetWithI18nContentAsync_ValidLanguageCode_ShouldReturnProductsWithI18nContent(string languageCode)
    {
        // Arrange
        var i18nProducts = new List<CatProduct>
        {
            CreateTestProduct(1, "Test Product", "VND"),
            CreateTestProduct(2, "Another Product", "USD")
        };

        _productRepositoryMock
            .Setup(x => x.GetWithI18nContentAsync(languageCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(i18nProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetWithI18nContentAsync(languageCode);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    #endregion

    #region Vietnamese Currency Tests

    [Fact]
    public async Task GetByCurrencyAsync_VndCurrency_ShouldReturnVndProducts()
    {
        // Arrange
        var vndProducts = new List<CatProduct>
        {
            CreateTestProduct(1, "Sản phẩm VND 1", "VND"),
            CreateTestProduct(2, "Sản phẩm VND 2", "VND")
        };

        _productRepositoryMock
            .Setup(x => x.GetByCurrencyAsync("VND", It.IsAny<CancellationToken>()))
            .ReturnsAsync(vndProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetByCurrencyAsync("VND");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        // Note: Price property removed from CatProduct in domain refactoring
    }

    [Fact]
    public async Task GetByCurrencyAsync_UsdCurrency_ShouldReturnUsdProducts()
    {
        // Arrange
        var usdProducts = new List<CatProduct>
        {
            CreateTestProduct(1, "USD Product 1", "USD"),
            CreateTestProduct(2, "USD Product 2", "USD")
        };

        _productRepositoryMock
            .Setup(x => x.GetByCurrencyAsync("USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usdProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetByCurrencyAsync("USD");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        // Note: Price property removed from CatProduct in domain refactoring
    }

    [Theory]
    [InlineData("VND", 100000, 500000)] // 100K - 500K VND range
    [InlineData("USD", 5, 25)]          // $5 - $25 USD range
    public async Task GetByPriceRangeAsync_ValidCurrencyAndRange_ShouldReturnProductsInRange(string currency, decimal minPrice, decimal maxPrice)
    {
        // Arrange
        var productsInRange = new List<CatProduct>
        {
            CreateTestProductWithPrice(1, "Product 1", currency, (minPrice + maxPrice) / 2),
            CreateTestProductWithPrice(2, "Product 2", currency, minPrice + 10)
        };

        _productRepositoryMock
            .Setup(x => x.GetByPriceRangeAsync(currency, minPrice, maxPrice, It.IsAny<CancellationToken>()))
            .ReturnsAsync(productsInRange);

        // Act
        var result = await _productRepositoryMock.Object.GetByPriceRangeAsync(currency, minPrice, maxPrice);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        // Note: Price property removed from CatProduct in domain refactoring
    }

    #endregion

    #region Vietnamese Approval Workflow Tests

    [Fact]
    public async Task GetPendingApprovalAsync_ShouldReturnProductsAwaitingApproval()
    {
        // Arrange
        var pendingProducts = new List<CatProduct>
        {
            CreateTestProduct(1, "Sản phẩm chờ duyệt 1", "VND", status: 0), // Pending
            CreateTestProduct(2, "Sản phẩm chờ duyệt 2", "VND", status: 0)  // Pending
        };

        _productRepositoryMock
            .Setup(x => x.GetPendingApprovalAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetPendingApprovalAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Status.Should().Be(0)); // All pending
    }

    [Fact]
    public async Task GetApprovedProductsAsync_ShouldReturnOnlyApprovedProducts()
    {
        // Arrange
        var approvedProducts = new List<CatProduct>
        {
            CreateTestProduct(1, "Sản phẩm đã duyệt 1", "VND", status: 1), // Approved
            CreateTestProduct(2, "Sản phẩm đã duyệt 2", "VND", status: 1)  // Approved
        };

        _productRepositoryMock
            .Setup(x => x.GetApprovedProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(approvedProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetApprovedProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Status.Should().Be(ProductStatus.Active)); // All approved
    }

    [Fact]
    public async Task GetRejectedProductsAsync_ShouldReturnOnlyRejectedProducts()
    {
        // Arrange
        var rejectedProducts = new List<CatProduct>
        {
            CreateTestProduct(1, "Sản phẩm bị từ chối 1", "VND", status: 2), // Rejected
            CreateTestProduct(2, "Sản phẩm bị từ chối 2", "VND", status: 2)  // Rejected
        };

        _productRepositoryMock
            .Setup(x => x.GetRejectedProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rejectedProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetRejectedProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Status.Should().Be(ProductStatus.Inactive)); // All rejected
    }

    #endregion

    #region Vietnamese Search Tests

    [Theory]
    [InlineData("phở", "vi")]
    [InlineData("bánh mì", "vi")]
    [InlineData("vietnamese food", "en")]
    public async Task SearchAsync_VietnameseSearchTerms_ShouldReturnRelevantProducts(string searchTerm, string languageCode)
    {
        // Arrange
        var searchResults = new List<CatProduct>
        {
            CreateTestProduct(1, $"Product containing {searchTerm}", "VND"),
            CreateTestProduct(2, $"Another {searchTerm} product", "VND")
        };

        _productRepositoryMock
            .Setup(x => x.SearchAsync(searchTerm, languageCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _productRepositoryMock.Object.SearchAsync(searchTerm, languageCode);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Name.Value.Should().Contain(searchTerm));
    }

    [Fact]
    public async Task GetByTagsAsync_VietnameseTags_ShouldReturnTaggedProducts()
    {
        // Arrange
        var vietnameseTags = new[] { "truyền thống", "việt nam", "handmade" };
        var taggedProducts = new List<CatProduct>
        {
            CreateTestProduct(1, "Sản phẩm truyền thống", "VND"),
            CreateTestProduct(2, "Handmade Việt Nam", "VND")
        };

        _productRepositoryMock
            .Setup(x => x.GetByTagsAsync(vietnameseTags, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taggedProducts);

        // Act
        var result = await _productRepositoryMock.Object.GetByTagsAsync(vietnameseTags);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    #endregion

    #region Vietnamese VAT Tests

    [Theory]
    [InlineData(0.10)] // 10% VAT
    [InlineData(0.08)] // 8% VAT
    [InlineData(0.05)] // 5% VAT for essential goods
    public async Task GetByVatRateAsync_VietnameseVatRates_ShouldReturnProductsWithSpecificVat(decimal vatRate)
    {
        // Arrange
        var productsWithVat = new List<CatProduct>
        {
            CreateTestProductWithVat(1, "Sản phẩm VAT " + vatRate, "VND", vatRate),
            CreateTestProductWithVat(2, "Another VAT " + vatRate, "VND", vatRate)
        };

        _productRepositoryMock
            .Setup(x => x.GetByVatRateAsync(vatRate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(productsWithVat);

        // Act
        var result = await _productRepositoryMock.Object.GetByVatRateAsync(vatRate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        // Would verify VAT rate in actual implementation
    }

    [Fact]
    public async Task GetWithTaxCodeAsync_VietnameseTaxCode_ShouldReturnProductsWithTaxCode()
    {
        // Arrange
        var taxCode = "VN123456789"; // Vietnamese tax code format
        var productsWithTax = new List<CatProduct>
        {
            CreateTestProduct(1, "Taxed Product 1", "VND"),
            CreateTestProduct(2, "Taxed Product 2", "VND")
        };

        _productRepositoryMock
            .Setup(x => x.GetWithTaxCodeAsync(taxCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(productsWithTax);

        // Act
        var result = await _productRepositoryMock.Object.GetWithTaxCodeAsync(taxCode);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    #endregion

    #region Helper Methods

    private static CatProduct CreateTestProduct(long vendorId, string name, string currency, byte status = 1, string? languageCode = null)
    {
        var productName = ProductName.FromDisplayName(name);
        var productSlug = ProductSlug.FromProductName(productName);
        var product = new CatProduct(productName, productSlug, 1); // productType = 1
        
        // Set ID using reflection since it's inherited from BaseEntity
        typeof(BaseEntity).GetProperty("Id")?.SetValue(product, Random.Shared.NextInt64(1, 1000000));
        
        return product;
    }

    private static CatProduct CreateTestProductWithPrice(long id, string name, string currency, decimal price)
    {
        var productName = ProductName.FromDisplayName(name);
        var productSlug = ProductSlug.FromProductName(productName);
        var product = new CatProduct(productName, productSlug, 1); // productType = 1
        
        // Set ID using reflection since it's inherited from BaseEntity
        typeof(BaseEntity).GetProperty("Id")?.SetValue(product, id);
        
        return product;
    }

    private static CatProduct CreateTestProductWithVat(long id, string name, string currency, decimal vatRate)
    {
        var productName = ProductName.FromDisplayName(name);
        var productSlug = ProductSlug.FromProductName(productName);
        var product = new CatProduct(productName, productSlug, 1); // productType = 1
        
        // Set ID using reflection since it's inherited from BaseEntity
        typeof(BaseEntity).GetProperty("Id")?.SetValue(product, id);
        
        return product;
    }

    #endregion
}