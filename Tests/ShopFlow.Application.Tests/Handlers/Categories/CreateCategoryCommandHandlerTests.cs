using FluentAssertions;
using FluentAssertions;

using Moq;
using Moq;

using ShopFlow.Application.Commands.Categories;
using ShopFlow.Application.Commands.Categories;

using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Contracts.Response;

using ShopFlow.Application.Exceptions;
using ShopFlow.Application.Exceptions;

using ShopFlow.Application.Handlers.Categories;
using ShopFlow.Application.Handlers.Categories;

using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Entities;

using ShopFlow.Domain.Enums;
using ShopFlow.Domain.Enums;

using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.ValueObjects;

using Xunit;
using Xunit;



namespace ShopFlow.Application.Tests.Handlers.Categories;

namespace ShopFlow.Application.Tests.Handlers.Categories;



/// <summary>/// <summary>

/// Unit tests for CreateCategoryCommandHandler/// Unit tests for CreateCategoryCommandHandler

/// </summary>/// </summary>

public class CreateCategoryCommandHandlerTests : CategoryTestBasepublic class CreateCategoryCommandHandlerTests : CategoryTestBase

{{

    private readonly CreateCategoryCommandHandler _handler; private readonly CreateCategoryCommandHandler _handler;



    public CreateCategoryCommandHandlerTests()    public CreateCategoryCommandHandlerTests()

    {
        {

            _handler = new CreateCategoryCommandHandler(MockCategoryRepository.Object, MockUnitOfWork.Object); _handler = new CreateCategoryCommandHandler(MockCategoryRepository.Object, MockUnitOfWork.Object);

        }
    }



    [Fact]
    [Fact]

    public async Task Handle_ValidCommand_ShouldCreateCategorySuccessfully()    public async Task Handle_ValidCommand_ShouldCreateCategorySuccessfully()

    {
        {

            // Arrange        // Arrange

            var command = new CreateCategoryCommand(var command = new CreateCategoryCommand(

                Name: "Electronics", Name: "Electronics",

                Slug: "electronics", Slug: "electronics",

                Description: "Electronic products and accessories", Description: "Electronic products and accessories",

                ParentId: null, ParentId: null,

                SortOrder: 1, SortOrder: 1,

                ImageUrl: null, ImageUrl: null,

                IconUrl: null, IconUrl: null,

                IsActive: true            IsActive: true

            );        );



            MockCategoryRepository MockCategoryRepository

            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken)).Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))

            .ReturnsAsync(false);            .ReturnsAsync(false);



            MockCategoryRepository MockCategoryRepository

            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken)).Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))

            .ReturnsAsync(false);            .ReturnsAsync(false);



            MockCategoryRepository MockCategoryRepository

            .Setup(x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken)).Setup(x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken))

            .ReturnsAsync((CatCategory category, CancellationToken _) => category);            .Returns(Task.CompletedTask);



            // Act        // Act

            var result = await _handler.Handle(command, TestCancellationToken); var result = await _handler.Handle(command, TestCancellationToken);



            // Assert        // Assert

            result.Should().NotBeNull(); result.Should().NotBeNull();

            result.Name.Should().Be("Electronics"); result.Name.Should().Be("Electronics");

            result.Slug.Should().Be("electronics"); result.Slug.Should().Be("electronics");

            result.Description.Should().Be("Electronic products and accessories"); result.Description.Should().Be("Electronic products and accessories");

            result.ParentId.Should().BeNull(); result.ParentId.Should().BeNull();

            result.SortOrder.Should().Be(1); result.SortOrder.Should().Be(1);

            result.IsActive.Should().BeTrue(); result.IsActive.Should().BeTrue();



            MockCategoryRepository.Verify(MockCategoryRepository.Verify(

                x => x.ExistsByNameAsync(It.Is<CategoryName>(n => n.Value == "Electronics"), TestCancellationToken), x => x.ExistsByNameAsync(It.Is<CategoryName>(n => n.Value == "Electronics"), TestCancellationToken),

                Times.Once); Times.Once);



            MockCategoryRepository.Verify(MockCategoryRepository.Verify(

                x => x.ExistsBySlugAsync(It.Is<CategorySlug>(s => s.Value == "electronics"), TestCancellationToken), x => x.ExistsBySlugAsync(It.Is<CategorySlug>(s => s.Value == "electronics"), TestCancellationToken),

                Times.Once); Times.Once);



            MockCategoryRepository.Verify(MockCategoryRepository.Verify(

                x => x.AddAsync(It.Is<CatCategory>(c => x => x.AddAsync(It.Is<CatCategory>(c =>

                    c.Name.Value == "Electronics" && c.Name.Value == "Electronics" &&

                    c.Slug.Value == "electronics" && c.Slug.Value == "electronics" &&

                    c.Description == "Electronic products and accessories" && c.Description == "Electronic products and accessories" &&

                    c.ParentId == null && c.ParentId == null &&

                    c.SortOrder == 1 && c.SortOrder == 1 &&

                    c.Status == CategoryStatus.Active), TestCancellationToken), c.Status == CategoryStatus.Active), TestCancellationToken),

                Times.Once); Times.Once);



            MockUnitOfWork.Verify(MockUnitOfWork.Verify(

                x => x.SaveChangesAsync(TestCancellationToken), x => x.SaveChangesAsync(TestCancellationToken),

                Times.Once); Times.Once);

        }
    }



    [Fact]
    [Fact]

    public async Task Handle_DuplicateName_ShouldThrowDomainException()    public async Task Handle_NoSlugProvided_ShouldGenerateSlugFromName()

    {
        {

            // Arrange        // Arrange

            var command = new CreateCategoryCommand(var command = new CreateCategoryCommand(

                Name: "Electronics", Name: "Electronics & Technology",

                Slug: "electronics", Slug: null,

                Description: "Electronic products", Description: "Electronic products",

                ParentId: null, ParentId: null,

                SortOrder: 1, SortOrder: 1,

                ImageUrl: null, ImageUrl: null,

                IconUrl: null, IconUrl: null,

                IsActive: true            IsActive: true

            );        );



            MockCategoryRepository MockCategoryRepository

            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken)).Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))

            .ReturnsAsync(true);            .ReturnsAsync(false);



            // Act & Assert        MockCategoryRepository

            var exception = await Assert.ThrowsAsync<DomainException>(() =>             .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))
    

                _handler.Handle(command, TestCancellationToken));            .ReturnsAsync(false);



            exception.Message.Should().Contain("Category with name 'Electronics' already exists"); MockCategoryRepository

                .Setup(x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken))

        MockCategoryRepository.Verify(            .Returns(Task.CompletedTask);

            x => x.ExistsByNameAsync(It.Is<CategoryName>(n => n.Value == "Electronics"), TestCancellationToken),

            Times.Once);        // Act

            var result = await _handler.Handle(command, TestCancellationToken);

            MockCategoryRepository.Verify(

                x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken),        // Assert

                Times.Never); result.Should().NotBeNull();

            result.Name.Should().Be("Electronics & Technology");

            MockUnitOfWork.Verify(result.Slug.Should().Be("electronics-technology"); // Generated from name

            x => x.SaveChangesAsync(TestCancellationToken),

            Times.Never); MockCategoryRepository.Verify(

    }
        x => x.AddAsync(It.Is<CatCategory>(c =>

                c.Slug.Value == "electronics-technology"), TestCancellationToken),

    [Fact] Times.Once);

    public async Task Handle_DuplicateSlug_ShouldThrowDomainException()    }

    {

        // Arrange    [Fact]

        var command = new CreateCategoryCommand(    public async Task Handle_CategoryWithParent_ShouldCreateSubcategorySuccessfully()

            Name: "New Electronics",    {

            Slug: "electronics",        // Arrange

            Description: "Electronic products",        var parentId = 123L;

ParentId: null,        var parentCategory = CreateSimpleCategory(name: "Electronics", slug: "electronics");

SortOrder: 1,        

            ImageUrl: null,        var command = new CreateCategoryCommand(

            IconUrl: null, Name: "Smartphones",

            IsActive: true            Slug: "smartphones",

        ); Description: "Mobile phones and smartphones",

            ParentId: parentId,

        MockCategoryRepository SortOrder: 2,

            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))            ImageUrl: null,

            .ReturnsAsync(false); IconUrl: null,

            IsActive: true

        MockCategoryRepository        );

            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))

            .ReturnsAsync(true); MockCategoryRepository

            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))

        // Act & Assert            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>

            _handler.Handle(command, TestCancellationToken)); MockCategoryRepository

            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))

        exception.Message.Should().Contain("Category with slug 'electronics' already exists");            .ReturnsAsync(false);



