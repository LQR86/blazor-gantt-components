using GanttComponents.Models;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// MonthWeek40px pattern implementation for TimelineView.
/// Handles zoom level: MonthWeekOptimal40px (level 29).
/// Primary Header: Month-Year ("February 2025", "March 2025")
/// Secondary Header: Week start dates ("2/17", "2/24", "3/3") - Monday dates
/// Cell Width: 42px week cells (6px day width × 7 days/week)
/// INTEGRAL DAY WIDTHS ONLY - no fractional calculations for better visuals
/// </summary>
public partial class TimelineView
{
    /// <summary>
    /// Main rendering method for MonthWeek40px pattern.
    /// Combines primary (month-year) and secondary (week start dates) headers.
    /// </summary>
    /// <returns>Complete SVG markup for MonthWeek40px headers</returns>
    private string RenderMonthWeek40pxHeaders()
    {
        try
        {
            Logger.LogDebugInfo($"MonthWeek40px pattern rendering - StartDate: {StartDate}, EndDate: {EndDate}, DayWidth: {DayWidth}");

            var primaryHeader = RenderMonthWeek40pxPrimaryHeader();
            var secondaryHeader = RenderMonthWeek40pxSecondaryHeader();

            return $@"
                <!-- MonthWeek40px Pattern Headers -->
                <g class=""monthweek-40px-headers"">
                    {primaryHeader}
                    {secondaryHeader}
                </g>";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error rendering MonthWeek40px headers: {ex.Message}");
            return $"<!-- Error in MonthWeek40px pattern: {ex.Message} -->";
        }
    }

    /// <summary>
    /// Renders the primary header with month-year displays for MonthWeek40px.
    /// Shows month boundaries like "February 2025", "March 2025" for each month in the timeline.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    private string RenderMonthWeek40pxPrimaryHeader()
    {
        var monthPeriods = GenerateMonthWeek40pxMonthPeriods();
        var headerElements = new List<string>();

        foreach (var period in monthPeriods)
        {
            // Create background rectangle for the month
            var rect = CreateSVGRect(
                period.XPosition,
                0,
                period.Width,
                HeaderMonthHeight,
                GetHeaderCellClass(isPrimary: true)
            );

            // Create centered text label for the month-year
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
            <!-- Primary Header: Month-Year (MonthWeek40px) -->
            <g class=""monthweek-40px-primary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Renders the secondary header with week start dates for MonthWeek40px.
    /// Shows Monday dates ("2/17", "2/24", "3/3") for each week start in the timeline.
    /// Uses M/d format for compact display.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    private string RenderMonthWeek40pxSecondaryHeader()
    {
        var weekPeriods = GenerateMonthWeek40pxWeekPeriods();
        var headerElements = new List<string>();

        foreach (var period in weekPeriods)
        {
            // Create background rectangle for the week
            var rect = CreateSVGRect(
                period.XPosition,
                HeaderMonthHeight,
                period.Width,
                HeaderDayHeight,
                GetHeaderCellClass(isPrimary: false)
            );

            // Create centered text label for the week start date
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
            <!-- Secondary Header: Week Start Dates (MonthWeek40px) -->
            <g class=""monthweek-40px-secondary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Generates month periods for MonthWeek40px timeline range.
    /// Each period represents one month with start/end dates, width, and formatted label.
    /// Uses integral day width calculations only.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing months</returns>
    private List<HeaderPeriod> GenerateMonthWeek40pxMonthPeriods()
    {
        var periods = new List<HeaderPeriod>();

        // Start from the first day of the month containing StartDate
        var current = new DateTime(StartDate.Year, StartDate.Month, 1);

        // Generate month periods until we cover the entire timeline
        while (current <= EndDate)
        {
            var monthStart = current;
            var monthEnd = current.AddMonths(1).AddDays(-1); // Last day of the month

            // Calculate the visible portion of this month within our timeline
            var visibleStart = monthStart < StartDate ? StartDate : monthStart;
            var visibleEnd = monthEnd > EndDate ? EndDate : monthEnd;

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
                    Label = FormatMonthWeek40pxMonthYear(monthStart)
                };

                periods.Add(period);
            }

            // Move to first day of next month
            current = current.AddMonths(1);
        }

        return periods;
    }

    /// <summary>
    /// Generates week periods for MonthWeek40px pattern.
    /// Each period represents one week with Monday start date and proper width.
    /// Uses integral day width calculations for week boundaries.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing weeks</returns>
    private List<HeaderPeriod> GenerateMonthWeek40pxWeekPeriods()
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
                    Level = HeaderLevel.Secondary,
                    Label = FormatMonthWeek40pxWeekStartDate(weekStart)
                };

                periods.Add(period);
            }

            // Move to next Monday
            current = current.AddDays(7);
        }

        return periods;
    }

    /// <summary>
    /// Formats a month-year for display in the MonthWeek40px primary header.
    /// Simple format: "February 2025", "March 2025"
    /// </summary>
    /// <param name="date">Date within the month</param>
    /// <returns>Formatted month-year string</returns>
    private string FormatMonthWeek40pxMonthYear(DateTime date)
    {
        try
        {
            // Simple format: "February 2025"
            return date.ToString("MMMM yyyy");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error formatting MonthWeek40px month-year {date:yyyy-MM-dd}: {ex.Message}");
            return $"{date:MMM yyyy}";
        }
    }

    /// <summary>
    /// Formats a week start date for display in the MonthWeek40px secondary header.
    /// Uses M/d format for compact display: "2/17", "2/24", "3/3"
    /// </summary>
    /// <param name="weekStart">Monday date for the week</param>
    /// <returns>Formatted week start date string</returns>
    private string FormatMonthWeek40pxWeekStartDate(DateTime weekStart)
    {
        try
        {
            // Compact format: "2/17", "2/24", "3/3"
            return weekStart.ToString("M/d");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error formatting MonthWeek40px week start date {weekStart:yyyy-MM-dd}: {ex.Message}");
            return $"{weekStart.Month}/{weekStart.Day}";
        }
    }
}
