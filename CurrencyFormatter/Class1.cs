using System;
using System.Collections.Generic;
using System.Globalization;
using CurrencyFormatter.Exceptions;
using CurrencyFormatter.Models;
using CurrencyFormatter.Registry;

namespace CurrencyFormatter;

/// <summary>
/// ISO 4217 통화 포맷팅 및 파싱을 제공하는 정적 파사드 클래스.
/// </summary>
public static class Currency
{
    private static readonly CurrencyRegistry Registry = CurrencyRegistry.Default;

    /// <summary>
    /// 금액을 통화 형식 문자열로 포맷합니다.
    /// </summary>
    /// <param name="amount">포맷할 금액</param>
    /// <param name="isoCode">ISO 4217 통화 코드</param>
    /// <returns>통화 형식 문자열</returns>
    /// <exception cref="CurrencyNotFoundException">지원하지 않는 통화 코드</exception>
    public static string Format(decimal amount, string isoCode)
    {
        var info = Registry.GetCurrency(isoCode);
        return amount.ToString("C", info.Culture);
    }

    /// <summary>
    /// 금액을 통화 형식 문자열로 포맷합니다 (옵션 지정).
    /// </summary>
    /// <param name="amount">포맷할 금액</param>
    /// <param name="isoCode">ISO 4217 통화 코드</param>
    /// <param name="options">포맷 옵션</param>
    /// <returns>통화 형식 문자열</returns>
    /// <exception cref="CurrencyNotFoundException">지원하지 않는 통화 코드</exception>
    public static string Format(decimal amount, string isoCode, FormatOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        var info = Registry.GetCurrency(isoCode);
        var rounded = Math.Round(amount, options.DecimalDigits ?? info.DecimalDigits, options.Rounding);

        var nfi = (NumberFormatInfo)info.NumberFormat.Clone();
        if (options.DecimalDigits.HasValue)
            nfi.CurrencyDecimalDigits = options.DecimalDigits.Value;
        if (!options.UseGroupSeparator)
            nfi.CurrencyGroupSeparator = string.Empty;

        if (!options.IncludeSymbol)
            return rounded.ToString("N" + nfi.CurrencyDecimalDigits, nfi);

        return rounded.ToString("C", nfi);
    }

    /// <summary>
    /// 통화 형식 문자열을 decimal로 파싱합니다.
    /// </summary>
    /// <param name="input">파싱할 문자열</param>
    /// <param name="isoCode">ISO 4217 통화 코드</param>
    /// <returns>파싱된 금액</returns>
    /// <exception cref="CurrencyNotFoundException">지원하지 않는 통화 코드</exception>
    /// <exception cref="CurrencyParseException">파싱 실패</exception>
    public static decimal Parse(string input, string isoCode)
    {
        var info = Registry.GetCurrency(isoCode);
        try
        {
            return decimal.Parse(input, NumberStyles.Currency, info.Culture);
        }
        catch (FormatException ex)
        {
            throw new CurrencyParseException(input, isoCode, ex);
        }
    }

    /// <summary>
    /// 통화 형식 문자열을 decimal로 파싱을 시도합니다.
    /// </summary>
    public static bool TryParse(string input, string isoCode, out decimal result)
    {
        if (!Registry.TryGetCurrency(isoCode, out var info))
        {
            result = 0;
            return false;
        }
        return decimal.TryParse(input, NumberStyles.Currency, info.Culture, out result);
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// ReadOnlySpan 기반 고성능 통화 파싱을 시도합니다.
    /// </summary>
    /// <param name="input">파싱할 문자 스팬</param>
    /// <param name="isoCode">ISO 4217 통화 코드</param>
    /// <param name="result">파싱된 금액</param>
    public static bool TryParse(ReadOnlySpan<char> input, string isoCode, out decimal result)
    {
        if (!Registry.TryGetCurrency(isoCode, out var info))
        {
            result = 0;
            return false;
        }
        return decimal.TryParse(input, NumberStyles.Currency, info.Culture, out result);
    }
#endif

    /// <summary>
    /// ISO 4217 통화 코드의 지원 여부를 확인합니다.
    /// </summary>
    public static bool IsSupported(string isoCode) => Registry.IsSupported(isoCode);

    /// <summary>
    /// 지원되는 모든 ISO 4217 통화 코드를 반환합니다.
    /// </summary>
    public static IEnumerable<string> SupportedCodes => Registry.SupportedCodes;

    /// <summary>
    /// ISO 4217 코드로 통화 메타데이터를 가져옵니다.
    /// </summary>
    /// <param name="isoCode">ISO 4217 통화 코드</param>
    /// <returns>통화 정보</returns>
    /// <exception cref="CurrencyNotFoundException">지원하지 않는 통화 코드</exception>
    public static CurrencyInfo GetInfo(string isoCode) => Registry.GetCurrency(isoCode);

    /// <summary>
    /// ISO 4217 코드로 통화 메타데이터 조회를 시도합니다.
    /// </summary>
    public static bool TryGetInfo(string isoCode, out CurrencyInfo info) =>
        Registry.TryGetCurrency(isoCode, out info);

    /// <summary>
    /// 지원되는 모든 통화 정보를 반환합니다.
    /// </summary>
    public static IEnumerable<CurrencyInfo> SupportedCurrencies => Registry.SupportedCurrencies;

    /// <summary>
    /// 금액을 축약 형식으로 포맷합니다. (예: 1234 → "$1.2K", 1500000 → "$1.5M")
    /// </summary>
    /// <param name="amount">포맷할 금액</param>
    /// <param name="isoCode">ISO 4217 통화 코드</param>
    /// <param name="decimals">소수점 자릿수 (기본: 1)</param>
    /// <returns>축약 형식 문자열</returns>
    public static string FormatCompact(decimal amount, string isoCode, int decimals = 1)
    {
        var info = Registry.GetCurrency(isoCode);
        var abs = Math.Abs(amount);
        var sign = amount < 0 ? "-" : "";
        var fmt = $"F{decimals}";
        var inv = System.Globalization.CultureInfo.InvariantCulture;

        string scaled;
        if (abs >= 1_000_000_000_000m)
            scaled = $"{sign}{(abs / 1_000_000_000_000m).ToString(fmt, inv)}{CompactSuffix.Trillion}";
        else if (abs >= 1_000_000_000m)
            scaled = $"{sign}{(abs / 1_000_000_000m).ToString(fmt, inv)}{CompactSuffix.Billion}";
        else if (abs >= 1_000_000m)
            scaled = $"{sign}{(abs / 1_000_000m).ToString(fmt, inv)}{CompactSuffix.Million}";
        else if (abs >= 1_000m)
            scaled = $"{sign}{(abs / 1_000m).ToString(fmt, inv)}{CompactSuffix.Thousand}";
        else
            return Format(amount, isoCode);

        return $"{info.Symbol}{scaled}";
    }

    private static class CompactSuffix
    {
        public const string Thousand = "K";
        public const string Million = "M";
        public const string Billion = "B";
        public const string Trillion = "T";
    }
}
