using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Handlers.Users;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Messaging;
using ShopFlow.Application.Exceptions;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.DomainEvents;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Tests.Handlers.Users;

/// <summary>
/// Unit tests for RegisterUserCommandHandler
/// </summary>
public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IDomainEventPublisher> _domainEventPublisherMock;
    private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _domainEventPublisherMock = new Mock<IDomainEventPublisher>();
        _loggerMock = new Mock<ILogger<RegisterUserCommandHandler>>();

        _handler = new RegisterUserCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _domainEventPublisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnUserResponse()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "+84987654321",
            "John Doe",
            "M",
            new DateTime(1990, 1, 1));

        var hashedPassword = "hashed_password";
        var userId = 1L;

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .Callback<CoreUser, CancellationToken>((user, _) => SetUserIdViaReflection(user, userId));

        _domainEventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Email.Should().Be(command.Email.ToLowerInvariant());
        result.Phone.Should().Be("987654321"); // Normalized phone
        result.FullName.Should().Be(command.FullName);
        result.Gender.Should().Be(command.Gender);
        result.DateOfBirth.Should().Be(command.DateOfBirth);
        result.Status.Should().Be((byte)UserStatus.Active);
        result.EmailVerified.Should().BeFalse();
        result.Roles.Should().BeEmpty();

        // Verify interactions
        _userRepositoryMock.Verify(x => x.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(command.Password), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _domainEventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ShouldThrowEmailAlreadyExistsException()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "existing@example.com",
            "Password123!",
            null,
            null,
            null,
            null);

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EmailAlreadyExistsException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.Email.Should().Be(command.Email);

        // Verify that password hashing and user creation are not called
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _domainEventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "invalid-email",
            "Password123!",
            null,
            null,
            null,
            null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None));

        // Verify that repository methods are not called
        _userRepositoryMock.Verify(x => x.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _domainEventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidPhoneNumber_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "invalid-phone",
            null,
            null,
            null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithoutOptionalFields_ShouldReturnUserResponseWithNulls()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            null,
            null,
            null,
            null);

        var hashedPassword = "hashed_password";
        var userId = 1L;

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .Callback<CoreUser, CancellationToken>((user, _) => SetUserIdViaReflection(user, userId));

        _domainEventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Email.Should().Be(command.Email.ToLowerInvariant());
        result.Phone.Should().BeNull();
        result.FullName.Should().BeNull();
        result.Gender.Should().BeNull();
        result.DateOfBirth.Should().BeNull();
    }

    private static void SetUserIdViaReflection(CoreUser user, long id)
    {
        var idProperty = typeof(CoreUser).GetProperty("Id");
        idProperty?.SetValue(user, id);
    }
}