MockCategoryRepository.Verify(MockCategoryRepository

    x => x.ExistsBySlugAsync(It.Is<CategorySlug>(s => s.Value == "electronics"), TestCancellationToken),            .Setup(x => x.GetByIdAsync(parentId, TestCancellationToken))

            Times.Once);            .ReturnsAsync(parentCategory);



MockCategoryRepository.Verify(MockCategoryRepository

    x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken),            .Setup(x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken))

            Times.Never);            .Returns(Task.CompletedTask);



MockUnitOfWork.Verify(        // Act

    x => x.SaveChangesAsync(TestCancellationToken),        var result = await _handler.Handle(command, TestCancellationToken);

Times.Never);

    }        // Assert

        result.Should().NotBeNull();

[Fact] result.Name.Should().Be("Smartphones");

public async Task Handle_ParentNotFound_ShouldThrowDomainException()        result.ParentId.Should().Be(parentId);

{
    result.SortOrder.Should().Be(2);

    // Arrange

    var parentId = 999L; MockCategoryRepository.Verify(

    var command = new CreateCategoryCommand(x => x.GetByIdAsync(parentId, TestCancellationToken),

        Name: "Smartphones", Times.Once);

Slug: "smartphones",

            Description: "Mobile phones",        MockCategoryRepository.Verify(

            ParentId: parentId, x => x.AddAsync(It.Is<CatCategory>(c => c.ParentId == parentId), TestCancellationToken),

            SortOrder: 1, Times.Once);

ImageUrl: null,    }

IconUrl: null,

            IsActive: true[Fact]

        ); public async Task Handle_DuplicateName_ShouldThrowDomainException()

