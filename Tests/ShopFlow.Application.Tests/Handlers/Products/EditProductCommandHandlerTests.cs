using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Handlers.Products;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Tests.TestFixtures;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Application.Tests.Handlers.Products;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
[Trait("Component", "Handler")]
public class EditProductCommandHandlerTests : ApplicationTestBase
{
    private readonly EditProductCommandHandler _handler;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ILogger<EditProductCommandHandler>> _loggerMock;

    public EditProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<EditProductCommandHandler>>();
        _handler = new EditProductCommandHandler(_productRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidEditProductCommand_ShouldUpdateSuccessfully()
    {
        // Arrange
        var productId = 123L;
        var existingProduct = CreateTestProduct(productId, "Sản phẩm cũ", "VND");
        var command = new EditProductCommand(
            ProductId: productId,
            Name: "Sản phẩm đã chỉnh sửa",
            ShortDescription: "Mô tả ngắn mới",
            LongDescription: "Mô tả dài mới",
            ProductType: 2,
            ReturnDays: 30,
            VendorId: 1L
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("updated successfully");
        result.Name.Should().Be("Sản phẩm đã chỉnh sửa");

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<CatProduct>(p => p.Name.Value == "Sản phẩm đã chỉnh sửa"), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ShouldReturnFailure()
    {
        // Arrange
        var productId = 999L;
        var command = new EditProductCommand(
            ProductId: productId,
            Name: "Sản phẩm không tồn tại",
            VendorId: 1L
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatProduct?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DiscontinuedProduct_ShouldReturnFailure()
    {
        // Arrange
        var productId = 456L;
        var discontinuedProduct = CreateTestProduct(productId, "Sản phẩm đã ngừng", "VND");
        discontinuedProduct.Discontinue(); // Make product discontinued

        var command = new EditProductCommand(
            ProductId: productId,
            Name: "Cố gắng chỉnh sửa sản phẩm đã ngừng",
            VendorId: 1L
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discontinuedProduct);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("discontinued");

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-30)]
    [InlineData(-100)]
    public async Task Handle_InvalidReturnDays_ShouldReturnFailure(int invalidReturnDays)
    {
        // Arrange
        var productId = 789L;
        var existingProduct = CreateTestProduct(productId, "Sản phẩm test", "VND");
        var command = new EditProductCommand(
            ProductId: productId,
            Name: "Sản phẩm với return days không hợp lệ",
            ReturnDays: invalidReturnDays,
            VendorId: 1L
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("negative");

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("Áo dài truyền thống Việt Nam")]
    [InlineData("Phở bò Hà Nội")]
    [InlineData("Bánh mì Sài Gòn")]
    public async Task Handle_VietnameseProductNames_ShouldUpdateSuccessfully(string vietnameseName)
    {
        // Arrange
        var productId = 123L;
        var existingProduct = CreateTestProduct(productId, "Tên cũ", "VND");
        var command = new EditProductCommand(
            ProductId: productId,
            Name: vietnameseName,
            VendorId: 1L
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Name.Should().Be(vietnameseName);

        _productRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<CatProduct>(p => p.Name.Value == vietnameseName), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #region Helper Methods

    private static CatProduct CreateTestProduct(long productId, string name, string currency)
    {
        var productName = ProductName.FromDisplayName(name);
        var productSlug = ProductSlug.FromProductName(productName);
        var product = new CatProduct(productName, productSlug, 1);
        
        // Set ID using reflection since it's inherited from BaseEntity
        typeof(BaseEntity).GetProperty("Id")?.SetValue(product, productId);
        
        return product;
    }

    #endregion
}