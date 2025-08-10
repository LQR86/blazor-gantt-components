using GanttComponents.Models;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// WeekDay 40px level implementation for TimelineView partial classes.
/// Handles zoom level: WeekDayOptimal40px (level 33).
/// Primary Header: Week ranges ("Feb 17-23, 2025")
/// Secondary Header: Day numbers ("17", "18", "19")
/// Cell Width: 40px day cells = 280px week cells (40px × 7 days)
/// Optimized for comfortable wide timeline viewing.
/// </summary>
public partial class TimelineView
{
    /// <summary>
    /// Renders WeekDay 40px level headers with week ranges and day numbers.
    /// Optimized for 40px day width with 280px week cells.
    /// </summary>
    /// <returns>Complete SVG markup for WeekDay 40px headers</returns>
    private string RenderWeekDay40pxHeaders()
    {
        try
        {
            Logger.LogDebugInfo($"WeekDay 40px rendering - StartDate: {StartDate}, EndDate: {EndDate}, DayWidth: {DayWidth}");

            var primaryHeader = RenderWeekDay40pxPrimaryHeader();
            var secondaryHeader = RenderWeekDay40pxSecondaryHeader();

            return $@"
                <!-- WeekDay 40px Level Headers -->
                <g class=""weekday-40px-headers"">
                    {primaryHeader}
                    {secondaryHeader}
                </g>";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error rendering WeekDay 40px headers: {ex.Message}");
            return $"<!-- Error in WeekDay 40px level: {ex.Message} -->";
        }
    }

    /// <summary>
    /// Renders the primary header with week ranges for WeekDay 40px level.
    /// Shows week boundaries like "Feb 17-23, 2025" for each week.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    private string RenderWeekDay40pxPrimaryHeader()
    {
        var weekPeriods = GenerateWeekDay40pxWeekPeriods();
        var headerElements = new List<string>();

        foreach (var period in weekPeriods)
        {
            // Create background rectangle for the week
            var rect = CreateSVGRect(
                period.XPosition,
                0,
                period.Width,
                HeaderMonthHeight,
                "svg-weekday-40px-cell-primary"
            );

            // Create centered text label for the week range
            var textX = period.XPosition + (period.Width / 2);
            var textY = HeaderMonthHeight / 2;
            var text = CreateSVGText(
                textX,
                textY,
                period.Label,
                GetHeaderTextClass(isPrimary: true)
            );

            headerElements.Add(rect);
            headerElements.Add(text);
        }

        return $@"
            <!-- Primary Header: Week Ranges (WeekDay 40px) -->
            <g class=""weekday-40px-primary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Renders the secondary header with day numbers for WeekDay 40px level.
    /// Shows individual day numbers ("17", "18", "19") for each day.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    private string RenderWeekDay40pxSecondaryHeader()
    {
        var dayPeriods = GenerateWeekDay40pxDayPeriods();
        var headerElements = new List<string>();

        foreach (var period in dayPeriods)
        {
            // Create background rectangle for the day
            var rect = CreateSVGRect(
                period.XPosition,
                HeaderMonthHeight,
                period.Width,
                HeaderDayHeight,
                "svg-weekday-40px-cell-secondary"
            );

            // Create centered text label for the day number
            var textX = period.XPosition + (period.Width / 2);
            var textY = HeaderMonthHeight + (HeaderDayHeight / 2);
            var text = CreateSVGText(
                textX,
                textY,
                period.Label,
                GetHeaderTextClass(isPrimary: false)
            );

            headerElements.Add(rect);
            headerElements.Add(text);
        }

        return $@"
            <!-- Secondary Header: Day Numbers (WeekDay 40px) -->
            <g class=""weekday-40px-secondary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Generates week periods for WeekDay 40px level.
    /// Each period represents one week with Monday-Sunday range.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing weeks</returns>
    private List<HeaderPeriod> GenerateWeekDay40pxWeekPeriods()
    {
        var periods = new List<HeaderPeriod>();

        // Find the Monday of the week containing StartDate
        var current = StartDate;
        while (current.DayOfWeek != DayOfWeek.Monday)
        {
            current = current.AddDays(-1);
        }

        // Generate week periods until we cover the entire timeline
        while (current <= EndDate)
        {
            var weekStart = current;
            var weekEnd = current.AddDays(6); // Sunday of the same week

            // Calculate the visible portion of this week within our timeline
            var visibleStart = weekStart < StartDate ? StartDate : weekStart;
            var visibleEnd = weekEnd > EndDate ? EndDate : weekEnd;

            // Only add if there's a visible portion
            if (visibleStart <= visibleEnd)
            {
                var period = new HeaderPeriod
                {
                    Start = visibleStart,
                    End = visibleEnd,
                    XPosition = DayToSVGX(visibleStart),
                    Width = (visibleEnd - visibleStart).Days * DayWidth + DayWidth, // +1 day for inclusive end
                    Level = HeaderLevel.Primary,
                    Label = FormatWeekDay40pxWeekRange(weekStart, weekEnd)
                };

                periods.Add(period);
            }

            // Move to next Monday
            current = current.AddDays(7);
        }

        return periods;
    }

    /// <summary>
    /// Generates day periods for WeekDay 40px level secondary header.
    /// Each period represents one day with day number.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing days</returns>
    private List<HeaderPeriod> GenerateWeekDay40pxDayPeriods()
    {
        var periods = new List<HeaderPeriod>();
        var current = StartDate;

        while (current <= EndDate)
        {
            var period = new HeaderPeriod
            {
                Start = current,
                End = current,
                XPosition = DayToSVGX(current),
                Width = DayWidth,
                Level = HeaderLevel.Secondary,
                Label = current.Day.ToString()
            };

            periods.Add(period);
            current = current.AddDays(1);
        }

        return periods;
    }

    /// <summary>
    /// Formats a week range for WeekDay 40px level display.
    /// Enhanced format for 280px week cells with more space.
    /// </summary>
    /// <param name="weekStart">Monday of the week</param>
    /// <param name="weekEnd">Sunday of the week</param>
    /// <returns>Formatted week range string</returns>
    private string FormatWeekDay40pxWeekRange(DateTime weekStart, DateTime weekEnd)
    {
        try
        {
            // Enhanced format for 280px cells (40px day width) - more space available
            if (weekStart.Month == weekEnd.Month && weekStart.Year == weekEnd.Year)
            {
                // Same month: "February 17-23, 2025" (full month name)
                return $"{weekStart:MMMM} {weekStart.Day}-{weekEnd.Day}, {weekStart:yyyy}";
            }
            else if (weekStart.Year == weekEnd.Year)
            {
                // Different months: "Feb 28 - Mar 6, 2025"
                return $"{weekStart:MMM d} - {weekEnd:MMM d}, {weekStart:yyyy}";
            }
            else
            {
                // Different years: "Dec 30, 2024 - Jan 5, 2025"
                return $"{weekStart:MMM d, yyyy} - {weekEnd:MMM d, yyyy}";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error formatting WeekDay 40px week range {weekStart:yyyy-MM-dd} to {weekEnd:yyyy-MM-dd}: {ex.Message}");
            return $"{weekStart:MMM d} - {weekEnd:MMM d}, {weekStart:yyyy}";
        }
    }
}
