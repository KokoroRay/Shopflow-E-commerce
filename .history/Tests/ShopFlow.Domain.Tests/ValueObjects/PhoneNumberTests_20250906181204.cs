using FluentAssertions;
using ShopFlow.Domain.Tests.TestFixtures;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Component", "ValueObject")]
public class PhoneNumberTests : DomainTestBase
{
    #region Creation & Validation Tests (10 tests)

    [Theory]
    [InlineData("0901234567", "901234567")]
    [InlineData("0912345678", "912345678")]
    [InlineData("84901234567", "901234567")]
    [InlineData("+84901234567", "901234567")]
    public void Constructor_ValidVietnamesePhone_ShouldCreatePhoneNumberObject(string input, string expected)
    {
        // Act
        var phoneNumber = new PhoneNumber(input);

        // Assert
        phoneNumber.Value.Should().Be(expected);
    }

    [Fact]
    public void Constructor_ValidPhoneWithNormalization_ShouldNormalizeCorrectly()
    {
        // Arrange
        var phoneWithCountryCode = "84901234567";

        // Act
        var phoneNumber = new PhoneNumber(phoneWithCountryCode);

        // Assert
        phoneNumber.Value.Should().Be("901234567");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_EmptyOrNullPhone_ShouldThrowArgumentException(string invalidPhone)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new PhoneNumber(invalidPhone), "Phone number cannot be empty");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    [InlineData("0123456789012")]
    [InlineData("abc1234567")]
    [InlineData("0801234567")] // Invalid prefix for Vietnamese numbers
    public void Constructor_InvalidPhoneFormat_ShouldThrowArgumentException(string invalidPhone)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new PhoneNumber(invalidPhone), "Invalid phone number format");
    }

    [Theory]
    [InlineData("090-123-4567")]
    [InlineData("090 123 4567")]
    [InlineData("(090) 123-4567")]
    [InlineData("+84-90-123-4567")]
    public void Constructor_PhoneWithSpecialCharacters_ShouldNormalizeProperly(string phoneWithSpecialChars)
    {
        // Act
        var phoneNumber = new PhoneNumber(phoneWithSpecialChars);

        // Assert
        phoneNumber.Value.Should().MatchRegex(@"^\d{9}$");
    }

    [Fact]
    public void Constructor_PhoneStartingWithZero_ShouldRemoveLeadingZero()
    {
        // Arrange
        var phoneWithZero = "0901234567";

        // Act
        var phoneNumber = new PhoneNumber(phoneWithZero);

        // Assert
        phoneNumber.Value.Should().Be("901234567");
        phoneNumber.Value.Should().NotStartWith("0");
    }

    [Fact]
    public void Constructor_PhoneStartingWith84_ShouldRemoveCountryCode()
    {
        // Arrange
        var phoneWith84 = "84901234567";

        // Act
        var phoneNumber = new PhoneNumber(phoneWith84);

        // Assert
        phoneNumber.Value.Should().Be("901234567");
        phoneNumber.Value.Should().NotStartWith("84");
    }

    [Theory]
    [InlineData("0901234567")]
    [InlineData("0912345678")]
    [InlineData("0923456789")]
    [InlineData("0934567890")]
    [InlineData("0945678901")]
    public void Constructor_ValidVietnameseMobileNumbers_ShouldCreatePhoneNumberObject(string validPhone)
    {
        // Act
        var phoneNumber = new PhoneNumber(validPhone);

        // Assert
        phoneNumber.Value.Should().HaveLength(9);
        phoneNumber.Value.Should().MatchRegex(@"^9\d{8}$");
    }

    [Fact]
    public void Constructor_PhoneNumberNormalization_ShouldHandleComplexFormats()
    {
        // Arrange
        var complexPhone = "+84 (0) 90-123-4567";

        // Act
        var phoneNumber = new PhoneNumber(complexPhone);

        // Assert
        phoneNumber.Value.Should().Be("901234567");
    }

    [Theory]
    [InlineData("0801234567")] // Invalid Vietnamese mobile prefix
    [InlineData("0701234567")] // Invalid Vietnamese mobile prefix
    [InlineData("0001234567")] // Invalid prefix
    public void Constructor_InvalidVietnamesePrefixes_ShouldThrowArgumentException(string invalidPrefix)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new PhoneNumber(invalidPrefix), "Invalid phone number format");
    }

    #endregion

    #region Equality & Comparison Tests (5 tests)

    [Fact]
    public void Equals_SamePhoneValues_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = new PhoneNumber("0901234567");
        var phone2 = new PhoneNumber("901234567");

        // Act & Assert
        phone1.Equals(phone2).Should().BeTrue();
        phone1.Should().Be(phone2);
    }

    [Fact]
    public void Equals_DifferentFormatsButSameNumber_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = new PhoneNumber("0901234567");
        var phone2 = new PhoneNumber("84901234567");
        var phone3 = new PhoneNumber("+84901234567");

        // Act & Assert
        phone1.Equals(phone2).Should().BeTrue();
        phone2.Equals(phone3).Should().BeTrue();
        phone1.Equals(phone3).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_NormalizedPhoneValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var phone1 = new PhoneNumber("0901234567");
        var phone2 = new PhoneNumber("84901234567");

        // Act & Assert
        phone1.GetHashCode().Should().Be(phone2.GetHashCode());
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var phone = new PhoneNumber("0901234567");

        // Act & Assert
        phone.Equals(null).Should().BeFalse();
        phone.Equals((object)null).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentPhoneValues_ShouldReturnFalse()
    {
        // Arrange
        var phone1 = new PhoneNumber("0901234567");
        var phone2 = new PhoneNumber("0912345678");

        // Act & Assert
        phone1.Equals(phone2).Should().BeFalse();
        phone1.Should().NotBe(phone2);
    }

    #endregion

    #region Operators & Conversion Tests (5 tests)

    [Fact]
    public void ImplicitStringConversion_ShouldReturnNormalizedPhoneValue()
    {
        // Arrange
        var phone = new PhoneNumber("0901234567");

        // Act
        string phoneString = phone;

        // Assert
        phoneString.Should().Be("901234567");
    }

    [Fact]
    public void ExplicitPhoneConversion_ValidString_ShouldCreatePhone()
    {
        // Arrange
        var phoneString = "0901234567";

        // Act
        var phone = (PhoneNumber)phoneString;

        // Assert
        phone.Value.Should().Be("901234567");
    }

    [Fact]
    public void ToString_ShouldReturnNormalizedPhoneValue()
    {
        // Arrange
        var phone = new PhoneNumber("84901234567");

        // Act
        var result = phone.ToString();

        // Assert
        result.Should().Be("901234567");
    }

    [Fact]
    public void EqualityOperator_NormalizedValues_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = new PhoneNumber("0901234567");
        var phone2 = new PhoneNumber("84901234567");

        // Act & Assert
        (phone1 == phone2).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_DifferentValues_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = new PhoneNumber("0901234567");
        var phone2 = new PhoneNumber("0912345678");

        // Act & Assert
        (phone1 != phone2).Should().BeTrue();
    }

    #endregion
}
