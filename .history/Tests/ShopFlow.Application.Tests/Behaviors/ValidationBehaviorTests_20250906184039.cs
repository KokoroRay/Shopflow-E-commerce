using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using ShopFlow.Application.Behaviors;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Tests.TestFixtures;
using Xunit;
using ValidationException = ShopFlow.Application.Exceptions.ValidationException;

namespace ShopFlow.Application.Tests.Behaviors;

/// <summary>
/// Comprehensive tests for ValidationBehavior ensuring proper MediatR pipeline validation
/// </summary>
public class ValidationBehaviorTests : ApplicationTestBase
{
    private readonly Mock<IValidator<CreateUserCommand>> _mockValidator;
    private readonly Mock<RequestHandlerDelegate<UserResponse>> _mockNext;
    private readonly ValidationBehavior<CreateUserCommand, UserResponse> _behavior;

    public ValidationBehaviorTests()
    {
        _mockValidator = new Mock<IValidator<CreateUserCommand>>();
        _mockNext = new Mock<RequestHandlerDelegate<UserResponse>>();
        
        var validators = new List<IValidator<CreateUserCommand>> { _mockValidator.Object };
        _behavior = new ValidationBehavior<CreateUserCommand, UserResponse>(validators);
    }

    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        // Arrange
        var emptyValidators = new List<IValidator<CreateUserCommand>>();
        var behavior = new ValidationBehavior<CreateUserCommand, UserResponse>(emptyValidators);
        var command = Fixture.Create<CreateUserCommand>();
        var expectedResponse = Fixture.Create<UserResponse>();
        
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        var result = await behavior.Handle(command, _mockNext.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        _mockNext.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCallNext()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var expectedResponse = Fixture.Create<UserResponse>();
        var validationResult = new ValidationResult();

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        var result = await _behavior.Handle(command, _mockNext.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        _mockNext.Verify(x => x(), Times.Once);
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required"),
            new ValidationFailure("Password", "Password is too short")
        };
        var validationResult = new ValidationResult(validationFailures);

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _behavior.Handle(command, _mockNext.Object, CancellationToken.None));

        exception.Message.Should().Be("Validation failed");
        exception.Errors.Should().ContainKey("Email");
        exception.Errors.Should().ContainKey("Password");
        exception.Errors["Email"].Should().Contain("Email is required");
        exception.Errors["Password"].Should().Contain("Password is too short");
        
