using AutoFixture;
using MediatR;
using Moq;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Tests.TestFixtures;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Tests.Handlers.Categories;

/// <summary>
/// Base class for category-related application tests providing category-specific test infrastructure
/// </summary>
public abstract class CategoryTestBase : ApplicationTestBase
{
    protected readonly Mock<ICategoryRepository> MockCategoryRepository;
    protected readonly Mock<IUnitOfWork> MockUnitOfWork;

    protected CategoryTestBase()
    {
        MockCategoryRepository = new Mock<ICategoryRepository>();
        MockUnitOfWork = new Mock<IUnitOfWork>();

        ConfigureCategoryMocks();
    }

    /// <summary>
    /// Configure category-specific mocks
    /// </summary>
    protected virtual void ConfigureCategoryMocks()
    {
        // Default UnitOfWork mock configuration
        MockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    /// <summary>
    /// Helper method to create a simple category for testing
    /// </summary>
    protected CatCategory CreateSimpleCategory(
        string name = "Test Category",
        string description = "Test Description",
        string slug = "test-category",
        long? parentId = null,
        int sortOrder = 1)
    {
        var categoryName = CategoryName.FromDisplayName(name);
        var categorySlug = new CategorySlug(slug);

        return new CatCategory(
            categoryName,
            categorySlug,
            description,
            parentId,
            sortOrder
        );
    }

    /// <summary>
    /// Helper method to create multiple categories for testing
    /// </summary>
    protected List<CatCategory> CreateCategoryList(int count = 3)
    {
        var categories = new List<CatCategory>();
        for (int i = 1; i <= count; i++)
        {
            categories.Add(CreateSimpleCategory(
                name: $"Category {i}",
                description: $"Description {i}",
                slug: $"category-{i}",
                sortOrder: i
            ));
        }
        return categories;
    }

    /// <summary>
    /// Reset category-specific mocks
    /// </summary>
    protected new void ResetMocks()
    {
        base.ResetMocks();
        MockCategoryRepository.Reset();
        MockUnitOfWork.Reset();
    }

    /// <summary>
    /// Generate a cancellation token for testing
    /// </summary>
    protected CancellationToken TestCancellationToken => GenerateCancellationToken();
}