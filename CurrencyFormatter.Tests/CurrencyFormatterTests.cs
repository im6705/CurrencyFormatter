using CurrencyFormatter.Exceptions;
using CurrencyFormatter.Models;
using CurrencyFormatter.Registry;

namespace CurrencyFormatter.Tests;

public class FormatTests
{
    [Theory]
    [InlineData("USD", 1234.56, "$1,234.56")]
    [InlineData("EUR", 1234.56, "1.234,56 €")]
    [InlineData("KRW", 1234, "₩1,234")]
    [InlineData("JPY", 1234, "￥1,234")]
    public void Format_KnownCurrencies_ReturnsExpected(string iso, decimal amount, string expected)
    {
        var result = Currency.Format(amount, iso);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Format_Zero_ReturnsFormattedZero()
    {
        var result = Currency.Format(0m, "USD");
        Assert.Equal("$0.00", result);
    }

    [Fact]
    public void Format_Negative_IncludesSign()
    {
        var result = Currency.Format(-100m, "USD");
        Assert.Contains("-", result);
        Assert.Contains("100", result);
    }

    [Fact]
    public void Format_CaseInsensitive()
    {
        var upper = Currency.Format(100m, "USD");
        var lower = Currency.Format(100m, "usd");
        Assert.Equal(upper, lower);
    }

    [Fact]
    public void Format_UnsupportedCode_ThrowsCurrencyNotFoundException()
    {
        var ex = Assert.Throws<CurrencyNotFoundException>(() => Currency.Format(100m, "XYZ"));
        Assert.Equal("XYZ", ex.IsoCode);
    }

    [Fact]
    public void Format_NullCode_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Currency.Format(100m, null!));
    }
}

public class ParseTests
{
    [Theory]
    [InlineData("USD", "$1,234.56", 1234.56)]
    [InlineData("KRW", "₩1,234", 1234)]
    public void Parse_ValidInput_ReturnsDecimal(string iso, string input, decimal expected)
    {
        var result = Currency.Parse(input, iso);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_InvalidInput_ThrowsCurrencyParseException()
    {
        var ex = Assert.Throws<CurrencyParseException>(() => Currency.Parse("not_a_number", "USD"));
        Assert.Equal("USD", ex.IsoCode);
        Assert.Equal("not_a_number", ex.Input);
    }

    [Fact]
    public void Parse_UnsupportedCode_ThrowsCurrencyNotFoundException()
    {
        Assert.Throws<CurrencyNotFoundException>(() => Currency.Parse("$100", "XYZ"));
    }

    [Fact]
    public void Parse_NullCode_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Currency.Parse("$100", null!));
    }
}

public class TryParseTests
{
    [Fact]
    public void TryParse_ValidInput_ReturnsTrueAndValue()
    {
        var success = Currency.TryParse("$1,234.56", "USD", out var result);
        Assert.True(success);
        Assert.Equal(1234.56m, result);
    }

    [Fact]
    public void TryParse_InvalidInput_ReturnsFalseAndZero()
    {
        var success = Currency.TryParse("not_a_number", "USD", out var result);
        Assert.False(success);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void TryParse_UnsupportedCode_ReturnsFalseAndZero()
    {
        var success = Currency.TryParse("$100", "XYZ", out var result);
        Assert.False(success);
        Assert.Equal(0m, result);
    }
}

public class RoundtripTests
{
    [Theory]
    [InlineData("USD", 1234.56)]
    [InlineData("EUR", 9999.99)]
    [InlineData("KRW", 50000)]
    [InlineData("JPY", 12345)]
    [InlineData("GBP", 100.50)]
    public void FormatThenParse_Roundtrip_PreservesValue(string iso, decimal original)
    {
        var formatted = Currency.Format(original, iso);
        var parsed = Currency.Parse(formatted, iso);
        Assert.Equal(original, parsed);
    }
}

public class SupportTests
{
    [Theory]
    [InlineData("USD", true)]
    [InlineData("EUR", true)]
    [InlineData("KRW", true)]
    [InlineData("XYZ", false)]
    public void IsSupported_ReturnsExpected(string iso, bool expected)
    {
        Assert.Equal(expected, Currency.IsSupported(iso));
    }

    [Fact]
    public void SupportedCodes_ContainsCommonCurrencies()
    {
        var codes = Currency.SupportedCodes.ToList();
        Assert.Contains("USD", codes);
        Assert.Contains("EUR", codes);
        Assert.Contains("KRW", codes);
        Assert.Contains("JPY", codes);
        Assert.Contains("GBP", codes);
    }

