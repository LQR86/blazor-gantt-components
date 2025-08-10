using GanttComponents.Models;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// WeekDay pattern implementation for TimelineView partial classes.
/// Handles zoom levels: WeekDayOptimal30px through WeekDayOptimal60px (levels 32-35).
/// Primary Header: Week ranges ("Feb 17-23, 2025")
/// Secondary Header: Day numbers ("17", "18", "19")
/// Cell Widths: 30-60px day cells for optimal visual density
/// </summary>
public partial class TimelineView
{
    /// <summary>
    /// Main rendering method for WeekDay pattern.
    /// Combines primary (week ranges) and secondary (day numbers) headers.
    /// </summary>
    /// <returns>Complete SVG markup for WeekDay headers</returns>
    private string RenderWeekDayHeaders()
    {
        try
        {
            Logger.LogDebugInfo($"WeekDay pattern rendering - StartDate: {StartDate}, EndDate: {EndDate}, DayWidth: {DayWidth}");

            var primaryHeader = RenderWeekDayPrimaryHeader();
            var secondaryHeader = RenderWeekDaySecondaryHeader();

            return $@"
                <!-- WeekDay Pattern Headers -->
                <g class=""weekday-headers"">
                    {primaryHeader}
                    {secondaryHeader}
                </g>";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error rendering WeekDay headers: {ex.Message}");
            return $"<!-- Error in WeekDay pattern: {ex.Message} -->";
        }
    }    /// <summary>
         /// Renders the primary header with week ranges.
         /// Shows week boundaries like "Feb 17-23, 2025" for each week in the timeline.
         /// </summary>
         /// <returns>SVG markup for primary header</returns>
    private string RenderWeekDayPrimaryHeader()
    {
        var weekPeriods = GenerateWeekPeriods();
        var headerElements = new List<string>();

        foreach (var period in weekPeriods)
        {
            // Create background rectangle for the week
            var rect = CreateSVGRect(
                period.XPosition,
                0,
                period.Width,
                HeaderMonthHeight,
                GetHeaderCellClass(isPrimary: true)
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
            <!-- Primary Header: Week Ranges -->
            <g class=""weekday-primary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Renders the secondary header with day numbers.
    /// Shows individual day numbers ("17", "18", "19") for each day in the timeline.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    private string RenderWeekDaySecondaryHeader()
    {
        var headerElements = new List<string>();
        var currentDate = StartDate;

        while (currentDate <= EndDate)
        {
            var dayX = DayToSVGX(currentDate);

            // Create background rectangle for the day
            var rect = CreateSVGRect(
                dayX,
                HeaderMonthHeight,
                DayWidth,
                HeaderDayHeight,
                GetHeaderCellClass(isPrimary: false)
            );

            // Create centered text label for the day number
            var textX = dayX + (DayWidth / 2);
            var textY = HeaderMonthHeight + (HeaderDayHeight / 2);
            var dayLabel = currentDate.Day.ToString(); // Simple day number
            var text = CreateSVGText(
                textX,
                textY,
                dayLabel,
                GetHeaderTextClass(isPrimary: false)
            );

            headerElements.Add(rect);
            headerElements.Add(text);

            currentDate = currentDate.AddDays(1);
        }

        return $@"
            <!-- Secondary Header: Day Numbers -->
            <g class=""weekday-secondary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Generates week periods for the current timeline range.
    /// Each period represents one week with start/end dates, width, and formatted label.
    /// Uses Monday as week start (ISO week standard).
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing weeks</returns>
    private List<HeaderPeriod> GenerateWeekPeriods()
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
                    Label = FormatWeekRange(weekStart, weekEnd)
                };

                periods.Add(period);
            }

            // Move to next Monday
            current = current.AddDays(7);
        }

        return periods;
    }

    /// <summary>
    /// Formats a week range for display in the primary header.
    /// Simple format for MVP: "Feb 17-23, 2025"
    /// </summary>
    /// <param name="weekStart">Monday of the week</param>
    /// <param name="weekEnd">Sunday of the week</param>
    /// <returns>Formatted week range string</returns>
    private string FormatWeekRange(DateTime weekStart, DateTime weekEnd)
    {
        try
        {
            // Simple format for MVP - will enhance with I18N later
            if (weekStart.Month == weekEnd.Month && weekStart.Year == weekEnd.Year)
            {
                // Same month: "Feb 17-23, 2025"
                return $"{weekStart:MMM} {weekStart.Day}-{weekEnd.Day}, {weekStart:yyyy}";
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
            Logger.LogError($"Error formatting week range {weekStart:yyyy-MM-dd} to {weekEnd:yyyy-MM-dd}: {ex.Message}");
            return $"{weekStart:MMM d} - {weekEnd:MMM d}, {weekStart:yyyy}";
        }
    }
}
