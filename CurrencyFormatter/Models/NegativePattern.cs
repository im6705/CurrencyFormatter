namespace CurrencyFormatter.Models;

/// <summary>
/// 음수 금액 표시 패턴.
/// </summary>
public enum NegativePattern
{
    /// <summary>통화의 기본 음수 패턴 사용 (예: -$100.00)</summary>
    Default,

    /// <summary>괄호로 감싸기, 회계 스타일 (예: ($100.00))</summary>
    Parentheses
}
