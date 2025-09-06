using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Tests.TestFixtures;
using ShopFlow.Application.Validators.Users;
using Xunit;

namespace ShopFlow.Application.Tests.Validators.Users;

/// <summary>
/// Comprehensive tests for CreateUserCommandValidator ensuring all validation rules work correctly
/// </summary>
public class CreateUserCommandValidatorTests : ApplicationTestBase
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    #region Email Validation Tests

    [Fact]
    public void Validate_ValidEmail_ShouldPass()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.FullName, "John Doe")
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyEmail_ShouldFail(string email)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, email)
            .Create();

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
    [InlineData("test@.com")]
    [InlineData("test@example.")]
    public void Validate_InvalidEmailFormat_ShouldFail(string email)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, email)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email format is invalid");
    }

    [Fact]
    public void Validate_EmailTooLong_ShouldFail()
    {
        // Arrange
        var longEmail = new string('a', 90) + "@example.com"; // Over 100 characters
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, longEmail)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must not exceed 100 characters");
    }

    [Fact]
    public void Validate_EmailExactly100Characters_ShouldPass()
    {
        // Arrange
        var email = new string('a', 88) + "@example.com"; // Exactly 100 characters
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, email)
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.FullName, "John Doe")
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    #endregion

    #region Password Validation Tests

    [Fact]
    public void Validate_ValidPassword_ShouldPass()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.FullName, "John Doe")
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyPassword_ShouldFail(string password)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Password, password)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    [InlineData("Pass123")]
    public void Validate_PasswordTooShort_ShouldFail(string password)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Password, password)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters");
    }

    [Fact]
    public void Validate_PasswordTooLong_ShouldFail()
    {
        // Arrange
        var longPassword = new string('a', 101); // Over 100 characters
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Password, longPassword)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must not exceed 100 characters");
    }

    [Theory]
    [InlineData("password123!")] // No uppercase
    [InlineData("PASSWORD123!")] // No lowercase
    [InlineData("Password!")] // No digit
    [InlineData("Password123")] // No special character
    [InlineData("password")] // No uppercase, digit, or special character
    [InlineData("12345678")] // No letters or special characters
    [InlineData("PASSWORD")] // No lowercase, digit, or special character
    public void Validate_PasswordMissingRequiredCharacters_ShouldFail(string password)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Password, password)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit and one special character");
    }

    [Theory]
    [InlineData("Password123!")]
    [InlineData("MySecure1@")]
    [InlineData("Complex9$")]
    [InlineData("Valid1Pass!")]
    public void Validate_ValidPasswordFormats_ShouldPass(string password)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, password)
            .With(x => x.FullName, "John Doe")
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    #endregion

    #region Phone Validation Tests

    [Fact]
    public void Validate_ValidPhone_ShouldPass()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.Phone, "1234567890")
            .With(x => x.FullName, "John Doe")
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validate_NullPhone_ShouldPass()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.Phone, (string?)null)
            .With(x => x.FullName, "John Doe")
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validate_EmptyPhone_ShouldPass()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.Phone, "")
            .With(x => x.FullName, "John Doe")
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Theory]
    [InlineData("0123456789")] // Starts with 0
    [InlineData("abc123")] // Contains letters
    [InlineData("123-456-789")] // Contains dashes
    [InlineData("123 456 789")] // Contains spaces
    [InlineData("123456789012345678")] // Too long (over 15 digits)
    [InlineData("1")] // Too short
    [InlineData("+0123456789")] // Starts with +0
    public void Validate_InvalidPhoneFormat_ShouldFail(string phone)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Phone, phone)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number format is invalid");
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("+1234567890")]
    [InlineData("987654321")]
    [InlineData("+84987654321")]
    [InlineData("12345")]
    [InlineData("+112345")]
    public void Validate_ValidPhoneFormats_ShouldPass(string phone)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.Phone, phone)
            .With(x => x.FullName, "John Doe")
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    #endregion

    #region FullName Validation Tests

    [Fact]
    public void Validate_ValidFullName_ShouldPass()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.FullName, "John Doe")
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FullName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyFullName_ShouldFail(string fullName)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.FullName, fullName)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name is required");
    }

    [Fact]
    public void Validate_FullNameTooLong_ShouldFail()
    {
        // Arrange
        var longName = new string('a', 101); // Over 100 characters
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.FullName, longName)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name must not exceed 100 characters");
    }

    [Fact]
    public void Validate_FullNameExactly100Characters_ShouldPass()
    {
        // Arrange
        var name = new string('a', 100); // Exactly 100 characters
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.FullName, name)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FullName);
    }

    #endregion

    #region Gender Validation Tests

    [Theory]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("Other")]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ValidGender_ShouldPass(string gender)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.FullName, "John Doe")
            .With(x => x.Gender, gender)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Gender);
    }

    [Theory]
    [InlineData("male")] // Wrong case
    [InlineData("female")] // Wrong case
    [InlineData("other")] // Wrong case
    [InlineData("Unknown")]
    [InlineData("NonBinary")]
    [InlineData("Invalid")]
    public void Validate_InvalidGender_ShouldFail(string gender)
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Gender, gender)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Gender)
            .WithErrorMessage("Gender must be Male, Female, or Other");
    }

    #endregion

    #region DateOfBirth Validation Tests

    [Fact]
    public void Validate_NullDateOfBirth_ShouldPass()
    {
        // Arrange
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.FullName, "John Doe")
            .With(x => x.DateOfBirth, (DateTime?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
    }

    [Fact]
    public void Validate_ValidAge_ShouldPass()
    {
        // Arrange
        var validDateOfBirth = DateTime.Now.AddYears(-20); // 20 years old
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.FullName, "John Doe")
            .With(x => x.DateOfBirth, validDateOfBirth)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
    }

    [Fact]
    public void Validate_TooYoung_ShouldFail()
    {
        // Arrange
        var tooYoungDateOfBirth = DateTime.Now.AddYears(-10); // 10 years old
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.DateOfBirth, tooYoungDateOfBirth)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Must be at least 13 years old");
    }

    [Fact]
    public void Validate_Exactly13YearsOld_ShouldPass()
    {
        // Arrange
        var exactlyThirteenYearsAgo = DateTime.Now.AddYears(-13).AddDays(-1); // Just over 13 years old
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "ValidPass123!")
            .With(x => x.FullName, "John Doe")
            .With(x => x.DateOfBirth, exactlyThirteenYearsAgo)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
    }

    [Fact]
    public void Validate_FutureDate_ShouldFail()
    {
        // Arrange
        var futureDate = DateTime.Now.AddYears(1);
        var command = Fixture.Build<CreateUserCommand>()
            .With(x => x.DateOfBirth, futureDate)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Must be at least 13 years old");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Validate_FullyValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            Phone: "1234567890",
            FullName: "John Doe",
            Gender: "Male",
            DateOfBirth: DateTime.Now.AddYears(-25)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_MultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "invalid-email",
            Password: "weak",
            Phone: "invalid-phone",
            FullName: "",
            Gender: "invalid",
            DateOfBirth: DateTime.Now.AddYears(-5)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
        result.ShouldHaveValidationErrorFor(x => x.Gender);
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
    }

    #endregion
}
