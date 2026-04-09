using System;

namespace CurrencyFormatter.Exceptions;

/// <summary>
/// 통화 문자열 파싱에 실패했을 때 발생합니다.
/// </summary>
public class CurrencyParseException : FormatException
{
    /// <summary>파싱 대상 ISO 4217 코드</summary>
    public string IsoCode { get; }

    /// <summary>파싱에 실패한 원본 문자열</summary>
    public string Input { get; }

    public CurrencyParseException(string input, string isoCode)
        : base($"Failed to parse '{input}' as {isoCode} currency.")
    {
        Input = input;
        IsoCode = isoCode;
    }

    public CurrencyParseException(string input, string isoCode, Exception innerException)
        : base($"Failed to parse '{input}' as {isoCode} currency.", innerException)
    {
        Input = input;
        IsoCode = isoCode;
    }
}
