using GanttComponents.Models;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// Shared SVG utility methods for TimelineView partial classes.
/// These are helper methods that don't contain pattern-specific logic.
/// </summary>
public partial class TimelineView
{
    /// <summary>
    /// Converts a date to SVG X coordinate using day-to-pixel calculation.
    /// Used by all pattern implementations for consistent positioning.
    /// </summary>
    /// <param name="date">Date to convert</param>
    /// <returns>SVG X coordinate</returns>
    protected double DayToSVGX(DateTime date) => (date - StartDate).Days * DayWidth;

    /// <summary>
    /// Converts a task index to SVG Y coordinate in the timeline body.
    /// </summary>
    /// <param name="taskIndex">Zero-based task index</param>
    /// <returns>SVG Y coordinate relative to timeline body</returns>
    protected double TaskToSVGY(int taskIndex) => taskIndex * RowHeight;

    /// <summary>
    /// Gets the SVG viewBox string for the entire timeline.
    /// </summary>
    /// <returns>ViewBox string in format "0 0 width height"</returns>
    protected string GetSVGViewBox() => $"0 0 {TotalWidth} {TotalHeight + TotalHeaderHeight}";

    /// <summary>
    /// Total width of the timeline SVG in pixels.
    /// </summary>
    protected double TotalSVGWidth => TotalWidth;

    /// <summary>
    /// Total height of the timeline SVG including headers and body.
    /// </summary>
    protected double TotalSVGHeight => TotalHeaderHeight + TotalHeight;

    /// <summary>
    /// Formats a coordinate value for SVG, ensuring integral positioning.
    /// </summary>
    /// <param name="value">Coordinate value</param>
    /// <returns>Formatted coordinate string</returns>
    protected string FormatSVGCoordinate(double value) => Math.Round(value, 0).ToString();

    /// <summary>
    /// Gets CSS class for header cells based on type and state.
    /// </summary>
    /// <param name="isPrimary">True for primary header cells, false for secondary</param>
    /// <param name="isSelected">True if cell is selected</param>
    /// <returns>CSS class string</returns>
    protected string GetHeaderCellClass(bool isPrimary, bool isSelected = false)
    {
        var baseClass = isPrimary ? "svg-primary-cell" : "svg-secondary-cell";
        return isSelected ? $"{baseClass} svg-cell-selected" : baseClass;
    }

    /// <summary>
    /// Gets CSS class for header text based on current pattern and level.
    /// Each pattern uses its own font sizes optimized for cell widths.
    /// </summary>
    /// <param name="isPrimary">True for primary header text, false for secondary</param>
    /// <returns>CSS class string appropriate for current zoom pattern</returns>
    protected virtual string GetHeaderTextClass(bool isPrimary)
    {
        // MonthWeek patterns use smaller fonts for narrow cells (35-70px)
        if (IsMonthWeekPattern)
        {
            return isPrimary ? "svg-monthweek-primary-text" : "svg-monthweek-secondary-text";
        }

        // WeekDay patterns use standard fonts for wide cells (210-420px)
        if (IsWeekDayPattern)
        {
            return isPrimary ? "svg-weekday-primary-text" : "svg-weekday-secondary-text";
        }

        // Default/fallback for any other patterns
        return isPrimary ? "svg-primary-text" : "svg-secondary-text";
    }

    /// <summary>
    /// Creates an SVG rect element for header cells.
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="cssClass">CSS class for styling</param>
    /// <returns>SVG rect element string</returns>
    protected string CreateSVGRect(double x, double y, double width, double height, string cssClass)
    {
        return $@"<rect x=""{FormatSVGCoordinate(x)}"" y=""{FormatSVGCoordinate(y)}"" 
                       width=""{FormatSVGCoordinate(width)}"" height=""{FormatSVGCoordinate(height)}"" 
                       class=""{cssClass}"" />";
    }

    /// <summary>
    /// Creates an SVG text element for header labels.
    /// </summary>
    /// <param name="x">X coordinate (typically center of cell)</param>
    /// <param name="y">Y coordinate (typically center of cell)</param>
    /// <param name="text">Text content</param>
    /// <param name="cssClass">CSS class for styling</param>
    /// <returns>SVG text element string</returns>
    protected string CreateSVGText(double x, double y, string text, string cssClass)
    {
        return $@"<text x=""{FormatSVGCoordinate(x)}"" y=""{FormatSVGCoordinate(y)}"" 
                       text-anchor=""middle"" dominant-baseline=""middle"" 
                       class=""{cssClass}"">{System.Net.WebUtility.HtmlEncode(text)}</text>";
    }

    /// <summary>
    /// Creates a complete SVG header cell (rect + text).
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Cell width</param>
    /// <param name="height">Cell height</param>
    /// <param name="text">Text content</param>
    /// <param name="isPrimary">True for primary header level</param>
    /// <returns>SVG group with rect and text</returns>
    protected string CreateSVGHeaderCell(double x, double y, double width, double height, string text, bool isPrimary)
    {
        var cellClass = GetHeaderCellClass(isPrimary);
        var textClass = GetHeaderTextClass(isPrimary);
        var centerX = x + (width / 2);
        var centerY = y + (height / 2);

        return $@"<g class=""svg-header-cell-group"">
                    {CreateSVGRect(x, y, width, height, cellClass)}
                    {CreateSVGText(centerX, centerY, text, textClass)}
                  </g>";
    }

    /// <summary>
    /// Calculates the number of days between two dates (inclusive).
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Number of days</returns>
    protected int CalculateDaysBetween(DateTime startDate, DateTime endDate)
    {
        return (endDate.Date - startDate.Date).Days + 1;
    }

    /// <summary>
    /// Gets the start of the week for a given date (Monday-based).
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>Start of week (Monday)</returns>
    protected DateTime GetWeekStart(DateTime date)
    {
        var daysFromMonday = ((int)date.DayOfWeek - 1 + 7) % 7;
        return date.Date.AddDays(-daysFromMonday);
    }

    /// <summary>
    /// Gets the end of the week for a given date (Sunday-based).
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>End of week (Sunday)</returns>
    protected DateTime GetWeekEnd(DateTime date)
    {
        return GetWeekStart(date).AddDays(6);
    }

    /// <summary>
    /// Gets the start of the month for a given date.
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>First day of the month</returns>
    protected DateTime GetMonthStart(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// Gets the end of the month for a given date.
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>Last day of the month</returns>
    protected DateTime GetMonthEnd(DateTime date)
    {
        return GetMonthStart(date).AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// Gets the start of the quarter for a given date.
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>First day of the quarter</returns>
    protected DateTime GetQuarterStart(DateTime date)
    {
        var quarterStartMonth = ((date.Month - 1) / 3) * 3 + 1;
        return new DateTime(date.Year, quarterStartMonth, 1);
    }

    /// <summary>
    /// Gets the end of the quarter for a given date.
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>Last day of the quarter</returns>
    protected DateTime GetQuarterEnd(DateTime date)
    {
        return GetQuarterStart(date).AddMonths(3).AddDays(-1);
    }
}
