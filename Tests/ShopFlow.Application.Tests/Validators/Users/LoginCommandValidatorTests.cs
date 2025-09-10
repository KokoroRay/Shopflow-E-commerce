using FluentAssertions;
using FluentValidation.TestHelper;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Validators.Users;

namespace ShopFlow.Application.Tests.Validators.Users;

/// <summary>
/// Unit tests for LoginCommandValidator
/// </summary>
public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new LoginCommand(
            "test@example.com",
            "Password123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyEmail_ShouldHaveValidationError(string email)
    {
        // Arrange
        var command = new LoginCommand(email, "Password123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("test.example.com")]
    [InlineData("test@example")]
    public void Validate_InvalidEmailFormat_ShouldHaveValidationError(string email)
    {
        // Arrange
        var command = new LoginCommand(email, "Password123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyPassword_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new LoginCommand("test@example.com", password);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Theory]
    [InlineData("1234567")] // 7 characters
    [InlineData("short")]   // 5 characters
    [InlineData("a")]       // 1 character
    public void Validate_PasswordTooShort_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new LoginCommand("test@example.com", password);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long");
    }

    [Theory]
    [InlineData("test@example.com", "Password123!")]
    [InlineData("user.name+tag@example.com", "12345678")]
    [InlineData("test@sub.example.com", "verylongpassword")]
    [InlineData("test123@example.org", "SimplePass")]
    public void Validate_ValidEmailAndPassword_ShouldNotHaveValidationErrors(string email, string password)
    {
        // Arrange
        var command = new LoginCommand(email, password);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmailWithSpecialCharacters_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new LoginCommand("user.name+tag@example.com", "Password123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrors(x => x.Email);
    }

    [Fact]
    public void Validate_ExactlyEightCharacterPassword_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "12345678");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrors(x => x.Password);
    }

    [Fact]
    public void Validate_LongPassword_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var longPassword = new string('a', 100); // 100 characters
        var command = new LoginCommand("test@example.com", longPassword);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrors(x => x.Password);
    }

    [Fact]
    public void Validate_MultipleValidationErrors_ShouldHaveAllErrors()
    {
        // Arrange
        var command = new LoginCommand("", "123"); // Invalid email and short password

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
        result.Errors.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("TEST@EXAMPLE.COM")]
    [InlineData("Test@Example.Com")]
    [InlineData("test@EXAMPLE.COM")]
    public void Validate_EmailWithDifferentCasing_ShouldNotHaveValidationErrors(string email)
    {
        // Arrange
        var command = new LoginCommand(email, "Password123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrors(x => x.Email);
    }
}
