namespace GanttComponents.Models.ValueObjects;

/// <summary>
/// CHOKE POINT 1: Domain value object enforcing UTC Date-only constraint.
/// 
/// This is the PRIMARY choke point for date constraint enforcement.
/// ALL date values in the Gantt domain MUST flow through this type to ensure:
/// 1. UTC timezone enforcement (no local time ambiguity)
/// 2. Date-only precision (no time components in scheduling)
/// </summary>
public readonly struct GanttDate : IEquatable<GanttDate>, IComparable<GanttDate>
{
    private readonly DateOnly _value;

    /// <summary>
    /// Creates a GanttDate from a DateOnly value (safest constructor).
    /// </summary>
    public GanttDate(DateOnly date) => _value = date;

    /// <summary>
    /// Creates a GanttDate from year, month, day components.
    /// </summary>
    public GanttDate(int year, int month, int day) => _value = new(year, month, day);

    /// <summary>
    /// CHOKE POINT METHOD: Converts any DateTime to GanttDate with UTC date-only enforcement.
    /// This enforces both constraints:
    /// 1. Convert to UTC if not already
    /// 2. Strip time component completely
    /// </summary>
    public static GanttDate FromDateTime(DateTime dateTime)
    {
        var utc = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
        return new(DateOnly.FromDateTime(utc));
    }

    /// <summary>
    /// Parse string date (ISO format: "yyyy-MM-dd").
    /// </summary>
    public static GanttDate Parse(string dateString) => new(DateOnly.Parse(dateString));

    /// <summary>
    /// Safe parsing with null handling.
    /// </summary>
    public static GanttDate? TryParse(string? dateString) =>
        DateOnly.TryParse(dateString, out var date) ? new(date) : null;

    /// <summary>
    /// Get today's date as GanttDate (UTC).
    /// </summary>
    public static GanttDate Today => FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Converts to UTC DateTime at midnight for interop.
    /// </summary>
    public DateTime ToUtcDateTime() => _value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

    /// <summary>
    /// Gets the underlying DateOnly value.
    /// </summary>
    public DateOnly ToDateOnly() => _value;

    /// <summary>
    /// Standard ISO 8601 format: "yyyy-MM-dd"
    /// </summary>
    public override string ToString() => _value.ToString("yyyy-MM-dd");

    // Essential comparison operations
    public bool Equals(GanttDate other) => _value.Equals(other._value);
    public int CompareTo(GanttDate other) => _value.CompareTo(other._value);
    public override bool Equals(object? obj) => obj is GanttDate other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();

    // Essential operators
    public static bool operator ==(GanttDate left, GanttDate right) => left.Equals(right);
    public static bool operator !=(GanttDate left, GanttDate right) => !left.Equals(right);

    // Implicit conversions for ease of use
    public static implicit operator DateOnly(GanttDate ganttDate) => ganttDate._value;
    public static implicit operator GanttDate(DateOnly dateOnly) => new(dateOnly);
}
