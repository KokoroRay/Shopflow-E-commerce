using FluentAssertions;
using ShopFlow.Domain.Tests.TestFixtures;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Component", "ValueObject")]
public class VietnameseTextHandlingTests : DomainTestBase
{
    #region ProductName Vietnamese Text Tests

    [Theory]
    [InlineData("Áo dài truyền thống Việt Nam")]
    [InlineData("Phở bò Hà Nội")]
    [InlineData("Bánh mì thịt nướng")]
    [InlineData("Cà phê sữa đá")]
    [InlineData("Nón lá Huế")]
    public void Constructor_ValidVietnameseProductName_ShouldCreateSuccessfully(string vietnameseName)
    {
        // Act
        var productName = ProductName.FromDisplayName(vietnameseName);

        // Assert
        productName.Value.Should().Be(vietnameseName);
        productName.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("Áo dài")]
    [InlineData("Phở")]
    [InlineData("Bánh")]
    public void Constructor_ShortVietnameseProductName_ShouldCreateSuccessfully(string shortName)
    {
        // Act
        var productName = ProductName.FromDisplayName(shortName);

        // Assert
        productName.Value.Should().Be(shortName);
        productName.Value.Length.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData("Vietnamese Traditional Dress")]
    [InlineData("Hanoi Beef Pho")]
    [InlineData("Vietnamese Coffee")]
    public void Constructor_EnglishProductName_ShouldCreateSuccessfully(string englishName)
    {
        // Act
        var productName = ProductName.FromDisplayName(englishName);

        // Assert
        productName.Value.Should().Be(englishName);
        productName.Value.Should().NotContainAny("ă", "â", "ê", "ô", "ư", "đ"); // No Vietnamese diacritics
    }

    #endregion

    #region ProductDescription Vietnamese Text Tests

    [Fact]
    public void Constructor_LongVietnameseDescription_ShouldCreateSuccessfully()
    {
        // Arrange
        var longVietnameseDescription =
            "Áo dài là trang phục truyền thống của phụ nữ Việt Nam, được may từ chất liệu lụa cao cấp. " +
            "Sản phẩm này được thiết kế tinh xảo với họa tiết hoa sen, thể hiện vẻ đẹp duyên dáng và thanh lịch " +
            "của người phụ nữ Việt. Phù hợp cho các dịp lễ tết, đám cưới hoặc sự kiện quan trọng.";

        // Act
        var productDescription = ProductDescription.FromContent(longVietnameseDescription);

        // Assert
        productDescription.Value.Should().Be(longVietnameseDescription);
        productDescription.Value.Should().Contain("Áo dài");
        productDescription.Value.Should().Contain("truyền thống");
        productDescription.Value.Should().Contain("Việt Nam");
    }

    [Fact]
    public void Constructor_BilingualDescription_ShouldCreateSuccessfully()
    {
        // Arrange
        var bilingualDescription =
            "Phở bò Hà Nội - Vietnamese traditional beef noodle soup. " +
            "Món ăn truyền thống của Việt Nam với nước dùng trong vắt, thịt bò tươi ngon và bánh phở dai ngon.";

        // Act
        var productDescription = ProductDescription.FromContent(bilingualDescription);

        // Assert
        productDescription.Value.Should().Be(bilingualDescription);
        productDescription.Value.Should().Contain("Phở bò");
        productDescription.Value.Should().Contain("Vietnamese traditional");
        productDescription.Value.Should().Contain("truyền thống");
    }

    #endregion

    #region ProductSlug Vietnamese Text Tests

    [Theory]
    [InlineData("ao-dai-truyen-thong-viet-nam")]
    [InlineData("pho-bo-ha-noi")]
    [InlineData("banh-mi-thit-nuong")]
    [InlineData("ca-phe-sua-da")]
    [InlineData("non-la-hue")]
    public void Constructor_ValidVietnameseSlug_ShouldCreateSuccessfully(string vietnameseSlug)
    {
        // Act
        var productSlug = ProductSlug.FromString(vietnameseSlug);

        // Assert
        productSlug.Value.Should().Be(vietnameseSlug);
        productSlug.Value.Should().NotContain(" "); // No spaces
        productSlug.Value.Should().NotContainAny("ă", "â", "ê", "ô", "ư", "đ"); // No Vietnamese diacritics
        productSlug.Value.Should().MatchRegex("^[a-z0-9-]+$"); // Only lowercase, numbers, and hyphens
    }

    [Theory]
    [InlineData("vietnamese-traditional-dress")]
    [InlineData("hanoi-beef-pho")]
    [InlineData("vietnamese-coffee")]
    public void Constructor_EnglishSlug_ShouldCreateSuccessfully(string englishSlug)
    {
        // Act
        var productSlug = ProductSlug.FromString(englishSlug);

        // Assert
        productSlug.Value.Should().Be(englishSlug);
        productSlug.Value.Should().MatchRegex("^[a-z0-9-]+$");
    }

