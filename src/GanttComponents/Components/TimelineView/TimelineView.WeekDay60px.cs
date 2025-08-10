using GanttComponents.Models;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// WeekDay 60px level implementation for TimelineView partial classes.
/// Handles zoom level: WeekDayMaximum60px (level 35).
/// Primary Header: Week ranges ("February 17-23, 2025")
/// Secondary Header: Full day names with numbers ("Monday 17", "Tuesday 18")
/// Cell Width: 60px day cells = 420px week cells (60px × 7 days)
/// Optimized for maximum luxury timeline viewing with full information display.
/// </summary>
public partial class TimelineView
{
    /// <summary>
    /// Renders WeekDay 60px level headers with week ranges and full day names+numbers.
    /// Optimized for 60px day width with 420px week cells.
    /// </summary>
    /// <returns>Complete SVG markup for WeekDay 60px headers</returns>
    private string RenderWeekDay60pxHeaders()
    {
        try
        {
            Logger.LogDebugInfo($"WeekDay 60px rendering - StartDate: {StartDate}, EndDate: {EndDate}, DayWidth: {DayWidth}");

            var primaryHeader = RenderWeekDay60pxPrimaryHeader();
            var secondaryHeader = RenderWeekDay60pxSecondaryHeader();

            return $@"
                <!-- WeekDay 60px Level Headers -->
                <g class=""weekday-60px-headers"">
                    {primaryHeader}
                    {secondaryHeader}
                </g>";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error rendering WeekDay 60px headers: {ex.Message}");
            return $"<!-- Error in WeekDay 60px level: {ex.Message} -->";
        }
    }

    /// <summary>
    /// Renders the primary header with week ranges for WeekDay 60px level.
    /// Shows week boundaries like "February 17-23, 2025" for each week.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    private string RenderWeekDay60pxPrimaryHeader()
    {
        var weekPeriods = GenerateWeekDay60pxWeekPeriods();
        var headerElements = new List<string>();

        foreach (var period in weekPeriods)
        {
            // Create background rectangle for the week
            var rect = CreateSVGRect(
                period.XPosition,
                0,
                period.Width,
                HeaderMonthHeight,
                "svg-weekday-60px-cell-primary"
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
            <!-- Primary Header: Week Ranges (WeekDay 60px) -->
            <g class=""weekday-60px-primary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Renders the secondary header with full day names and numbers for WeekDay 60px level.
    /// Shows complete day names with numbers ("Monday 17", "Tuesday 18") for each day.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    private string RenderWeekDay60pxSecondaryHeader()
    {
        var dayPeriods = GenerateWeekDay60pxDayPeriods();
        var headerElements = new List<string>();

        foreach (var period in dayPeriods)
        {
            // Create background rectangle for the day
            var rect = CreateSVGRect(
                period.XPosition,
                HeaderMonthHeight,
                period.Width,
                HeaderDayHeight,
                "svg-weekday-60px-cell-secondary"
            );

            // Create centered text label for the full day name and number
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
            <!-- Secondary Header: Full Day Names + Numbers (WeekDay 60px) -->
            <g class=""weekday-60px-secondary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Generates week periods for WeekDay 60px level.
    /// Each period represents one week with Monday-Sunday range.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing weeks</returns>
    private List<HeaderPeriod> GenerateWeekDay60pxWeekPeriods()
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
                    Label = FormatWeekDay60pxWeekRange(weekStart, weekEnd)
                };

                periods.Add(period);
            }

            // Move to next Monday
            current = current.AddDays(7);
        }

        return periods;
    }

    /// <summary>
    /// Generates day periods for WeekDay 60px level secondary header.
    /// Each period represents one day with full day name and number.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing days</returns>
    private List<HeaderPeriod> GenerateWeekDay60pxDayPeriods()
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
                Label = FormatWeekDay60pxFullDayNameNumber(current)
            };

            periods.Add(period);
            current = current.AddDays(1);
        }

        return periods;
    }

    /// <summary>
    /// Formats a week range for WeekDay 60px level display.
    /// Ultimate format for 420px week cells with maximum space and full detail.
    /// </summary>
    /// <param name="weekStart">Monday of the week</param>
    /// <param name="weekEnd">Sunday of the week</param>
    /// <returns>Formatted week range string</returns>
    private string FormatWeekDay60pxWeekRange(DateTime weekStart, DateTime weekEnd)
    {
        try
        {
            // Ultimate format for 420px cells (60px day width) - maximum space for full detail
            if (weekStart.Month == weekEnd.Month && weekStart.Year == weekEnd.Year)
            {
                // Same month: "February 17-23, 2025" (full month name)
                return $"{weekStart:MMMM} {weekStart.Day}-{weekEnd.Day}, {weekStart:yyyy}";
            }
            else if (weekStart.Year == weekEnd.Year)
            {
                // Different months: "February 28 - March 6, 2025" (full month names)
                return $"{weekStart:MMMM d} - {weekEnd:MMMM d}, {weekStart:yyyy}";
            }
            else
            {
                // Different years: "December 30, 2024 - January 5, 2025" (full month names)
                return $"{weekStart:MMMM d, yyyy} - {weekEnd:MMMM d, yyyy}";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error formatting WeekDay 60px week range {weekStart:yyyy-MM-dd} to {weekEnd:yyyy-MM-dd}: {ex.Message}");
            return $"{weekStart:MMM d} - {weekEnd:MMM d}, {weekStart:yyyy}";
        }
    }

    /// <summary>
    /// Formats a full day name with number for WeekDay 60px level secondary header.
    /// Maximum format for 60px day cells with full day names and numbers.
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted full day name and number string</returns>
    private string FormatWeekDay60pxFullDayNameNumber(DateTime date)
    {
        try
        {
            // Maximum format for 60px cells - full day name with number
            return $"{date:dddd} {date.Day}";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error formatting WeekDay 60px full day name+number for {date:yyyy-MM-dd}: {ex.Message}");
            return $"{date:ddd} {date.Day}";
        }
    }
}
