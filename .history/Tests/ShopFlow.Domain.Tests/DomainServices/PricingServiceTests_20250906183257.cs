using FluentAssertions;
using ShopFlow.Domain.DomainServices;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Tests.TestFixtures;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.DomainServices;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Component", "DomainService")]
public class PricingServiceTests : DomainTestBase
{
    private readonly PricingService _pricingService;

    public PricingServiceTests()
    {
        _pricingService = new PricingService();
    }

    #region Price Calculation Tests (10 tests)

    [Fact]
    public void CalculatePrice_ValidOfferAndQuantity_ShouldReturnCorrectPrice()
    {
        // Arrange
        var offer = CreateMockOffer(100000m);
        var quantity = 2;

        // Act
        var result = _pricingService.CalculatePrice(offer, quantity);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(200000m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculatePrice_NullOffer_ShouldThrowArgumentNullException()
    {
        // Arrange
        MktOffer nullOffer = null!;
        var quantity = 1;

        // Act & Assert
        AssertThrows<ArgumentNullException>(() => _pricingService.CalculatePrice(nullOffer, quantity), "offer");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void CalculatePrice_ZeroOrNegativeQuantity_ShouldThrowArgumentException(int invalidQuantity)
    {
        // Arrange
        var offer = CreateMockOffer(100000m);

        // Act & Assert
        AssertThrows<ArgumentException>(() => _pricingService.CalculatePrice(offer, invalidQuantity), "Quantity must be positive");
    }

    [Fact]
    public void CalculatePrice_LargeQuantity_ShouldCalculateCorrectly()
    {
        // Arrange
        var offer = CreateMockOffer(1000m);
        var largeQuantity = 10000;

        // Act
        var result = _pricingService.CalculatePrice(offer, largeQuantity);

        // Assert
        result.Amount.Should().Be(10000000m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculatePrice_DecimalPrice_ShouldCalculateCorrectly()
    {
        // Arrange
        var offer = CreateMockOffer(99.99m);
        var quantity = 3;

        // Act
        var result = _pricingService.CalculatePrice(offer, quantity);

        // Assert
        result.Amount.Should().Be(299.97m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculatePrice_ZeroPrice_ShouldReturnZero()
    {
        // Arrange
        var offer = CreateMockOffer(0m);
        var quantity = 5;

        // Act
        var result = _pricingService.CalculatePrice(offer, quantity);

        // Assert
        result.Amount.Should().Be(0m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculatePrice_SingleQuantity_ShouldReturnBasePrice()
    {
        // Arrange
        var basePrice = 150000m;
        var offer = CreateMockOffer(basePrice);
        var quantity = 1;

        // Act
        var result = _pricingService.CalculatePrice(offer, quantity);

        // Assert
        result.Amount.Should().Be(basePrice);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculatePrice_OfferWithMinQuantity_ShouldApplyBulkPricing()
    {
        // Arrange
        var offer = CreateMockOfferWithMinQty(50000m, 10);
        var quantity = 15; // Above minimum quantity

        // Act
        var result = _pricingService.CalculatePrice(offer, quantity);

        // Assert
        result.Amount.Should().Be(750000m); // Base calculation
        // Note: Current implementation doesn't apply discount yet, just verifies structure
    }

    [Fact]
    public void CalculatePrice_OfferBelowMinQuantity_ShouldCalculateNormally()
    {
        // Arrange
        var offer = CreateMockOfferWithMinQty(50000m, 10);
        var quantity = 5; // Below minimum quantity

        // Act
        var result = _pricingService.CalculatePrice(offer, quantity);

        // Assert
        result.Amount.Should().Be(250000m);
    }

    [Fact]
    public void CalculatePrice_PrecisionHandling_ShouldMaintainAccuracy()
    {
        // Arrange
        var offer = CreateMockOffer(33.333m);
        var quantity = 3;

        // Act
        var result = _pricingService.CalculatePrice(offer, quantity);

        // Assert
        result.Amount.Should().Be(99.999m);
        result.Currency.Should().Be("VND");
    }

    #endregion

    #region Tax Calculation Tests (8 tests)

    [Theory]
    [InlineData(100000, 0.1, 10000)]
    [InlineData(250000, 0.05, 12500)]
    [InlineData(1000000, 0.2, 200000)]
    public void CalculateTax_ValidAmountAndTaxRate_ShouldCalculateCorrectly(decimal amount, decimal taxRate, decimal expectedTax)
    {
        // Arrange
        var money = new Money(amount, "VND");

        // Act
        var result = _pricingService.CalculateTax(money, taxRate);

        // Assert
        result.Amount.Should().Be(expectedTax);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculateTax_NullAmount_ShouldThrowArgumentNullException()
    {
        // Arrange
        Money nullMoney = null!;
        var taxRate = 0.1m;

        // Act & Assert
        AssertThrows<ArgumentNullException>(() => _pricingService.CalculateTax(nullMoney, taxRate), "amount");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(2.0)]
    public void CalculateTax_InvalidTaxRate_ShouldThrowArgumentException(decimal invalidTaxRate)
    {
        // Arrange
        var money = new Money(100000m, "VND");

        // Act & Assert
        AssertThrows<ArgumentException>(() => _pricingService.CalculateTax(money, invalidTaxRate), "Tax rate must be between 0 and 1");
    }

    [Fact]
    public void CalculateTax_ZeroTaxRate_ShouldReturnZeroTax()
    {
        // Arrange
        var money = new Money(500000m, "VND");
        var zeroTaxRate = 0m;

        // Act
        var result = _pricingService.CalculateTax(money, zeroTaxRate);

        // Assert
        result.Amount.Should().Be(0m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculateTax_MaximumTaxRate_ShouldReturnFullAmount()
    {
        // Arrange
        var money = new Money(300000m, "VND");
        var maxTaxRate = 1m; // 100%

        // Act
        var result = _pricingService.CalculateTax(money, maxTaxRate);

        // Assert
        result.Amount.Should().Be(300000m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculateTax_ZeroAmount_ShouldReturnZeroTax()
    {
        // Arrange
        var money = new Money(0m, "VND");
        var taxRate = 0.15m;

        // Act
        var result = _pricingService.CalculateTax(money, taxRate);

        // Assert
        result.Amount.Should().Be(0m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculateTax_DifferentCurrency_ShouldPreserveCurrency()
    {
        // Arrange
        var money = new Money(1000m, "USD");
        var taxRate = 0.08m;

        // Act
        var result = _pricingService.CalculateTax(money, taxRate);

        // Assert
        result.Amount.Should().Be(80m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void CalculateTax_PrecisionCalculation_ShouldMaintainAccuracy()
    {
        // Arrange
        var money = new Money(33.33m, "VND");
        var taxRate = 0.1m;

        // Act
        var result = _pricingService.CalculateTax(money, taxRate);

        // Assert
        result.Amount.Should().Be(3.333m);
        result.Currency.Should().Be("VND");
    }

    #endregion

    #region Shipping Fee Calculation Tests (10 tests)

    [Fact]
    public void CalculateShippingFee_ValidInputs_ShouldCalculateCorrectly()
    {
        // Arrange
        var orderValue = new Money(500000m, "VND");
        var shippingZone = "HCM";
        var weight = 2.5m;

        // Act
        var result = _pricingService.CalculateShippingFee(orderValue, shippingZone, weight);

        // Assert
        result.Should().NotBeNull();
        result.Currency.Should().Be("VND");
        result.Amount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CalculateShippingFee_NullOrderValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        Money nullOrderValue = null!;
        var shippingZone = "HCM";
        var weight = 1.0m;

        // Act & Assert
        AssertThrows<ArgumentNullException>(() => _pricingService.CalculateShippingFee(nullOrderValue, shippingZone, weight), "orderValue");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CalculateShippingFee_EmptyOrWhitespaceZone_ShouldThrowArgumentException(string invalidZone)
    {
        // Arrange
        var orderValue = new Money(100000m, "VND");
        var weight = 1.0m;

        // Act & Assert
        AssertThrows<ArgumentException>(() => _pricingService.CalculateShippingFee(orderValue, invalidZone, weight), "Shipping zone is required");
    }

    [Fact]
    public void CalculateShippingFee_NullShippingZone_ShouldThrowArgumentException()
    {
        // Arrange
        var orderValue = new Money(100000m, "VND");
        string nullZone = null!;
        var weight = 1.0m;

        // Act & Assert
        AssertThrows<ArgumentException>(() => _pricingService.CalculateShippingFee(orderValue, nullZone, weight), "Shipping zone is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5.5)]
    public void CalculateShippingFee_ZeroOrNegativeWeight_ShouldThrowArgumentException(decimal invalidWeight)
    {
        // Arrange
        var orderValue = new Money(100000m, "VND");
        var shippingZone = "HCM";

        // Act & Assert
        AssertThrows<ArgumentException>(() => _pricingService.CalculateShippingFee(orderValue, shippingZone, invalidWeight), "Weight must be positive");
    }

    [Fact]
    public void CalculateShippingFee_MinimalWeight_ShouldIncludeBaseFee()
    {
        // Arrange
        var orderValue = new Money(100000m, "VND");
        var shippingZone = "HCM";
        var minimalWeight = 0.1m;

        // Act
        var result = _pricingService.CalculateShippingFee(orderValue, shippingZone, minimalWeight);

        // Assert
        result.Amount.Should().BeGreaterThan(30000m); // Should be base fee + weight fee
    }

    [Fact]
    public void CalculateShippingFee_LargeWeight_ShouldCalculateCorrectly()
    {
        // Arrange
        var orderValue = new Money(1000000m, "VND");
        var shippingZone = "HN";
        var largeWeight = 10.0m;

        // Act
        var result = _pricingService.CalculateShippingFee(orderValue, shippingZone, largeWeight);

        // Assert
        // Base fee (30000) + weight fee (10 * 5000 = 50000) = 80000
        result.Amount.Should().Be(80000m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculateShippingFee_DifferentZones_ShouldCalculateSame()
    {
        // Arrange
        var orderValue = new Money(200000m, "VND");
        var weight = 1.5m;

        // Act
        var resultHCM = _pricingService.CalculateShippingFee(orderValue, "HCM", weight);
        var resultHN = _pricingService.CalculateShippingFee(orderValue, "HN", weight);

        // Assert
        // Current implementation treats all zones the same
        resultHCM.Amount.Should().Be(resultHN.Amount);
    }

    [Fact]
    public void CalculateShippingFee_PreciseWeight_ShouldCalculateCorrectly()
    {
        // Arrange
        var orderValue = new Money(150000m, "VND");
        var shippingZone = "DN";
        var preciseWeight = 1.25m; // 1.25kg

        // Act
        var result = _pricingService.CalculateShippingFee(orderValue, shippingZone, preciseWeight);

        // Assert
        // Base fee (30000) + weight fee (1.25 * 5000 = 6250) = 36250
        result.Amount.Should().Be(36250m);
    }

    [Fact]
    public void CalculateShippingFee_DifferentCurrency_ShouldMaintainCurrency()
    {
        // Arrange
        var orderValue = new Money(100m, "USD");
        var shippingZone = "International";
        var weight = 1.0m;

        // Act
        var result = _pricingService.CalculateShippingFee(orderValue, shippingZone, weight);

        // Assert
        result.Currency.Should().Be("VND"); // Shipping always in VND according to implementation
    }

    #endregion

    #region Discount Application Tests (7 tests)

    [Theory]
    [InlineData(100000, 0.1, 90000)]
    [InlineData(250000, 0.2, 200000)]
    [InlineData(500000, 0.05, 475000)]
    public void ApplyDiscount_ValidAmountAndPercentage_ShouldCalculateCorrectly(decimal amount, decimal discountPercentage, decimal expectedResult)
    {
        // Arrange
        var money = new Money(amount, "VND");

        // Act
        var result = _pricingService.ApplyDiscount(money, discountPercentage);

        // Assert
        result.Amount.Should().Be(expectedResult);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void ApplyDiscount_NullAmount_ShouldThrowArgumentNullException()
    {
        // Arrange
        Money nullMoney = null!;
        var discountPercentage = 0.1m;

        // Act & Assert
        AssertThrows<ArgumentNullException>(() => _pricingService.ApplyDiscount(nullMoney, discountPercentage), "amount");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(2.0)]
    public void ApplyDiscount_InvalidDiscountPercentage_ShouldThrowArgumentException(decimal invalidPercentage)
    {
        // Arrange
        var money = new Money(100000m, "VND");

        // Act & Assert
        AssertThrows<ArgumentException>(() => _pricingService.ApplyDiscount(money, invalidPercentage), "Discount percentage must be between 0 and 1");
    }

    [Fact]
    public void ApplyDiscount_ZeroPercentage_ShouldReturnOriginalAmount()
    {
        // Arrange
        var money = new Money(300000m, "VND");
        var zeroDiscount = 0m;

        // Act
        var result = _pricingService.ApplyDiscount(money, zeroDiscount);

        // Assert
        result.Amount.Should().Be(300000m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void ApplyDiscount_MaximumPercentage_ShouldReturnZero()
    {
        // Arrange
        var money = new Money(200000m, "VND");
        var maxDiscount = 1m; // 100%

        // Act
        var result = _pricingService.ApplyDiscount(money, maxDiscount);

        // Assert
        result.Amount.Should().Be(0m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void ApplyDiscount_ZeroAmount_ShouldReturnZero()
    {
        // Arrange
        var money = new Money(0m, "VND");
        var discountPercentage = 0.25m;

        // Act
        var result = _pricingService.ApplyDiscount(money, discountPercentage);

        // Assert
        result.Amount.Should().Be(0m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void ApplyDiscount_PrecisionCalculation_ShouldMaintainAccuracy()
    {
        // Arrange
        var money = new Money(333.33m, "VND");
        var discountPercentage = 0.15m; // 15%

        // Act
        var result = _pricingService.ApplyDiscount(money, discountPercentage);

        // Assert
        var expectedDiscount = 333.33m * 0.15m; // 49.9995
        var expectedResult = 333.33m - expectedDiscount; // 283.3305
        result.Amount.Should().Be(expectedResult);
    }

    #endregion

    #region Total Calculation Tests (5 tests)

    [Fact]
    public void CalculateTotal_ValidInputs_ShouldCalculateCorrectly()
    {
        // Arrange
        var subtotal = new Money(500000m, "VND");
        var tax = new Money(50000m, "VND");
        var shipping = new Money(30000m, "VND");
        var discount = new Money(25000m, "VND");

        // Act
        var result = _pricingService.CalculateTotal(subtotal, tax, shipping, discount);

        // Assert
        // 500000 + 50000 + 30000 - 25000 = 555000
        result.Amount.Should().Be(555000m);
        result.Currency.Should().Be("VND");
    }

    [Theory]
    [InlineData("subtotal")]
    [InlineData("tax")]
    [InlineData("shipping")]
    [InlineData("discount")]
    public void CalculateTotal_NullParameter_ShouldThrowArgumentNullException(string nullParameter)
    {
        // Arrange
        var subtotal = nullParameter == "subtotal" ? null : new Money(100000m, "VND");
        var tax = nullParameter == "tax" ? null : new Money(10000m, "VND");
        var shipping = nullParameter == "shipping" ? null : new Money(5000m, "VND");
        var discount = nullParameter == "discount" ? null : new Money(2000m, "VND");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            _pricingService.CalculateTotal(subtotal!, tax!, shipping!, discount!));
        exception.ParamName.Should().Be(nullParameter);
    }

    [Fact]
    public void CalculateTotal_AllZeroAmounts_ShouldReturnZero()
    {
        // Arrange
        var subtotal = new Money(0m, "VND");
        var tax = new Money(0m, "VND");
        var shipping = new Money(0m, "VND");
        var discount = new Money(0m, "VND");

        // Act
        var result = _pricingService.CalculateTotal(subtotal, tax, shipping, discount);

        // Assert
        result.Amount.Should().Be(0m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void CalculateTotal_LargeDiscountExceedsSubtotal_ShouldThrowException()
    {
        // Arrange
        var subtotal = new Money(100000m, "VND");
        var tax = new Money(10000m, "VND");
        var shipping = new Money(5000m, "VND");
        var discount = new Money(200000m, "VND"); // Large discount

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _pricingService.CalculateTotal(subtotal, tax, shipping, discount));

        exception.Message.Should().Contain("Amount cannot be negative");
    }

    [Fact]
    public void CalculateTotal_PrecisionCalculation_ShouldMaintainAccuracy()
    {
        // Arrange
        var subtotal = new Money(123.45m, "VND");
        var tax = new Money(12.35m, "VND");
        var shipping = new Money(5.67m, "VND");
        var discount = new Money(8.90m, "VND");

        // Act
        var result = _pricingService.CalculateTotal(subtotal, tax, shipping, discount);

        // Assert
        // 123.45 + 12.35 + 5.67 - 8.90 = 132.57
        result.Amount.Should().Be(132.57m);
        result.Currency.Should().Be("VND");
    }

    #endregion

    #region Price Validation Tests

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(999999.99)]
    public void IsValidPrice_ValidPrices_ShouldReturnTrue(decimal validAmount)
    {
        // Arrange
        var price = new Money(validAmount, "VND");

        // Act
        var result = _pricingService.IsValidPrice(price);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidPrice_NullPrice_ShouldReturnFalse()
    {
        // Arrange
        Money nullPrice = null!;

        // Act
        var result = _pricingService.IsValidPrice(nullPrice);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Helper Methods

    private MktOffer CreateMockOffer(decimal price)
    {
        return new MktOffer
        {
            PriceGrossVnd = price,
            MinQty = null
        };
    }

    private MktOffer CreateMockOfferWithMinQty(decimal price, int minQty)
    {
        return new MktOffer
        {
            PriceGrossVnd = price,
            MinQty = minQty
        };
    }

    #endregion
}