{

    MockCategoryRepository        // Arrange

        .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))        var command = new CreateCategoryCommand(

            .ReturnsAsync(false); Name: "Electronics",

            Slug: "electronics",

        MockCategoryRepository Description: "Electronic products",

            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))            ParentId: null,

            .ReturnsAsync(false); SortOrder: 1,

            ImageUrl: null,

        MockCategoryRepository IconUrl: null,

            .Setup(x => x.GetByIdAsync(parentId, TestCancellationToken))            IsActive: true

            .ReturnsAsync((CatCategory?)null);        );



// Act & Assert        MockCategoryRepository

var exception = await Assert.ThrowsAsync<DomainException>(() =>             .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))


    _handler.Handle(command, TestCancellationToken));            .ReturnsAsync(true);



exception.Message.Should().Contain($"Parent category with ID {parentId} not found");        // Act & Assert

var exception = await Assert.ThrowsAsync<DomainException>(() =>

MockCategoryRepository.Verify(_handler.Handle(command, TestCancellationToken));

x => x.GetByIdAsync(parentId, TestCancellationToken),

            Times.Once); exception.Message.Should().Contain("Category with name 'Electronics' already exists");



MockCategoryRepository.Verify(MockCategoryRepository.Verify(

    x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken), x => x.ExistsByNameAsync(It.Is<CategoryName>(n => n.Value == "Electronics"), TestCancellationToken),

    Times.Never); Times.Once);



MockUnitOfWork.Verify(MockCategoryRepository.Verify(

    x => x.SaveChangesAsync(TestCancellationToken), x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken),

    Times.Never); Times.Never);

    }

        MockUnitOfWork.Verify(

    [Fact] x => x.SaveChangesAsync(TestCancellationToken),

    public async Task Handle_IsActiveFalse_ShouldCreateInactiveCategory()            Times.Never);

{ }

// Arrange

