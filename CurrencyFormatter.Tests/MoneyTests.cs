using System.Text.Json;
using CurrencyFormatter.Extensions;
using CurrencyFormatter.Formatting;
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

public class PercentTests
{
    [Fact]
    public void Constructor_NormalizesRate()
    {
        var pct = new Percent(8.5m);
        Assert.Equal(8.5m, pct.Value);
        Assert.Equal(0.085m, pct.Rate);
    }

    [Fact]
    public void Money_MultiplyByPercent_Works()
    {
        var price = new Money(100m, "USD");
        var tax = price * new Percent(8.5m);
        Assert.Equal(8.5m, tax.Amount);
        Assert.Equal("USD", tax.IsoCode);
    }

    [Fact]
    public void Percent_MultiplyByMoney_Works()
    {
        var price = new Money(200m, "USD");
        var tax = new Percent(10m) * price;
        Assert.Equal(20m, tax.Amount);
    }

    [Fact]
    public void Money_PercentOf_ExtensionMethod_Works()
    {
        var price = new Money(100m, "USD");
        var tax = price.PercentOf(8.5m);
        Assert.Equal(8.5m, tax.Amount);
    }

    [Fact]
    public void Decimal_Percent_ExtensionMethod_Works()
    {
        var price = new Money(100m, "USD");
        var tax = price * 8.5m.Percent();
        Assert.Equal(8.5m, tax.Amount);
    }

    [Fact]
    public void Percent_Addition_Works()
    {
        var a = new Percent(5m);
        var b = new Percent(3m);
        Assert.Equal(new Percent(8m), a + b);
    }

    [Fact]
    public void Percent_Subtraction_Works()
    {
        var result = new Percent(10m) - new Percent(3m);
        Assert.Equal(new Percent(7m), result);
    }

    [Fact]
    public void Percent_Equality_Works()
    {
        Assert.True(new Percent(8.5m) == new Percent(8.5m));
        Assert.False(new Percent(8.5m) != new Percent(8.5m));
    }

    [Fact]
    public void Percent_ToString_ShowsPercent()
    {
        Assert.Equal("8.5%", new Percent(8.5m).ToString());
    }
}

public class RoundTests
{
    [Fact]
    public void Round_USD_RoundsTo2Decimals()
    {
        var money = new Money(10.555m, "USD");
        var rounded = money.Round();
        Assert.Equal(10.56m, rounded.Amount); // Banker's rounding
        Assert.Equal("USD", rounded.IsoCode);
    }

    [Fact]
    public void Round_JPY_RoundsTo0Decimals()
    {
        var money = new Money(100.7m, "JPY");
        var rounded = money.Round();
        Assert.Equal(101m, rounded.Amount);
    }

    [Fact]
    public void Round_CustomDecimals_Works()
    {
        var money = new Money(10.5555m, "USD");
        var rounded = money.Round(3);
        Assert.Equal(10.556m, rounded.Amount);
    }

    [Fact]
    public void Round_AwayFromZero_Works()
    {
        var money = new Money(10.555m, "USD");
        var rounded = money.Round(MidpointRounding.AwayFromZero);
        Assert.Equal(10.56m, rounded.Amount);
    }
}

public class FormattableTests
{
    [Fact]
    public void ToString_G_ReturnsCurrencyFormat()
    {
        var money = new Money(1234.56m, "USD");
        Assert.Equal("$1,234.56", money.ToString("G"));
    }

    [Fact]
    public void ToString_C_ReturnsCurrencyFormat()
    {
        var money = new Money(1234.56m, "USD");
        Assert.Equal("$1,234.56", money.ToString("C"));
    }

    [Fact]
    public void ToString_N_ReturnsNumberOnly()
    {
        var money = new Money(1234.56m, "USD");
        var result = money.ToString("N");
        Assert.DoesNotContain("$", result);
    }

    [Fact]
    public void ToString_K_ReturnsCompactFormat()
    {
        var money = new Money(1500000m, "USD");
        Assert.Equal("$1.5M", money.ToString("K"));
    }

    [Fact]
    public void ToString_Null_ReturnsDefault()
    {
        var money = new Money(1234.56m, "USD");
        Assert.Equal("$1,234.56", money.ToString(null, null));
    }
}

public class CompactFormatTests
{
    [Fact]
    public void FormatCompact_Thousands()
    {
        Assert.Equal("$1.2K", CurrencyFormatter.FormatCompact(1234m, "USD"));
    }

    [Fact]
    public void FormatCompact_Millions()
    {
        Assert.Equal("$1.5M", CurrencyFormatter.FormatCompact(1500000m, "USD"));
    }

    [Fact]
    public void FormatCompact_Billions()
    {
        Assert.Equal("$2.0B", CurrencyFormatter.FormatCompact(2000000000m, "USD"));
    }

    [Fact]
    public void FormatCompact_Trillions()
    {
        Assert.Equal("$1.0T", CurrencyFormatter.FormatCompact(1000000000000m, "USD"));
    }

    [Fact]
    public void FormatCompact_BelowThousand_ReturnsNormalFormat()
    {
        Assert.Equal("$999.00", CurrencyFormatter.FormatCompact(999m, "USD"));
    }

    [Fact]
    public void FormatCompact_Negative()
    {
        Assert.Equal("$-1.5M", CurrencyFormatter.FormatCompact(-1500000m, "USD"));
    }

    [Fact]
    public void FormatCompact_CustomDecimals()
    {
        Assert.Equal("$1.23M", CurrencyFormatter.FormatCompact(1234567m, "USD", 2));
    }

    [Fact]
    public void FormatCompact_KRW()
    {
        Assert.Equal("₩1.5M", CurrencyFormatter.FormatCompact(1500000m, "KRW"));
    }
}

public class JsonSerializationTests
{
    [Fact]
    public void Money_Serialize_Works()
    {
        var money = new Money(1234.56m, "USD");
        var json = JsonSerializer.Serialize(money);
        Assert.Contains("\"amount\":1234.56", json);
        Assert.Contains("\"currency\":\"USD\"", json);
    }

    [Fact]
    public void Money_Deserialize_Works()
    {
        var json = "{\"amount\":1234.56,\"currency\":\"USD\"}";
        var money = JsonSerializer.Deserialize<Money>(json);
        Assert.Equal(1234.56m, money.Amount);
        Assert.Equal("USD", money.IsoCode);
    }

    [Fact]
    public void Money_RoundTrip_Works()
    {
        var original = new Money(99.99m, "EUR");
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<Money>(json);
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void Percent_Serialize_Works()
    {
        var pct = new Percent(8.5m);
        var json = JsonSerializer.Serialize(pct);
        Assert.Contains("\"value\":8.5", json);
    }

    [Fact]
    public void Percent_Deserialize_Works()
    {
        var json = "{\"value\":8.5}";
        var pct = JsonSerializer.Deserialize<Percent>(json);
        Assert.Equal(8.5m, pct.Value);
        Assert.Equal(0.085m, pct.Rate);
    }

    [Fact]
    public void Percent_DeserializeFromNumber_Works()
    {
        var json = "8.5";
        var pct = JsonSerializer.Deserialize<Percent>(json);
        Assert.Equal(8.5m, pct.Value);
    }

    [Fact]
    public void Money_Deserialize_MissingCurrency_Throws()
    {
        var json = "{\"amount\":100}";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Money>(json));
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
