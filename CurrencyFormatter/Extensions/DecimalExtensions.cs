using CurrencyFormatter.Models;

namespace CurrencyFormatter.Extensions;

/// <summary>
/// decimal 타입에 대한 통화 포맷팅 확장 메서드.
/// </summary>
public static class DecimalExtensions
{
    /// <summary>
    /// 금액을 통화 형식 문자열로 포맷합니다.
    /// </summary>
    /// <param name="amount">포맷할 금액</param>
    /// <param name="isoCode">ISO 4217 통화 코드</param>
    public static string FormatAsCurrency(this decimal amount, string isoCode)
        => CurrencyFormatter.Format(amount, isoCode);

    /// <summary>
    /// 금액을 통화 형식 문자열로 포맷합니다 (옵션 지정).
    /// </summary>
    public static string FormatAsCurrency(this decimal amount, string isoCode, FormatOptions options)
        => CurrencyFormatter.Format(amount, isoCode, options);

    /// <summary>
    /// 금액을 Money 구조체로 변환합니다.
    /// </summary>
    public static Money AsMoney(this decimal amount, string isoCode)
        => new Money(amount, isoCode);
}
