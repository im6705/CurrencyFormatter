using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CurrencyFormatter.Models;

namespace CurrencyFormatter.Formatting;

/// <summary>
/// System.Text.Json용 Money 직렬화 변환기.
/// <code>{"amount": 1234.56, "currency": "USD"}</code> 형식으로 직렬화합니다.
/// </summary>
public sealed class MoneyJsonConverter : JsonConverter<Money>
{
    /// <inheritdoc />
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token for Money.");

        decimal amount = 0;
        string? currency = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token.");

            var prop = reader.GetString();
            reader.Read();

            switch (prop?.ToLowerInvariant())
            {
                case "amount":
                    amount = reader.GetDecimal();
                    break;
                case "currency":
                    currency = reader.GetString();
                    break;
            }
        }

        if (currency == null)
            throw new JsonException("Missing 'currency' property in Money JSON.");

        return new Money(amount, currency);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("amount", value.Amount);
        writer.WriteString("currency", value.IsoCode);
        writer.WriteEndObject();
    }
}

/// <summary>
/// System.Text.Json용 Percent 직렬화 변환기.
/// <code>{"value": 8.5}</code> 형식으로 직렬화합니다.
/// </summary>
public sealed class PercentJsonConverter : JsonConverter<Percent>
{
    /// <inheritdoc />
    public override Percent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return new Percent(reader.GetDecimal());

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected Number or StartObject token for Percent.");

        decimal value = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token.");

            var prop = reader.GetString();
            reader.Read();

            if (string.Equals(prop, "value", StringComparison.OrdinalIgnoreCase))
                value = reader.GetDecimal();
        }

        return new Percent(value);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Percent value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("value", value.Value);
        writer.WriteEndObject();
    }
}
