using System.Collections;
using System.Globalization;

namespace PureQL.CSharp.Model.Graph;

/// <summary>
/// The store-neutral representation of the model's scalar values, spelled as
/// <c>PureQL.CSharp.Model.Serialization</c> writes them in JSON: <c>long</c>,
/// <c>bool</c> and <c>string</c> as they are, a <c>decimal</c> as an invariant string
/// without exponent or trailing zeros, a <c>Guid</c> in the "D" format, a
/// <c>DateOnly</c> as <c>yyyy-MM-dd</c>, a <c>TimeOnly</c> as <c>HH:mm:ss[.f]</c> and a
/// <c>DateTimeOffset</c> as <c>yyyy-MM-ddTHH:mm:ss[.f]Z</c> in UTC, the fraction without
/// trailing zeros. Reading accepts that representation, any integer CLR type for a
/// <c>long</c>, the CLR value itself, and any list for an array.
/// </summary>
internal static class GraphValues
{
    private const string DateFormat = "yyyy'-'MM'-'dd";

    private const string TimeFormat = "HH':'mm':'ss";

    public static string Decimal(decimal value)
    {
        string text = value.ToString(CultureInfo.InvariantCulture);
        if (text.Contains('.', StringComparison.Ordinal))
        {
            text = text.TrimEnd('0').TrimEnd('.');
        }

        return text == "-0" ? "0" : text;
    }

    public static string Uuid(Guid value)
    {
        return value.ToString("D");
    }

    public static string Date(DateOnly value)
    {
        return value.ToString(DateFormat, CultureInfo.InvariantCulture);
    }

    public static string Time(TimeOnly value)
    {
        return Time(value.Ticks);
    }

    public static string Datetime(DateTimeOffset value)
    {
        DateTime utc = value.UtcDateTime;
        return Date(DateOnly.FromDateTime(utc)) + "T" + Time(utc.TimeOfDay.Ticks) + "Z";
    }

    public static string[] Strings<T>(IEnumerable<T> values, Func<T, string> each)
    {
        return [.. values.Select(each)];
    }

    public static bool TryInteger(object value, out long result)
    {
        switch (value)
        {
            case long number:
                result = number;
                return true;
            case int number:
                result = number;
                return true;
            case short number:
                result = number;
                return true;
            case sbyte number:
                result = number;
                return true;
            case byte number:
                result = number;
                return true;
            case ushort number:
                result = number;
                return true;
            case uint number:
                result = number;
                return true;
            case ulong number when number <= long.MaxValue:
                result = (long)number;
                return true;
            default:
                result = 0;
                return false;
        }
    }

    public static bool TryBoolean(object value, out bool result)
    {
        result = value is true;
        return value is bool;
    }

    public static bool TryString(object value, out string result)
    {
        result = value as string ?? "";
        return value is string;
    }

    public static bool TryDecimal(object value, out decimal result)
    {
        if (value is decimal number)
        {
            result = number;
            return true;
        }

        result = 0;
        return value is string text
            && decimal.TryParse(
                text,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out result
            );
    }

    public static bool TryUuid(object value, out Guid result)
    {
        if (value is Guid uuid)
        {
            result = uuid;
            return true;
        }

        result = default;
        return value is string text && Guid.TryParseExact(text, "D", out result);
    }

    public static bool TryDate(object value, out DateOnly result)
    {
        if (value is DateOnly date)
        {
            result = date;
            return true;
        }

        result = default;
        return value is string text
            && DateOnly.TryParseExact(
                text,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result
            );
    }

    public static bool TryTime(object value, out TimeOnly result)
    {
        if (value is TimeOnly time)
        {
            result = time;
            return true;
        }

        bool parsed = TryTimeTicks(value as string, out long ticks);
        result = parsed ? new TimeOnly(ticks) : default;
        return parsed;
    }

    public static bool TryDatetime(object value, out DateTimeOffset result)
    {
        if (value is DateTimeOffset datetime)
        {
            result = datetime;
            return true;
        }

        result = default;
        if (
            value is string { Length: >= 20 } text
            && (text[10] == 'T')
            && (text[^1] == 'Z')
            && TryDate(text[..10], out DateOnly date)
            && TryTimeTicks(text[11..^1], out long ticks)
        )
        {
            result = new DateTimeOffset(
                date.ToDateTime(TimeOnly.MinValue).Ticks + ticks,
                TimeSpan.Zero
            );
            return true;
        }

        return false;
    }

    /// <summary>The items of a list value: any enumerable but a string.</summary>
    public static bool TryList(object value, out IEnumerable items)
    {
        items = value as IEnumerable ?? Array.Empty<object>();
        return value is IEnumerable and not string;
    }

    private static string Time(long ticks)
    {
        string text = new TimeOnly(ticks - (ticks % TimeSpan.TicksPerSecond)).ToString(
            TimeFormat,
            CultureInfo.InvariantCulture
        );
        long fraction = ticks % TimeSpan.TicksPerSecond;
        return fraction == 0
            ? text
            : text
                + "."
                + fraction.ToString("D7", CultureInfo.InvariantCulture).TrimEnd('0');
    }

    /// <summary><c>HH:mm:ss[.f]</c>, with one to seven fraction digits.</summary>
    private static bool TryTimeTicks(string? text, out long ticks)
    {
        ticks = 0;
        if (
            text is not { Length: >= 8 }
            || (text[2] != ':')
            || (text[5] != ':')
            || !TryDigits(text, 0, 2, out long hours)
            || !TryDigits(text, 3, 2, out long minutes)
            || !TryDigits(text, 6, 2, out long seconds)
            || (hours > 23)
            || (minutes > 59)
            || (seconds > 59)
        )
        {
            return false;
        }

        long fraction = 0;
        if (text.Length > 8)
        {
            int digits = text.Length - 9;
            if (
                (text[8] != '.')
                || (digits is < 1 or > 7)
                || !TryDigits(text, 9, digits, out fraction)
            )
            {
                return false;
            }

            for (int i = digits; i < 7; i++)
            {
                fraction *= 10;
            }
        }

        ticks =
            (((((hours * 60) + minutes) * 60) + seconds) * TimeSpan.TicksPerSecond)
            + fraction;
        return true;
    }

    private static bool TryDigits(string text, int start, int length, out long value)
    {
        value = 0;
        for (int i = start; i < (start + length); i++)
        {
            if (!char.IsAsciiDigit(text[i]))
            {
                return false;
            }

            value = (value * 10) + (text[i] - '0');
        }

        return true;
    }
}
