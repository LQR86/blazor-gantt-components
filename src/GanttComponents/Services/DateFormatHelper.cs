using System.Globalization;

namespace GanttComponents.Services;

/// <summary>
/// Culture-aware date formatting helper for internationalization support.
/// Provides consistent date display formats across English and Chinese locales.
/// </summary>
public class DateFormatHelper
{
    private readonly IGanttI18N _i18n;

    public DateFormatHelper(IGanttI18N i18n)
    {
        _i18n = i18n ?? throw new ArgumentNullException(nameof(i18n));
    }

    /// <summary>
    /// Format a date using culture-specific patterns from I18N translations.
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <param name="formatKey">I18N key for the date format pattern (default: "date.short-format")</param>
    /// <returns>Formatted date string using current culture</returns>
    public string FormatDate(DateTime date, string formatKey = "date.short-format")
    {
        // Get the format pattern from I18N translations
        var pattern = _i18n.T(formatKey);

        // Use the pattern to format the date with current culture
        return date.ToString(pattern, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Format a date with explicit culture specification.
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <param name="culture">Specific culture to use for formatting</param>
    /// <param name="formatKey">I18N key for the date format pattern</param>
    /// <returns>Formatted date string using specified culture</returns>
    public string FormatDate(DateTime date, CultureInfo culture, string formatKey = "date.short-format")
    {
        var pattern = _i18n.T(formatKey);
        return date.ToString(pattern, culture);
    }

    /// <summary>
    /// Get the current date format pattern for display purposes.
    /// </summary>
    /// <param name="formatKey">I18N key for the date format pattern</param>
    /// <returns>The date format pattern string</returns>
    public string GetDateFormatPattern(string formatKey = "date.short-format")
    {
        return _i18n.T(formatKey);
    }

    /// <summary>
    /// Format a date for timeline headers (month-year display).
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted date string for timeline month headers</returns>
    public string FormatTimelineMonth(DateTime date)
    {
        return FormatDate(date, "date.month-year");
    }

    /// <summary>
    /// Format a date for timeline day headers.
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted date string for timeline day headers</returns>
    public string FormatTimelineDay(DateTime date)
    {
        return FormatDate(date, "date.day-number");
    }
}
