using Xunit;
using Moq;
using FluentAssertions;
using AutoFixture;
using ShopFlow.Application.Handlers.Users;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using ShopFlow.Application.Tests.Common;

namespace ShopFlow.Application.Tests.Handlers.Users;

public class CreateUserCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "password123")
            .With(x => x.Phone, "0123456789")
            .With(x => x.FullName, "Test User")
            .Create();

        var hashedPassword = "hashed_password";
        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(command.Email);
        result.FullName.Should().Be(command.FullName);
        result.Phone.Should().Be(command.Phone);
        result.Gender.Should().Be(command.Gender);
        result.DateOfBirth.Should().Be(command.DateOfBirth);
        result.Status.Should().Be((byte)UserStatus.Active);
        result.EmailVerified.Should().BeFalse();
        result.Roles.Should().NotBeNull().And.BeEmpty();

        _passwordHasherMock.Verify(x => x.HashPassword(command.Password), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CoreUser>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommandWithoutPhone_ShouldCreateUserWithNullPhone()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "password123")
            .With(x => x.Phone, (string?)null)
            .With(x => x.FullName, "Test User")
            .Create();

        var hashedPassword = "hashed_password";
        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(command.Email);
        result.Phone.Should().BeNull();
        result.FullName.Should().Be(command.FullName);
    }

    [Fact]
    public async Task Handle_ValidCommandWithEmptyPhone_ShouldCreateUserWithNullPhone()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "password123")
            .With(x => x.Phone, string.Empty)
            .With(x => x.FullName, "Test User")
            .Create();

        var hashedPassword = "hashed_password";
        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Phone.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    public async Task Handle_InvalidEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, invalidEmail)
            .With(x => x.Password, "password123")
            .With(x => x.FullName, "Test User")
            .Create();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abc123def")]
    [InlineData("01234567890123456789")]
    [InlineData("+84 123 456 789 123")]
    public async Task Handle_InvalidPhone_ShouldThrowArgumentException(string invalidPhone)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "password123")
            .With(x => x.Phone, invalidPhone)
            .With(x => x.FullName, "Test User")
            .Create();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "password123")
            .With(x => x.FullName, "Test User")
            .Create();

        var hashedPassword = "hashed_password";
        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        var expectedException = new InvalidOperationException("Database error");
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Should().Be(expectedException);
        _passwordHasherMock.Verify(x => x.HashPassword(command.Password), Times.Once);
    }

    [Fact]
    public async Task Handle_PasswordHasherThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "password123")
            .With(x => x.FullName, "Test User")
            .Create();

        var expectedException = new ArgumentException("Invalid password");
        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Throws(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Should().Be(expectedException);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CreatedUserHasCorrectDomainProperties()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "password123")
            .With(x => x.Phone, "0123456789")
            .With(x => x.FullName, "Test User")
            .Create();

        var hashedPassword = "hashed_password";
        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        CoreUser? capturedUser = null;
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .Callback<CoreUser, CancellationToken>((user, ct) => capturedUser = user)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser!.Email.Value.Should().Be(command.Email);
        capturedUser.PasswordHash.Should().Be(hashedPassword);
        capturedUser.Phone!.Value.Should().Be(command.Phone);
        capturedUser.Status.Should().Be(UserStatus.Active);
        capturedUser.EmailVerified.Should().BeFalse();
        capturedUser.Id.Should().NotBe(0);
        capturedUser.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ShouldPassCancellationToken()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "password123")
            .With(x => x.FullName, "Test User")
            .Create();

        var cancellationToken = new CancellationToken(true);
        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns("hashed_password");

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CoreUser>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _handler.Handle(command, cancellationToken));
    }

    [Theory]
    [InlineData("test@example.com", "0123456789")]
    [InlineData("user@domain.co.uk", "+84987654321")]
    [InlineData("complex.email+tag@sub.domain.org", "84123456789")]
    public async Task Handle_VariousValidInputs_ShouldCreateUsersSuccessfully(string email, string phone)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, email)
            .With(x => x.Password, "password123")
            .With(x => x.Phone, phone)
            .With(x => x.FullName, "Test User")
            .Create();

        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns("hashed_password");

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.Phone.Should().Be(phone);
    }

    [Fact]
    public async Task Handle_ResponseIdShouldMatchDomainEntityId()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "password123")
            .With(x => x.FullName, "Test User")
            .Create();

        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns("hashed_password");

        CoreUser? capturedUser = null;
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .Callback<CoreUser, CancellationToken>((user, ct) => capturedUser = user)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().Be(capturedUser!.Id);
    }

    [Fact]
    public async Task Handle_DefaultValuesAreSetCorrectly()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Password: "password123",
            Phone: null,
            FullName: "Test User",
            Gender: null,
            DateOfBirth: null
        );

        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns("hashed_password");

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CoreUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Phone.Should().BeNull();
        result.Gender.Should().BeNull();
        result.DateOfBirth.Should().BeNull();
        result.Status.Should().Be((byte)UserStatus.Active);
        result.EmailVerified.Should().BeFalse();
        result.Roles.Should().NotBeNull().And.BeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
