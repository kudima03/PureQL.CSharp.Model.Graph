using System.Text.Json;
using System.Text.Json.Serialization;
using PureQL.CSharp.Model.Literals;
using PureQL.CSharp.Model.Serialization;

namespace PureQL.CSharp.Model.Graph.Tests;

public sealed record GraphValuesTests
{
    private static readonly JsonSerializerOptions Json = Options();

    [Theory]
    [InlineData("1.50", "1.5")]
    [InlineData("0", "0")]
    [InlineData("0.000", "0")]
    [InlineData("-0.0", "0")]
    [InlineData("100", "100")]
    [InlineData("-12.340", "-12.34")]
    [InlineData("0.0000000000000000000000000001", "0.0000000000000000000000000001")]
    public void DecimalsAreSpelledWithoutTrailingZeros(string value, string expected)
    {
        decimal number = decimal.Parse(
            value,
            System.Globalization.CultureInfo.InvariantCulture
        );

        Assert.Equal(expected, GraphValues.Decimal(number));
        Assert.Equal(expected, JsonValue(new LiteralDecimal(number)));
    }

    [Fact]
    public void UuidsAreSpelledInTheDFormat()
    {
        Guid uuid = Guid.Parse("8F2A5C1E-3B7D-4E9A-A1C2-6D4B8E0F2A37");

        Assert.Equal("8f2a5c1e-3b7d-4e9a-a1c2-6d4b8e0f2a37", GraphValues.Uuid(uuid));
        Assert.Equal(GraphValues.Uuid(uuid), JsonValue(new LiteralUuid(uuid)));
    }

    [Theory]
    [InlineData(2024, 2, 29, "2024-02-29")]
    [InlineData(1, 1, 1, "0001-01-01")]
    [InlineData(9999, 12, 31, "9999-12-31")]
    public void DatesAreSpelledAsYearMonthDay(
        int year,
        int month,
        int day,
        string expected
    )
    {
        DateOnly date = new DateOnly(year, month, day);

        Assert.Equal(expected, GraphValues.Date(date));
        Assert.Equal(expected, JsonValue(new LiteralDate(date)));
    }

    [Theory]
    [InlineData(0L, "00:00:00")]
    [InlineData(5_000_000L, "00:00:00.5")]
    [InlineData(863_991_234_560L, "23:59:59.123456")]
    [InlineData(450_000_000_000L, "12:30:00")]
    public void TimesAreSpelledAsTheSerializationWritesThem(long ticks, string expected)
    {
        TimeOnly time = new TimeOnly(ticks);

        Assert.Equal(expected, GraphValues.Time(time));
        Assert.Equal(expected, JsonValue(new LiteralTime(time)));
    }

    [Fact]
    public void TimesFinerThanAMicrosecondKeepEveryTick()
    {
        TimeOnly time = new TimeOnly(new TimeOnly(1, 2, 3).Ticks + 1);

        Assert.Equal("01:02:03.0000001", GraphValues.Time(time));
        Assert.True(GraphValues.TryTime(GraphValues.Time(time), out TimeOnly back));
        Assert.Equal(time, back);
    }

    [Fact]
    public void DatetimesAreSpelledInUtc()
    {
        DateTimeOffset local = new DateTimeOffset(
            2024,
            3,
            1,
            1,
            30,
            0,
            TimeSpan.FromHours(2)
        ).AddTicks(1_234_560);
        DateTimeOffset utc = new DateTimeOffset(2024, 1, 31, 18, 30, 5, TimeSpan.Zero);

        Assert.Equal("2024-02-29T23:30:00.123456Z", GraphValues.Datetime(local));
        Assert.Equal(GraphValues.Datetime(local), JsonValue(new LiteralDatetime(local)));
        Assert.Equal("2024-01-31T18:30:05Z", GraphValues.Datetime(utc));
        Assert.Equal(GraphValues.Datetime(utc), JsonValue(new LiteralDatetime(utc)));
    }

