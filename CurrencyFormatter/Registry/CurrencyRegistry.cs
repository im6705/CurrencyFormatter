using System;
using System.Collections.Generic;
using System.Globalization;
using CurrencyFormatter.Exceptions;
using CurrencyFormatter.Models;

namespace CurrencyFormatter.Registry;

/// <summary>
/// ISO 4217 통화 레지스트리. CultureInfo 기반 자동 매핑 + 수동 보정을 제공합니다.
/// </summary>
public sealed class CurrencyRegistry : ICurrencyRegistry
{
    private readonly Dictionary<string, CurrencyInfo> _map;

    /// <summary>기본 레지스트리 인스턴스 (시스템 CultureInfo 기반).</summary>
    public static CurrencyRegistry Default { get; } = new CurrencyRegistry();

    /// <summary>시스템 CultureInfo 기반으로 레지스트리를 초기화합니다.</summary>
    public CurrencyRegistry()
    {
        _map = BuildMap();
    }

    private CurrencyRegistry(Dictionary<string, CurrencyInfo> map)
    {
        _map = map;
    }

    /// <summary>
    /// 기존 레지스트리에 커스텀 통화를 추가하여 새 레지스트리를 생성합니다.
    /// </summary>
    public CurrencyRegistry WithCurrency(CurrencyInfo info)
    {
        if (info == null) throw new ArgumentNullException(nameof(info));
        var copy = new Dictionary<string, CurrencyInfo>(_map, StringComparer.OrdinalIgnoreCase);
        copy[Normalize(info.IsoCode)] = info;
        return new CurrencyRegistry(copy);
    }

    /// <inheritdoc />
    public CurrencyInfo GetCurrency(string isoCode)
    {
        var key = Normalize(isoCode);
        if (!_map.TryGetValue(key, out var info))
            throw new CurrencyNotFoundException(isoCode);
        return info;
    }

    /// <inheritdoc />
    public bool TryGetCurrency(string isoCode, out CurrencyInfo info)
    {
        if (isoCode == null)
        {
            info = null!;
            return false;
        }
        return _map.TryGetValue(Normalize(isoCode), out info!);
    }

    /// <inheritdoc />
    public bool IsSupported(string isoCode) =>
        isoCode != null && _map.ContainsKey(Normalize(isoCode));

    /// <inheritdoc />
    public IEnumerable<string> SupportedCodes => _map.Keys;

    /// <inheritdoc />
    public IEnumerable<CurrencyInfo> SupportedCurrencies => _map.Values;

    private static string Normalize(string isoCode) =>
        isoCode?.Trim().ToUpperInvariant()
        ?? throw new ArgumentNullException(nameof(isoCode));

    private static Dictionary<string, CurrencyInfo> BuildMap()
    {
        var map = new Dictionary<string, CurrencyInfo>(StringComparer.OrdinalIgnoreCase);

        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            try
            {
                var region = new RegionInfo(culture.Name);
                var isoCode = region.ISOCurrencySymbol;
                if (!map.ContainsKey(isoCode))
                {
                    map[isoCode] = CurrencyInfo.FromCulture(culture, region);
                }
            }
            catch (ArgumentException)
            {
                // 일부 culture는 RegionInfo 생성 실패 → 무시
            }
        }

        // .NET이 못 잡는 예외 케이스 수동 보정
        Override(map, "KRW", "ko-KR");
        Override(map, "JPY", "ja-JP");
        Override(map, "CNY", "zh-CN");
        Override(map, "EUR", "de-DE");
        Override(map, "INR", "hi-IN");

        return map;
    }

    private static void Override(Dictionary<string, CurrencyInfo> map, string iso, string cultureName)
    {
        try
        {
            var culture = new CultureInfo(cultureName);
            var region = new RegionInfo(cultureName);
            map[iso] = CurrencyInfo.FromCulture(culture, region);
        }
        catch (ArgumentException)
        {
            // 환경에 없으면 무시
        }
    }
}
