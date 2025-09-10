using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Handlers.Users;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Exceptions;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Tests.Handlers.Users;

/// <summary>
/// Unit tests for LoginCommandHandler
/// </summary>
public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();

        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var command = new LoginCommand(email, password);

        var emailValueObject = new Email(email);
        var user = CreateTestUser(emailValueObject);
        var hashedPassword = "hashedPassword123";
        var accessToken = "access.token.here";
        var refreshToken = "refresh.token.here";
        var tokenExpiration = DateTime.UtcNow.AddHours(1);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.Is<Email>(e => e.Value == email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(true);

        _jwtTokenServiceMock
            .Setup(x => x.GenerateAccessToken(user.Id, email, It.IsAny<IEnumerable<string>>()))
            .Returns(accessToken);

        _jwtTokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(refreshToken);

        _jwtTokenServiceMock
            .Setup(x => x.GetTokenExpiration(accessToken))
            .Returns(tokenExpiration);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshToken);
        result.ExpiresAt.Should().Be(tokenExpiration);
        result.User.Should().NotBeNull();
        result.User.Id.Should().Be(user.Id);
        result.User.Email.Should().Be(email);
        result.User.Roles.Should().Contain("Customer");

        // Verify interactions
        _userRepositoryMock.Verify(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(password, user.PasswordHash), Times.Once);
        _jwtTokenServiceMock.Verify(x => x.GenerateAccessToken(user.Id, email, It.IsAny<IEnumerable<string>>()), Times.Once);
        _jwtTokenServiceMock.Verify(x => x.GenerateRefreshToken(), Times.Once);
        _jwtTokenServiceMock.Verify(x => x.GetTokenExpiration(accessToken), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowAuthenticationException()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "Password123!";
        var command = new LoginCommand(email, password);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoreUser?)null);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid email or password");

        // Verify that password verification was not called
        _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _jwtTokenServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldThrowAuthenticationException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword";
        var command = new LoginCommand(email, password);

        var emailValueObject = new Email(email);
        var user = CreateTestUser(emailValueObject);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(false);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid email or password");

        // Verify that JWT token generation was not called
        _jwtTokenServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InactiveUser_ShouldThrowAuthenticationException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var command = new LoginCommand(email, password);

        var emailValueObject = new Email(email);
        var user = CreateTestUser(emailValueObject, UserStatus.Inactive);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(true);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("User account is not active");

        // Verify that JWT token generation was not called
        _jwtTokenServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidEmailFormat_ShouldThrowAuthenticationException()
    {
        // Arrange
        var invalidEmail = "invalid-email-format";
        var password = "Password123!";
        var command = new LoginCommand(invalidEmail, password);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid email format");

        // Verify that repository was not called
        _userRepositoryMock.Verify(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = async () => await _handler.Handle(null!, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldThrowDomainException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var command = new LoginCommand(email, password);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("An error occurred during login. Please try again.");
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new LoginCommandHandler(
            null!,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _loggerMock.Object);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("userRepository");
    }

    [Fact]
    public void Constructor_WithNullPasswordHasher_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new LoginCommandHandler(
            _userRepositoryMock.Object,
            null!,
            _jwtTokenServiceMock.Object,
            _loggerMock.Object);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("passwordHasher");
    }

    [Fact]
    public void Constructor_WithNullJwtTokenService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new LoginCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            null!,
            _loggerMock.Object);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("jwtTokenService");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new LoginCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    private static CoreUser CreateTestUser(Email email, UserStatus status = UserStatus.Active)
    {
        var user = new CoreUser(email, "hashedPassword123");
        
        // Use reflection to set the status since it might be private set
        var statusProperty = typeof(CoreUser).GetProperty("Status");
        statusProperty?.SetValue(user, status);
        
        // Set ID using reflection
        var idProperty = typeof(CoreUser).GetProperty("Id");
        idProperty?.SetValue(user, 123L);

        return user;
    }
}
