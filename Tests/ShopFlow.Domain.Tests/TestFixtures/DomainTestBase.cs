using AutoFixture;
using FluentAssertions;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.TestFixtures;

/// <summary>
/// Base class for all domain unit tests providing common test infrastructure
/// </summary>
public abstract class DomainTestBase
{
    protected readonly IFixture Fixture;

    protected DomainTestBase()
    {
        Fixture = new Fixture();

        // Configure fixture to handle circular references
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Configure value objects for AutoFixture
        Fixture.Register(() => new Email("test@example.com"));
        Fixture.Register(() => new PhoneNumber("0901234567"));

        // Configure DateOnly for AutoFixture
        Fixture.Register(() => new DateOnly(1990, 1, 1));

        // Configure entities to avoid circular references
        ConfigureEntityCustomizations();

        ConfigureFixture();
    }

    /// <summary>
    /// Configure AutoFixture to avoid circular references in entities
    /// </summary>
    private void ConfigureEntityCustomizations()
    {
        // Configure CatSku creation
        Fixture.Customize<CatSku>(composer => composer
            .Without(s => s.Product)
            .Without(s => s.CartItems)
            .Without(s => s.CatSkuMedia)
            .Without(s => s.InvStocks)
            .Without(s => s.OrdOrderItems)
            .Without(s => s.CeReviews)
            .Without(s => s.InvAdjustmentLines)
            .Without(s => s.InvReservations)
            .Without(s => s.MktOffers)
            .Without(s => s.CatSkuOptionValues)
            .Without(s => s.TaxClasses)
            .With(s => s.IsActive, true)
            .With(s => s.SkuCode, () => $"SKU-{Fixture.Create<string>()[..8]}")
            .With(s => s.ProductId, () => Fixture.Create<long>()));

        // Configure CatCategory creation - skip customization since it has complex constructor
        // We'll create categories manually in CreateSimpleCategory method

        // Configure CoreAddress creation
        Fixture.Customize<CoreAddress>(composer => composer
            .Without(a => a.User)
            .With(a => a.Line1, () => Fixture.Create<string>())
            .With(a => a.Province, () => Fixture.Create<string>())
            .With(a => a.Country, () => Fixture.Create<string>()));

        // Configure CoreUserRole creation
        Fixture.Customize<CoreUserRole>(composer => composer
            .Without(ur => ur.User)
            .Without(ur => ur.Role)
            .Without(ur => ur.RoleAdminProfiles)
            .Without(ur => ur.RoleCustomerProfiles)
            .Without(ur => ur.RoleModeratorProfiles)
            .Without(ur => ur.RoleVendorStaffs)
            .Without(ur => ur.RoleWarehouseStaffs));
    }

    /// <summary>
    /// Override this method to configure AutoFixture for specific test scenarios
    /// </summary>
    protected virtual void ConfigureFixture()
    {
        // Default configuration - can be overridden in derived classes
    }

    /// <summary>
    /// Helper method to assert that an action throws a specific exception with expected message
    /// </summary>
    protected void AssertThrows<TException>(Action action, string expectedMessage)
        where TException : Exception
    {
        var exception = Assert.Throws<TException>(action);
        exception.Message.Should().Contain(expectedMessage);
    }

    /// <summary>
    /// Helper method to assert that an async action throws a specific exception with expected message
    /// </summary>
    protected async Task AssertThrowsAsync<TException>(Func<Task> action, string expectedMessage)
        where TException : Exception
    {
        var exception = await Assert.ThrowsAsync<TException>(action);
        exception.Message.Should().Contain(expectedMessage);
    }

    /// <summary>
    /// Generate a random valid email for testing
    /// </summary>
    protected string GenerateValidEmail()
    {
        return $"test{Fixture.Create<int>()}@example.com";
    }

    /// <summary>
    /// Generate a random valid Vietnamese phone number for testing
    /// </summary>
    protected string GenerateValidPhoneNumber()
    {
        var numbers = new[] { "901", "902", "903", "904", "905", "906", "907", "908", "909" };
        var prefix = numbers[Fixture.Create<int>() % numbers.Length];
        var suffix = Fixture.Create<int>() % 10000000;
        return $"0{prefix}{suffix:D6}";
    }

    /// <summary>
    /// Generate a random decimal with specified precision
    /// </summary>
    protected decimal GenerateDecimal(decimal min = 0, decimal max = 10000, int decimals = 2)
    {
        var value = (decimal)(Fixture.Create<double>() * (double)(max - min) + (double)min);
        return Math.Round(value, decimals);
    }

    /// <summary>
    /// Creates a simple CatSku for testing without circular references
    /// </summary>
    protected CatSku CreateSimpleSku(bool isActive = true, decimal price = 100m)
    {
        return new CatSku
        {
            Id = Fixture.Create<long>(),
            ProductId = Fixture.Create<long>(),
            SkuCode = Fixture.Create<string>(),
            IsActive = isActive,
            Barcode = Fixture.Create<string>()
        };
    }

    /// <summary>
    /// Creates a simple CatCategory for testing
    /// </summary>
    protected CatCategory CreateSimpleCategory(string? name = null)
    {
        var categoryName = new CategoryName(name ?? "Test Category");
        var categorySlug = new CategorySlug((name ?? "test-category").ToLowerInvariant().Replace(" ", "-", StringComparison.Ordinal));
        return new CatCategory(categoryName, categorySlug, "Test description", null, 1);
    }
}
