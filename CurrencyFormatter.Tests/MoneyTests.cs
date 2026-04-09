using CurrencyFormatter.Extensions;
using CurrencyFormatter.Models;

namespace CurrencyFormatter.Tests;

public class MoneyTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var money = new Money(100.50m, "USD");
        Assert.Equal(100.50m, money.Amount);
        Assert.Equal("USD", money.IsoCode);
    }

    [Fact]
    public void Constructor_NormalizesIsoCode()
    {
        var money = new Money(100m, "usd");
        Assert.Equal("USD", money.IsoCode);
    }

    [Fact]
    public void Constructor_NullIsoCode_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Money(100m, null!));
    }

    [Fact]
    public void Addition_SameCurrency_Works()
    {
        var a = new Money(100m, "USD");
        var b = new Money(200m, "USD");
        var result = a + b;
        Assert.Equal(300m, result.Amount);
        Assert.Equal("USD", result.IsoCode);
    }

    [Fact]
    public void Addition_DifferentCurrency_Throws()
    {
        var a = new Money(100m, "USD");
        var b = new Money(200m, "EUR");
        Assert.Throws<InvalidOperationException>(() => a + b);
    }

    [Fact]
    public void Subtraction_SameCurrency_Works()
    {
        var result = new Money(300m, "KRW") - new Money(100m, "KRW");
        Assert.Equal(200m, result.Amount);
    }

    [Fact]
    public void Multiplication_ByScalar_Works()
    {
        var money = new Money(100m, "USD");
        Assert.Equal(250m, (money * 2.5m).Amount);
        Assert.Equal(250m, (2.5m * money).Amount);
    }

    [Fact]
    public void Division_ByScalar_Works()
    {
        var money = new Money(100m, "USD");
        Assert.Equal(25m, (money / 4m).Amount);
    }

    [Fact]
    public void Negation_Works()
    {
        var money = new Money(100m, "USD");
        Assert.Equal(-100m, (-money).Amount);
    }

    [Fact]
    public void Equality_SameAmountAndCurrency()
    {
        var a = new Money(100m, "USD");
        var b = new Money(100m, "USD");
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Comparison_SameCurrency_Works()
    {
        var small = new Money(50m, "USD");
        var big = new Money(100m, "USD");
        Assert.True(small < big);
        Assert.True(big > small);
        Assert.True(small <= big);
        Assert.True(big >= small);
    }

    [Fact]
    public void Comparison_DifferentCurrency_Throws()
    {
        var a = new Money(100m, "USD");
        var b = new Money(100m, "EUR");
        Assert.Throws<InvalidOperationException>(() => a.CompareTo(b));
    }

    [Fact]
    public void Format_ReturnsFormattedString()
    {
        var money = new Money(1234.56m, "USD");
        Assert.Equal("$1,234.56", money.Format());
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var money = new Money(1234.56m, "USD");
        Assert.Equal("$1,234.56", money.ToString());
    }

    [Fact]
    public void Parse_ValidInput_ReturnsMoney()
    {
        var money = Money.Parse("$1,234.56", "USD");
        Assert.Equal(1234.56m, money.Amount);
        Assert.Equal("USD", money.IsoCode);
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsTrueAndMoney()
    {
        var success = Money.TryParse("$1,234.56", "USD", out var money);
        Assert.True(success);
        Assert.Equal(1234.56m, money.Amount);
    }

    [Fact]
    public void TryParse_InvalidInput_ReturnsFalse()
    {
        var success = Money.TryParse("invalid", "USD", out var money);
        Assert.False(success);
        Assert.Equal(default, money);
    }
}

public class DecimalExtensionsTests
{
    [Fact]
    public void FormatAsCurrency_Works()
    {
        Assert.Equal("$1,234.56", 1234.56m.FormatAsCurrency("USD"));
    }

    [Fact]
    public void FormatAsCurrency_WithOptions_Works()
    {
        var options = new FormatOptions(includeSymbol: false);
        var result = 1234.56m.FormatAsCurrency("USD", options);
        Assert.DoesNotContain("$", result);
    }

    [Fact]
    public void AsMoney_CreatesMoney()
    {
        var money = 100m.AsMoney("USD");
        Assert.Equal(100m, money.Amount);
        Assert.Equal("USD", money.IsoCode);
    }
}
