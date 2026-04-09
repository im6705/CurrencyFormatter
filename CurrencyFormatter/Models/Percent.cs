using System;

namespace CurrencyFormatter.Models;

/// <summary>
/// 백분율을 표현하는 불변 값 타입.
/// 8.5를 전달하면 내부적으로 0.085 비율로 저장합니다.
/// </summary>
public readonly struct Percent : IEquatable<Percent>
{
    /// <summary>정규화된 비율 (예: 8.5% → 0.085)</summary>
    public decimal Rate { get; }

    /// <summary>원래 백분율 값 (예: 8.5)</summary>
    public decimal Value { get; }

    /// <summary>새 Percent 인스턴스를 생성합니다.</summary>
    /// <param name="value">백분율 값 (예: 8.5 = 8.5%)</param>
    public Percent(decimal value)
    {
        Value = value;
        Rate = value / 100m;
    }

    /// <summary>Percent 끼리 더합니다.</summary>
    public static Percent operator +(Percent left, Percent right)
        => new Percent(left.Value + right.Value);

    /// <summary>Percent 끼리 뺍니다.</summary>
    public static Percent operator -(Percent left, Percent right)
        => new Percent(left.Value - right.Value);

    /// <summary>부호 반전.</summary>
    public static Percent operator -(Percent pct)
        => new Percent(-pct.Value);

#pragma warning disable CS1591
    public static bool operator ==(Percent left, Percent right) => left.Equals(right);
    public static bool operator !=(Percent left, Percent right) => !left.Equals(right);

    public bool Equals(Percent other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is Percent other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"{Value}%";
#pragma warning restore CS1591
}