    [Fact]
    public void SupportedCodes_IsNotEmpty()
    {
        Assert.NotEmpty(Currency.SupportedCodes);
    }
}

public class CurrencyInfoTests
{
    [Fact]
    public void GetInfo_USD_ReturnsCorrectMetadata()
    {
        var info = Currency.GetInfo("USD");
        Assert.Equal("USD", info.IsoCode);
        Assert.Equal("$", info.Symbol);
        Assert.Equal(2, info.DecimalDigits);
        Assert.NotEmpty(info.EnglishName);
    }

    [Fact]
    public void GetInfo_KRW_HasZeroDecimalDigits()
    {
        var info = Currency.GetInfo("KRW");
        Assert.Equal("KRW", info.IsoCode);
        Assert.Equal("₩", info.Symbol);
        Assert.Equal(0, info.DecimalDigits);
    }

    [Fact]
    public void TryGetInfo_ValidCode_ReturnsTrueAndInfo()
    {
        var success = Currency.TryGetInfo("USD", out var info);
        Assert.True(success);
        Assert.Equal("USD", info.IsoCode);
    }

    [Fact]
    public void TryGetInfo_InvalidCode_ReturnsFalse()
    {
        var success = Currency.TryGetInfo("XYZ", out _);
        Assert.False(success);
    }

    [Fact]
    public void SupportedCurrencies_ReturnsInfoObjects()
    {
        var currencies = Currency.SupportedCurrencies.ToList();
        Assert.NotEmpty(currencies);
        Assert.All(currencies, c =>
        {
            Assert.NotNull(c.IsoCode);
            Assert.NotNull(c.Symbol);
            Assert.True(c.DecimalDigits >= 0);
        });
    }
}

public class CurrencyRegistryTests
{
    [Fact]
    public void Default_ContainsCommonCurrencies()
    {
        var registry = CurrencyRegistry.Default;
        Assert.True(registry.IsSupported("USD"));
        Assert.True(registry.IsSupported("EUR"));
        Assert.True(registry.IsSupported("KRW"));
    }

    [Fact]
    public void WithCurrency_AddsCustomCurrency()
    {
        var custom = new CurrencyInfo(
            "XTS", "T$", "Test Currency", "테스트 통화", 2,
            new System.Globalization.CultureInfo("en-US").NumberFormat,
            new System.Globalization.CultureInfo("en-US"));

        var registry = CurrencyRegistry.Default.WithCurrency(custom);
        Assert.True(registry.IsSupported("XTS"));
        Assert.Equal("XTS", registry.GetCurrency("XTS").IsoCode);
    }

    [Fact]
    public void GetCurrency_UnsupportedCode_ThrowsCurrencyNotFoundException()
    {
        Assert.Throws<CurrencyNotFoundException>(() => CurrencyRegistry.Default.GetCurrency("XYZ"));
    }
}

public class FormatOptionsTests
{
    [Fact]
    public void Format_WithCustomDecimalDigits()
    {
        var options = new FormatOptions(decimalDigits: 4);
        var result = Currency.Format(1234.5678m, "USD", options);
        Assert.Contains("1,234.5678", result);
    }

    [Fact]
    public void Format_WithZeroDecimalDigits()
    {
        var options = new FormatOptions(decimalDigits: 0);
        var result = Currency.Format(1234.56m, "USD", options);
        Assert.Contains("1,235", result); // Banker's rounding
    }

    [Fact]
    public void Format_WithoutGroupSeparator()
    {
        var options = new FormatOptions(useGroupSeparator: false);
        var result = Currency.Format(1234.56m, "USD", options);
        Assert.DoesNotContain(",", result);
        Assert.Contains("1234", result);
    }

    [Fact]
    public void Format_WithoutSymbol()
    {
        var options = new FormatOptions(includeSymbol: false);
        var result = Currency.Format(1234.56m, "USD", options);
        Assert.DoesNotContain("$", result);
        Assert.Contains("1,234.56", result);
    }

    [Fact]
    public void Format_AwayFromZeroRounding()
    {
        var options = new FormatOptions(decimalDigits: 0, rounding: MidpointRounding.AwayFromZero);
        var result = Currency.Format(1234.5m, "USD", options);
        Assert.Contains("1,235", result);
    }

    [Fact]
    public void Format_NegativeDecimalDigits_ThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new FormatOptions(decimalDigits: -1));
    }
}
