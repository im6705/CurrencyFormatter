using System;
using System.Globalization;
using CurrencyFormatter.Registry;

namespace CurrencyFormatter.Formatting;

/// <summary>
/// <see cref="string.Format(IFormatProvider, string, object[])"/>에서 사용할 수 있는 통화 포맷 제공자.
/// <code>
/// var provider = new CurrencyFormatProvider("USD");
/// string.Format(provider, "{0}", 1234.56m);  // "$1,234.56"
/// </code>
/// </summary>
public sealed class CurrencyFormatProvider : IFormatProvider, ICustomFormatter
{
    private readonly string _isoCode;
    private readonly CurrencyRegistry _registry;

    /// <summary>지정된 ISO 4217 코드로 포맷 제공자를 생성합니다.</summary>
    /// <param name="isoCode">ISO 4217 통화 코드</param>
    public CurrencyFormatProvider(string isoCode)
        : this(isoCode, CurrencyRegistry.Default) { }

    /// <summary>지정된 레지스트리와 ISO 코드로 포맷 제공자를 생성합니다.</summary>
    public CurrencyFormatProvider(string isoCode, CurrencyRegistry registry)
    {
        _isoCode = isoCode ?? throw new ArgumentNullException(nameof(isoCode));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    /// <inheritdoc />
    public object? GetFormat(Type? formatType)
    {
        return formatType == typeof(ICustomFormatter) ? this : null;
    }

    /// <inheritdoc />
    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        if (arg is decimal d)
            return Currency.Format(d, _isoCode);
        if (arg is double dbl)
            return Currency.Format((decimal)dbl, _isoCode);
        if (arg is int i)
            return Currency.Format(i, _isoCode);
        if (arg is long l)
            return Currency.Format(l, _isoCode);

        // 숫자가 아닌 경우 기본 포맷
        if (arg is IFormattable formattable)
            return formattable.ToString(format, CultureInfo.CurrentCulture);

        return arg?.ToString() ?? string.Empty;
    }
}
