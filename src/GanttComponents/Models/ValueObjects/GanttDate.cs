using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GanttComponents.Models.ValueObjects;

/// <summary>
/// Domain value object enforcing UTC Date-only constraint for Gantt scheduling.
/// 
/// DESIGN PRINCIPLE: This is the CHOKE POINT for date constraint enforcement.
/// ALL date values in the Gantt domain MUST flow through this type to ensure:
/// 1. UTC timezone enforcement (no local time ambiguity)
/// 2. Date-only precision (no time components in scheduling)
/// 
/// This value object acts as the last line of defense - any DateTime that makes
/// it into the database MUST be UTC date-only because it passes through here.
/// </summary>
[JsonConverter(typeof(GanttDateJsonConverter))]
[TypeConverter(typeof(GanttDateTypeConverter))]
public readonly struct GanttDate : IComparable<GanttDate>, IEquatable<GanttDate>
{
    private readonly DateOnly _value;

    /// <summary>
    /// Creates a GanttDate from a DateOnly value.
    /// This is the safest constructor as DateOnly guarantees no time component.
    /// </summary>
    public GanttDate(DateOnly date)
    {
        _value = date;
    }

    /// <summary>
    /// Creates a GanttDate from year, month, day components.
    /// Enforces valid date construction at the value object level.
    /// </summary>
    public GanttDate(int year, int month, int day)
    {
        _value = new DateOnly(year, month, day);
    }

    /// <summary>
    /// CHOKE POINT METHOD: Converts any DateTime to GanttDate with UTC date-only enforcement.
    /// This is where we enforce both constraints:
    /// 1. Convert to UTC if not already
    /// 2. Strip time component completely
    /// 3. Return type-safe GanttDate that cannot violate constraints
    /// </summary>
    public static GanttDate FromDateTime(DateTime dateTime)
    {
        // CONSTRAINT ENFORCEMENT 1: Ensure UTC
        var utcDateTime = dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
            _ => throw new ArgumentException($"Unsupported DateTimeKind: {dateTime.Kind}")
        };

        // CONSTRAINT ENFORCEMENT 2: Strip time component
        var dateOnly = DateOnly.FromDateTime(utcDateTime);

        return new GanttDate(dateOnly);
    }

    /// <summary>
    /// CHOKE POINT METHOD: Parse string dates with UTC date-only enforcement.
    /// Expected format: "yyyy-MM-dd" (ISO 8601 date-only)
    /// </summary>
    public static GanttDate Parse(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            throw new ArgumentException("Date string cannot be null or empty", nameof(dateString));

        // Try DateOnly first with invariant culture (preferred format: "yyyy-MM-dd")
        if (DateOnly.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
        {
            return new GanttDate(dateOnly);
        }

        // Try DateTime with invariant culture and convert (strips time automatically)
        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
        {
            return FromDateTime(dateTime);
        }

        // Try specific common formats explicitly
        string[] formats = {
            "yyyy-MM-dd",       // ISO format
            "MM/dd/yyyy",       // US format
            "dd/MM/yyyy",       // European format
            "yyyy/MM/dd",       // Alternative ISO
            "dd-MM-yyyy",       // European with dashes
            "MM-dd-yyyy",       // US with dashes
            "yyyy-MM-ddTHH:mm:ss", // ISO with time
            "yyyy-MM-ddTHH:mm:ssZ", // ISO with UTC
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exactDateTime))
            {
                return FromDateTime(exactDateTime);
            }
        }

        // Final fallback: Try with current culture for any other localized formats
        if (DateTime.TryParse(dateString, out var localDateTime))
        {
            return FromDateTime(localDateTime);
        }

        throw new FormatException($"Unable to parse '{dateString}' as a valid date. Expected formats: yyyy-MM-dd, MM/dd/yyyy, dd/MM/yyyy, or other common date formats");
    }

    /// <summary>
    /// Safe parsing with null handling for external data sources.
    /// </summary>
    public static GanttDate? TryParse(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        try
        {
            return Parse(dateString);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get today's date as GanttDate (UTC).
    /// This replaces DateTime.Today usage throughout the application.
    /// </summary>
    public static GanttDate Today => FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Converts to UTC DateTime at midnight for interop with existing DateTime-based APIs.
    /// The DateTime will have DateTimeKind.Utc and TimeOfDay = 00:00:00.
    /// </summary>
    public DateTime ToUtcDateTime() => _value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

    /// <summary>
    /// Gets the underlying DateOnly value for .NET 6+ APIs.
    /// </summary>
    public DateOnly ToDateOnly() => _value;

    /// <summary>
    /// Standard ISO 8601 date format for JSON serialization and database storage.
    /// Format: "yyyy-MM-dd"
    /// </summary>
    public override string ToString() => _value.ToString("yyyy-MM-dd");

    /// <summary>
    /// Custom format support for UI display.
    /// </summary>
    public string ToString(string format) => _value.ToString(format);

    // Arithmetic operations
    public GanttDate AddDays(int days) => new(_value.AddDays(days));
    public GanttDate AddMonths(int months) => new(_value.AddMonths(months));
    public GanttDate AddYears(int years) => new(_value.AddYears(years));

    public int DaysUntil(GanttDate other) => other._value.DayNumber - _value.DayNumber;

    // Properties for common date operations
    public int Year => _value.Year;
    public int Month => _value.Month;
    public int Day => _value.Day;
    public DayOfWeek DayOfWeek => _value.DayOfWeek;

    // Comparison operations
    public int CompareTo(GanttDate other) => _value.CompareTo(other._value);
    public bool Equals(GanttDate other) => _value.Equals(other._value);
    public override bool Equals(object? obj) => obj is GanttDate other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();

    // Operators
    public static bool operator ==(GanttDate left, GanttDate right) => left.Equals(right);
    public static bool operator !=(GanttDate left, GanttDate right) => !left.Equals(right);
    public static bool operator <(GanttDate left, GanttDate right) => left.CompareTo(right) < 0;
    public static bool operator >(GanttDate left, GanttDate right) => left.CompareTo(right) > 0;
    public static bool operator <=(GanttDate left, GanttDate right) => left.CompareTo(right) <= 0;
    public static bool operator >=(GanttDate left, GanttDate right) => left.CompareTo(right) >= 0;

    // Implicit conversions for ease of use (while maintaining type safety at boundaries)
    public static implicit operator DateOnly(GanttDate ganttDate) => ganttDate._value;
    public static implicit operator GanttDate(DateOnly dateOnly) => new(dateOnly);

    // Explicit conversion from DateTime to force conscious decision
    public static explicit operator GanttDate(DateTime dateTime) => FromDateTime(dateTime);
}

/// <summary>
/// JSON converter for GanttDate that enforces "yyyy-MM-dd" format.
/// This ensures clean API boundaries and prevents time component leakage in JSON.
/// </summary>
public class GanttDateJsonConverter : JsonConverter<GanttDate>
{
    public override GanttDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        return GanttDate.Parse(dateString ?? throw new JsonException("Date value cannot be null"));
    }

    public override void Write(Utf8JsonWriter writer, GanttDate value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString()); // "yyyy-MM-dd"
    }
}

/// <summary>
/// Type converter for GanttDate to support model binding and validation.
/// </summary>
public class GanttDateTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || sourceType == typeof(DateTime) || sourceType == typeof(DateOnly)
               || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return value switch
        {
            string s => GanttDate.Parse(s),
            DateTime dt => GanttDate.FromDateTime(dt),
            DateOnly d => new GanttDate(d),
            _ => base.ConvertFrom(context, culture, value)
        };
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || destinationType == typeof(DateTime) || destinationType == typeof(DateOnly)
               || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is GanttDate ganttDate)
        {
            return destinationType.Name switch
            {
                nameof(String) => ganttDate.ToString(),
                nameof(DateTime) => ganttDate.ToUtcDateTime(),
                nameof(DateOnly) => ganttDate.ToDateOnly(),
                _ => base.ConvertTo(context, culture, value, destinationType)
            };
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
