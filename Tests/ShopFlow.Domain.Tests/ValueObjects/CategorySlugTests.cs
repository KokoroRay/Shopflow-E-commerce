using FluentAssertions;
using ShopFlow.Domain.Tests.TestFixtures;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.ValueObjects;

public class CategorySlugTests : DomainTestBase
{
    [Fact]
    public void Constructor_ValidSlug_ShouldCreateCategorySlugObject()
    {
        // Arrange
        const string validSlug = "electronics";

        // Act
        var categorySlug = new CategorySlug(validSlug);

        // Assert
        categorySlug.Value.Should().Be(validSlug);
    }

    [Theory]
    [InlineData("electronics")]
    [InlineData("home-garden")]
    [InlineData("sports-outdoors")]
    [InlineData("books-media")]
    [InlineData("health-beauty")]
    [InlineData("toys-games")]
    [InlineData("auto-tools")]
    [InlineData("office-supplies")]
    public void Constructor_ValidSlugs_ShouldCreateCategorySlugObject(string validSlug)
    {
        // Act
        var categorySlug = new CategorySlug(validSlug);

        // Assert
        categorySlug.Value.Should().Be(validSlug);
    }

    [Fact]
    public void Constructor_EmptySlug_ShouldThrowArgumentException()
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategorySlug(""), "Category slug cannot be empty");
    }

    [Fact]
    public void Constructor_WhitespaceSlug_ShouldThrowArgumentException()
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategorySlug("   "), "Category slug cannot be empty");
    }

    [Fact]
    public void Constructor_NullSlug_ShouldThrowArgumentException()
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategorySlug(null!), "Category slug cannot be empty");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("x")]
    public void Constructor_TooShortSlug_ShouldThrowArgumentException(string shortSlug)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategorySlug(shortSlug), "Category slug must be at least 2 characters long");
    }

    [Fact]
    public void Constructor_TooLongSlug_ShouldThrowArgumentException()
    {
        // Arrange
        var longSlug = new string('a', 101);

        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategorySlug(longSlug), "Category slug cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("Category Slug")]
    [InlineData("Category@Slug")]
    [InlineData("Category#Slug")]
    [InlineData("Category$Slug")]
    [InlineData("Category%Slug")]
    [InlineData("Category^Slug")]
    [InlineData("Category*Slug")]
    [InlineData("Category+Slug")]
    [InlineData("Category=Slug")]
    [InlineData("Category[Slug]")]
    [InlineData("Category{Slug}")]
    [InlineData("Category|Slug")]
    [InlineData("Category\\Slug")]
    [InlineData("Category\"Slug")]
    [InlineData("Category'Slug")]
    [InlineData("Category<Slug>")]
    [InlineData("Category,Slug")]
    [InlineData("Category?Slug")]
    [InlineData("Category/Slug")]
    [InlineData("CategorySlug.")]
    [InlineData("CategorySlug_")]
    [InlineData("Category&Slug")]
    [InlineData("Category(Slug)")]
    public void Constructor_InvalidCharacters_ShouldThrowArgumentException(string invalidSlug)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategorySlug(invalidSlug), "Category slug must contain only lowercase letters, numbers, and hyphens");
    }

    [Theory]
    [InlineData("ELECTRONICS", "electronics")]
    [InlineData("Electronics", "electronics")]
    [InlineData("eLECTRONICS", "electronics")]
    [InlineData("HOME-GARDEN", "home-garden")]
    public void Constructor_UppercaseSlug_ShouldConvertToLowercase(string inputSlug, string expectedSlug)
    {
        // Act
        var categorySlug = new CategorySlug(inputSlug);

        // Assert
        categorySlug.Value.Should().Be(expectedSlug);
    }

    [Theory]
    [InlineData("  electronics  ", "electronics")]
    [InlineData("\thome-garden\t", "home-garden")]
    [InlineData("\nsports-outdoors\n", "sports-outdoors")]
    public void Constructor_SlugWithWhitespace_ShouldTrimWhitespace(string inputSlug, string expectedSlug)
    {
        // Act
        var categorySlug = new CategorySlug(inputSlug);

        // Assert
        categorySlug.Value.Should().Be(expectedSlug);
    }

    [Fact]
    public void ImplicitOperator_FromString_ShouldCreateCategorySlug()
    {
        // Arrange
        const string slug = "electronics";

        // Act
        CategorySlug categorySlug = new(slug);

        // Assert
        categorySlug.Value.Should().Be(slug);
    }

    [Fact]
    public void ImplicitOperator_ToString_ShouldReturnValue()
    {
        // Arrange
        var categorySlug = new CategorySlug("electronics");

        // Act
        string result = categorySlug.Value;

        // Assert
        result.Should().Be("electronics");
    }

    [Fact]
    public void Equals_SameCategorySlug_ShouldReturnTrue()
    {
        // Arrange
        var categorySlug1 = new CategorySlug("electronics");
        var categorySlug2 = new CategorySlug("electronics");

        // Act & Assert
        categorySlug1.Equals(categorySlug2).Should().BeTrue();
        (categorySlug1 == categorySlug2).Should().BeTrue();
        (categorySlug1 != categorySlug2).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentCategorySlug_ShouldReturnFalse()
    {
        // Arrange
        var categorySlug1 = new CategorySlug("electronics");
        var categorySlug2 = new CategorySlug("books");

        // Act & Assert
        categorySlug1.Equals(categorySlug2).Should().BeFalse();
        (categorySlug1 == categorySlug2).Should().BeFalse();
        (categorySlug1 != categorySlug2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var categorySlug = new CategorySlug("electronics");

        // Act & Assert
        categorySlug.Equals(null).Should().BeFalse();
        (categorySlug == null).Should().BeFalse();
        (null == categorySlug).Should().BeFalse();
        (categorySlug != null).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameCategorySlug_ShouldReturnSameHashCode()
    {
        // Arrange
        var categorySlug1 = new CategorySlug("electronics");
        var categorySlug2 = new CategorySlug("electronics");

        // Act & Assert
        categorySlug1.GetHashCode().Should().Be(categorySlug2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentCategorySlug_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var categorySlug1 = new CategorySlug("electronics");
        var categorySlug2 = new CategorySlug("books");

        // Act & Assert
        categorySlug1.GetHashCode().Should().NotBe(categorySlug2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var categorySlug = new CategorySlug("electronics");

        // Act
        var result = categorySlug.ToString();

        // Assert
        result.Should().Be("electronics");
    }

    [Fact]
    public void Constructor_MaxValidLength_ShouldCreateCategorySlug()
    {
        // Arrange
        var maxLengthSlug = new string('a', 100);

        // Act
        var categorySlug = new CategorySlug(maxLengthSlug);

        // Assert
        categorySlug.Value.Should().Be(maxLengthSlug);
        categorySlug.Value.Length.Should().Be(100);
    }

    [Fact]
    public void Constructor_MinValidLength_ShouldCreateCategorySlug()
    {
        // Arrange
        const string minLengthSlug = "ab";

        // Act
        var categorySlug = new CategorySlug(minLengthSlug);

        // Assert
        categorySlug.Value.Should().Be(minLengthSlug);
        categorySlug.Value.Length.Should().Be(2);
    }

    [Theory]
    [InlineData("product-name")]
    [InlineData("product123")]
    [InlineData("123product")]
    [InlineData("abc-def-ghi")]
    [InlineData("test-product-category")]
    public void Constructor_ValidFormats_ShouldCreateCategorySlug(string validSlug)
    {
        // Act
        var categorySlug = new CategorySlug(validSlug);

        // Assert
        categorySlug.Value.Should().Be(validSlug);
    }

    [Theory]
    [InlineData("-electronics")]
    [InlineData("electronics-")]
    [InlineData("--electronics")]
    [InlineData("electronics--")]
    [InlineData("elect--ronics")]
    public void Constructor_InvalidHyphenUsage_ShouldThrowArgumentException(string invalidSlug)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new CategorySlug(invalidSlug), "Category slug must contain only lowercase letters, numbers, and hyphens");
    }

    [Fact]
    public void Constructor_OnlyNumbers_ShouldCreateCategorySlug()
    {
        // Arrange
        const string numberSlug = "123456";

        // Act
        var categorySlug = new CategorySlug(numberSlug);

        // Assert
        categorySlug.Value.Should().Be(numberSlug);
    }

    [Fact]
    public void Constructor_OnlyLetters_ShouldCreateCategorySlug()
    {
        // Arrange
        const string letterSlug = "electronics";

        // Act
        var categorySlug = new CategorySlug(letterSlug);

        // Assert
        categorySlug.Value.Should().Be(letterSlug);
    }
}