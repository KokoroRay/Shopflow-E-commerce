using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Handlers.Products;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Tests.Handlers.Products;

/// <summary>
/// Unit tests for CreateProductCommandHandler with Vietnamese marketplace support
/// </summary>
public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidPhysicalProduct_ShouldCreateSuccessfully()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Áo thun Vietnamese Designer",
            ShortDescription: "Áo thun thiết kế Việt Nam cao cấp",
            LongDescription: "Áo thun được thiết kế bởi designer Việt Nam, chất liệu cotton 100%",
            Slug: "ao-thun-vietnamese-designer",
            ProductType: (byte)1, // Physical
            ReturnDays: 30,
            CategoryIds: new List<long> { 1L, 2L },
            VendorId: 1L,
            PrimaryLanguage: "vi",
            SecondaryLanguageContent: null,
            Pricing: new List<CreateProductPricingRequest>
            {
                new("VND", 299000m, null, null, null, null, null)
            },
            Variants: null,
            MetaTitle: "Áo thun designer Việt Nam",
            MetaDescription: "Áo thun cao cấp từ designer Việt Nam",
            Tags: new List<string> { "fashion", "vietnamese", "designer" },
            VatRate: 0.1m,
            IsVatIncluded: true,
            TaxCode: "1234567890",
            AdminNotes: null,
            RequestImmediateReview: false
        );

        var expectedResponse = new ProductResponse
        {
            Success = true,
            Id = 123L,
            Name = command.Name,
            ShortDescription = command.ShortDescription,
            LongDescription = command.LongDescription,
            VendorId = command.VendorId,
            ReturnDays = command.ReturnDays
        };

        // Setup category repository to return valid categories
        foreach (var categoryId in command.CategoryIds)
        {
            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockCategory(categoryId));
        }

        _productRepositoryMock
            .Setup(x => x.ExistsByNameAndVendorAsync(It.IsAny<ProductName>(), command.VendorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatProduct p, CancellationToken _) => 
            {
                // Simulate setting ID after save
                p.GetType().GetProperty("Id")?.SetValue(p, 123L);
                return p;
            });

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(123L);
        result.Name.Should().Be(command.Name);
        result.ShortDescription.Should().Be(command.ShortDescription);
        result.Slug.Should().Be(command.Slug);
        result.ReturnDays.Should().Be(command.ReturnDays);

        // Verify repository calls
        _productRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCategoryId_ShouldThrowDomainException()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Áo thun Vietnamese Designer",
            ShortDescription: "Áo thun thiết kế Việt Nam cao cấp",
            LongDescription: "Áo thun được thiết kế bởi designer Việt Nam, chất liệu cotton 100%",
            Slug: "ao-thun-vietnamese-designer",
            ProductType: (byte)1, // Physical
            ReturnDays: 30,
            CategoryIds: new List<long> { 999L }, // Invalid category ID
            VendorId: 1L,
            PrimaryLanguage: "vi",
            SecondaryLanguageContent: null,
            Pricing: new List<CreateProductPricingRequest>
            {
                new("VND", 299000m, null, null, null, null, null)
            },
            Variants: null,
            MetaTitle: null,
            MetaDescription: null,
            Tags: null,
            VatRate: 0.1m,
            IsVatIncluded: true,
            TaxCode: null,
            AdminNotes: null,
            RequestImmediateReview: false
        );

        // Setup category repository to return null for invalid category
        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(999L, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatCategory?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Application.Exceptions.DomainException>(() => 
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain("Category with ID 999 not found");

        // Verify repository calls
        _categoryRepositoryMock.Verify(
            x => x.GetByIdAsync(999L, It.IsAny<CancellationToken>()),
            Times.Once);
        _productRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("VND", (byte)1)] // Physical VND
    [InlineData("USD", (byte)2)] // Digital USD
    [InlineData("VND", (byte)3)] // Service VND
    public async Task Handle_DifferentCurrenciesAndTypes_ShouldCreateSuccessfully(string currency, byte productType)
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: $"Test Product {currency} {productType}",
            ShortDescription: "Test description",
            LongDescription: "Test long description",
            Slug: $"test-product-{currency.ToLower()}-{productType}",
            ProductType: productType,
            ReturnDays: productType == 2 ? null : 30, // Digital products don't have return policy
            CategoryIds: new List<long> { 1L },
            VendorId: 1L,
            PrimaryLanguage: "vi",
            SecondaryLanguageContent: null,
            Pricing: new List<CreateProductPricingRequest>
            {
                new(currency, currency == "VND" ? 100000m : 50m, null, null, null, null, null)
            },
            Variants: null,
            MetaTitle: null,
            MetaDescription: null,
            Tags: null,
            VatRate: 0.1m,
            IsVatIncluded: true,
            TaxCode: null,
            AdminNotes: null,
            RequestImmediateReview: false
        );

        var expectedResponse = new ProductResponse
        {
            Success = true,
            Id = 456L,
            Name = command.Name,
            VendorId = command.VendorId
        };

        // Setup category repository to return valid categories
        foreach (var categoryId in command.CategoryIds)
        {
            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockCategory(categoryId));
        }

        _productRepositoryMock
            .Setup(x => x.ExistsByNameAndVendorAsync(It.IsAny<ProductName>(), command.VendorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatProduct p, CancellationToken _) => 
            {
                p.GetType().GetProperty("Id")?.SetValue(p, 456L);
                return p;
            });

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(456L);
        result.Name.Should().Be(command.Name);
        result.VendorId.Should().Be(command.VendorId);
    }

    private static CatCategory CreateMockCategory(long id)
    {
        var categoryName = CategoryName.FromDisplayName($"Category {id}");
        var categorySlug = new CategorySlug($"category-{id}");
        
        return new CatCategory(
            categoryName,
            categorySlug,
            $"Description for category {id}",
            null,
            0
        );
    }

    private static CreateProductCommand CreateValidCommand(string name, long vendorId) =>
        new(
            Name: name,
            ShortDescription: "Test description",
            LongDescription: "Test long description",
            Slug: name.ToLower().Replace(" ", "-"),
            ProductType: (byte)1,
            ReturnDays: 30,
            CategoryIds: new List<long> { 1L },
            VendorId: vendorId,
            PrimaryLanguage: "vi",
            SecondaryLanguageContent: null,
            Pricing: new List<CreateProductPricingRequest>
            {
                new("VND", 100000m, null, null, null, null, null)
            },
            Variants: null,
            MetaTitle: null,
            MetaDescription: null,
            Tags: null,
            VatRate: 0.1m,
            IsVatIncluded: true,
            TaxCode: null,
            AdminNotes: null,
            RequestImmediateReview: false
        );
}