using FluentAssertions;
using Moq;
using ShopFlow.Application.Commands.Categories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Exceptions;
using ShopFlow.Application.Handlers.Categories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Application.Tests.Handlers.Categories;

/// <summary>
/// Unit tests for CreateCategoryCommandHandler
/// </summary>
public class CreateCategoryCommandHandlerTests : CategoryTestBase
{
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _handler = new CreateCategoryCommandHandler(MockCategoryRepository.Object, MockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCategorySuccessfully()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Name: "Electronics",
            Slug: "electronics",
            Description: "Electronic products and accessories",
            ParentId: null,
            SortOrder: 1,
            ImageUrl: null,
            IconUrl: null,
            IsActive: true
        );

        MockCategoryRepository
            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken))
            .ReturnsAsync((CatCategory category, CancellationToken _) => category);

        // Act
        var result = await _handler.Handle(command, TestCancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Electronics");
        result.Slug.Should().Be("electronics");
        result.Description.Should().Be("Electronic products and accessories");
        result.ParentId.Should().BeNull();
        result.SortOrder.Should().Be(1);
        result.ImageUrl.Should().BeNull();
        result.IconUrl.Should().BeNull();
        result.IsActive.Should().BeTrue();
        result.Status.Should().Be((byte)CategoryStatus.Active);
    }

    [Fact]
    public async Task Handle_CategoryWithParent_ShouldCreateSubcategorySuccessfully()
    {
        // Arrange
        var parentId = 123L;
        var parentCategory = CreateSimpleCategory(name: "Electronics", slug: "electronics");
        parentCategory.Id = parentId;

        var command = new CreateCategoryCommand(
            Name: "Smartphones",
            Slug: "smartphones",
            Description: "Mobile phones and smartphones",
            ParentId: parentId,
            SortOrder: 2,
            ImageUrl: null,
            IconUrl: null,
            IsActive: true
        );

        MockCategoryRepository.Reset();
        MockCategoryRepository
            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parentCategory);

        MockCategoryRepository
            .Setup(x => x.AddAsync(It.IsAny<CatCategory>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatCategory category, CancellationToken _) => category);

        // Act
        var result = await _handler.Handle(command, TestCancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Smartphones");
        result.Slug.Should().Be("smartphones");
        result.Description.Should().Be("Mobile phones and smartphones");
        result.ParentId.Should().Be(parentId);
        result.SortOrder.Should().Be(2);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DuplicateName_ShouldThrowDomainException()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Name: "Electronics",
            Slug: "electronics",
            Description: "Electronic products",
            ParentId: null,
            SortOrder: 1,
            ImageUrl: null,
            IconUrl: null,
            IsActive: true
        );

        MockCategoryRepository.Reset();
        MockCategoryRepository
            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, TestCancellationToken));

        exception.Message.Should().Contain("Category with name 'Electronics' already exists.");
    }

    [Fact]
    public async Task Handle_DuplicateSlug_ShouldThrowDomainException()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Name: "Electronics",
            Slug: "electronics",
            Description: "Electronic products",
            ParentId: null,
            SortOrder: 1,
            ImageUrl: null,
            IconUrl: null,
            IsActive: true
        );

        MockCategoryRepository.Reset();
        MockCategoryRepository
            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, TestCancellationToken));

        exception.Message.Should().Contain("Category with slug 'electronics' already exists.");
    }

    [Fact]
    public async Task Handle_InvalidParentId_ShouldThrowDomainException()
    {
        // Arrange
        var invalidParentId = 999L;
        var command = new CreateCategoryCommand(
            Name: "Smartphones",
            Slug: "smartphones",
            Description: "Mobile phones",
            ParentId: invalidParentId,
            SortOrder: 1,
            ImageUrl: null,
            IconUrl: null,
            IsActive: true
        );

        MockCategoryRepository
            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.GetByIdAsync(invalidParentId, TestCancellationToken))
            .ReturnsAsync((CatCategory?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, TestCancellationToken));

        exception.Message.Should().Contain("Parent category with ID 999 not found.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_InvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Name: invalidName,
            Slug: "electronics",
            Description: "Electronic products",
            ParentId: null,
            SortOrder: 1,
            ImageUrl: null,
            IconUrl: null,
            IsActive: true
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, TestCancellationToken));

        exception.Message.Should().Contain("Display name cannot be empty");
    }

    [Fact]
    public async Task Handle_WithImageAndIconUrls_ShouldCreateCategorySuccessfully()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Name: "Fashion",
            Slug: "fashion",
            Description: "Fashion and clothing",
            ParentId: null,
            SortOrder: 3,
            ImageUrl: "https://example.com/fashion.jpg",
            IconUrl: "https://example.com/fashion-icon.svg",
            IsActive: true
        );

        MockCategoryRepository
            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken))
            .ReturnsAsync((CatCategory category, CancellationToken _) => category);

        // Act
        var result = await _handler.Handle(command, TestCancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Fashion");
        result.Slug.Should().Be("fashion");
        result.ImageUrl.Should().Be("https://example.com/fashion.jpg");
        result.IconUrl.Should().Be("https://example.com/fashion-icon.svg");
        result.SortOrder.Should().Be(3);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_InactiveCategory_ShouldCreateWithInactiveStatus()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Name: "Discontinued",
            Slug: "discontinued",
            Description: "Discontinued products",
            ParentId: null,
            SortOrder: 999,
            ImageUrl: null,
            IconUrl: null,
            IsActive: false
        );

        MockCategoryRepository
            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))
            .ReturnsAsync(false);

        MockCategoryRepository
            .Setup(x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken))
            .ReturnsAsync((CatCategory category, CancellationToken _) => category);

        // Act
        var result = await _handler.Handle(command, TestCancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Discontinued");
        result.IsActive.Should().BeFalse();
        result.Status.Should().Be((byte)CategoryStatus.Inactive);
    }
}