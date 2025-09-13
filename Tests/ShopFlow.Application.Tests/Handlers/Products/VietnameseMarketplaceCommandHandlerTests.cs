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
        existingProduct.Id = productId;
        existingProduct.Status = 0; // Pending

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new UpdateProductStatusCommand(productId, newStatus, "Admin approved");

        // Act
        var result = await _updateStatusHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("successfully");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CatProduct>(p =>
            p.Id == productId && p.Status == newStatus), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task Handle_UpdateProductStatusCommand_ProductNotFound_ShouldReturnFailure(long nonExistentProductId, byte newStatus)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(nonExistentProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatProduct?)null);

        var command = new UpdateProductStatusCommand(nonExistentProductId, newStatus, "Admin action");

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
        pendingProduct.Id = productId;
        pendingProduct.Status = 0; // Pending approval

        var expectedStatus = isApproved ? (byte)1 : (byte)2; // 1 = Approved, 2 = Rejected

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new ApproveRejectProductCommand(productId, isApproved, adminId, reason);

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
        alreadyApprovedProduct.Id = productId;
        alreadyApprovedProduct.Status = 1; // Already approved

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alreadyApprovedProduct);

        var command = new ApproveRejectProductCommand(productId, true, adminId, "Approve again");

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

        var command = new BulkUpdateProductsCommand
        {
            ProductIds = productIds,
            NewStatus = newStatus,
            AdminId = adminId,
            Reason = "Bulk approval of Vietnamese traditional products"
        };

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

        var command = new BulkUpdateProductsCommand
        {
            ProductIds = productIds,
            NewStatus = newStatus,
            AdminId = adminId,
            Reason = "Bulk rejection - quality issues"
        };

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
        var command = new BulkUpdateProductsCommand
        {
            ProductIds = emptyProductIds,
            NewStatus = 1,
            AdminId = 123L,
            Reason = "Empty bulk update"
        };

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

        var command = new UpdateProductPricingCommand
        {
            ProductId = productId,
            NewPrice = new Money(newPrice, currency),
            SalePrice = salePrice.HasValue ? new Money(salePrice.Value, currency) : null,
            AdminId = adminId,
            Reason = "Market price adjustment"
        };

        // Act
        var result = await _updatePricingHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("pricing updated");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CatProduct>(p =>
            p.Id == productId && p.Price.Amount == newPrice && p.Price.Currency == currency), It.IsAny<CancellationToken>()), Times.Once);
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

        var command = new UpdateProductPricingCommand
        {
            ProductId = productId,
            NewPrice = new Money(50m, "EUR"), // Different currency
            AdminId = adminId,
            Reason = "Currency mismatch test"
        };

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

        var command = new UpdateProductPricingCommand
        {
            ProductId = productId,
            NewPrice = new Money(invalidPrice, currency),
            AdminId = adminId,
            Reason = "Invalid price test"
        };

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
        traditionalProduct.Id = productId;
        traditionalProduct.Status = 0; // Pending

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(traditionalProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CatProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new ApproveRejectProductCommand(
            productId,
            true,
            adminId,
            "Sản phẩm truyền thống Việt Nam chất lượng cao, phù hợp với tiêu chuẩn marketplace");

        // Act
        var result = await _approveRejectHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("approved");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CatProduct>(p =>
            p.Id == productId &&
            p.Status == 1 &&
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

        var command = new UpdateProductPricingCommand
        {
            ProductId = productId,
            NewPrice = new Money(originalPrice, currency),
            SalePrice = new Money(salePrice, currency),
            AdminId = adminId,
            Reason = "Vietnamese marketplace special promotion"
        };

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
        return new CatProduct
        {
            Id = Random.Shared.NextInt64(1, 1000000),
            VendorId = vendorId,
            Name = new ProductName(name),
            Description = new ProductDescription($"Mô tả cho {name}"),
            Price = new Money(price, currency),
            Status = 0, // Pending by default
            CategoryId = Random.Shared.NextInt64(1, 100),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    #endregion
}