using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using FluentAssertions;
using ShopFlow.API.Controllers;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Exceptions;

namespace ShopFlow.Application.Tests.Controllers;

/// <summary>
/// Unit tests for UsersController Login endpoint
/// </summary>
public class UsersControllerLoginTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UsersController _controller;

    public UsersControllerLoginTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new UsersController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Login_ValidRequest_ShouldReturnOkResultWithLoginResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var expectedResponse = new LoginResponse
        {
            AccessToken = "access_token_here",
            RefreshToken = "refresh_token_here",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserInfo
            {
                Id = 1,
                Email = "test@example.com",
                FullName = "Test User"
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<LoginCommand>(cmd =>
                    cmd.Email == request.Email &&
                    cmd.Password == request.Password),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_NullRequest_ShouldReturnBadRequest()
    {
        // Arrange
        LoginRequest? request = null;

        // Act
        var result = await _controller.Login(request!, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Request cannot be null");

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Login_AuthenticationException_ShouldThrowException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AuthenticationException("Invalid credentials"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(
            () => _controller.Login(request, CancellationToken.None));

        exception.Message.Should().Be("Invalid credentials");

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_DomainException_ShouldThrowException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DomainException("User account is disabled"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _controller.Login(request, CancellationToken.None));

        exception.Message.Should().Be("User account is disabled");

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_CancellationRequested_ShouldThrowOperationCancelledException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _controller.Login(request, cancellationTokenSource.Token));

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_ValidRequest_ShouldCreateCorrectLoginCommand()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "user@test.com",
            Password = "MySecurePassword"
        };

        var expectedResponse = new LoginResponse
        {
            AccessToken = "token",
            RefreshToken = "refresh",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserInfo
            {
                Id = 123,
                Email = request.Email,
                FullName = "Test User"
            }
        };

        LoginCommand? capturedCommand = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<LoginResponse>, CancellationToken>((cmd, ct) => capturedCommand = cmd as LoginCommand)
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.Login(request, CancellationToken.None);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.Email.Should().Be(request.Email);
        capturedCommand.Password.Should().Be(request.Password);
    }

    [Fact]
    public async Task Login_EmptyEmailAndPassword_ShouldStillCallMediator()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "",
            Password = ""
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Email is required"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _controller.Login(request, CancellationToken.None));

        exception.Message.Should().Be("Email is required");

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<LoginCommand>(cmd =>
                    cmd.Email == "" &&
                    cmd.Password == ""),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_SuccessfulLogin_ShouldReturnCompleteLoginResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "complete@test.com",
            Password = "CompletePassword123!"
        };

        var expectedResponse = new LoginResponse
        {
            AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
            RefreshToken = "refresh_token_sample",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserInfo
            {
                Id = 456,
                Email = "complete@test.com",
                FullName = "Complete Test User"
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as LoginResponse;

        response.Should().NotBeNull();
        response!.AccessToken.Should().Be(expectedResponse.AccessToken);
        response.RefreshToken.Should().Be(expectedResponse.RefreshToken);
        response.User.Id.Should().Be(expectedResponse.User.Id);
        response.User.Email.Should().Be(expectedResponse.User.Email);
        response.User.FullName.Should().Be(expectedResponse.User.FullName);
        response.ExpiresAt.Should().BeCloseTo(expectedResponse.ExpiresAt, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("test@example.com", "password")]
    [InlineData("another@test.com", "different_password")]
    [InlineData("user123@domain.org", "MyPassword123!")]
    public async Task Login_VariousValidInputs_ShouldCallMediatorCorrectly(string email, string password)
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var expectedResponse = new LoginResponse
        {
            AccessToken = "token",
            RefreshToken = "refresh",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserInfo
            {
                Id = 1,
                Email = email,
                FullName = "Test User"
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<LoginCommand>(cmd =>
                    cmd.Email == email &&
                    cmd.Password == password),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_UnexpectedException_ShouldThrowException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Login(request, CancellationToken.None));

        exception.Message.Should().Be("Database connection failed");

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
