using FluentAssertions;
using ShopFlow.Domain.Tests.TestFixtures;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Component", "ValueObject")]
public class EmailTests : DomainTestBase
{
    #region Creation & Validation Tests (8 tests)

    [Fact]
    public void Constructor_ValidEmail_ShouldCreateEmailObject()
    {
        // Arrange
        var validEmail = "test@example.com";

        // Act
        var email = new Email(validEmail);

        // Assert
        email.Value.Should().Be(validEmail.ToLowerInvariant());
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("test123@sub.domain.org")]
    [InlineData("a@b.co")]
    public void Constructor_ValidEmailFormats_ShouldCreateEmailObject(string validEmail)
    {
        // Act
        var email = new Email(validEmail);

        // Assert
        email.Value.Should().Be(validEmail.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_EmptyOrWhitespaceEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new Email(invalidEmail), "Email cannot be empty");
    }

    [Fact]
    public void Constructor_NullEmail_ShouldThrowArgumentException()
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new Email(null!), "Email cannot be empty");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("test@")]
    [InlineData("test.domain.com")]
    [InlineData("test@domain")]
    [InlineData("test@.com")]
    [InlineData("test@domain.")]
    public void Constructor_InvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new Email(invalidEmail), "Invalid email format");
    }

    [Fact]
    public void Constructor_EmailWithUpperCase_ShouldNormalizeToLowerCase()
    {
        // Arrange
        var mixedCaseEmail = "Test.User@EXAMPLE.COM";

        // Act
        var email = new Email(mixedCaseEmail);

        // Assert
        email.Value.Should().Be("test.user@example.com");
    }

    [Fact]
    public void Constructor_EmailWithWhitespace_ShouldHandleCorrectly()
    {
        // Arrange
        var emailWithSpaces = " test@example.com ";

        // Act
        var email = new Email(emailWithSpaces);

        // Assert
        email.Value.Should().Be("test@example.com"); // Should be trimmed and normalized to lowercase
    }

    [Theory]
    [InlineData("test+tag@example.com")]
    [InlineData("test.name+tag@example.com")]
    [InlineData("test_name@example.com")]
    [InlineData("test-name@example.com")]
    public void Constructor_EmailWithSpecialCharacters_ShouldCreateEmailObject(string emailWithSpecialChars)
    {
        // Act
        var email = new Email(emailWithSpecialChars);

        // Assert
        email.Value.Should().Be(emailWithSpecialChars.ToLowerInvariant());
    }

    [Fact]
    public void Constructor_VeryLongEmail_ShouldCreateEmailObject()
    {
        // Arrange
        var longEmail = "a-very-long-email-address-that-might-be-used-in-testing@a-very-long-domain-name-example.com";

        // Act
        var email = new Email(longEmail);

        // Assert
        email.Value.Should().Be(longEmail.ToLowerInvariant());
    }

    #endregion

    #region Equality & Comparison Tests (6 tests)

    [Fact]
    public void Equals_SameEmailValues_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeTrue();
        email1.Should().Be(email2);
    }

    [Fact]
    public void Equals_DifferentCaseEmailValues_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("Test@Example.Com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeTrue();
        email1.Should().Be(email2);
    }

    [Fact]
    public void GetHashCode_SameEmailValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("TEST@EXAMPLE.COM");

        // Act & Assert
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act & Assert
        email.Equals(null).Should().BeFalse();
        email.Equals((object)null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameReference_ShouldReturnTrue()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act & Assert
        email.Equals(email).Should().BeTrue();
        ReferenceEquals(email, email).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentEmailValues_ShouldReturnFalse()
    {
        // Arrange
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeFalse();
        email1.Should().NotBe(email2);
    }

    #endregion

    #region Operators & Conversion Tests (6 tests)

    [Fact]
    public void ImplicitStringConversion_ShouldReturnEmailValue()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        string emailString = email;

        // Assert
        emailString.Should().Be("test@example.com");
    }

    [Fact]
    public void ExplicitEmailConversion_ValidString_ShouldCreateEmail()
    {
        // Arrange
        var emailString = "test@example.com";

        // Act
        var email = (Email)emailString;

        // Assert
        email.Value.Should().Be(emailString);
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        var result = email.ToString();

        // Assert
        result.Should().Be("test@example.com");
    }

    [Fact]
    public void EqualityOperator_SameValues_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        (email1 == email2).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_DifferentValues_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");

        // Act & Assert
        (email1 != email2).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_WithNull_ShouldHandleCorrectly()
    {
        // Arrange
        var email = new Email("test@example.com");
        Email? nullEmail = null;

        // Act & Assert
        (email == nullEmail).Should().BeFalse();
        (nullEmail == email).Should().BeFalse();
        (nullEmail == null).Should().BeTrue();
    }

    #endregion
}