var command = new CreateCategoryCommand([Fact]


    Name: "Draft Electronics",    public async Task Handle_DuplicateSlug_ShouldThrowDomainException()

            Slug: "draft-electronics",    {

Description: "Draft category",        // Arrange

            ParentId: null,        var command = new CreateCategoryCommand(

            SortOrder: 1, Name: "New Electronics",

            ImageUrl: null, Slug: "electronics",

            IconUrl: null, Description: "Electronic products",

            IsActive: false            ParentId: null,

        ); SortOrder: 1,

            ImageUrl: null,

        MockCategoryRepository IconUrl: null,

            .Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))            IsActive: true

            .ReturnsAsync(false);        );



    MockCategoryRepository MockCategoryRepository

            .Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken)).Setup(x => x.ExistsByNameAsync(It.IsAny<CategoryName>(), TestCancellationToken))

            .ReturnsAsync(false);            .ReturnsAsync(false);



    MockCategoryRepository MockCategoryRepository

            .Setup(x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken)).Setup(x => x.ExistsBySlugAsync(It.IsAny<CategorySlug>(), TestCancellationToken))

            .ReturnsAsync((CatCategory category, CancellationToken _) => category);            .ReturnsAsync(true);



    // Act        // Act & Assert

    var result = await _handler.Handle(command, TestCancellationToken); var exception = await Assert.ThrowsAsync<DomainException>(() =>

        _handler.Handle(command, TestCancellationToken));

    // Assert

    result.Should().NotBeNull(); exception.Message.Should().Contain("Category with slug 'electronics' already exists");

    result.IsActive.Should().BeFalse();

    result.Status.Should().Be((byte)CategoryStatus.Inactive); MockCategoryRepository.Verify(

        x => x.ExistsBySlugAsync(It.Is<CategorySlug>(s => s.Value == "electronics"), TestCancellationToken),

    MockCategoryRepository.Verify(Times.Once);

    x => x.AddAsync(It.Is<CatCategory>(c =>

        c.Status == CategoryStatus.Inactive), TestCancellationToken),        MockCategoryRepository.Verify(

    Times.Once); x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken),

    }
Times.Never);

}
        MockUnitOfWork.Verify(
            x => x.SaveChangesAsync(TestCancellationToken),
            Times.Never);
    }

    [Fact]
public async Task Handle_ParentNotFound_ShouldThrowDomainException()
{
    // Arrange
    var parentId = 999L;
    var command = new CreateCategoryCommand(
        Name: "Smartphones",
        Slug: "smartphones",
        Description: "Mobile phones",
        ParentId: parentId,
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
        .Setup(x => x.GetByIdAsync(parentId, TestCancellationToken))
        .ReturnsAsync((CatCategory?)null);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<DomainException>(() =>
        _handler.Handle(command, TestCancellationToken));

    exception.Message.Should().Contain($"Parent category with ID {parentId} not found");

    MockCategoryRepository.Verify(
        x => x.GetByIdAsync(parentId, TestCancellationToken),
        Times.Once);

    MockCategoryRepository.Verify(
        x => x.AddAsync(It.IsAny<CatCategory>(), TestCancellationToken),
        Times.Never);

    MockUnitOfWork.Verify(
        x => x.SaveChangesAsync(TestCancellationToken),
        Times.Never);
}

[Fact]
public async Task Handle_WithImageUrls_ShouldUpdateCategoryImages()
{
    // Arrange
    var command = new CreateCategoryCommand(
        Name: "Electronics",
        Slug: "electronics",
        Description: "Electronic products",
        ParentId: null,
        SortOrder: 1,
        ImageUrl: "https://example.com/electronics.jpg",
        IconUrl: "https://example.com/electronics-icon.png",
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
        .Returns(Task.CompletedTask);

    // Act
    var result = await _handler.Handle(command, TestCancellationToken);

    // Assert
    result.Should().NotBeNull();
    result.ImageUrl.Should().Be("https://example.com/electronics.jpg");
    result.IconUrl.Should().Be("https://example.com/electronics-icon.png");

    MockCategoryRepository.Verify(
        x => x.AddAsync(It.Is<CatCategory>(c =>
            c.ImageUrl == "https://example.com/electronics.jpg" &&
            c.IconUrl == "https://example.com/electronics-icon.png"), TestCancellationToken),
        Times.Once);
}

[Fact]
public async Task Handle_IsActiveFalse_ShouldCreateInactiveCategory()
{
    // Arrange
    var command = new CreateCategoryCommand(
        Name: "Draft Electronics",
        Slug: "draft-electronics",
        Description: "Draft category",
        ParentId: null,
        SortOrder: 1,
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
        .Returns(Task.CompletedTask);

    // Act
    var result = await _handler.Handle(command, TestCancellationToken);

    // Assert
    result.Should().NotBeNull();
    result.IsActive.Should().BeFalse();
    result.Status.Should().Be((byte)CategoryStatus.Inactive);

    MockCategoryRepository.Verify(
        x => x.AddAsync(It.Is<CatCategory>(c =>
            c.Status == CategoryStatus.Inactive), TestCancellationToken),
        Times.Once);
}
}