using FluentAssertions;
using ShopFlow.Domain.Tests.TestFixtures;
using ShopFlow.Domain.ValueObjects;
using Xunit;

namespace ShopFlow.Domain.Tests.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Component", "ValueObject")]
public class MoneyTests : DomainTestBase
{
    #region Creation & Validation Tests (8 tests)

    [Fact]
    public void Constructor_ValidAmountAndCurrency_ShouldCreateMoneyObject()
    {
        // Arrange
        var amount = 100.50m;
        var currency = "USD";

        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Constructor_DefaultCurrency_ShouldUseVND()
    {
        // Arrange
        var amount = 100.50m;

        // Act
        var money = new Money(amount);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be("VND");
    }

    [Fact]
    public void Constructor_NegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var negativeAmount = -10.50m;

        // Act & Assert
        AssertThrows<ArgumentException>(() => new Money(negativeAmount), "Amount cannot be negative");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_EmptyOrNullCurrency_ShouldThrowArgumentException(string invalidCurrency)
    {
        // Act & Assert
        AssertThrows<ArgumentException>(() => new Money(100m, invalidCurrency), "Currency cannot be empty");
    }

    [Fact]
    public void Constructor_CurrencyWithLowerCase_ShouldNormalizeToUpperCase()
    {
        // Arrange
        var amount = 100m;
        var lowerCaseCurrency = "usd";

        // Act
        var money = new Money(amount, lowerCaseCurrency);

        // Assert
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Constructor_ZeroAmount_ShouldCreateValidMoneyObject()
    {
        // Arrange
        var zeroAmount = 0m;

        // Act
        var money = new Money(zeroAmount);

        // Assert
        money.Amount.Should().Be(0m);
        money.Currency.Should().Be("VND");
    }

    [Fact]
    public void Constructor_VeryLargeAmount_ShouldCreateValidMoneyObject()
    {
        // Arrange
        var largeAmount = 999999999.9999m;

        // Act
        var money = new Money(largeAmount);

        // Assert
        money.Amount.Should().Be(largeAmount);
    }

    [Fact]
    public void Constructor_AmountWithManyDecimals_ShouldRoundToFourDecimals()
    {
        // Arrange
        var preciseAmount = 100.123456789m;

        // Act
        var money = new Money(preciseAmount);

        // Assert
        money.Amount.Should().Be(100.1235m); // Rounded to 4 decimal places
    }

    #endregion

    #region Arithmetic Operations Tests (8 tests)

    [Fact]
    public void Addition_SameCurrency_ShouldAddAmounts()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Addition_DifferentCurrency_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        // Act & Assert
        AssertThrows<InvalidOperationException>(() => money1 + money2, "Cannot add money with different currencies");
    }

    [Fact]
    public void Subtraction_SameCurrency_ShouldSubtractAmounts()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(30m, "USD");

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(70m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Subtraction_DifferentCurrency_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(30m, "EUR");

        // Act & Assert
        AssertThrows<InvalidOperationException>(() => money1 - money2, "Cannot subtract money with different currencies");
    }

    [Fact]
    public void Multiplication_WithDecimalMultiplier_ShouldMultiplyAmount()
    {
        // Arrange
        var money = new Money(100m, "USD");
        var multiplier = 2.5m;

        // Act
        var result = money * multiplier;

        // Assert
        result.Amount.Should().Be(250m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Division_WithValidDivisor_ShouldDivideAmount()
    {
        // Arrange
        var money = new Money(100m, "USD");
        var divisor = 4m;

        // Act
        var result = money / divisor;

        // Assert
        result.Amount.Should().Be(25m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Division_WithZeroDivisor_ShouldThrowDivideByZeroException()
    {
        // Arrange
        var money = new Money(100m, "USD");
        var zeroDivisor = 0m;

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => money / zeroDivisor);
    }

    [Fact]
    public void Zero_WithSpecificCurrency_ShouldCreateZeroMoney()
    {
        // Arrange & Act
        var zeroMoney = Money.Zero("EUR");

        // Assert
        zeroMoney.Amount.Should().Be(0m);
        zeroMoney.Currency.Should().Be("EUR");
    }

    #endregion

    #region Equality & Comparison Tests (4 tests)

    [Fact]
    public void Equals_SameAmountAndCurrency_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(100m, "USD");

        // Act & Assert
        money1.Equals(money2).Should().BeTrue();
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentAmountSameCurrency_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(200m, "USD");

        // Act & Assert
        money1.Equals(money2).Should().BeFalse();
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_SameCurrency_ShouldCompareAmounts()
    {
        // Arrange
        var smallerMoney = new Money(50m, "USD");
        var largerMoney = new Money(100m, "USD");

        // Act & Assert
        smallerMoney.CompareTo(largerMoney).Should().BeLessThan(0);
        largerMoney.CompareTo(smallerMoney).Should().BeGreaterThan(0);
        smallerMoney.CompareTo(smallerMoney).Should().Be(0);

        (smallerMoney < largerMoney).Should().BeTrue();
        (largerMoney > smallerMoney).Should().BeTrue();
        (smallerMoney <= largerMoney).Should().BeTrue();
        (largerMoney >= smallerMoney).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_DifferentCurrency_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(100m, "EUR");

        // Act & Assert
        AssertThrows<InvalidOperationException>(() => money1.CompareTo(money2), "Cannot compare money with different currencies");
    }

    #endregion
}
