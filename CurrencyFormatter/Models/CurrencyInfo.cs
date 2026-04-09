using System.Globalization;

namespace CurrencyFormatter.Models;

/// <summary>
/// ISO 4217 통화에 대한 메타데이터를 제공합니다.
/// </summary>
public sealed class CurrencyInfo
{
    /// <summary>ISO 4217 통화 코드 (예: USD, KRW)</summary>
    public string IsoCode { get; }

    /// <summary>통화 기호 (예: $, ₩)</summary>
    public string Symbol { get; }

    /// <summary>영문 통화명 (예: US Dollar)</summary>
    public string EnglishName { get; }

    /// <summary>현지 통화명 (예: 대한민국 원)</summary>
    public string NativeName { get; }

    /// <summary>소수점 자릿수 (예: USD=2, KRW=0, JPY=0)</summary>
    public int DecimalDigits { get; }

    /// <summary>해당 통화의 NumberFormatInfo</summary>
    public NumberFormatInfo NumberFormat { get; }

    internal CultureInfo Culture { get; }

    public CurrencyInfo(string isoCode, string symbol, string englishName, string nativeName,
        int decimalDigits, NumberFormatInfo numberFormat, CultureInfo culture)
    {
        IsoCode = isoCode;
        Symbol = symbol;
        EnglishName = englishName;
        NativeName = nativeName;
        DecimalDigits = decimalDigits;
        NumberFormat = numberFormat;
        Culture = culture;
    }

    internal static CurrencyInfo FromCulture(CultureInfo culture, RegionInfo region)
    {
        return new CurrencyInfo(
            isoCode: region.ISOCurrencySymbol,
            symbol: region.CurrencySymbol,
            englishName: region.CurrencyEnglishName,
            nativeName: region.CurrencyNativeName,
            decimalDigits: culture.NumberFormat.CurrencyDecimalDigits,
            numberFormat: culture.NumberFormat,
            culture: culture);
    }

    /// <inheritdoc />
    public override string ToString() => $"{IsoCode} ({Symbol}) - {EnglishName}";
}