    [Fact]
    public void WrittenValuesReadBack()
    {
        DateTimeOffset datetime = new DateTimeOffset(
            2024,
            2,
            29,
            23,
            59,
            59,
            TimeSpan.Zero
        ).AddTicks(1_234_567);
        TimeOnly time = new TimeOnly(23, 59, 59).Add(TimeSpan.FromTicks(1_234_560));
        Guid uuid = Guid.NewGuid();

        Assert.True(
            GraphValues.TryDecimal(GraphValues.Decimal(-1.25m), out decimal number)
        );
        Assert.Equal(-1.25m, number);
        Assert.True(GraphValues.TryUuid(GraphValues.Uuid(uuid), out Guid readUuid));
        Assert.Equal(uuid, readUuid);
        Assert.True(GraphValues.TryDate("2024-02-29", out DateOnly date));
        Assert.Equal(new DateOnly(2024, 2, 29), date);
        Assert.True(GraphValues.TryTime(GraphValues.Time(time), out TimeOnly readTime));
        Assert.Equal(time, readTime);
        Assert.True(
            GraphValues.TryDatetime(
                GraphValues.Datetime(datetime),
                out DateTimeOffset back
            )
        );
        Assert.Equal(datetime, back);
        Assert.Equal(TimeSpan.Zero, back.Offset);
        Assert.True(GraphValues.TryDatetime("2024-02-29T23:59:59Z", out back));
        Assert.Equal(new DateTimeOffset(2024, 2, 29, 23, 59, 59, TimeSpan.Zero), back);
    }

    [Fact]
    public void ClrValuesAreAcceptedAsThey()
    {
        DateTimeOffset datetime = DateTimeOffset.UnixEpoch;

        Assert.True(GraphValues.TryDecimal(1.50m, out decimal number));
        Assert.Equal(1.50m, number);
        Assert.True(GraphValues.TryUuid(Guid.Empty, out Guid uuid));
        Assert.Equal(Guid.Empty, uuid);
        Assert.True(GraphValues.TryDate(DateOnly.MaxValue, out DateOnly date));
        Assert.Equal(DateOnly.MaxValue, date);
        Assert.True(GraphValues.TryTime(TimeOnly.MaxValue, out TimeOnly time));
        Assert.Equal(TimeOnly.MaxValue, time);
        Assert.True(GraphValues.TryDatetime(datetime, out DateTimeOffset back));
        Assert.Equal(datetime, back);
        Assert.True(GraphValues.TryBoolean(true, out bool flag));
        Assert.True(flag);
        Assert.True(GraphValues.TryString("text", out string text));
        Assert.Equal("text", text);
    }

    [Fact]
    public void AnyIntegerTypeIsALong()
    {
        object[] values = [-7L, -7, (short)-7, (sbyte)-7, (byte)7, (ushort)7, 7u];
        foreach (object value in values)
        {
            Assert.True(GraphValues.TryInteger(value, out long result));
            Assert.Equal(7L, Math.Abs(result));
        }

        Assert.True(GraphValues.TryInteger((ulong)long.MaxValue, out long max));
        Assert.Equal(long.MaxValue, max);
    }

    [Fact]
    public void OtherValuesAreRejected()
    {
        object[] values =
        [
            "not a value",
            "1e5",
            "2024-02-30",
            "24:00:00",
            "12:00:00.",
            "1a:00:00",
            "2024-02-29T23:59:59+02:00",
            "2024-02-29 23:59:59Z",
            ulong.MaxValue,
            1.5d,
        ];
        foreach (object value in values)
        {
            Assert.False(GraphValues.TryInteger(value, out _), $"integer {value}");
            Assert.False(GraphValues.TryBoolean(value, out _), $"boolean {value}");
            Assert.False(GraphValues.TryDecimal(value, out _), $"decimal {value}");
            Assert.False(GraphValues.TryUuid(value, out _), $"uuid {value}");
            Assert.False(GraphValues.TryDate(value, out _), $"date {value}");
            Assert.False(GraphValues.TryTime(value, out _), $"time {value}");
            Assert.False(GraphValues.TryDatetime(value, out _), $"datetime {value}");
            Assert.Equal(value is string, GraphValues.TryString(value, out _));
        }
    }

    [Fact]
    public void ListsAreAnyEnumerableButAString()
    {
        Assert.True(
            GraphValues.TryList(
                new List<object> { 1L },
                out System.Collections.IEnumerable items
            )
        );
        _ = Assert.Single(items.Cast<object>());
        Assert.True(GraphValues.TryList(Array.Empty<string>(), out _));
        Assert.False(GraphValues.TryList("text", out _));
        Assert.False(GraphValues.TryList(1L, out _));
    }

    private static string JsonValue(object literal)
    {
        string json = JsonSerializer.Serialize(literal, literal.GetType(), Json);
        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.GetProperty("value").GetString()!;
    }

    private static JsonSerializerOptions Options()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        foreach (JsonConverter converter in new PureQLConverters())
        {
            options.Converters.Add(converter);
        }

        return options;
    }
}