        _mockNext.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_MultipleValidationErrors_ShouldGroupErrorsByProperty()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required"),
            new ValidationFailure("Email", "Email format is invalid"),
            new ValidationFailure("Password", "Password is required"),
            new ValidationFailure("Password", "Password must contain uppercase"),
            new ValidationFailure("Password", "Password must contain special character")
        };
        var validationResult = new ValidationResult(validationFailures);

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _behavior.Handle(command, _mockNext.Object, CancellationToken.None));

        exception.Errors.Should().HaveCount(2);
        exception.Errors["Email"].Should().HaveCount(2);
        exception.Errors["Email"].Should().Contain("Email is required");
        exception.Errors["Email"].Should().Contain("Email format is invalid");
        exception.Errors["Password"].Should().HaveCount(3);
        exception.Errors["Password"].Should().Contain("Password is required");
        exception.Errors["Password"].Should().Contain("Password must contain uppercase");
        exception.Errors["Password"].Should().Contain("Password must contain special character");
    }

    [Fact]
    public async Task Handle_MultipleValidators_ShouldRunAllValidators()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var mockValidator2 = new Mock<IValidator<CreateUserCommand>>();
        var validators = new List<IValidator<CreateUserCommand>> { _mockValidator.Object, mockValidator2.Object };
        var behavior = new ValidationBehavior<CreateUserCommand, UserResponse>(validators);
        
        var validationResult1 = new ValidationResult();
        var validationResult2 = new ValidationResult();
        var expectedResponse = Fixture.Create<UserResponse>();

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult1);
        mockValidator2.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult2);
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        var result = await behavior.Handle(command, _mockNext.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()), Times.Once);
        mockValidator2.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockNext.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_MultipleValidatorsWithFailures_ShouldCombineAllFailures()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var mockValidator2 = new Mock<IValidator<CreateUserCommand>>();
        var validators = new List<IValidator<CreateUserCommand>> { _mockValidator.Object, mockValidator2.Object };
        var behavior = new ValidationBehavior<CreateUserCommand, UserResponse>(validators);

        var validationFailures1 = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required")
        };
        var validationFailures2 = new List<ValidationFailure>
        {
            new ValidationFailure("Password", "Password is required"),
            new ValidationFailure("Phone", "Phone format is invalid")
        };

        var validationResult1 = new ValidationResult(validationFailures1);
        var validationResult2 = new ValidationResult(validationFailures2);

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult1);
        mockValidator2.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult2);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(command, _mockNext.Object, CancellationToken.None));

        exception.Errors.Should().HaveCount(3);
        exception.Errors.Should().ContainKey("Email");
        exception.Errors.Should().ContainKey("Password");
        exception.Errors.Should().ContainKey("Phone");
        exception.Errors["Email"].Should().Contain("Email is required");
        exception.Errors["Password"].Should().Contain("Password is required");
        exception.Errors["Phone"].Should().Contain("Phone format is invalid");
    }

    [Fact]
    public async Task Handle_CancellationTokenPassed_ShouldPassToValidators()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var cancellationToken = new CancellationTokenSource().Token;
        var validationResult = new ValidationResult();
        var expectedResponse = Fixture.Create<UserResponse>();

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), cancellationToken))
            .ReturnsAsync(validationResult);
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        var result = await _behavior.Handle(command, _mockNext.Object, cancellationToken);

        // Assert
        result.Should().Be(expectedResponse);
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidatorThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var expectedException = new InvalidOperationException("Validator error");

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _behavior.Handle(command, _mockNext.Object, CancellationToken.None));

        exception.Should().Be(expectedException);
        _mockNext.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_NextHandlerThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var validationResult = new ValidationResult();
        var expectedException = new InvalidOperationException("Handler error");

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockNext.Setup(x => x()).ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _behavior.Handle(command, _mockNext.Object, CancellationToken.None));

        exception.Should().Be(expectedException);
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationContextCreatedCorrectly_ShouldUseCorrectRequest()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var validationResult = new ValidationResult();
        var expectedResponse = Fixture.Create<UserResponse>();
        IValidationContext? capturedContext = null;

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .Callback<IValidationContext, CancellationToken>((context, ct) => capturedContext = context)
            .ReturnsAsync(validationResult);
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        await _behavior.Handle(command, _mockNext.Object, CancellationToken.None);

        // Assert
        capturedContext.Should().NotBeNull();
        capturedContext!.InstanceToValidate.Should().Be(command);
    }

    [Theory]
    [InlineData(true, 1)]  // Valid request should call next
    [InlineData(false, 0)] // Invalid request should not call next
    public async Task Handle_ValidationResult_ShouldCallNextAppropriately(bool isValid, int expectedNextCalls)
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var validationResult = isValid 
            ? new ValidationResult() 
            : new ValidationResult(new[] { new ValidationFailure("Test", "Test error") });
        var expectedResponse = Fixture.Create<UserResponse>();

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act & Assert
        if (isValid)
        {
            var result = await _behavior.Handle(command, _mockNext.Object, CancellationToken.None);
            result.Should().Be(expectedResponse);
        }
        else
        {
            await Assert.ThrowsAsync<ValidationException>(() =>
                _behavior.Handle(command, _mockNext.Object, CancellationToken.None));
        }

        _mockNext.Verify(x => x(), Times.Exactly(expectedNextCalls));
    }

    [Fact]
    public async Task Handle_EmptyValidationFailures_ShouldCallNext()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var validationResult = new ValidationResult(new List<ValidationFailure>());
        var expectedResponse = Fixture.Create<UserResponse>();

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        var result = await _behavior.Handle(command, _mockNext.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        _mockNext.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFailureWithNullPropertyName_ShouldHandleGracefully()
    {
        // Arrange
        var command = Fixture.Create<CreateUserCommand>();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("", "Empty property validation error"),
            new ValidationFailure("Email", "Email specific error")
        };
        var validationResult = new ValidationResult(validationFailures);

        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _behavior.Handle(command, _mockNext.Object, CancellationToken.None));

        exception.Errors.Should().ContainKey("");
        exception.Errors.Should().ContainKey("Email");
        exception.Errors[""].Should().HaveCount(1);
        exception.Errors["Email"].Should().HaveCount(1);
    }
}
