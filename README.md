# CurrencyFormatter

ISO 4217 currency formatting, parsing, and metadata library for .NET.

## Features

- Format and parse currency amounts using ISO 4217 codes
- `CurrencyInfo` metadata: symbol, decimal digits, English/native names
- `Money` value type with arithmetic operators and type-safe currency enforcement
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
using CurrencyFormatter;
using CurrencyFormatter.Extensions;
using CurrencyFormatter.Models;

// Basic formatting
CurrencyFormatter.Format(1234.56m, "USD");   // "$1,234.56"
CurrencyFormatter.Format(1234m, "KRW");      // "₩1,234"
CurrencyFormatter.Format(1234.56m, "EUR");   // "1.234,56 €"

// Parsing
decimal amount = CurrencyFormatter.Parse("$1,234.56", "USD");  // 1234.56

// Safe parsing
if (CurrencyFormatter.TryParse("$100", "USD", out var result))
    Console.WriteLine(result);  // 100

// Currency metadata
var info = CurrencyFormatter.GetInfo("USD");
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

// Money prevents cross-currency errors at compile/runtime
// new Money(100, "USD") + new Money(100, "EUR")  → throws InvalidOperationException

// Format options
var options = new FormatOptions(
    decimalDigits: 4,
    rounding: MidpointRounding.AwayFromZero,
    useGroupSeparator: true,
    includeSymbol: true);
CurrencyFormatter.Format(1234.5678m, "USD", options);  // "$1,234.5678"

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
foreach (var code in CurrencyFormatter.SupportedCodes)
    Console.WriteLine(code);

foreach (var currency in CurrencyFormatter.SupportedCurrencies)
    Console.WriteLine($"{currency.IsoCode} ({currency.Symbol}) - {currency.EnglishName}");
```

## License

MIT
