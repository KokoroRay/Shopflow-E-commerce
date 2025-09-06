using AutoFixture;
using FluentAssertions;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.Tests.TestFixtures;
using Xunit;

namespace ShopFlow.Domain.Tests.Entities;

/// <summary>
/// Comprehensive tests for CatProduct entity business logic and product lifecycle management
/// </summary>
public class CatProductTests : DomainTestBase
{
    private const byte ValidProductType = 1;
    private const int ValidReturnDays = 30;

    #region Constructor Tests

    [Fact]
    public void Constructor_ValidParameters_ShouldCreateProduct()
    {
        // Act
        var product = new CatProduct(ValidProductType, ValidReturnDays);

        // Assert
        product.ProductType.Should().Be(ValidProductType);
        product.Status.Should().Be(ProductStatus.Draft);
        product.ReturnDays.Should().Be(ValidReturnDays);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.Skus.Should().BeEmpty();
        product.Reviews.Should().BeEmpty();
        product.Categories.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithoutReturnDays_ShouldCreateProduct()
    {
        // Act
        var product = new CatProduct(ValidProductType);

        // Assert
        product.ProductType.Should().Be(ValidProductType);
        product.Status.Should().Be(ProductStatus.Draft);
        product.ReturnDays.Should().BeNull();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(90)]
    [InlineData(365)]
    public void Constructor_VariousReturnDays_ShouldCreateProduct(int returnDays)
    {
        // Act
        var product = new CatProduct(ValidProductType, returnDays);

        // Assert
        product.ReturnDays.Should().Be(returnDays);
        product.Status.Should().Be(ProductStatus.Draft);
    }

    #endregion

    #region Status Management Tests

    [Fact]
    public void Activate_DraftProduct_ShouldActivateProduct()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.Activate();

        // Assert
        product.Status.Should().Be(ProductStatus.Active);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Activate_InactiveProduct_ShouldActivateProduct()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Deactivate();
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.Activate();

        // Assert
        product.Status.Should().Be(ProductStatus.Active);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Activate_DiscontinuedProduct_ShouldThrowException()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Discontinue();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => product.Activate());
        exception.Message.Should().Contain("Cannot activate a discontinued product");
    }

    [Fact]
    public void Deactivate_ActiveProduct_ShouldDeactivateProduct()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Activate();
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.Deactivate();

        // Assert
        product.Status.Should().Be(ProductStatus.Inactive);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Deactivate_DraftProduct_ShouldDeactivateProduct()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.Deactivate();

        // Assert
        product.Status.Should().Be(ProductStatus.Inactive);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Discontinue_AnyProduct_ShouldDiscontinueProduct()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Activate();
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.Discontinue();

        // Assert
        product.Status.Should().Be(ProductStatus.Discontinued);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Discontinue_InactiveProduct_ShouldDiscontinueProduct()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Deactivate();
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.Discontinue();

        // Assert
        product.Status.Should().Be(ProductStatus.Discontinued);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData(ProductStatus.Draft)]
    [InlineData(ProductStatus.Inactive)]
    [InlineData(ProductStatus.Discontinued)]
    public void IsActive_NonActiveProduct_ShouldReturnFalse(ProductStatus status)
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        
        // Set status using appropriate method
        switch (status)
        {
            case ProductStatus.Draft:
                // Already in Draft state
                break;
            case ProductStatus.Inactive:
                product.Deactivate();
                break;
            case ProductStatus.Discontinued:
                product.Discontinue();
                break;
        }

        // Act & Assert
        product.IsActive().Should().BeFalse();
    }

    [Fact]
    public void IsActive_ActiveProduct_ShouldReturnTrue()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Activate();

        // Act & Assert
        product.IsActive().Should().BeTrue();
    }

    #endregion

    #region Return Days Management Tests

    [Fact]
    public void UpdateReturnDays_ValidDays_ShouldUpdateReturnDays()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var newReturnDays = 60;
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.UpdateReturnDays(newReturnDays);

        // Assert
        product.ReturnDays.Should().Be(newReturnDays);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdateReturnDays_NullDays_ShouldSetToNull()
    {
        // Arrange
        var product = new CatProduct(ValidProductType, ValidReturnDays);
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.UpdateReturnDays(null);

        // Assert
        product.ReturnDays.Should().BeNull();
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdateReturnDays_ZeroDays_ShouldUpdateReturnDays()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.UpdateReturnDays(0);

        // Assert
        product.ReturnDays.Should().Be(0);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdateReturnDays_NegativeDays_ShouldThrowException()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => product.UpdateReturnDays(-1));
        exception.ParamName.Should().Be("returnDays");
        exception.Message.Should().Contain("Return days cannot be negative");
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(-100)]
    [InlineData(-999)]
    public void UpdateReturnDays_VariousNegativeValues_ShouldThrowException(int negativeDays)
    {
        // Arrange
        var product = new CatProduct(ValidProductType);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => product.UpdateReturnDays(negativeDays));
        exception.ParamName.Should().Be("returnDays");
    }

    #endregion

    #region SKU Management Tests

    [Fact]
    public void AddSku_ValidSku_ShouldAddSku()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var sku = CreateSimpleSku();
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.AddSku(sku);

        // Assert
        product.Skus.Should().Contain(sku);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void AddSku_NullSku_ShouldThrowException()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => product.AddSku(null!));
        exception.ParamName.Should().Be("sku");
    }

    [Fact]
    public void AddSku_MultipleSkus_ShouldAddAllSkus()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var sku1 = CreateSimpleSku();
        var sku2 = CreateSimpleSku();

        // Act
        product.AddSku(sku1);
        product.AddSku(sku2);

        // Assert
        product.Skus.Should().HaveCount(2);
        product.Skus.Should().Contain(sku1);
        product.Skus.Should().Contain(sku2);
    }

    [Fact]
    public void CanBeOrdered_ActiveProductWithActiveSku_ShouldReturnTrue()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Activate();
        
        var activeSku = CreateSimpleSku(isActive: true);
        product.AddSku(activeSku);

        // Act & Assert
        product.CanBeOrdered().Should().BeTrue();
    }

    [Fact]
    public void CanBeOrdered_ActiveProductWithInactiveSku_ShouldReturnFalse()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Activate();
        
        var inactiveSku = CreateSimpleSku(isActive: false);
        product.AddSku(inactiveSku);

        // Act & Assert
        product.CanBeOrdered().Should().BeFalse();
    }

    [Fact]
    public void CanBeOrdered_InactiveProductWithActiveSku_ShouldReturnFalse()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        // Product remains in Draft status (not active)
        
        var activeSku = CreateSimpleSku(isActive: true);
        product.AddSku(activeSku);

        // Act & Assert
        product.CanBeOrdered().Should().BeFalse();
    }

    [Fact]
    public void CanBeOrdered_ActiveProductWithoutSkus_ShouldReturnFalse()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Activate();

        // Act & Assert
        product.CanBeOrdered().Should().BeFalse();
    }

    [Fact]
    public void CanBeOrdered_ActiveProductWithMixedSkus_ShouldReturnTrue()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        product.Activate();
        
        var activeSku = CreateSimpleSku(isActive: true);
        var inactiveSku = CreateSimpleSku(isActive: false);
            
        product.AddSku(activeSku);
        product.AddSku(inactiveSku);

        // Act & Assert
        product.CanBeOrdered().Should().BeTrue();
    }

    #endregion

    #region Category Management Tests

    [Fact]
    public void AddCategory_ValidCategory_ShouldAddCategory()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var category = Fixture.Create<CatCategory>();
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.AddCategory(category);

        // Assert
        product.Categories.Should().Contain(category);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void AddCategory_NullCategory_ShouldThrowException()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => product.AddCategory(null!));
        exception.ParamName.Should().Be("category");
    }

    [Fact]
    public void AddCategory_DuplicateCategory_ShouldNotAddDuplicate()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var category = Fixture.Create<CatCategory>();
        product.AddCategory(category);

        // Act
        product.AddCategory(category);

        // Assert
        product.Categories.Should().HaveCount(1);
        product.Categories.Should().Contain(category);
    }

    [Fact]
    public void AddCategory_MultipleCategories_ShouldAddAllCategories()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var category1 = Fixture.Create<CatCategory>();
        var category2 = Fixture.Create<CatCategory>();

        // Act
        product.AddCategory(category1);
        product.AddCategory(category2);

        // Assert
        product.Categories.Should().HaveCount(2);
        product.Categories.Should().Contain(category1);
        product.Categories.Should().Contain(category2);
    }

    [Fact]
    public void RemoveCategory_ExistingCategory_ShouldRemoveCategory()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var category = Fixture.Create<CatCategory>();
        product.AddCategory(category);
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        product.RemoveCategory(category);

        // Assert
        product.Categories.Should().NotContain(category);
        product.Categories.Should().BeEmpty();
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void RemoveCategory_NonExistingCategory_ShouldNotThrow()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var category = Fixture.Create<CatCategory>();

        // Act & Assert - Should not throw
        product.RemoveCategory(category);
        product.Categories.Should().BeEmpty();
    }

    [Fact]
    public void RemoveCategory_NullCategory_ShouldThrowException()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => product.RemoveCategory(null!));
        exception.ParamName.Should().Be("category");
    }

    [Fact]
    public void RemoveCategory_OneOfMultipleCategories_ShouldRemoveOnlySpecified()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var category1 = Fixture.Create<CatCategory>();
        var category2 = Fixture.Create<CatCategory>();
        product.AddCategory(category1);
        product.AddCategory(category2);

        // Act
        product.RemoveCategory(category1);

        // Assert
        product.Categories.Should().HaveCount(1);
        product.Categories.Should().Contain(category2);
        product.Categories.Should().NotContain(category1);
    }

    #endregion

    #region Navigation Properties Tests

    [Fact]
    public void Skus_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);

        // Act & Assert
        product.Skus.Should().BeAssignableTo<IReadOnlyCollection<CatSku>>();
        product.Skus.Should().BeEmpty();
    }

    [Fact]
    public void Reviews_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);

        // Act & Assert
        product.Reviews.Should().BeAssignableTo<IReadOnlyCollection<CeReview>>();
        product.Reviews.Should().BeEmpty();
    }

    [Fact]
    public void Categories_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);

        // Act & Assert
        product.Categories.Should().BeAssignableTo<IReadOnlyCollection<CatCategory>>();
        product.Categories.Should().BeEmpty();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void ProductLifecycle_CompleteFlow_ShouldWorkCorrectly()
    {
        // Arrange & Act - Create product
        var product = new CatProduct(ValidProductType, ValidReturnDays);
        var originalCreatedAt = product.CreatedAt;

        // Act - Add categories and SKUs
        var category1 = Fixture.Create<CatCategory>();
        var category2 = Fixture.Create<CatCategory>();
        product.AddCategory(category1);
        product.AddCategory(category2);

        var sku1 = Fixture.Build<CatSku>()
            .With(s => s.IsActive, true)
            .Create();
        var sku2 = Fixture.Build<CatSku>()
            .With(s => s.IsActive, false)
            .Create();
        product.AddSku(sku1);
        product.AddSku(sku2);

        // Act - Activate product
        product.Activate();

        // Act - Update return days
        product.UpdateReturnDays(45);

        // Act - Deactivate and reactivate
        product.Deactivate();
        product.Activate();

        // Act - Remove one category
        product.RemoveCategory(category1);

        // Assert
        product.ProductType.Should().Be(ValidProductType);
        product.Status.Should().Be(ProductStatus.Active);
        product.ReturnDays.Should().Be(45);
        product.Categories.Should().HaveCount(1);
        product.Categories.Should().Contain(category2);
        product.Categories.Should().NotContain(category1);
        product.Skus.Should().HaveCount(2);
        product.IsActive().Should().BeTrue();
        product.CanBeOrdered().Should().BeTrue(); // Has active SKU
        product.CreatedAt.Should().Be(originalCreatedAt);
        product.UpdatedAt.Should().BeAfter(originalCreatedAt);
    }

    [Fact]
    public void ProductStatusTransitions_AllValidTransitions_ShouldWork()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);

        // Draft -> Active
        product.Activate();
        product.Status.Should().Be(ProductStatus.Active);

        // Active -> Inactive
        product.Deactivate();
        product.Status.Should().Be(ProductStatus.Inactive);

        // Inactive -> Active
        product.Activate();
        product.Status.Should().Be(ProductStatus.Active);

        // Active -> Discontinued
        product.Discontinue();
        product.Status.Should().Be(ProductStatus.Discontinued);

        // Discontinued -> Cannot be activated (should throw)
        Assert.Throws<InvalidOperationException>(() => product.Activate());
    }

    [Fact]
    public void ProductWithComplexCategoryManagement_ShouldHandleCorrectly()
    {
        // Arrange
        var product = new CatProduct(ValidProductType);
        var categories = Fixture.CreateMany<CatCategory>(5).ToList();

        // Act - Add all categories
        foreach (var category in categories)
        {
            product.AddCategory(category);
        }

        // Act - Try to add duplicates
        foreach (var category in categories.Take(3))
        {
            product.AddCategory(category);
        }

        // Act - Remove some categories
        product.RemoveCategory(categories[0]);
        product.RemoveCategory(categories[2]);
        product.RemoveCategory(categories[4]);

        // Assert
        product.Categories.Should().HaveCount(2);
        product.Categories.Should().Contain(categories[1]);
        product.Categories.Should().Contain(categories[3]);
        product.Categories.Should().NotContain(categories[0]);
        product.Categories.Should().NotContain(categories[2]);
        product.Categories.Should().NotContain(categories[4]);
    }

    #endregion
}
