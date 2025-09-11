using FluentAssertions;
using ShopFlow.Domain.DomainEvents;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.Tests.TestFixtures;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.Entities;

public class CatCategoryTests : DomainTestBase
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateCategory()
    {
        // Arrange
        var name = new CategoryName("Electronics");
        var slug = new CategorySlug("electronics");
        const string description = "Electronic products";
        long? parentId = null;
        const int sortOrder = 1;

        // Act
        var category = new CatCategory(name, slug, description, parentId, sortOrder);

        // Assert
        category.Name.Should().Be(name);
        category.Slug.Should().Be(slug);
        category.Description.Should().Be(description);
        category.ParentId.Should().Be(parentId);
        category.SortOrder.Should().Be(sortOrder);
        category.Status.Should().Be(CategoryStatus.Active);
        category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        category.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_ValidParameters_ShouldRaiseCategoryCreatedEvent()
    {
        // Arrange
        var name = new CategoryName("Electronics");
        var slug = new CategorySlug("electronics");
        const string description = "Electronic products";

        // Act
        var category = new CatCategory(name, slug, description, null, 1);

        // Assert
        var domainEvent = category.DomainEvents.Should().ContainSingle().Which;
        domainEvent.Should().BeOfType<CategoryCreatedEvent>();

        var createdEvent = (CategoryCreatedEvent)domainEvent;
        createdEvent.CategoryId.Should().Be(category.Id);
        createdEvent.Name.Should().Be(name.Value);
        createdEvent.Slug.Should().Be(slug.Value);
    }

    [Fact]
    public void Constructor_NullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var slug = new CategorySlug("electronics");

        // Act & Assert
        AssertThrows<ArgumentNullException>(() => new CatCategory(null!, slug, "description", null, 1), "name");
    }

    [Fact]
    public void Constructor_NullSlug_ShouldThrowArgumentNullException()
    {
        // Arrange
        var name = new CategoryName("Electronics");

        // Act & Assert
        AssertThrows<ArgumentNullException>(() => new CatCategory(name, null!, "description", null, 1), "slug");
    }

    [Fact]
    public void UpdateName_ValidName_ShouldUpdateNameAndRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory();
        var newName = new CategoryName("Updated Electronics");
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.UpdateName(newName);

        // Assert
        category.Name.Should().Be(newName);
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);

        var domainEvent = category.DomainEvents.Should().ContainSingle(e => e is CategoryUpdatedEvent).Which;
        var updatedEvent = (CategoryUpdatedEvent)domainEvent;
        updatedEvent.CategoryId.Should().Be(category.Id);
        updatedEvent.NewName.Should().Be(newName.Value);
    }

    [Fact]
    public void UpdateName_NullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var category = CreateSimpleCategory();

        // Act & Assert
        AssertThrows<ArgumentNullException>(() => category.UpdateName(null!), "name");
    }

    [Fact]
    public void UpdateDescription_ValidDescription_ShouldUpdateDescription()
    {
        // Arrange
        var category = CreateSimpleCategory();
        const string newDescription = "Updated description";
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.UpdateDescription(newDescription);

        // Assert
        category.Description.Should().Be(newDescription);
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdateDescription_NullDescription_ShouldSetDescriptionToNull()
    {
        // Arrange
        var category = CreateSimpleCategory();
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.UpdateDescription(null);

        // Assert
        category.Description.Should().BeNull();
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdateSlug_ValidSlug_ShouldUpdateSlugAndRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory();
        var newSlug = new CategorySlug("updated-electronics");
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.UpdateSlug(newSlug);

        // Assert
        category.Slug.Should().Be(newSlug);
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);

        var domainEvent = category.DomainEvents.Should().ContainSingle(e => e is CategorySlugChangedEvent).Which;
        var slugChangedEvent = (CategorySlugChangedEvent)domainEvent;
        slugChangedEvent.CategoryId.Should().Be(category.Id);
        slugChangedEvent.NewSlug.Should().Be(newSlug.Value);
    }

    [Fact]
    public void UpdateSlug_NullSlug_ShouldThrowArgumentNullException()
    {
        // Arrange
        var category = CreateSimpleCategory();

        // Act & Assert
        AssertThrows<ArgumentNullException>(() => category.UpdateSlug(null!), "slug");
    }

    [Fact]
    public void Activate_InactiveCategory_ShouldActivateAndRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.Deactivate(); // First deactivate it
        category.ClearDomainEvents(); // Clear the deactivate event
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.Activate();

        // Assert
        category.Status.Should().Be(CategoryStatus.Active);
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);

        var domainEvent = category.DomainEvents.Should().ContainSingle().Which;
        domainEvent.Should().BeOfType<CategoryStatusChangedEvent>();

        var statusChangedEvent = (CategoryStatusChangedEvent)domainEvent;
        statusChangedEvent.CategoryId.Should().Be(category.Id);
        statusChangedEvent.NewStatus.Should().Be(CategoryStatus.Active);
    }

    [Fact]
    public void Activate_AlreadyActiveCategory_ShouldNotChangeStatusOrRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory(); // Already active by default
        var originalUpdatedAt = category.UpdatedAt;
        category.ClearDomainEvents(); // Clear creation events

        // Act
        category.Activate();

        // Assert
        category.Status.Should().Be(CategoryStatus.Active);
        category.UpdatedAt.Should().Be(originalUpdatedAt);
        category.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Deactivate_ActiveCategory_ShouldDeactivateAndRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory(); // Active by default
        category.ClearDomainEvents(); // Clear creation events
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.Deactivate();

        // Assert
        category.Status.Should().Be(CategoryStatus.Inactive);
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);

        var domainEvent = category.DomainEvents.Should().ContainSingle().Which;
        domainEvent.Should().BeOfType<CategoryStatusChangedEvent>();

        var statusChangedEvent = (CategoryStatusChangedEvent)domainEvent;
        statusChangedEvent.CategoryId.Should().Be(category.Id);
        statusChangedEvent.NewStatus.Should().Be(CategoryStatus.Inactive);
    }

    [Fact]
    public void Deactivate_AlreadyInactiveCategory_ShouldNotChangeStatusOrRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.Deactivate(); // First deactivate it
        category.ClearDomainEvents(); // Clear the deactivate event
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.Deactivate();

        // Assert
        category.Status.Should().Be(CategoryStatus.Inactive);
        category.UpdatedAt.Should().Be(originalUpdatedAt);
        category.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Delete_ActiveOrInactiveCategory_ShouldMarkAsDeletedAndRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.ClearDomainEvents(); // Clear creation events
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.Delete();

        // Assert
        category.Status.Should().Be(CategoryStatus.Deleted);
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);

        var domainEvent = category.DomainEvents.Should().ContainSingle().Which;
        domainEvent.Should().BeOfType<CategoryDeletedEvent>();

        var deletedEvent = (CategoryDeletedEvent)domainEvent;
        deletedEvent.CategoryId.Should().Be(category.Id);
        deletedEvent.CategoryName.Should().Be(category.Name.Value);
    }

    [Fact]
    public void Delete_AlreadyDeletedCategory_ShouldNotChangeStatusOrRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.Delete(); // First delete it
        category.ClearDomainEvents(); // Clear the delete event
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.Delete();

        // Assert
        category.Status.Should().Be(CategoryStatus.Deleted);
        category.UpdatedAt.Should().Be(originalUpdatedAt);
        category.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ChangeParent_ValidParentId_ShouldUpdateParentAndRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.ClearDomainEvents(); // Clear creation events
        const long newParentId = 100L;
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.ChangeParent(newParentId);

        // Assert
        category.ParentId.Should().Be(newParentId);
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);

        var domainEvent = category.DomainEvents.Should().ContainSingle().Which;
        domainEvent.Should().BeOfType<CategoryParentChangedEvent>();

        var parentChangedEvent = (CategoryParentChangedEvent)domainEvent;
        parentChangedEvent.CategoryId.Should().Be(category.Id);
        parentChangedEvent.NewParentId.Should().Be(newParentId);
        parentChangedEvent.OldParentId.Should().BeNull();
    }

    [Fact]
    public void ChangeParent_NullParentId_ShouldRemoveParentAndRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.ChangeParent(100L); // Set a parent first
        category.ClearDomainEvents(); // Clear events
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.ChangeParent(null);

        // Assert
        category.ParentId.Should().BeNull();
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);

        var domainEvent = category.DomainEvents.Should().ContainSingle().Which;
        domainEvent.Should().BeOfType<CategoryParentChangedEvent>();

        var parentChangedEvent = (CategoryParentChangedEvent)domainEvent;
        parentChangedEvent.CategoryId.Should().Be(category.Id);
        parentChangedEvent.NewParentId.Should().BeNull();
        parentChangedEvent.OldParentId.Should().Be(100L);
    }

    [Fact]
    public void ChangeParent_SameParentId_ShouldNotChangeParentOrRaiseEvent()
    {
        // Arrange
        var category = CreateSimpleCategory();
        const long parentId = 100L;
        category.ChangeParent(parentId);
        category.ClearDomainEvents(); // Clear events
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.ChangeParent(parentId);

        // Assert
        category.ParentId.Should().Be(parentId);
        category.UpdatedAt.Should().Be(originalUpdatedAt);
        category.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void UpdateSortOrder_ValidSort_ShouldUpdateSort()
    {
        // Arrange
        var category = CreateSimpleCategory();
        const int newSort = 10;
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        category.UpdateSortOrder(newSort);

        // Assert
        category.SortOrder.Should().Be(newSort);
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Status_ActiveCategory_ShouldBeActive()
    {
        // Arrange
        var category = CreateSimpleCategory();

        // Act & Assert
        category.Status.Should().Be(CategoryStatus.Active);
    }

    [Fact]
    public void Status_InactiveCategory_ShouldBeInactive()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.Deactivate();

        // Act & Assert
        category.Status.Should().Be(CategoryStatus.Inactive);
    }

    [Fact]
    public void Status_DeletedCategory_ShouldBeDeleted()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.Delete();

        // Act & Assert
        category.Status.Should().Be(CategoryStatus.Deleted);
    }

    [Fact]
    public void ParentId_CategoryWithoutParent_ShouldBeNull()
    {
        // Arrange
        var category = CreateSimpleCategory();

        // Act & Assert
        category.ParentId.Should().BeNull();
    }

    [Fact]
    public void ParentId_CategoryWithParent_ShouldHaveParentId()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.ChangeParent(100L);

        // Act & Assert
        category.ParentId.Should().Be(100L);
    }

    [Fact]
    public void Constructor_WithParentId_ShouldCreateCategoryWithParent()
    {
        // Arrange
        var name = new CategoryName("Sub Electronics");
        var slug = new CategorySlug("sub-electronics");
        const string description = "Sub category";
        const long parentId = 100L;
        const int sortOrder = 1;

        // Act
        var category = new CatCategory(name, slug, description, parentId, sortOrder);

        // Assert
        category.ParentId.Should().Be(parentId);
    }

    [Fact]
    public void CanBeDeleted_CategoryWithoutProductsAndChildren_ShouldReturnTrue()
    {
        // Arrange
        var category = CreateSimpleCategory();

        // Act & Assert
        category.CanBeDeleted().Should().BeTrue();
    }

    [Fact]
    public void CanHaveChildren_ActiveCategory_ShouldReturnTrue()
    {
        // Arrange
        var category = CreateSimpleCategory();

        // Act & Assert
        category.CanHaveChildren().Should().BeTrue();
    }

    [Fact]
    public void CanHaveChildren_InactiveCategory_ShouldReturnFalse()
    {
        // Arrange
        var category = CreateSimpleCategory();
        category.Deactivate();

        // Act & Assert
        category.CanHaveChildren().Should().BeFalse();
    }

    [Fact]
    public void IsLeaf_CategoryWithoutChildren_ShouldReturnTrue()
    {
        // Arrange
        var category = CreateSimpleCategory();

        // Act & Assert
        category.IsLeaf.Should().BeTrue();
    }
}