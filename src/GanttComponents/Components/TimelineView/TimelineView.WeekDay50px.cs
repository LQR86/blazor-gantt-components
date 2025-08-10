using GanttComponents.Models;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// WeekDay 50px level implementation for TimelineView partial classes.
/// Handles zoom level: WeekDayLuxury50px (level 34).
/// Primary Header: Week ranges ("February 17-23, 2025")
/// Secondary Header: Day names with numbers ("Mon 17", "Tue 18")
/// Cell Width: 50px day cells = 350px week cells (50px × 7 days)
/// Optimized for luxury wide timeline viewing with maximum information.
/// </summary>
public partial class TimelineView
{
    /// <summary>
    /// Renders WeekDay 50px level headers with week ranges and day names+numbers.
    /// Optimized for 50px day width with 350px week cells.
    /// </summary>
    /// <returns>Complete SVG markup for WeekDay 50px headers</returns>
    private string RenderWeekDay50pxHeaders()
    {
        try
        {
            Logger.LogDebugInfo($"WeekDay 50px rendering - StartDate: {StartDate}, EndDate: {EndDate}, DayWidth: {DayWidth}");

            var primaryHeader = RenderWeekDay50pxPrimaryHeader();
            var secondaryHeader = RenderWeekDay50pxSecondaryHeader();

            return $@"
                <!-- WeekDay 50px Level Headers -->
                <g class=""weekday-50px-headers"">
                    {primaryHeader}
                    {secondaryHeader}
                </g>";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error rendering WeekDay 50px headers: {ex.Message}");
            return $"<!-- Error in WeekDay 50px level: {ex.Message} -->";
        }
    }

    /// <summary>
    /// Renders the primary header with week ranges for WeekDay 50px level.
    /// Shows week boundaries like "February 17-23, 2025" for each week.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    private string RenderWeekDay50pxPrimaryHeader()
    {
        var weekPeriods = GenerateWeekDay50pxWeekPeriods();
        var headerElements = new List<string>();

        foreach (var period in weekPeriods)
        {
            // Create background rectangle for the week
            var rect = CreateSVGRect(
                period.XPosition,
                0,
                period.Width,
                HeaderMonthHeight,
                "svg-weekday-50px-cell-primary"
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
            <!-- Primary Header: Week Ranges (WeekDay 50px) -->
            <g class=""weekday-50px-primary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Renders the secondary header with day names and numbers for WeekDay 50px level.
    /// Shows combined day names with numbers ("Mon 17", "Tue 18") for each day.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    private string RenderWeekDay50pxSecondaryHeader()
    {
        var dayPeriods = GenerateWeekDay50pxDayPeriods();
        var headerElements = new List<string>();

        foreach (var period in dayPeriods)
        {
            // Create background rectangle for the day
            var rect = CreateSVGRect(
                period.XPosition,
                HeaderMonthHeight,
                period.Width,
                HeaderDayHeight,
                "svg-weekday-50px-cell-secondary"
            );

            // Create centered text label for the day name and number
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
            <!-- Secondary Header: Day Names + Numbers (WeekDay 50px) -->
            <g class=""weekday-50px-secondary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Generates week periods for WeekDay 50px level.
    /// Each period represents one week with Monday-Sunday range.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing weeks</returns>
    private List<HeaderPeriod> GenerateWeekDay50pxWeekPeriods()
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
                    Label = FormatWeekDay50pxWeekRange(weekStart, weekEnd)
                };

                periods.Add(period);
            }

            // Move to next Monday
            current = current.AddDays(7);
        }

        return periods;
    }

    /// <summary>
    /// Generates day periods for WeekDay 50px level secondary header.
    /// Each period represents one day with day name and number.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing days</returns>
    private List<HeaderPeriod> GenerateWeekDay50pxDayPeriods()
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
                Label = FormatWeekDay50pxDayNameNumber(current)
            };

            periods.Add(period);
            current = current.AddDays(1);
        }

        return periods;
    }

    /// <summary>
    /// Formats a week range for WeekDay 50px level display.
    /// Premium format for 350px week cells with maximum space and full month names.
    /// </summary>
    /// <param name="weekStart">Monday of the week</param>
    /// <param name="weekEnd">Sunday of the week</param>
    /// <returns>Formatted week range string</returns>
    private string FormatWeekDay50pxWeekRange(DateTime weekStart, DateTime weekEnd)
    {
        try
        {
            // Premium format for 350px cells (50px day width) - maximum space available
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
            Logger.LogError($"Error formatting WeekDay 50px week range {weekStart:yyyy-MM-dd} to {weekEnd:yyyy-MM-dd}: {ex.Message}");
            return $"{weekStart:MMM d} - {weekEnd:MMM d}, {weekStart:yyyy}";
        }
    }

    /// <summary>
    /// Formats a day name with number for WeekDay 50px level secondary header.
    /// Enhanced format for 50px day cells with room for both name and number.
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted day name and number string</returns>
    private string FormatWeekDay50pxDayNameNumber(DateTime date)
    {
        try
        {
            // Enhanced format for 50px cells - room for day name and number
            return $"{date:ddd} {date.Day}";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error formatting WeekDay 50px day name+number for {date:yyyy-MM-dd}: {ex.Message}");
            return date.Day.ToString();
        }
    }
}
