using AutoFixture;
using FluentAssertions;
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
        ConfigureFixture();
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
}
