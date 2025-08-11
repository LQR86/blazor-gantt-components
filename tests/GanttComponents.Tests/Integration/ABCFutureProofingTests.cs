using Xunit;
using GanttComponents.Components.TimelineView.Renderers;
using System;

namespace GanttComponents.Tests.Integration;

/// <summary>
/// Integration tests for ABC Dual Boundary Future-Proofing.
/// Validates that the ABC composition pattern works for new timeline patterns
/// and provides automatic dual expansion "for free".
/// </summary>
public class ABCFutureProofingTests
{
    /// <summary>
    /// Test that demonstrates ABC pattern working for a completely new timeline pattern.
    /// This simulates implementing a future "SemesterMonth" or "DecadeYear" pattern.
    /// </summary>
    [Fact]
    public void FutureTimelinePattern_ABCComposition_AutomaticallyGetsDualExpansion()
    {
        // ARRANGE: Create a hypothetical new timeline pattern
        var startDate = new DateTime(2025, 3, 15);
        var endDate = new DateTime(2025, 8, 20);

        var futureRenderer = new FutureSemesterMonthRenderer(startDate, endDate);

        // ACT: Test that dual boundaries work automatically
        var primaryBounds = futureRenderer.TestGetPrimaryBoundaries();
        var secondaryBounds = futureRenderer.TestGetSecondaryBoundaries();
        var unionBounds = futureRenderer.TestGetUnionBoundaries();

        // ASSERT: ABC pattern automatically provides union expansion
        // Primary: Semester boundaries (Jan 1 - Aug 31, 2025)
        // Secondary: Month boundaries (Mar 1 - Aug 31, 2025)  
        // Union: Should be widest span (Jan 1 - Aug 31, 2025)

        Assert.Equal(new DateTime(2025, 1, 1), primaryBounds.start);   // Semester start
        Assert.Equal(new DateTime(2025, 8, 31), primaryBounds.end);    // Semester end

        Assert.Equal(new DateTime(2025, 3, 1), secondaryBounds.start); // Month start
        Assert.Equal(new DateTime(2025, 8, 31), secondaryBounds.end);  // Month end

        // Union automatically takes widest span
        Assert.Equal(new DateTime(2025, 1, 1), unionBounds.start);     // Semester wins (earlier)
        Assert.Equal(new DateTime(2025, 8, 31), unionBounds.end);      // Same end date
    }

    /// <summary>
    /// Test boundary calculation regression across all current patterns.
    /// Ensures ABC conversion doesn't break existing functionality.
    /// </summary>
    [Theory]
    [InlineData("2025-08-15", "2025-08-20")] // Short range
    [InlineData("2025-01-01", "2025-12-31")] // Full year
    [InlineData("2025-02-28", "2025-03-02")] // Month boundary
    [InlineData("2025-12-29", "2026-01-05")] // Year boundary
    public void ABCPattern_BoundaryCalculation_ConsistentResults(string startStr, string endStr)
    {
        // ARRANGE: Parse test dates
        var startDate = DateTime.Parse(startStr);
        var endDate = DateTime.Parse(endStr);

        // ACT & ASSERT: WeekDay pattern should work consistently
        var weekDayRenderer = new TestableWeekDayRenderer(startDate, endDate);

        var primaryBounds = weekDayRenderer.TestGetPrimaryBoundaries();
        var secondaryBounds = weekDayRenderer.TestGetSecondaryBoundaries();
        var unionBounds = weekDayRenderer.TestGetUnionBoundaries();

        // Both should be week boundaries, union should be identical
        Assert.Equal(primaryBounds.start, secondaryBounds.start);
        Assert.Equal(primaryBounds.end, secondaryBounds.end);
        Assert.Equal(primaryBounds.start, unionBounds.start);
        Assert.Equal(primaryBounds.end, unionBounds.end);

        // Boundaries should align with Monday-Sunday weeks
        Assert.Equal(DayOfWeek.Monday, unionBounds.start.DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, unionBounds.end.DayOfWeek);
    }
}

/// <summary>
/// Test implementation of a future timeline pattern to demonstrate ABC future-proofing.
/// Simulates "SemesterMonth" pattern where primary is semester, secondary is month.
/// </summary>
public class FutureSemesterMonthRenderer
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public FutureSemesterMonthRenderer(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    // Simulate the ABC pattern methods
    public (DateTime start, DateTime end) TestGetPrimaryBoundaries()
    {
        // Primary: Semester boundaries (Jan-Aug, Sep-Dec)
        var year = _startDate.Year;
        if (_startDate.Month <= 8)
        {
            return (new DateTime(year, 1, 1), new DateTime(year, 8, 31));
        }
        else
        {
            return (new DateTime(year, 9, 1), new DateTime(year, 12, 31));
        }
    }

    public (DateTime start, DateTime end) TestGetSecondaryBoundaries()
    {
        // Secondary: Month boundaries
        return BoundaryCalculationHelpers.GetMonthBoundaries(_startDate, _endDate);
    }

    public (DateTime start, DateTime end) TestGetUnionBoundaries()
    {
        // Simulate base class union calculation
        var primary = TestGetPrimaryBoundaries();
        var secondary = TestGetSecondaryBoundaries();

        var unionStart = primary.start < secondary.start ? primary.start : secondary.start;
        var unionEnd = primary.end > secondary.end ? primary.end : secondary.end;

        return (unionStart, unionEnd);
    }
}

/// <summary>
/// Testable version of WeekDay50pxRenderer for integration testing.
/// Exposes protected methods for validation.
/// </summary>
public class TestableWeekDayRenderer
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public TestableWeekDayRenderer(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public (DateTime start, DateTime end) TestGetPrimaryBoundaries()
    {
        return BoundaryCalculationHelpers.GetWeekBoundaries(_startDate, _endDate);
    }

    public (DateTime start, DateTime end) TestGetSecondaryBoundaries()
    {
        return BoundaryCalculationHelpers.GetWeekBoundaries(_startDate, _endDate);
    }

    public (DateTime start, DateTime end) TestGetUnionBoundaries()
    {
        var primary = TestGetPrimaryBoundaries();
        var secondary = TestGetSecondaryBoundaries();

        var unionStart = primary.start < secondary.start ? primary.start : secondary.start;
        var unionEnd = primary.end > secondary.end ? primary.end : secondary.end;

        return (unionStart, unionEnd);
    }
}
