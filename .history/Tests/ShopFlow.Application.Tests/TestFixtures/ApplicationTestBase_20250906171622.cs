using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Abstractions.Mappings;
using Xunit;

namespace ShopFlow.Application.Tests.TestFixtures;

/// <summary>
/// Base class for all application unit tests providing common test infrastructure
/// </summary>
public abstract class ApplicationTestBase
{
    protected readonly IFixture Fixture;
    protected readonly Mock<IUserRepository> MockUserRepository;
    protected readonly Mock<IPasswordHasher> MockPasswordHasher;
    protected readonly Mock<IUserMapper> MockUserMapper;

    protected ApplicationTestBase()
    {
        Fixture = new Fixture();
        MockUserRepository = new Mock<IUserRepository>();
        MockPasswordHasher = new Mock<IPasswordHasher>();
        MockUserMapper = new Mock<IUserMapper>();
        
        ConfigureFixture();
        ConfigureMocks();
    }

    /// <summary>
    /// Override this method to configure AutoFixture for specific test scenarios
    /// </summary>
    protected virtual void ConfigureFixture()
    {
        // Prevent AutoFixture from creating recursive object graphs
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    /// <summary>
    /// Override this method to configure mocks for specific test scenarios
    /// </summary>
    protected virtual void ConfigureMocks()
    {
        // Default mock configurations - can be overridden in derived classes
        MockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns((string password) => $"hashed_{password}");
    }

    /// <summary>
    /// Helper method to verify that a mock was called with specific parameters
    /// </summary>
    protected void VerifyMockCall<T>(Mock<T> mock, Times times) where T : class
    {
        mock.Verify(times);
    }

    /// <summary>
    /// Helper method to reset all mocks
    /// </summary>
    protected void ResetMocks()
    {
        MockUserRepository.Reset();
        MockPasswordHasher.Reset();
        MockUserMapper.Reset();
    }

    /// <summary>
    /// Generate a cancellation token for testing
    /// </summary>
    protected CancellationToken GenerateCancellationToken(int timeoutMs = 5000)
    {
        return new CancellationTokenSource(timeoutMs).Token;
    }
}
