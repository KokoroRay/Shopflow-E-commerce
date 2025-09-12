using FluentAssertions;
using ShopFlow.Domain.Tests.TestFixtures;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.ValueObjects;

public class CategoryNameTests : DomainTestBase
{
    [Fact]
    public void Constructor_ValidName_ShouldCreateCategoryNameObject()
    {
        // Arrange
        const string validName = "Electronics";

        // Act
        var categoryName = new CategoryName(validName);

        // Assert
        categoryName.Value.Should().Be(validName);
    }

    [Theory]
    [InlineData("Electronics")]
    [InlineData("Home & Garden")]
    [InlineData("Sports/Outdoors")]
    [InlineData("Books & Media")]
    [InlineData("Health & Beauty")]
    [InlineData("Toys & Games")]
    [InlineData("Auto & Tools")]
    [InlineData("Office Supplies")]
    public void Constructor_ValidCategoryNames_ShouldCreateCategoryNameObject(string validName)
    {
        // Act
        var categoryName = new CategoryName(validName);

        // Assert
        categoryName.Value.Should().Be(validName);
    }

    [Fact]
    public void Constructor_EmptyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategoryName(""), "Category name cannot be empty");
    }

    [Fact]
    public void Constructor_WhitespaceName_ShouldThrowArgumentException()
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategoryName("   "), "Category name cannot be empty");
    }

    [Fact]
    public void Constructor_NullName_ShouldThrowArgumentException()
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategoryName(null!), "Category name cannot be empty");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("X")]
    public void Constructor_TooShortName_ShouldThrowArgumentException(string shortName)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategoryName(shortName), "Category name must be at least 2 characters long");
    }

    [Fact]
    public void Constructor_TooLongName_ShouldThrowArgumentException()
    {
        // Arrange
        var longName = new string('A', 101);

        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategoryName(longName), "Category name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("Category@Name")]
    [InlineData("Category#Name")]
    [InlineData("Category$Name")]
    [InlineData("Category%Name")]
    [InlineData("Category^Name")]
    [InlineData("Category*Name")]
    [InlineData("Category+Name")]
    [InlineData("Category=Name")]
    [InlineData("Category[Name]")]
    [InlineData("Category{Name}")]
    [InlineData("Category|Name")]
    [InlineData("Category\\Name")]
    [InlineData("Category\"Name")]
    [InlineData("Category'Name")]
    [InlineData("Category<Name>")]
    [InlineData("Category,Name")]
    [InlineData("Category?Name")]
    public void Constructor_InvalidCharacters_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategoryName(invalidName), "Category name contains invalid characters");
    }

    [Theory]
    [InlineData("  Electronics  ", "Electronics")]
    [InlineData("\tHome & Garden\t", "Home & Garden")]
    [InlineData("\nSports/Outdoors\n", "Sports/Outdoors")]
    public void Constructor_NameWithWhitespace_ShouldTrimWhitespace(string inputName, string expectedName)
    {
        // Act
        var categoryName = new CategoryName(inputName);

        // Assert
        categoryName.Value.Should().Be(expectedName);
    }

    [Fact]
    public void ImplicitOperator_FromString_ShouldCreateCategoryName()
    {
        // Arrange
        const string name = "Electronics";

        // Act
        CategoryName categoryName = name;

        // Assert
        categoryName.Value.Should().Be(name);
    }

    [Fact]
    public void ImplicitOperator_ToString_ShouldReturnValue()
    {
        // Arrange
        var categoryName = new CategoryName("Electronics");

        // Act
        string result = categoryName;

        // Assert
        result.Should().Be("Electronics");
    }

    [Fact]
    public void Equals_SameCategoryName_ShouldReturnTrue()
    {
        // Arrange
        var categoryName1 = new CategoryName("Electronics");
        var categoryName2 = new CategoryName("Electronics");

        // Act & Assert
        categoryName1.Equals(categoryName2).Should().BeTrue();
        (categoryName1 == categoryName2).Should().BeTrue();
        (categoryName1 != categoryName2).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentCategoryName_ShouldReturnFalse()
    {
        // Arrange
        var categoryName1 = new CategoryName("Electronics");
        var categoryName2 = new CategoryName("Books");

        // Act & Assert
        categoryName1.Equals(categoryName2).Should().BeFalse();
        (categoryName1 == categoryName2).Should().BeFalse();
        (categoryName1 != categoryName2).Should().BeTrue();
    }

    [Fact]
    public void Equals_CaseInsensitive_ShouldReturnTrue()
    {
        // Arrange
        var categoryName1 = new CategoryName("Electronics");
        var categoryName2 = new CategoryName("electronics");

        // Act & Assert
        categoryName1.Equals(categoryName2).Should().BeTrue();
        (categoryName1 == categoryName2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var categoryName = new CategoryName("Electronics");

        // Act & Assert
        categoryName.Equals(null).Should().BeFalse();
        (categoryName == null).Should().BeFalse();
        (null == categoryName).Should().BeFalse();
        (categoryName != null).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameCategoryName_ShouldReturnSameHashCode()
    {
        // Arrange
        var categoryName1 = new CategoryName("Electronics");
        var categoryName2 = new CategoryName("Electronics");

        // Act & Assert
        categoryName1.GetHashCode().Should().Be(categoryName2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentCategoryName_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var categoryName1 = new CategoryName("Electronics");
        var categoryName2 = new CategoryName("Books");

        // Act & Assert
        categoryName1.GetHashCode().Should().NotBe(categoryName2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var categoryName = new CategoryName("Electronics");

        // Act
        var result = categoryName.ToString();

        // Assert
        result.Should().Be("Electronics");
    }

    [Fact]
    public void Constructor_MaxValidLength_ShouldCreateCategoryName()
    {
        // Arrange
        var maxLengthName = new string('A', 100);

        // Act
        var categoryName = new CategoryName(maxLengthName);

        // Assert
        categoryName.Value.Should().Be(maxLengthName);
        categoryName.Value.Length.Should().Be(100);
    }

    [Fact]
    public void Constructor_MinValidLength_ShouldCreateCategoryName()
    {
        // Arrange
        const string minLengthName = "AB";

        // Act
        var categoryName = new CategoryName(minLengthName);

        // Assert
        categoryName.Value.Should().Be(minLengthName);
        categoryName.Value.Length.Should().Be(2);
    }

    [Theory]
    [InlineData("Product-Name")]
    [InlineData("Product_Name")]
    [InlineData("Product Name")]
    [InlineData("Product123")]
    [InlineData("Product & Co.")]
    [InlineData("Product/Service")]
    [InlineData("Product (New)")]
    [InlineData("123Product")]
    public void Constructor_ValidSpecialCharacters_ShouldCreateCategoryName(string validName)
    {
        // Act
        var categoryName = new CategoryName(validName);

        // Assert
        categoryName.Value.Should().Be(validName);
    }
}