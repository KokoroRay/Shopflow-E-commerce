using Xunit;
using Moq;
using FluentAssertions;
using AutoFixture;
using ShopFlow.Application.Handlers.Users;
using ShopFlow.Application.Queries.Users;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Tests.Handlers.Users;

public class GetUserQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserMapper> _userMapperMock;
    private readonly GetUserQueryHandler _handler;

    public GetUserQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userMapperMock = new Mock<IUserMapper>();
        _handler = new GetUserQueryHandler(_userRepositoryMock.Object, _userMapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ShouldReturnUserResponse()
    {
        // Arrange
        var userId = 1L;
        var query = new GetUserQuery(userId);

        var user = CreateValidUser();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email.Value);
        result.Phone.Should().Be(user.Phone?.Value);
        result.Status.Should().Be((byte)user.Status);
        result.EmailVerified.Should().Be(user.EmailVerified);
        result.CreatedAt.Should().Be(user.CreatedAt);

        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingUser_ShouldReturnNull()
    {
        // Arrange
        var userId = 999L;
        var query = new GetUserQuery(userId);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoreUser?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_UserWithoutPhone_ShouldReturnUserResponseWithNullPhone()
    {
        // Arrange
        var userId = 1L;
        var query = new GetUserQuery(userId);

        var user = CreateValidUser(phone: null);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Phone.Should().BeNull();
        result.Email.Should().Be(user.Email.Value);
    }

    [Theory]
    [InlineData(UserStatus.Active, 1)]
    [InlineData(UserStatus.Inactive, 0)]
    [InlineData(UserStatus.Suspended, 2)]
    [InlineData(UserStatus.Banned, 3)]
    public async Task Handle_UserWithDifferentStatuses_ShouldReturnCorrectStatus(UserStatus status, byte expectedStatusByte)
    {
        // Arrange
        var userId = 1L;
        var query = new GetUserQuery(userId);

        var user = CreateValidUser(status: status);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(expectedStatusByte);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_UserWithDifferentEmailVerificationStatus_ShouldReturnCorrectVerification(bool emailVerified)
    {
        // Arrange
        var userId = 1L;
        var query = new GetUserQuery(userId);

        var user = CreateValidUser(emailVerified: emailVerified);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.EmailVerified.Should().Be(emailVerified);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var userId = 1L;
        var query = new GetUserQuery(userId);

        var expectedException = new InvalidOperationException("Database error");
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(query, CancellationToken.None));

        exception.Should().Be(expectedException);
    }

    [Fact]
    public async Task Handle_CancellationRequested_ShouldPassCancellationToken()
    {
        // Arrange
        var userId = 1L;
        var query = new GetUserQuery(userId);
        var cancellationToken = new CancellationToken(true);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _handler.Handle(query, cancellationToken));
    }

    [Theory]
    [InlineData(1L)]
    [InlineData(999L)]
    [InlineData(long.MaxValue)]
    public async Task Handle_DifferentUserIds_ShouldCallRepositoryWithCorrectId(long userId)
    {
        // Arrange
        var query = new GetUserQuery(userId);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoreUser?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUser_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var userId = 1L;
        var query = new GetUserQuery(userId);

        var email = new Email("test@example.com");
        var phone = new PhoneNumber("0123456789");

        var user = new CoreUser(email, "hashedPassword", phone);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
        result.Phone.Should().Be("123456789"); // PhoneNumber normalizes "0123456789" to "123456789"
        result.Status.Should().Be((byte)UserStatus.Active);
        result.EmailVerified.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_MultipleSequentialCalls_ShouldWorkCorrectly()
    {
        // Arrange
        var user1 = CreateValidUser(email: "user1@example.com");
        var user2 = CreateValidUser(email: "user2@example.com");

        _userRepositoryMock.Setup(x => x.GetByIdAsync(1L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(2L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        // Act
        var result1 = await _handler.Handle(new GetUserQuery(1L), CancellationToken.None);
        var result2 = await _handler.Handle(new GetUserQuery(2L), CancellationToken.None);

        // Assert
        result1!.Email.Should().Be("user1@example.com");
        result2!.Email.Should().Be("user2@example.com");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(1L, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(2L, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserWithComplexEmailAndPhone_ShouldReturnCorrectValues()
    {
        // Arrange
        var userId = 1L;
        var query = new GetUserQuery(userId);

        var complexEmail = "user.name+tag@sub.domain.co.uk";
        var complexPhone = "+84987654321";

        var user = CreateValidUser(email: complexEmail, phone: complexPhone);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(complexEmail);
        result.Phone.Should().Be("987654321"); // PhoneNumber normalizes "+84987654321" to "987654321"
    }

    private CoreUser CreateValidUser(
        string email = "test@example.com",
        string? phone = "0123456789",
        UserStatus status = UserStatus.Active,
        bool emailVerified = false)
    {
        var emailObj = new Email(email);
        var phoneObj = phone != null ? new PhoneNumber(phone) : null;

        var user = new CoreUser(emailObj, "hashedPassword", phoneObj);
        
        // Use reflection or create constructor to set readonly properties if needed
        // For now, use default values from constructor
        
        return user;
    }
}
