using FluentAssertions;
using FluentValidation.TestHelper;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Validators.Users;

namespace ShopFlow.Application.Tests.Validators.Users;

/// <summary>
/// Unit tests for RegisterUserCommandValidator
/// </summary>
public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator;

    public RegisterUserCommandValidatorTests()
    {
        _validator = new RegisterUserCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "+84987654321",
            "John Doe",
            "M",
            new DateTime(1990, 1, 1));

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
        var command = new RegisterUserCommand(
            email,
            "Password123!",
            null,
            null,
            null,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    public void Validate_InvalidEmailFormat_ShouldHaveValidationError(string email)
    {
        // Arrange
        var command = new RegisterUserCommand(
            email,
            "Password123!",
            null,
            null,
            null,
            null);

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
        var command = new RegisterUserCommand(
            "test@example.com",
            password,
            null,
            null,
            null,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Validate_ShortPassword_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Pass1!",
            null,
            null,
            null,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long");
    }

    [Theory]
    [InlineData("password123!")] // No uppercase
    [InlineData("PASSWORD123!")] // No lowercase
    [InlineData("Password!")] // No digit
    [InlineData("Password123")] // No special character
    public void Validate_WeakPassword_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            password,
            null,
            null,
            null,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");
    }

    [Theory]
    [InlineData("invalid-phone")]
    [InlineData("123")]
    [InlineData("abc")]
    public void Validate_InvalidPhoneNumber_ShouldHaveValidationError(string phone)
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            phone,
            null,
            null,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Invalid phone number format");
    }

    [Theory]
    [InlineData("+84987654321")]
    [InlineData("0987654321")]
    [InlineData("84987654321")]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ValidPhoneNumber_ShouldNotHaveValidationError(string phone)
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            phone,
            null,
            null,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validate_TooLongFullName_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('A', 201);
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            null,
            longName,
            null,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("X")]
    [InlineData("Male")]
    [InlineData("Female")]
    public void Validate_InvalidGender_ShouldHaveValidationError(string gender)
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            null,
            null,
            gender,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Gender)
            .WithErrorMessage("Gender must be 'M', 'F', or 'O'");
    }

    [Theory]
    [InlineData("M")]
    [InlineData("F")]
    [InlineData("O")]
    [InlineData("m")]
    [InlineData("f")]
    [InlineData("o")]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ValidGender_ShouldNotHaveValidationError(string gender)
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            null,
            null,
            gender,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public void Validate_FutureDateOfBirth_ShouldHaveValidationError()
    {
        // Arrange
        var futureDate = DateTime.Today.AddDays(1);
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            null,
            null,
            null,
            futureDate);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Date of birth must be in the past");
    }

    [Fact]
    public void Validate_TooOldDateOfBirth_ShouldHaveValidationError()
    {
        // Arrange
        var tooOldDate = DateTime.Today.AddYears(-121);
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            null,
            null,
            null,
            tooOldDate);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Date of birth cannot be more than 120 years ago");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-50)]
    [InlineData(-119)]
    public void Validate_ValidDateOfBirth_ShouldNotHaveValidationError(int yearsAgo)
    {
        // Arrange
        var validDate = DateTime.Today.AddYears(yearsAgo);
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            null,
            null,
            null,
            validDate);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
    }
}
