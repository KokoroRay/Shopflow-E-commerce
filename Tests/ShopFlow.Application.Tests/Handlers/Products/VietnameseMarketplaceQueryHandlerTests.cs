using AutoFixture.Xunit2;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Handlers.Products;
using ShopFlow.Application.Queries.Products;
using ShopFlow.Application.Tests.TestFixtures;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Application.Tests.Handlers.Products;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
[Trait("Component", "QueryHandler")]
public class VietnameseMarketplaceQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ILogger<GetProductsByVendorQueryHandler>> _getProductsByVendorLoggerMock;
    private readonly Mock<ILogger<GetProductsByLanguageQueryHandler>> _getProductsByLanguageLoggerMock;
    private readonly Mock<ILogger<GetProductsByPriceRangeQueryHandler>> _getProductsByPriceRangeLoggerMock;
    private readonly Mock<ILogger<SearchProductsQueryHandler>> _searchProductsLoggerMock;
    private readonly GetProductsByVendorQueryHandler _getProductsByVendorHandler;
    private readonly GetProductsByLanguageQueryHandler _getProductsByLanguageHandler;
    private readonly GetProductsByPriceRangeQueryHandler _getProductsByPriceRangeHandler;
    private readonly SearchProductsQueryHandler _searchProductsHandler;

    public VietnameseMarketplaceQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _getProductsByVendorLoggerMock = new Mock<ILogger<GetProductsByVendorQueryHandler>>();
        _getProductsByLanguageLoggerMock = new Mock<ILogger<GetProductsByLanguageQueryHandler>>();
        _getProductsByPriceRangeLoggerMock = new Mock<ILogger<GetProductsByPriceRangeQueryHandler>>();
        _searchProductsLoggerMock = new Mock<ILogger<SearchProductsQueryHandler>>();

        _getProductsByVendorHandler = new GetProductsByVendorQueryHandler(_productRepositoryMock.Object, _getProductsByVendorLoggerMock.Object);
        _getProductsByLanguageHandler = new GetProductsByLanguageQueryHandler(_productRepositoryMock.Object, _getProductsByLanguageLoggerMock.Object);
        _getProductsByPriceRangeHandler = new GetProductsByPriceRangeQueryHandler(_productRepositoryMock.Object, _getProductsByPriceRangeLoggerMock.Object);
        _searchProductsHandler = new SearchProductsQueryHandler(_productRepositoryMock.Object, _searchProductsLoggerMock.Object);
    }

    #region GetProductsByVendor Tests

    [Theory, AutoData]
    public async Task Handle_GetProductsByVendorQuery_ValidVendorId_ShouldReturnPagedResponse(
        long vendorId, int page, int pageSize)
    {
        // Arrange
        var products = new List<CatProduct>
        {
            CreateVietnameseProduct(vendorId, "Áo dài truyền thống", "VND", 450000),
            CreateVietnameseProduct(vendorId, "Nón lá Việt Nam", "VND", 120000),
            CreateVietnameseProduct(vendorId, "Bánh tráng Tây Ninh", "VND", 85000)
        };

        var totalCount = 25;
        var skip = (page - 1) * pageSize;

        _productRepositoryMock
            .Setup(x => x.GetPaginatedAsync(skip, pageSize, vendorId, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, totalCount));

        var query = new GetProductsByVendorQuery(vendorId, page, pageSize);

        // Act
        var result = await _getProductsByVendorHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.TotalCount.Should().Be(totalCount);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
        result.Data.Should().AllSatisfy(p => p.VendorId.Should().Be(vendorId));
    }

    [Theory, AutoData]
    public async Task Handle_GetProductsByVendorQuery_InvalidVendorId_ShouldReturnEmptyResponse(int page, int pageSize)
    {
        // Arrange
        var invalidVendorId = -1L;
        _productRepositoryMock
            .Setup(x => x.GetPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), invalidVendorId, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<CatProduct>(), 0));

        var query = new GetProductsByVendorQuery(invalidVendorId, page, pageSize);

        // Act
        var result = await _getProductsByVendorHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region GetProductsByLanguage Tests

    [Theory]
    [InlineData("vi", 1, 10)]
    [InlineData("en", 2, 5)]
    public async Task Handle_GetProductsByLanguageQuery_ValidLanguageCode_ShouldReturnLocalizedProducts(
        string languageCode, int page, int pageSize)
    {
        // Arrange
        var products = languageCode == "vi"
            ? new List<CatProduct>
            {
                CreateVietnameseProduct(1, "Phở bò Hà Nội", "VND", 65000),
                CreateVietnameseProduct(2, "Bánh mì thịt nướng", "VND", 25000)
            }
            : new List<CatProduct>
            {
                CreateVietnameseProduct(1, "Hanoi Beef Pho", "USD", 2.75m),
                CreateVietnameseProduct(2, "Grilled Pork Sandwich", "USD", 1.05m)
            };

        var totalCount = products.Count;
        var skip = (page - 1) * pageSize;

        _productRepositoryMock
            .Setup(x => x.GetByLanguageAsync(languageCode, skip, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _productRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        var query = new GetProductsByLanguageQuery(languageCode, page, pageSize);

        // Act
        var result = await _getProductsByLanguageHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(products.Count);
        result.TotalCount.Should().Be(totalCount);

        if (languageCode == "vi")
        {
            result.Data.Should().AllSatisfy(p => p.Name.Should().ContainAny("Phở", "Bánh"));
        }
        else
        {
            result.Data.Should().AllSatisfy(p => p.Name.Should().ContainAny("Pho", "Sandwich"));
        }
    }

    #endregion

    #region GetProductsByPriceRange Tests

    [Theory]
    [InlineData("VND", 50000, 500000, 1, 10)]
    [InlineData("USD", 2, 50, 1, 5)]
    public async Task Handle_GetProductsByPriceRangeQuery_ValidRange_ShouldReturnProductsInRange(
        string currency, decimal minPrice, decimal maxPrice, int page, int pageSize)
    {
        // Arrange
        var products = currency == "VND"
            ? new List<CatProduct>
            {
                CreateVietnameseProduct(1, "Sản phẩm VND 1", "VND", 125000),
                CreateVietnameseProduct(2, "Sản phẩm VND 2", "VND", 275000),
                CreateVietnameseProduct(3, "Sản phẩm VND 3", "VND", 450000)
            }
            : new List<CatProduct>
            {
                CreateVietnameseProduct(1, "USD Product 1", "USD", 5.50m),
                CreateVietnameseProduct(2, "USD Product 2", "USD", 12.75m),
                CreateVietnameseProduct(3, "USD Product 3", "USD", 28.99m)
            };

        var totalCount = products.Count;

        _productRepositoryMock
            .Setup(x => x.GetByPriceRangeAsync(currency, minPrice, maxPrice, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var query = new GetProductsByPriceRangeQuery(currency, minPrice, maxPrice, page, pageSize);

        // Act
        var result = await _getProductsByPriceRangeHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(products.Count);
        result.Data.Should().AllSatisfy(p =>
        {
            p.Price.Currency.Should().Be(currency);
            p.Price.Amount.Should().BeInRange(minPrice, maxPrice);
        });
    }

    [Theory]
    [InlineData("VND", 1000000, 500000)] // Invalid range (min > max)
    [InlineData("USD", 100, 50)]        // Invalid range (min > max)
    public async Task Handle_GetProductsByPriceRangeQuery_InvalidRange_ShouldReturnEmptyResponse(
        string currency, decimal minPrice, decimal maxPrice)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetByPriceRangeAsync(currency, minPrice, maxPrice, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CatProduct>());

        var query = new GetProductsByPriceRangeQuery(currency, minPrice, maxPrice, 1, 10);

        // Act
        var result = await _getProductsByPriceRangeHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region SearchProducts Tests

    [Theory]
    [InlineData("phở", "vi", 1, 10)]
    [InlineData("bánh mì", "vi", 1, 5)]
    [InlineData("vietnamese food", "en", 1, 20)]
    [InlineData("coffee", null, 1, 15)]
    public async Task Handle_SearchProductsQuery_ValidSearchTerm_ShouldReturnRelevantProducts(
        string searchTerm, string? languageCode, int page, int pageSize)
    {
        // Arrange
        var searchResults = searchTerm switch
        {
            "phở" => new List<CatProduct>
            {
                CreateVietnameseProduct(1, "Phở bò Hà Nội", "VND", 65000),
                CreateVietnameseProduct(2, "Phở gà Nam Định", "VND", 55000)
            },
            "bánh mì" => new List<CatProduct>
            {
                CreateVietnameseProduct(1, "Bánh mì thịt nướng", "VND", 25000),
                CreateVietnameseProduct(2, "Bánh mì chả cá", "VND", 30000)
            },
            "vietnamese food" => new List<CatProduct>
            {
                CreateVietnameseProduct(1, "Vietnamese Spring Rolls", "USD", 3.50m),
                CreateVietnameseProduct(2, "Vietnamese Curry", "USD", 8.75m)
            },
            "coffee" => new List<CatProduct>
            {
                CreateVietnameseProduct(1, "Cà phê sữa đá", "VND", 35000),
                CreateVietnameseProduct(2, "Vietnamese Coffee", "USD", 1.50m)
            },
            _ => new List<CatProduct>()
        };

        var totalCount = searchResults.Count;

        _productRepositoryMock
            .Setup(x => x.SearchAsync(searchTerm, languageCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResults);

        var query = new SearchProductsQuery(searchTerm, languageCode, page, pageSize);

        // Act
        var result = await _searchProductsHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(searchResults.Count);
        result.Data.Should().AllSatisfy(p =>
            p.Name.Should().Contain(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            p.Name.Should().ContainAny("Phở", "Bánh", "Vietnamese", "Coffee", "Cà phê"));
    }

    [Fact]
    public async Task Handle_SearchProductsQuery_EmptySearchTerm_ShouldReturnEmptyResponse()
    {
        // Arrange
        var emptySearchTerm = string.Empty;
        _productRepositoryMock
            .Setup(x => x.SearchAsync(emptySearchTerm, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CatProduct>());

        var query = new SearchProductsQuery(emptySearchTerm, null, 1, 10);

        // Act
        var result = await _searchProductsHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Theory]
    [InlineData("@#$%^&*()", "vi")]  // Special characters
    [InlineData("12345", "en")]      // Numbers only
    [InlineData("   ", null)]        // Whitespace only
    public async Task Handle_SearchProductsQuery_InvalidSearchTerms_ShouldReturnEmptyResponse(
        string invalidSearchTerm, string? languageCode)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.SearchAsync(invalidSearchTerm, languageCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CatProduct>());

        var query = new SearchProductsQuery(invalidSearchTerm, languageCode, 1, 10);

        // Act
        var result = await _searchProductsHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static CatProduct CreateVietnameseProduct(long vendorId, string name, string currency, decimal price)
    {
        return new CatProduct
        {
            Id = Random.Shared.NextInt64(1, 1000000),
            VendorId = vendorId,
            Name = new ProductName(name),
            Description = new ProductDescription($"Description for {name}"),
            Price = new Money(price, currency),
            Status = 1, // Active
            CategoryId = Random.Shared.NextInt64(1, 100),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    #endregion
}