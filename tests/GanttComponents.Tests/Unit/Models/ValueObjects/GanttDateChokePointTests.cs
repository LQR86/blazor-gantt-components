using Xunit;
using GanttComponents.Models.ValueObjects;
using System;
using System.Text.Json;

namespace GanttComponents.Tests.Unit.Models.ValueObjects;

/// <summary>
/// Comprehensive tests demonstrating the GanttDate choke point design.
/// These tests prove that both UTC and Date-only constraints are enforced
/// regardless of how external code tries to create invalid dates.
/// 
/// DESIGN VALIDATION: These tests demonstrate that GanttDate acts as a
/// foolproof choke point - no matter what invalid input is provided,
/// the output is always a valid UTC date-only value.
/// </summary>
public class GanttDateChokePointTests
{
    #region UTC Constraint Enforcement Tests

    [Fact]
    public void GanttDate_FromLocalDateTime_ForcesUtcConversion()
    {
        // Arrange: Local DateTime that could cause timezone issues
        var localDateTime = new DateTime(2025, 8, 13, 14, 30, 45, DateTimeKind.Local);

        // Act: CHOKE POINT - Force through GanttDate
        var ganttDate = GanttDate.FromDateTime(localDateTime);

        // Assert: Always converts to UTC date-only
        var resultDateTime = ganttDate.ToUtcDateTime();
        Assert.Equal(DateTimeKind.Utc, resultDateTime.Kind);
        Assert.Equal(TimeSpan.Zero, resultDateTime.TimeOfDay);
        Assert.Equal(2025, resultDateTime.Year);
        Assert.Equal(8, resultDateTime.Month);
        Assert.Equal(13, resultDateTime.Day);
    }

    [Fact]
    public void GanttDate_FromUnspecifiedDateTime_AssumeUtc()
    {
        // Arrange: Unspecified DateTime (common from parsing)
        var unspecifiedDateTime = new DateTime(2025, 8, 13, 9, 15, 30, DateTimeKind.Unspecified);

        // Act: CHOKE POINT - Force through GanttDate
        var ganttDate = GanttDate.FromDateTime(unspecifiedDateTime);

        // Assert: Treats as UTC and strips time
        var resultDateTime = ganttDate.ToUtcDateTime();
        Assert.Equal(DateTimeKind.Utc, resultDateTime.Kind);
        Assert.Equal(TimeSpan.Zero, resultDateTime.TimeOfDay);
        Assert.Equal(new DateTime(2025, 8, 13), resultDateTime.Date);
    }

    [Fact]
    public void GanttDate_FromUtcDateTime_PreservesUtcStripTime()
    {
        // Arrange: Already UTC DateTime with time components
        var utcDateTime = new DateTime(2025, 8, 13, 23, 59, 59, DateTimeKind.Utc);

        // Act: CHOKE POINT - Force through GanttDate
        var ganttDate = GanttDate.FromDateTime(utcDateTime);

        // Assert: Maintains UTC, strips time
        var resultDateTime = ganttDate.ToUtcDateTime();
        Assert.Equal(DateTimeKind.Utc, resultDateTime.Kind);
        Assert.Equal(TimeSpan.Zero, resultDateTime.TimeOfDay);
        Assert.Equal(new DateTime(2025, 8, 13), resultDateTime.Date);
    }

    #endregion

    #region Date-Only Constraint Enforcement Tests

    [Fact]
    public void GanttDate_FromDateTimeWithTime_StripsTimeCompletely()
    {
        // Arrange: DateTime with significant time components
        var dateTimeWithTime = new DateTime(2025, 12, 25, 18, 45, 32, 123, DateTimeKind.Utc);

        // Act: CHOKE POINT - Force through GanttDate
        var ganttDate = GanttDate.FromDateTime(dateTimeWithTime);

        // Assert: All time components stripped
        var resultDateTime = ganttDate.ToUtcDateTime();
        Assert.Equal(0, resultDateTime.Hour);
        Assert.Equal(0, resultDateTime.Minute);
        Assert.Equal(0, resultDateTime.Second);
        Assert.Equal(0, resultDateTime.Millisecond);
        Assert.Equal(new DateTime(2025, 12, 25), resultDateTime.Date);
    }

    [Fact]
    public void GanttDate_FromMidnight_RemainsMidnight()
    {
        // Arrange: DateTime exactly at midnight (should be unchanged)
        var midnightDateTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act: CHOKE POINT - Force through GanttDate
        var ganttDate = GanttDate.FromDateTime(midnightDateTime);

        // Assert: Remains exactly the same
        var resultDateTime = ganttDate.ToUtcDateTime();
        Assert.Equal(midnightDateTime, resultDateTime);
        Assert.Equal(TimeSpan.Zero, resultDateTime.TimeOfDay);
    }

