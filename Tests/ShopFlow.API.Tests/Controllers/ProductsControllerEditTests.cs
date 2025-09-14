using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using FluentAssertions;
using ShopFlow.API.Controllers;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.Contracts.Response;
using Xunit;

namespace ShopFlow.API.Tests.Controllers;

[Trait("Category", "Unit")]
[Trait("Layer", "API")]
[Trait("Component", "Controller")]
public class ProductsControllerEditTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProductsController _controller;

    public ProductsControllerEditTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ProductsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task UpdateProduct_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        var productId = 123L;
        var request = new EditProductRequest
        {
            Name = "Sản phẩm đã chỉnh sửa",
            ShortDescription = "Mô tả ngắn",
            LongDescription = "Mô tả dài",
            ProductType = 1,
            ReturnDays = 30
        };

        var expectedResponse = new ProductResponse
        {
            Success = true,
            Message = "Product updated successfully",
            Id = productId,
            Name = "Sản phẩm đã chỉnh sửa"
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<EditProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.UpdateProduct(productId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);

        _mediatorMock.Verify(x => x.Send(
            It.Is<EditProductCommand>(cmd =>
                cmd.ProductId == productId &&
                cmd.Name == request.Name &&
                cmd.ShortDescription == request.ShortDescription &&
                cmd.LongDescription == request.LongDescription &&
                cmd.ProductType == request.ProductType &&
                cmd.ReturnDays == request.ReturnDays),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ProductNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var productId = 999L;
        var request = new EditProductRequest
        {
            Name = "Sản phẩm không tồn tại"
        };

        var expectedResponse = new ProductResponse
        {
            Success = false,
            Message = "Product with ID 999 not found"
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<EditProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.UpdateProduct(productId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task UpdateProduct_ValidationFailure_ShouldReturnBadRequest()
    {
        // Arrange
        var productId = 123L;
        var request = new EditProductRequest
        {
            Name = "Test Product"
        };

        var expectedResponse = new ProductResponse
        {
            Success = false,
            Message = "Return days cannot be negative"
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<EditProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.UpdateProduct(productId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task UpdateProduct_NullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        var productId = 123L;
        EditProductRequest? request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _controller.UpdateProduct(productId, request!, CancellationToken.None));
    }
}