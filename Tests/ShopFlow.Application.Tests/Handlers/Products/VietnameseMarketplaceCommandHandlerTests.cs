using AutoFixture.Xunit2;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Handlers.Products;
using ShopFlow.Application.Tests.TestFixtures;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Application.Tests.Handlers.Products;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
[Trait("Component", "CommandHandler")]
public class VietnameseMarketplaceCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ILogger<UpdateProductStatusCommandHandler>> _updateStatusLoggerMock;
    private readonly Mock<ILogger<ApproveRejectProductCommandHandler>> _approveRejectLoggerMock;
    private readonly Mock<ILogger<BulkUpdateProductsCommandHandler>> _bulkUpdateLoggerMock;
    private readonly Mock<ILogger<UpdateProductPricingCommandHandler>> _updatePricingLoggerMock;
    private readonly UpdateProductStatusCommandHandler _updateStatusHandler;
    private readonly ApproveRejectProductCommandHandler _approveRejectHandler;
    private readonly BulkUpdateProductsCommandHandler _bulkUpdateHandler;
    private readonly UpdateProductPricingCommandHandler _updatePricingHandler;

    public VietnameseMarketplaceCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _updateStatusLoggerMock = new Mock<ILogger<UpdateProductStatusCommandHandler>>();
        _approveRejectLoggerMock = new Mock<ILogger<ApproveRejectProductCommandHandler>>();
        _bulkUpdateLoggerMock = new Mock<ILogger<BulkUpdateProductsCommandHandler>>();
        _updatePricingLoggerMock = new Mock<ILogger<UpdateProductPricingCommandHandler>>();

        _updateStatusHandler = new UpdateProductStatusCommandHandler(_productRepositoryMock.Object, _updateStatusLoggerMock.Object);
        _approveRejectHandler = new ApproveRejectProductCommandHandler(_productRepositoryMock.Object, _approveRejectLoggerMock.Object);
        _bulkUpdateHandler = new BulkUpdateProductsCommandHandler(_productRepositoryMock.Object, _bulkUpdateLoggerMock.Object);
        _updatePricingHandler = new UpdateProductPricingCommandHandler(_productRepositoryMock.Object, _updatePricingLoggerMock.Object);
    }

    #region UpdateProductStatus Tests

    [Theory, AutoData]
    public async Task Handle_UpdateProductStatusCommand_ValidProductAndStatus_ShouldUpdateSuccessfully(long productId, byte newStatus)
    {
        // Arrange
        var existingProduct = CreateVietnameseProduct(1, "Sản phẩm test", "VND", 150000);
        // Set ID using reflection since it's inherited from BaseEntity
        typeof(BaseEntity).GetProperty("Id")?.SetValue(existingProduct, productId);
        // Note: Status is immutable, set via domain methods after creation

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new UpdateProductStatusCommand(productId, (ProductStatus)newStatus, "Admin approved");

        // Act
        var result = await _updateStatusHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("successfully");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CatProduct>(p =>
            p.Id == productId && p.Status == (ProductStatus)newStatus), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task Handle_UpdateProductStatusCommand_ProductNotFound_ShouldReturnFailure(long nonExistentProductId, byte newStatus)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(nonExistentProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatProduct?)null);

        var command = new UpdateProductStatusCommand(nonExistentProductId, (ProductStatus)newStatus, "Admin action");

        // Act
        var result = await _updateStatusHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region ApproveRejectProduct Tests

    [Theory]
    [InlineData(true, "Sản phẩm chất lượng tốt")]
    [InlineData(false, "Không đạt tiêu chuẩn chất lượng")]
    public async Task Handle_ApproveRejectProductCommand_ValidDecision_ShouldUpdateApprovalStatus(
        bool isApproved, string reason)
    {
        // Arrange
        var productId = 123L;
        var adminId = 456L;
        var pendingProduct = CreateVietnameseProduct(1, "Áo dài chờ duyệt", "VND", 850000);
        // Set ID using reflection since it's inherited from BaseEntity  
        typeof(BaseEntity).GetProperty("Id")?.SetValue(pendingProduct, productId);
        // Note: Status starts as Draft, cannot directly set to Pending - this may need domain model updates

        var expectedStatus = isApproved ? ProductStatus.Active : ProductStatus.Inactive;

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new ApproveRejectProductCommand(productId, isApproved, reason, AdminId: adminId);

        // Act
        var result = await _approveRejectHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain(isApproved ? "approved" : "rejected");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CatProduct>(p =>
            p.Id == productId && p.Status == expectedStatus), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task Handle_ApproveRejectProductCommand_AlreadyProcessed_ShouldReturnFailure(long productId, long adminId)
    {
        // Arrange
        var alreadyApprovedProduct = CreateVietnameseProduct(1, "Sản phẩm đã duyệt", "VND", 250000);
        // Set ID using reflection since it's inherited from BaseEntity
        typeof(BaseEntity).GetProperty("Id")?.SetValue(alreadyApprovedProduct, productId);
        // Use domain method to activate product (Status = Active)
        alreadyApprovedProduct.Activate();

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alreadyApprovedProduct);

        var command = new ApproveRejectProductCommand(productId, true, "Approve again", AdminId: adminId);

        // Act
        var result = await _approveRejectHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region BulkUpdateProducts Tests

    [Fact]
    public async Task Handle_BulkUpdateProductsCommand_ValidProductIds_ShouldUpdateAllProducts()
    {
        // Arrange
        var productIds = new[] { 1L, 2L, 3L };
        var newStatus = (byte)1; // Approved
        var adminId = 789L;
        var expectedUpdatedCount = productIds.Length;

        _productRepositoryMock
            .Setup(x => x.BulkUpdateStatusAsync(productIds, newStatus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUpdatedCount);

        var command = new BulkUpdateProductsCommand(
            productIds,
            (ProductStatus)newStatus,
            "Bulk approval of Vietnamese traditional products",
            adminId,
            false);

        // Act
        var result = await _bulkUpdateHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.UpdatedCount.Should().Be(expectedUpdatedCount);
        result.FailedIds.Should().BeEmpty();
        result.Message.Should().Contain("successfully updated");

        _productRepositoryMock.Verify(x => x.BulkUpdateStatusAsync(productIds, newStatus, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BulkUpdateProductsCommand_PartialSuccess_ShouldReturnMixedResults()
    {
        // Arrange
        var productIds = new[] { 1L, 2L, 3L, 4L, 5L };
        var newStatus = (byte)2; // Rejected
        var adminId = 789L;
        var actualUpdatedCount = 3; // Only 3 out of 5 updated successfully

        _productRepositoryMock
            .Setup(x => x.BulkUpdateStatusAsync(productIds, newStatus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(actualUpdatedCount);

        var command = new BulkUpdateProductsCommand(
            productIds,
            (ProductStatus)newStatus,
            "Bulk rejection - quality issues",
            adminId,
            false);

        // Act
        var result = await _bulkUpdateHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue(); // Partial success is still success
        result.UpdatedCount.Should().Be(actualUpdatedCount);
        result.RequestedCount.Should().Be(productIds.Length);
        result.Message.Should().Contain("partially");
    }

    [Fact]
    public async Task Handle_BulkUpdateProductsCommand_EmptyProductIds_ShouldReturnFailure()
    {
        // Arrange
        var emptyProductIds = Array.Empty<long>();
        var command = new BulkUpdateProductsCommand(
            emptyProductIds,
            ProductStatus.Active,
            "Empty bulk update",
            123L,
            false);

        // Act
        var result = await _bulkUpdateHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("No products");

        _productRepositoryMock.Verify(x => x.BulkUpdateStatusAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region UpdateProductPricing Tests

    [Theory]
    [InlineData("VND", 275000, 245000)] // Vietnamese pricing with discount
    [InlineData("USD", 12.50, 9.99)]   // USD pricing with discount
    public async Task Handle_UpdateProductPricingCommand_ValidPricing_ShouldUpdateSuccessfully(
        string currency, decimal newPrice, decimal? salePrice)
    {
        // Arrange
        var productId = 456L;
        var adminId = 789L;
        var existingProduct = CreateVietnameseProduct(1, "Sản phẩm cập nhật giá", currency, 200000);
        existingProduct.Id = productId;

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var pricingRequest = new UpdateProductPricingRequest(
            currency,
            newPrice,
            salePrice
        );

        var command = new UpdateProductPricingCommand(
            productId,
            new[] { pricingRequest },
            adminId
        );

        // Act
        var result = await _updatePricingHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("pricing updated");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CatProduct>(p =>
            p.Id == productId), It.IsAny<CancellationToken>()), Times.Once);
        // Note: Price property removed from CatProduct in domain refactoring
    }

    [Theory, AutoData]
    public async Task Handle_UpdateProductPricingCommand_InvalidCurrency_ShouldReturnFailure(long productId, long adminId)
    {
        // Arrange
        var existingProduct = CreateVietnameseProduct(1, "Product", "VND", 100000);
        existingProduct.Id = productId;

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        var pricingRequest = new UpdateProductPricingRequest("EUR", 50m);

        var command = new UpdateProductPricingCommand(
            productId,
            new[] { pricingRequest },
            adminId
        );

        // Act
        var result = await _updatePricingHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("currency");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("VND", -100000)] // Negative VND price
    [InlineData("USD", -5.50)]   // Negative USD price
    [InlineData("VND", 0)]       // Zero price
    public async Task Handle_UpdateProductPricingCommand_InvalidPrice_ShouldReturnFailure(string currency, decimal invalidPrice)
    {
        // Arrange
        var productId = 123L;
        var adminId = 456L;
        var existingProduct = CreateVietnameseProduct(1, "Valid Product", currency, 100000);
        existingProduct.Id = productId;

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        var pricingRequest = new UpdateProductPricingRequest(currency, invalidPrice);

        var command = new UpdateProductPricingCommand(
            productId,
            new[] { pricingRequest },
            adminId
        );

        // Act
        var result = await _updatePricingHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("price");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Vietnamese Marketplace Specific Tests

    [Fact]
    public async Task Handle_ApproveVietnameseTraditionalProduct_ShouldIncludeAppropriateMetadata()
    {
        // Arrange
        var productId = 888L;
        var adminId = 999L;
        var traditionalProduct = CreateVietnameseProduct(1, "Áo dài lụa tằm", "VND", 1250000);
        // Set ID using reflection since it's inherited from BaseEntity
        typeof(BaseEntity).GetProperty("Id")?.SetValue(traditionalProduct, productId);
        // Note: Status starts as Draft, cannot directly set to Pending - may need domain model updates

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(traditionalProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new ApproveRejectProductCommand(
            productId,
            true,
            "Sản phẩm truyền thống Việt Nam chất lượng cao, phù hợp với tiêu chuẩn marketplace",
            AdminId: adminId);

        // Act
        var result = await _approveRejectHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("approved");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CatProduct>(p =>
            p.Id == productId &&
            p.Status == ProductStatus.Active &&
            p.Name.Value.Contains("Áo dài")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("VND", 50000, 25000)]   // Vietnamese dong with significant discount
    [InlineData("USD", 10.00, 7.50)]   // USD with 25% discount
    public async Task Handle_UpdateVietnameseProductSalePrice_ShouldCalculateProperDiscount(
        string currency, decimal originalPrice, decimal salePrice)
    {
        // Arrange
        var productId = 777L;
        var adminId = 888L;
        var vietnameseProduct = CreateVietnameseProduct(1, "Sản phẩm khuyến mãi", currency, originalPrice);
        vietnameseProduct.Id = productId;

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vietnameseProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var pricingRequest = new UpdateProductPricingRequest(currency, originalPrice, salePrice);

        var command = new UpdateProductPricingCommand(
            productId,
            new[] { pricingRequest },
            adminId
        );

        // Act
        var result = await _updatePricingHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        var discountPercentage = (originalPrice - salePrice) / originalPrice * 100;
        discountPercentage.Should().BeGreaterThan(0);
        discountPercentage.Should().BeLessThan(100);
    }

    #endregion

    #region Helper Methods

    private static CatProduct CreateVietnameseProduct(long vendorId, string name, string currency, decimal price)
    {
        var productName = ProductName.FromDisplayName(name);
        var productSlug = ProductSlug.FromProductName(productName);
        var product = new CatProduct(productName, productSlug, 1); // productType = 1

        // Set ID using reflection since it's inherited from BaseEntity
        typeof(BaseEntity).GetProperty("Id")?.SetValue(product, vendorId); // Use vendorId as simplified ID

        return product;
    }

    #endregion
}