    #endregion

    #region Input Parsing Choke Point Tests

    [Theory]
    [InlineData("2025-08-13")]                    // Standard ISO format
    [InlineData("2025-08-13T00:00:00")]          // ISO with midnight time
    [InlineData("2025-08-13T15:30:45")]          // ISO with time (should strip)
    [InlineData("2025-08-13T15:30:45Z")]         // ISO with UTC indicator
    [InlineData("08/13/2025")]                   // US format
    [InlineData("13/08/2025")]                   // European format
    public void GanttDate_ParseVariousFormats_AlwaysProducesUtcDateOnly(string dateString)
    {
        // Act: CHOKE POINT - Parse any string format
        var ganttDate = GanttDate.Parse(dateString);

        // Assert: Always results in UTC date-only
        var resultDateTime = ganttDate.ToUtcDateTime();
        Assert.Equal(DateTimeKind.Utc, resultDateTime.Kind);
        Assert.Equal(TimeSpan.Zero, resultDateTime.TimeOfDay);
        Assert.Equal(2025, resultDateTime.Year);
        Assert.Equal(8, resultDateTime.Month);
        Assert.Equal(13, resultDateTime.Day);
    }

    [Fact]
    public void GanttDate_ParseInvalidString_ThrowsFormatException()
    {
        // Arrange: Invalid date string
        var invalidDateString = "not-a-date";

        // Act & Assert: CHOKE POINT - Rejects invalid input
        Assert.Throws<FormatException>(() => GanttDate.Parse(invalidDateString));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void GanttDate_ParseNullOrEmpty_ThrowsArgumentException(string? invalidInput)
    {
        // Act & Assert: CHOKE POINT - Rejects null/empty input
        Assert.Throws<ArgumentException>(() => GanttDate.Parse(invalidInput!));
    }

    #endregion

    #region JSON Serialization Choke Point Tests

    [Fact]
    public void GanttDate_JsonSerialization_ProducesIsoDateFormat()
    {
        // Arrange: GanttDate with specific date
        var ganttDate = new GanttDate(2025, 8, 13);

        // Act: CHOKE POINT - JSON serialization
        var json = JsonSerializer.Serialize(ganttDate);

        // Assert: Always produces ISO date-only format
        Assert.Equal("\"2025-08-13\"", json);
    }

    [Fact]
    public void GanttDate_JsonDeserialization_AcceptsIsoDateFormat()
    {
        // Arrange: JSON with ISO date format
        var json = "\"2025-08-13\"";

        // Act: CHOKE POINT - JSON deserialization
        var ganttDate = JsonSerializer.Deserialize<GanttDate>(json);

        // Assert: Creates valid GanttDate
        Assert.Equal(2025, ganttDate.Year);
        Assert.Equal(8, ganttDate.Month);
        Assert.Equal(13, ganttDate.Day);
    }

    [Fact]
    public void GanttDate_JsonRoundTrip_MaintainsExactValue()
    {
        // Arrange: Original GanttDate
        var original = new GanttDate(2025, 12, 31);

        // Act: CHOKE POINT - Full serialization round trip
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<GanttDate>(json);

        // Assert: Exact value preservation
        Assert.Equal(original, deserialized);
        Assert.Equal(original.ToString(), deserialized.ToString());
    }

    #endregion

    #region Type Safety Choke Point Tests

    [Fact]
    public void GanttDate_CannotAccidentallyCreateInvalidDate()
    {
        // This test demonstrates that it's impossible to create an invalid GanttDate
        // because all constructors and factory methods enforce constraints

        // Valid construction methods (all safe)
        var fromDateOnly = new GanttDate(new DateOnly(2025, 8, 13));
        var fromComponents = new GanttDate(2025, 8, 13);
        var fromDateTime = GanttDate.FromDateTime(DateTime.Now);
        var fromString = GanttDate.Parse("2025-08-13");
        var today = GanttDate.Today;

        // All should produce valid UTC date-only values
        Assert.Equal(DateTimeKind.Utc, fromDateOnly.ToUtcDateTime().Kind);
        Assert.Equal(DateTimeKind.Utc, fromComponents.ToUtcDateTime().Kind);
        Assert.Equal(DateTimeKind.Utc, fromDateTime.ToUtcDateTime().Kind);
        Assert.Equal(DateTimeKind.Utc, fromString.ToUtcDateTime().Kind);
        Assert.Equal(DateTimeKind.Utc, today.ToUtcDateTime().Kind);

        // All should have zero time components
        Assert.Equal(TimeSpan.Zero, fromDateOnly.ToUtcDateTime().TimeOfDay);
        Assert.Equal(TimeSpan.Zero, fromComponents.ToUtcDateTime().TimeOfDay);
        Assert.Equal(TimeSpan.Zero, fromDateTime.ToUtcDateTime().TimeOfDay);
        Assert.Equal(TimeSpan.Zero, fromString.ToUtcDateTime().TimeOfDay);
        Assert.Equal(TimeSpan.Zero, today.ToUtcDateTime().TimeOfDay);
    }

    [Fact]
    public void GanttDate_ExplicitConversionFromDateTime_RequiresConsciousal()
    {
        // Arrange: DateTime with time components
        var dateTimeWithTime = new DateTime(2025, 8, 13, 15, 30, 45);

        // Act: CHOKE POINT - Explicit conversion required (cannot be accidental)
        var ganttDate = (GanttDate)dateTimeWithTime;

        // Assert: Conversion enforces constraints
        var result = ganttDate.ToUtcDateTime();
        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(TimeSpan.Zero, result.TimeOfDay);
    }

    #endregion

    #region Edge Case Choke Point Tests

    [Fact]
    public void GanttDate_TimezoneTransition_HandledCorrectly()
    {
        // Arrange: DateTime during daylight saving time transition
        var dstDateTime = new DateTime(2025, 3, 9, 2, 30, 0, DateTimeKind.Local); // Spring forward

        // Act: CHOKE POINT - Handle timezone edge case
        var ganttDate = GanttDate.FromDateTime(dstDateTime);

        // Assert: Converted to stable UTC date
        var result = ganttDate.ToUtcDateTime();
        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(TimeSpan.Zero, result.TimeOfDay);
        // Date should be preserved (exact date depends on local timezone, but should be stable)
        Assert.True(result.Year == 2025);
        Assert.True(result.Month >= 3);
    }

    [Fact]
    public void GanttDate_MaxAndMinValues_HandleCorrectly()
    {
        // Arrange: Edge case dates
        var minDate = DateTime.MinValue;
        var maxDate = DateTime.MaxValue;

        // Act: CHOKE POINT - Handle extreme values
        var minGanttDate = GanttDate.FromDateTime(minDate);
        var maxGanttDate = GanttDate.FromDateTime(maxDate);

        // Assert: Extreme values handled safely
        Assert.Equal(TimeSpan.Zero, minGanttDate.ToUtcDateTime().TimeOfDay);
        Assert.Equal(TimeSpan.Zero, maxGanttDate.ToUtcDateTime().TimeOfDay);
        Assert.Equal(DateTimeKind.Utc, minGanttDate.ToUtcDateTime().Kind);
        Assert.Equal(DateTimeKind.Utc, maxGanttDate.ToUtcDateTime().Kind);
    }

    #endregion

    #region Today Helper Choke Point Test

    [Fact]
    public void GanttDate_Today_AlwaysUtcDateOnly()
    {
        // Act: CHOKE POINT - Get today's date
        var today = GanttDate.Today;

        // Assert: Always UTC date-only, never local time
        var todayDateTime = today.ToUtcDateTime();
        Assert.Equal(DateTimeKind.Utc, todayDateTime.Kind);
        Assert.Equal(TimeSpan.Zero, todayDateTime.TimeOfDay);

        // Should be reasonably close to actual UTC date
        var utcNow = DateTime.UtcNow.Date;
        var daysDifference = Math.Abs((todayDateTime.Date - utcNow).TotalDays);
        Assert.True(daysDifference <= 1, "GanttDate.Today should be within 1 day of actual UTC date");
    }

    #endregion

    #region Arithmetic Operations Preserve Constraints

    [Fact]
    public void GanttDate_ArithmeticOperations_PreserveConstraints()
    {
        // Arrange: Base GanttDate
        var baseDate = new GanttDate(2025, 8, 13);

        // Act: CHOKE POINT - Arithmetic operations
        var plusDays = baseDate.AddDays(10);
        var plusMonths = baseDate.AddMonths(3);
        var plusYears = baseDate.AddYears(1);

        // Assert: All results maintain UTC date-only constraints
        Assert.Equal(TimeSpan.Zero, plusDays.ToUtcDateTime().TimeOfDay);
        Assert.Equal(TimeSpan.Zero, plusMonths.ToUtcDateTime().TimeOfDay);
        Assert.Equal(TimeSpan.Zero, plusYears.ToUtcDateTime().TimeOfDay);

        Assert.Equal(DateTimeKind.Utc, plusDays.ToUtcDateTime().Kind);
        Assert.Equal(DateTimeKind.Utc, plusMonths.ToUtcDateTime().Kind);
        Assert.Equal(DateTimeKind.Utc, plusYears.ToUtcDateTime().Kind);
    }

    #endregion
}
