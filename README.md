# CurrencyFormatter
[![Build and Publish](https://github.com/im6705/CurrencyFormatter/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/im6705/CurrencyFormatter/actions/workflows/build.yml)

ISO 4217 currency formatting, parsing, and metadata library for .NET.

## Features

- Format and parse currency amounts using ISO 4217 codes
- `CurrencyInfo` metadata: symbol, decimal digits, English/native names
- `Money` value type with arithmetic operators and type-safe currency enforcement
- `Percent` value type for intuitive percentage calculations (`price * 8.5m.Percent()`)
- Compact formatting: `$1.2K`, `$3.5M`, `$2.0B`
- `Money.Round()` for currency-aware rounding
- `IFormattable` support: `$"{price:K}"` for compact, `$"{price:N}"` for number-only
- Built-in JSON serialization (`System.Text.Json`)
- `FormatOptions` for custom decimal digits, rounding, group separators
- Extension methods: `decimal.FormatAsCurrency("USD")`
- Custom `IFormatProvider` for `string.Format` integration
- Targets `net8.0` and `netstandard2.0`

## Installation

```bash
dotnet add package CurrencyFormatter
```

## Quick Start

```csharp
using System.Text.Json;
using CurrencyFormatter;
using CurrencyFormatter.Extensions;
using CurrencyFormatter.Formatting;
using CurrencyFormatter.Models;

// Basic formatting
Currency.Format(1234.56m, "USD");   // "$1,234.56"
Currency.Format(1234m, "KRW");      // "₩1,234"
Currency.Format(1234.56m, "EUR");   // "1.234,56 €"

// Parsing
decimal amount = Currency.Parse("$1,234.56", "USD");  // 1234.56

// Safe parsing
if (Currency.TryParse("$100", "USD", out var result))
    Console.WriteLine(result);  // 100

// Currency metadata
var info = Currency.GetInfo("USD");
Console.WriteLine(info.Symbol);         // "$"
Console.WriteLine(info.DecimalDigits);  // 2
Console.WriteLine(info.EnglishName);    // "US Dollar"

// Extension methods
var formatted = 1234.56m.FormatAsCurrency("USD");  // "$1,234.56"

// Money type (type-safe currency arithmetic)
var price = new Money(100m, "USD");
var tax = new Money(8.5m, "USD");
var total = price + tax;           // Money(108.5, "USD")
Console.WriteLine(total);          // "$108.50"

// Percent type (type-safe percentage calculations)
var pctTax = price * new Percent(8.5m);   // Money(8.5, "USD")
var pctTax2 = price * 8.5m.Percent();     // same thing, extension method
var pctTax3 = price.PercentOf(8.5m);      // same thing, fluent style
var totalWithTax = price + pctTax;         // Money(108.5, "USD")
Console.WriteLine($"Total(Tax inc.) {totalWithTax}");

// Money prevents cross-currency errors at compile/runtime
// new Money(100, "USD") + new Money(100, "EUR")  → throws InvalidOperationException

// Compact formatting
var compact1 = Currency.FormatCompact(1500000m, "USD");     // "$1.5M"
var compact2 = Currency.FormatCompact(2500m, "USD");        // "$2.5K"
var compact3 = Currency.FormatCompact(3000000000m, "USD");  // "$3.0B"

// IFormattable (format strings)
var revenue = new Money(1500000m, "USD");
Console.WriteLine($"{revenue:K}");  // "$1.5M"  (compact)
Console.WriteLine($"{revenue:N}");  // "1,500,000.00" (number only)
Console.WriteLine($"{revenue:C}");  // "$1,500,000.00" (currency, default)

// Rounding (currency-aware)
var calculated = new Money(10.555m, "USD");
calculated.Round();                                  // $10.56 (USD → 2 decimals)
new Money(100.7m, "JPY").Round();                    // ¥101 (JPY → 0 decimals)
calculated.Round(MidpointRounding.AwayFromZero);     // $10.56

// JSON serialization (System.Text.Json, built-in)
var json = JsonSerializer.Serialize(price);   // {"amount":100,"currency":"USD"}
var money = JsonSerializer.Deserialize<Money>(json);

// Format options
var options = new FormatOptions(
    decimalDigits: 4,
    rounding: MidpointRounding.AwayFromZero,
    useGroupSeparator: true,
    includeSymbol: true);
Currency.Format(1234.5678m, "USD", options);  // "$1,234.5678"

// IFormatProvider integration
var provider = new CurrencyFormatProvider("USD");
string.Format(provider, "{0}", 1234.56m);  // "$1,234.56"
```

## Custom Currency Registration

```csharp
using CurrencyFormatter.Registry;

var custom = new CurrencyInfo("XBT", "₿", "Bitcoin", "비트코인", 8, ...);
var registry = CurrencyRegistry.Default.WithCurrency(custom);
```

## Supported Currencies

```csharp
foreach (var code in Currency.SupportedCodes)
    Console.WriteLine(code);

foreach (var currency in Currency.SupportedCurrencies)
    Console.WriteLine($"{currency.IsoCode} ({currency.Symbol}) - {currency.EnglishName}");
```

## License

MIT
