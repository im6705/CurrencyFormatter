namespace CurrencyFormatter.Models;

/// <summary>
/// 통화 기호 표시 모드.
/// </summary>
public enum SymbolMode
{
    /// <summary>통화 기호 사용 (예: $100.00)</summary>
    Symbol,

    /// <summary>ISO 4217 코드 사용 (예: 100.00 USD)</summary>
    IsoCode,

    /// <summary>기호 없이 숫자만 (예: 100.00)</summary>
    None
}
