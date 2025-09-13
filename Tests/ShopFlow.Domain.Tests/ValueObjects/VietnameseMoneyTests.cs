using FluentAssertions;
using ShopFlow.Domain.Tests.TestFixtures;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Component", "ValueObject")]
public class VietnameseMoneyTests : DomainTestBase
{
    #region Vietnamese Currency Validation Tests

    [Fact]
    public void Constructor_ValidVndAmount_ShouldCreateMoneyObject()
    {
        // Arrange
        var amount = 250000m; // 250,000 VND
        var currency = "VND";

        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Constructor_ValidUsdAmount_ShouldCreateMoneyObject()
    {
        // Arrange
        var amount = 10.50m; // $10.50 USD
        var currency = "USD";

        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Theory]
    [InlineData("VND", 1000)]
    [InlineData("USD", 1.50)]
    public void Constructor_VietnameseMarketplaceCurrencies_ShouldBeValid(string currency, decimal amount)
    {
        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Currency.Should().Be(currency);
        money.Amount.Should().Be(amount);
    }

    #endregion

    #region Vietnamese Price Range Tests

    [Fact]
    public void IsInRange_VndPriceWithinRange_ShouldReturnTrue()
    {
        // Arrange
        var price = new Money(500000m, "VND"); // 500,000 VND
        var minPrice = new Money(100000m, "VND"); // 100,000 VND
        var maxPrice = new Money(1000000m, "VND"); // 1,000,000 VND

        // Act
        var isInRange = price >= minPrice && price <= maxPrice;

        // Assert
        isInRange.Should().BeTrue();
    }

    [Fact]
    public void IsInRange_UsdPriceWithinRange_ShouldReturnTrue()
    {
        // Arrange
        var price = new Money(25.50m, "USD");
        var minPrice = new Money(10.00m, "USD");
        var maxPrice = new Money(50.00m, "USD");

        // Act
        var isInRange = price >= minPrice && price <= maxPrice;

        // Assert
        isInRange.Should().BeTrue();
    }

    [Fact]
    public void Comparison_DifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var vndPrice = new Money(250000m, "VND");
        var usdPrice = new Money(10.50m, "USD");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => vndPrice > usdPrice);
    }

    #endregion

    #region Vietnamese VAT Calculation Tests

    [Theory]
    [InlineData(100000, 0.10, 110000)] // 10% VAT
    [InlineData(500000, 0.08, 540000)] // 8% VAT for some products
    [InlineData(1000000, 0.05, 1050000)] // 5% VAT for essential goods
    public void CalculateVat_VietnameseVatRates_ShouldReturnCorrectAmount(decimal baseAmount, decimal vatRate, decimal expectedTotal)
    {
        // Arrange
        var basePrice = new Money(baseAmount, "VND");
        var vatAmount = basePrice * vatRate;
        var totalWithVat = basePrice + vatAmount;

        // Act & Assert
        totalWithVat.Amount.Should().Be(expectedTotal);
        totalWithVat.Currency.Should().Be("VND");
    }

    #endregion

    #region Vietnamese Price Display Tests

    [Fact]
    public void ToString_VndAmount_ShouldFormatCorrectly()
    {
        // Arrange
        var price = new Money(250000m, "VND");

        // Act
        var formattedPrice = price.ToString();

        // Assert
        formattedPrice.Should().Contain("250000");
        formattedPrice.Should().Contain("VND");
    }

    [Fact]
    public void ToString_UsdAmount_ShouldFormatCorrectly()
    {
        // Arrange
        var price = new Money(10.50m, "USD");

        // Act
        var formattedPrice = price.ToString();

        // Assert
        formattedPrice.Should().Contain("10.50");
        formattedPrice.Should().Contain("USD");
    }

    #endregion

    #region Vietnamese Marketplace Business Logic Tests

    [Fact]
    public void IsMinimumOrderAmount_VndAmount_ShouldReturnCorrectResult()
    {
        // Arrange - Vietnamese marketplace minimum order is usually 50,000 VND
        var orderAmount = new Money(75000m, "VND");
        var minimumOrder = new Money(50000m, "VND");

        // Act
        var meetsMinimum = orderAmount >= minimumOrder;

        // Assert
        meetsMinimum.Should().BeTrue();
    }

    [Fact]
    public void IsMinimumOrderAmount_UsdAmount_ShouldReturnCorrectResult()
    {
        // Arrange - USD minimum order is usually $2
        var orderAmount = new Money(5.00m, "USD");
        var minimumOrder = new Money(2.00m, "USD");

        // Act
        var meetsMinimum = orderAmount >= minimumOrder;

        // Assert
        meetsMinimum.Should().BeTrue();
    }

    [Theory]
    [InlineData(25000, false)] // Below minimum
    [InlineData(50000, true)]  // At minimum
    [InlineData(100000, true)] // Above minimum
    public void IsMinimumOrderAmount_VariousVndAmounts_ShouldReturnExpectedResult(decimal amount, bool expectedResult)
    {
        // Arrange
        var orderAmount = new Money(amount, "VND");
        var minimumOrder = new Money(50000m, "VND");

        // Act
        var meetsMinimum = orderAmount >= minimumOrder;

        // Assert
        meetsMinimum.Should().Be(expectedResult);
    }

    #endregion
}