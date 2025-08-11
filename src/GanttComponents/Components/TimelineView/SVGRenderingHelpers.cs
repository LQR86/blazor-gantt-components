using GanttComponents.Models;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// Static helper class for SVG rendering utilities shared across timeline renderers.
/// Extracted from TimelineView.SVGRendering.cs to support composition architecture.
/// </summary>
public static class SVGRenderingHelpers
{
    // === COORDINATE CONVERSION ===

    /// <summary>
    /// Converts a date to SVG X coordinate using day-to-pixel calculation.
    /// Used by all renderer implementations for consistent positioning.
    /// </summary>
    /// <param name="date">Date to convert</param>
    /// <param name="startDate">Timeline start date</param>
    /// <param name="dayWidth">Width of each day in pixels</param>
    /// <returns>SVG X coordinate</returns>
    public static double DayToSVGX(DateTime date, DateTime startDate, double dayWidth)
    {
        return (date - startDate).Days * dayWidth;
    }

    /// <summary>
    /// Converts a task index to SVG Y coordinate in the timeline body.
    /// </summary>
    /// <param name="taskIndex">Zero-based task index</param>
    /// <param name="rowHeight">Height of each task row</param>
    /// <returns>SVG Y coordinate relative to timeline body</returns>
    public static double TaskToSVGY(int taskIndex, int rowHeight)
    {
        return taskIndex * rowHeight;
    }

    // === SVG ELEMENT CREATION ===

    /// <summary>
    /// Creates an SVG rectangle element with specified properties.
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="cssClass">CSS class name</param>
    /// <returns>SVG rect element</returns>
    public static string CreateSVGRect(double x, double y, double width, double height, string cssClass)
    {
        // Add inline styles as fallback for CSS class issues
        var inlineStyle = cssClass.Contains("primary")
            ? "fill: #f8f9fa; stroke: #dee2e6; stroke-width: 1px;"
            : "fill: #ffffff; stroke: #dee2e6; stroke-width: 1px;";

        return $@"<rect x=""{FormatSVGCoordinate(x)}"" y=""{FormatSVGCoordinate(y)}"" 
                       width=""{FormatSVGCoordinate(width)}"" height=""{FormatSVGCoordinate(height)}"" 
                       class=""{cssClass}"" style=""{inlineStyle}"" />";
    }

    /// <summary>
    /// Creates an SVG text element with specified properties.
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="text">Text content</param>
    /// <param name="cssClass">CSS class name</param>
    /// <returns>SVG text element</returns>
    public static string CreateSVGText(double x, double y, string text, string cssClass)
    {
        // Add inline styles as fallback for CSS class issues
        var inlineStyle = "fill: #333; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;";

        return $@"<text x=""{FormatSVGCoordinate(x)}"" y=""{FormatSVGCoordinate(y)}"" 
                       text-anchor=""middle"" dominant-baseline=""middle"" 
                       class=""{cssClass}"" style=""{inlineStyle}"">{System.Net.WebUtility.HtmlEncode(text)}</text>";
    }

    /// <summary>
    /// Creates a complete SVG header cell (rect + text).
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Cell width</param>
    /// <param name="height">Cell height</param>
    /// <param name="text">Text content</param>
    /// <param name="cellClass">CSS class for the cell rectangle</param>
    /// <param name="textClass">CSS class for the text element</param>
    /// <returns>SVG group with rect and text</returns>
    public static string CreateSVGHeaderCell(double x, double y, double width, double height,
        string text, string cellClass, string textClass)
    {
        var centerX = x + (width / 2);
        var centerY = y + (height / 2);

        return $@"<g class=""svg-header-cell-group"">
                    {CreateSVGRect(x, y, width, height, cellClass)}
                    {CreateSVGText(centerX, centerY, text, textClass)}
                  </g>";
    }

    // === COORDINATE FORMATTING ===

    /// <summary>
    /// Formats a coordinate value for SVG, ensuring integral positioning.
    /// </summary>
    /// <param name="value">Coordinate value</param>
    /// <returns>Formatted coordinate string</returns>
    public static string FormatSVGCoordinate(double value)
    {
        return Math.Round(value, 0).ToString();
    }

    // === CSS CLASS UTILITIES ===

    /// <summary>
    /// Gets CSS class for header cells based on type and state.
    /// </summary>
    /// <param name="isPrimary">True for primary header cells, false for secondary</param>
    /// <param name="isSelected">True if cell is selected</param>
    /// <returns>CSS class string</returns>
    public static string GetHeaderCellClass(bool isPrimary, bool isSelected = false)
    {
        var baseClass = isPrimary ? "svg-header-cell-primary" : "svg-header-cell-secondary";
        return isSelected ? $"{baseClass} svg-cell-selected" : baseClass;
    }

