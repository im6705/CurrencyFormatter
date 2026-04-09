using System.Collections.Generic;
using CurrencyFormatter.Models;

namespace CurrencyFormatter.Registry;

/// <summary>
/// 통화 정보 레지스트리 인터페이스.
/// </summary>
public interface ICurrencyRegistry
{
    /// <summary>ISO 코드로 통화 정보를 가져옵니다.</summary>
    CurrencyInfo GetCurrency(string isoCode);

    /// <summary>ISO 코드로 통화 정보를 시도합니다.</summary>
    bool TryGetCurrency(string isoCode, out CurrencyInfo info);

    /// <summary>해당 ISO 코드가 지원되는지 확인합니다.</summary>
    bool IsSupported(string isoCode);

    /// <summary>지원되는 모든 통화 코드를 반환합니다.</summary>
    IEnumerable<string> SupportedCodes { get; }

    /// <summary>지원되는 모든 통화 정보를 반환합니다.</summary>
    IEnumerable<CurrencyInfo> SupportedCurrencies { get; }
}
