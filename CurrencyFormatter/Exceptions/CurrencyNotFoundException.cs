using System;

namespace CurrencyFormatter.Exceptions;

/// <summary>
/// 지원하지 않는 ISO 4217 통화 코드가 사용되었을 때 발생합니다.
/// </summary>
public class CurrencyNotFoundException : ArgumentException
{
    /// <summary>요청된 ISO 4217 코드</summary>
    public string IsoCode { get; }

    public CurrencyNotFoundException(string isoCode)
        : base($"Unsupported ISO 4217 currency code: {isoCode}", nameof(isoCode))
    {
        IsoCode = isoCode;
    }

    public CurrencyNotFoundException(string isoCode, Exception innerException)
        : base($"Unsupported ISO 4217 currency code: {isoCode}", nameof(isoCode), innerException)
    {
        IsoCode = isoCode;
    }
}
