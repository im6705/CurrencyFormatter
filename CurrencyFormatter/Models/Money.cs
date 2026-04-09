using System;

namespace CurrencyFormatter.Models;

/// <summary>
/// 금액과 통화 코드를 함께 표현하는 불변 값 타입.
/// 통화가 다른 금액의 실수 연산을 타입 수준에서 방지합니다.
/// </summary>
public readonly struct Money : IEquatable<Money>, IComparable<Money>
{
    /// <summary>금액</summary>
    public decimal Amount { get; }

    /// <summary>ISO 4217 통화 코드</summary>
    public string IsoCode { get; }

    /// <summary>새 Money 인스턴스를 생성합니다.</summary>
    /// <param name="amount">금액</param>
    /// <param name="isoCode">ISO 4217 통화 코드</param>
    public Money(decimal amount, string isoCode)
    {
        Amount = amount;
        IsoCode = isoCode?.Trim().ToUpperInvariant()
            ?? throw new ArgumentNullException(nameof(isoCode));
    }

    /// <summary>통화 형식 문자열로 포맷합니다.</summary>
    public string Format(FormatOptions? options = null)
        => options != null
            ? CurrencyFormatter.Format(Amount, IsoCode, options)
            : CurrencyFormatter.Format(Amount, IsoCode);

    /// <summary>같은 통화의 금액을 더합니다.</summary>
    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.IsoCode);
    }

    /// <summary>같은 통화의 금액을 뺍니다.</summary>
    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount - right.Amount, left.IsoCode);
    }

    /// <summary>금액에 스칼라를 곱합니다.</summary>
    public static Money operator *(Money money, decimal multiplier)
        => new Money(money.Amount * multiplier, money.IsoCode);

    /// <summary>금액에 스칼라를 곱합니다.</summary>
    public static Money operator *(decimal multiplier, Money money)
        => money * multiplier;

    /// <summary>금액을 스칼라로 나눕니다.</summary>
    public static Money operator /(Money money, decimal divisor)
        => new Money(money.Amount / divisor, money.IsoCode);

    /// <summary>부호 반전.</summary>
    public static Money operator -(Money money)
        => new Money(-money.Amount, money.IsoCode);

#pragma warning disable CS1591 // 연산자 및 표준 오버라이드에 대한 XML 주석 생략
    public static bool operator ==(Money left, Money right) => left.Equals(right);
    public static bool operator !=(Money left, Money right) => !left.Equals(right);
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;
    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;

    public bool Equals(Money other) => Amount == other.Amount && IsoCode == other.IsoCode;
    public override bool Equals(object? obj) => obj is Money other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            return (Amount.GetHashCode() * 397) ^ (IsoCode?.GetHashCode() ?? 0);
        }
    }

    public int CompareTo(Money other)
    {
        EnsureSameCurrency(this, other);
        return Amount.CompareTo(other.Amount);
    }

    public override string ToString() => Format();
#pragma warning restore CS1591

    /// <summary>통화 문자열을 파싱하여 Money를 생성합니다.</summary>
    public static Money Parse(string input, string isoCode)
        => new Money(CurrencyFormatter.Parse(input, isoCode), isoCode);

    /// <summary>통화 문자열 파싱을 시도합니다.</summary>
    public static bool TryParse(string input, string isoCode, out Money result)
    {
        if (CurrencyFormatter.TryParse(input, isoCode, out var amount))
        {
            result = new Money(amount, isoCode);
            return true;
        }
        result = default;
        return false;
    }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (!string.Equals(left.IsoCode, right.IsoCode, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"Cannot perform operations between different currencies: {left.IsoCode} and {right.IsoCode}");
    }
}
