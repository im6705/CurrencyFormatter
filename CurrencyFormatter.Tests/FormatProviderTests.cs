using CurrencyFormatter.Formatting;

namespace CurrencyFormatter.Tests;

public class CurrencyFormatProviderTests
{
    [Fact]
    public void StringFormat_Decimal_FormatsAsCurrency()
    {
        var provider = new CurrencyFormatProvider("USD");
        var result = string.Format(provider, "{0}", 1234.56m);
        Assert.Equal("$1,234.56", result);
    }

    [Fact]
    public void StringFormat_Int_FormatsAsCurrency()
    {
        var provider = new CurrencyFormatProvider("KRW");
        var result = string.Format(provider, "{0}", 1234);
        Assert.Equal("₩1,234", result);
    }

    [Fact]
    public void StringFormat_Double_FormatsAsCurrency()
    {
        var provider = new CurrencyFormatProvider("USD");
        var result = string.Format(provider, "{0}", 1234.56);
        Assert.Equal("$1,234.56", result);
    }

    [Fact]
    public void StringFormat_Long_FormatsAsCurrency()
    {
        var provider = new CurrencyFormatProvider("USD");
        var result = string.Format(provider, "{0}", 1234L);
        Assert.Equal("$1,234.00", result);
    }

    [Fact]
    public void StringFormat_String_PassesThrough()
    {
        var provider = new CurrencyFormatProvider("USD");
        var result = string.Format(provider, "{0}", "hello");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void GetFormat_NonCustomFormatter_ReturnsNull()
    {
        var provider = new CurrencyFormatProvider("USD");
        Assert.Null(provider.GetFormat(typeof(string)));
    }

    [Fact]
    public void Constructor_NullIsoCode_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CurrencyFormatProvider(null!));
    }
}

public class SpanTryParseTests
{
#if NET8_0_OR_GREATER
    [Fact]
    public void TryParse_Span_ValidInput_ReturnsTrueAndValue()
    {
        ReadOnlySpan<char> input = "$1,234.56".AsSpan();
        var success = Currency.TryParse(input, "USD", out var result);
        Assert.True(success);
        Assert.Equal(1234.56m, result);
    }

    [Fact]
    public void TryParse_Span_InvalidInput_ReturnsFalse()
    {
        ReadOnlySpan<char> input = "not_a_number".AsSpan();
        var success = Currency.TryParse(input, "USD", out var result);
        Assert.False(success);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void TryParse_Span_UnsupportedCode_ReturnsFalse()
    {
        ReadOnlySpan<char> input = "$100".AsSpan();
        var success = Currency.TryParse(input, "XYZ", out var result);
        Assert.False(success);
    }
#endif
}