    /// <summary>
    /// Gets CSS class for header text based on zoom level and position.
    /// Each level gets its own optimized font sizes for maximum readability.
    /// </summary>
    /// <param name="zoomLevel">Current zoom level</param>
    /// <param name="isPrimary">True for primary header text, false for secondary</param>
    /// <returns>Level-specific CSS class string</returns>
    public static string GetHeaderTextClass(TimelineZoomLevel zoomLevel, bool isPrimary)
    {
        // ABC composition level-specific CSS classes for perfect alignment
        var cssClass = zoomLevel switch
        {
            // WeekDay50px - ABC implementation
            TimelineZoomLevel.WeekDayOptimal50px => isPrimary ? "svg-weekday-50px-primary-text" : "svg-weekday-50px-secondary-text",

            // MonthWeek50px - ABC implementation
            TimelineZoomLevel.MonthWeekOptimal50px => isPrimary ? "svg-monthweek-50px-primary-text" : "svg-monthweek-50px-secondary-text",

            // QuarterMonth60px - ABC implementation
            TimelineZoomLevel.QuarterMonthOptimal60px => isPrimary ? "svg-quartermonth-60px-primary-text" : "svg-quartermonth-60px-secondary-text",

            // YearQuarter70px - ABC implementation
            TimelineZoomLevel.YearQuarterOptimal70px => isPrimary ? "svg-yearquarter-70px-primary-text" : "svg-yearquarter-70px-secondary-text",

            // Fallback for any future patterns
            _ => isPrimary ? "svg-primary-text" : "svg-secondary-text"
        };

        return cssClass;
    }

    // === VIEWBOX UTILITIES ===

    /// <summary>
    /// Gets the SVG viewBox string for the entire timeline.
    /// </summary>
    /// <param name="totalWidth">Total timeline width</param>
    /// <param name="totalHeight">Total timeline height</param>
    /// <param name="headerHeight">Total header height</param>
    /// <returns>ViewBox string in format "0 0 width height"</returns>
    public static string GetSVGViewBox(int totalWidth, int totalHeight, int headerHeight)
    {
        return $"0 0 {totalWidth} {totalHeight + headerHeight}";
    }

    /// <summary>
    /// Gets the SVG viewBox string for the header only.
    /// </summary>
    /// <param name="totalWidth">Total timeline width</param>
    /// <param name="headerHeight">Total header height</param>
    /// <returns>ViewBox string for header</returns>
    public static string GetHeaderViewBox(int totalWidth, int headerHeight)
    {
        return $"0 0 {totalWidth} {headerHeight}";
    }

    /// <summary>
    /// Gets the SVG viewBox string for the body content only.
    /// </summary>
    /// <param name="totalWidth">Total timeline width</param>
    /// <param name="totalHeight">Total timeline height</param>
    /// <returns>ViewBox string for body</returns>
    public static string GetBodyViewBox(int totalWidth, int totalHeight)
    {
        return $"0 0 {totalWidth} {totalHeight}";
    }

    // === DATE UTILITIES ===

    /// <summary>
    /// Calculates the number of days between two dates (inclusive).
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Number of days</returns>
    public static int CalculateDaysBetween(DateTime startDate, DateTime endDate)
    {
        return (endDate.Date - startDate.Date).Days + 1;
    }

    /// <summary>
    /// Gets the start of the week for a given date (Monday-based).
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>Start of week (Monday)</returns>
    public static DateTime GetWeekStart(DateTime date)
    {
        var daysFromMonday = ((int)date.DayOfWeek - 1 + 7) % 7;
        return date.Date.AddDays(-daysFromMonday);
    }

    /// <summary>
    /// Gets the end of the week for a given date (Sunday-based).
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>End of week (Sunday)</returns>
    public static DateTime GetWeekEnd(DateTime date)
    {
        return GetWeekStart(date).AddDays(6);
    }

    /// <summary>
    /// Gets the start of the month for a given date.
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>First day of the month</returns>
    public static DateTime GetMonthStart(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// Gets the end of the month for a given date.
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>Last day of the month</returns>
    public static DateTime GetMonthEnd(DateTime date)
    {
        return GetMonthStart(date).AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// Gets the start of the quarter for a given date.
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>First day of the quarter</returns>
    public static DateTime GetQuarterStart(DateTime date)
    {
        var quarterStartMonth = ((date.Month - 1) / 3) * 3 + 1;
        return new DateTime(date.Year, quarterStartMonth, 1);
    }

    /// <summary>
    /// Gets the end of the quarter for a given date.
    /// </summary>
    /// <param name="date">Input date</param>
    /// <returns>Last day of the quarter</returns>
    public static DateTime GetQuarterEnd(DateTime date)
    {
        return GetQuarterStart(date).AddMonths(3).AddDays(-1);
    }
}