    [Theory]
    [InlineData("Áo Dài Truyền Thống", "ao-dai-truyen-thong")]
    [InlineData("Phở Bò Hà Nội", "pho-bo-ha-noi")]
    [InlineData("Bánh Mì Thịt Nướng", "banh-mi-thit-nuong")]
    public void CreateFromVietnameseText_ShouldGenerateProperSlug(string vietnameseText, string expectedSlug)
    {
        // This test demonstrates how Vietnamese text should be converted to URL-friendly slugs
        // In actual implementation, this would be handled by a service

        // Act - This would be the expected behavior of a slug generation service
        var normalizedText = vietnameseText
            .ToLowerInvariant()
            .Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
            .Replace("ă", "a").Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
            .Replace("â", "a").Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
            .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
            .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
            .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
            .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
            .Replace("ô", "o").Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
            .Replace("ơ", "o").Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
            .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
            .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
            .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y")
            .Replace("đ", "d")
            .Replace(" ", "-");

        var productSlug = ProductSlug.FromString(normalizedText);

        // Assert
        productSlug.Value.Should().Be(expectedSlug);
    }

    #endregion

    #region Vietnamese Character Handling Tests

    [Theory]
    [InlineData("Hà Nội", true)]
    [InlineData("Hồ Chí Minh", true)]
    [InlineData("Đà Nẵng", true)]
    [InlineData("Huế", true)]
    [InlineData("Hải Phòng", true)]
    [InlineData("New York", false)]
    [InlineData("London", false)]
    [InlineData("Tokyo", false)]
    public void ContainsVietnameseCharacters_ShouldDetectCorrectly(string text, bool expectedResult)
    {
        // Arrange
        var vietnameseChars = new[] { 'ă', 'â', 'á', 'à', 'ả', 'ã', 'ạ', 'ắ', 'ằ', 'ẳ', 'ẵ', 'ặ',
                                     'ấ', 'ầ', 'ẩ', 'ẫ', 'ậ', 'đ', 'ê', 'é', 'è', 'ẻ', 'ẽ', 'ẹ',
                                     'ế', 'ề', 'ể', 'ễ', 'ệ', 'í', 'ì', 'ỉ', 'ĩ', 'ị', 'ô', 'ó',
                                     'ò', 'ỏ', 'õ', 'ọ', 'ố', 'ồ', 'ổ', 'ỗ', 'ộ', 'ơ', 'ớ', 'ờ',
                                     'ở', 'ỡ', 'ợ', 'ú', 'ù', 'ủ', 'ũ', 'ụ', 'ư', 'ứ', 'ừ', 'ử',
                                     'ữ', 'ự', 'ý', 'ỳ', 'ỷ', 'ỹ', 'ỵ' };

        // Act
        var hasVietnameseChars = text.Any(c => vietnameseChars.Contains(c));

        // Assert
        hasVietnameseChars.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("Món ăn ngon", "Delicious food")]
    [InlineData("Sản phẩm chất lượng", "Quality product")]
    [InlineData("Giao hàng nhanh", "Fast delivery")]
    public void BilingualContent_ShouldSupportBothLanguages(string vietnamese, string english)
    {
        // Act
        var vietnameseDescription = ProductDescription.FromContent(vietnamese);
        var englishDescription = ProductDescription.FromContent(english);

        // Assert
        vietnameseDescription.Value.Should().Be(vietnamese);
        englishDescription.Value.Should().Be(english);

        // Vietnamese should contain diacritics
        vietnameseDescription.Value.Should().ContainAny("ă", "â", "ê", "ô", "ư", "đ", "á", "à", "ả", "ã", "ạ");

        // English should not contain diacritics
        englishDescription.Value.Should().NotContainAny("ă", "â", "ê", "ô", "ư", "đ", "á", "à", "ả", "ã", "ạ");
    }

    #endregion

    #region Vietnamese Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_EmptyVietnameseText_ShouldThrowException(string? invalidText)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => ProductName.FromDisplayName(invalidText!));
    }

    [Theory]
    [InlineData("A")] // Too short (length 1, minimum is 2)
    public void Constructor_TooShortVietnameseText_ShouldThrowException(string shortText)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => ProductName.FromDisplayName(shortText));
    }

    [Fact]
    public void Constructor_TooLongVietnameseText_ShouldThrowException()
    {
        // Arrange - Create a string longer than allowed limit (e.g., 500 characters)
        var tooLongText = new string('A', 501) + " Việt Nam";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ProductName.FromDisplayName(tooLongText));
    }

    [Theory]
    [InlineData("Sản phẩm <script>alert('xss')</script>")]
    [InlineData("Áo dài & <html>")]
    [InlineData("Phở bò > SELECT * FROM products")]
    public void Constructor_VietnameseTextWithUnsafeCharacters_ShouldValidateCorrectly(string potentiallyUnsafeText)
    {
        // This test ensures that Vietnamese text with potentially unsafe characters
        // is handled appropriately by the value object validation

        // Act & Assert
        // In a real implementation, this might either sanitize the input or throw an exception
        // For this test, we'll assume it throws an exception for unsafe content
        Assert.Throws<ArgumentException>(() => ProductName.FromDisplayName(potentiallyUnsafeText));
    }

    #endregion
}