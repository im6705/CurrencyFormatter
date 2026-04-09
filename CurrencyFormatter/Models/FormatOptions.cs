using System;

namespace CurrencyFormatter.Models;

/// <summary>
/// 통화 포맷팅 옵션.
/// </summary>
public sealed class FormatOptions
{
    /// <summary>소수점 자릿수 (null이면 통화 기본값 사용)</summary>
    public int? DecimalDigits { get; }

    /// <summary>반올림 모드 (기본: Banker's rounding)</summary>
    public MidpointRounding Rounding { get; }

    /// <summary>천 단위 구분자 사용 여부</summary>
    public bool UseGroupSeparator { get; }

    /// <summary>통화 기호 포함 여부 (SymbolMode.Symbol/None과 동일, 하위 호환용)</summary>
    public bool IncludeSymbol => Symbol != SymbolMode.None;

    /// <summary>통화 기호 표시 모드</summary>
    public SymbolMode Symbol { get; }

    /// <summary>음수 금액 표시 패턴</summary>
    public NegativePattern NegativePattern { get; }

    /// <summary>포맷 옵션을 생성합니다.</summary>
    /// <param name="decimalDigits">소수점 자릿수 (null이면 통화 기본값)</param>
    /// <param name="rounding">반올림 모드</param>
    /// <param name="useGroupSeparator">천 단위 구분자 사용 여부</param>
    /// <param name="includeSymbol">통화 기호 포함 여부 (하위 호환, symbol 파라미터 우선)</param>
    /// <param name="symbol">통화 기호 표시 모드</param>
    /// <param name="negativePattern">음수 금액 표시 패턴</param>
    public FormatOptions(
        int? decimalDigits = null,
        MidpointRounding rounding = MidpointRounding.ToEven,
        bool useGroupSeparator = true,
        bool includeSymbol = true,
        SymbolMode? symbol = null,
        NegativePattern negativePattern = NegativePattern.Default)
    {
        if (decimalDigits.HasValue && decimalDigits.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(decimalDigits), "Decimal digits must be non-negative.");

        DecimalDigits = decimalDigits;
        Rounding = rounding;
        UseGroupSeparator = useGroupSeparator;
        Symbol = symbol ?? (includeSymbol ? SymbolMode.Symbol : SymbolMode.None);
        NegativePattern = negativePattern;
    }

    /// <summary>기본 옵션 (통화 기본값 사용)</summary>
    public static FormatOptions Default { get; } = new FormatOptions();
}
