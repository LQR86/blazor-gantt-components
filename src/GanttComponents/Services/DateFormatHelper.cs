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

        // Handle special quarter formatting since .NET doesn't have a 'q' format specifier
        if (pattern.Contains("q"))
        {
            return FormatDateWithQuarter(date, pattern);
        }

        // Handle %d pattern (day without leading zero)
        if (pattern.Contains("%d"))
        {
            pattern = pattern.Replace("%d", "%d");  // Keep %d as is, we'll handle it specially
            return HandleCustomDayFormat(date, pattern);
        }

        // Use the correct culture matching the I18N setting instead of system culture
        var culture = GetCultureInfo(_i18n.CurrentCulture);
        return date.ToString(pattern, culture);
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

    /// <summary>
    /// Format a date for zoom-aware timeline headers using specified I18N key.
    /// Supports all timeline header units: quarter, year, decade, etc.
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <param name="formatKey">I18N key for the specific format</param>
    /// <returns>Formatted date string for zoom-appropriate headers</returns>
    public string FormatTimelineHeader(DateTime date, string formatKey)
    {
        return FormatDate(date, formatKey);
    }

    /// <summary>
    /// Format a date for quarter display (e.g., "Q1 2025").
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted quarter string</returns>
    public string FormatQuarter(DateTime date)
    {
        return FormatDate(date, "date.quarter-year");
    }

    /// <summary>
    /// Format a date for short quarter display (e.g., "Q1").
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted short quarter string</returns>
    public string FormatQuarterShort(DateTime date)
    {
        return FormatDate(date, "date.quarter-short");
    }

    /// <summary>
    /// Format a date for year display.
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted year string</returns>
    public string FormatYear(DateTime date)
    {
        return FormatDate(date, "date.year");
    }

    /// <summary>
    /// Format a date for decade display (e.g., "2020-2029").
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted decade string</returns>
    public string FormatDecade(DateTime date)
    {
        // Calculate decade start and end years
        var decadeStart = (date.Year / 10) * 10;
        var decadeEnd = decadeStart + 9;

        // Get the format pattern and culture
        var pattern = _i18n.T("date.decade");
        var culture = GetCultureInfo(_i18n.CurrentCulture);

        // For decade format, we need to substitute the start and end years
        // Pattern is expected to be like "yyyy'-'yyyy" 
        // Use regex to replace the first and second occurrence properly
        var regex = new System.Text.RegularExpressions.Regex("yyyy");
        var firstReplacement = regex.Replace(pattern, decadeStart.ToString("0000"), 1);
        var finalResult = regex.Replace(firstReplacement, decadeEnd.ToString("0000"), 1);

        return finalResult;
    }

    /// <summary>
    /// Format a date for short month display (e.g., "Jan").
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted short month string</returns>
    public string FormatMonthShort(DateTime date)
    {
        return FormatDate(date, "date.month-short");
    }

    /// <summary>
    /// Format a date with quarter support by replacing 'q' with calculated quarter number.
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <param name="pattern">Format pattern containing 'q' for quarter</param>
    /// <returns>Formatted date string with quarter number</returns>
    private string FormatDateWithQuarter(DateTime date, string pattern)
    {
        // Calculate quarter (1-4)
        var quarter = ((date.Month - 1) / 3) + 1;

        // Replace 'q' with the quarter number
        var quarterPattern = pattern.Replace("q", quarter.ToString());

        // Use the correct culture matching the I18N setting
        var culture = GetCultureInfo(_i18n.CurrentCulture);

        try
        {
            return date.ToString(quarterPattern, culture);
        }
        catch (FormatException)
        {
            // If the pattern is still invalid, fall back to a simple replacement approach
            // This handles cases where the pattern might have other invalid elements
            return ReplaceQuarterInText(pattern, quarter, date, culture);
        }
    }

    /// <summary>
    /// Fallback method to replace quarter in text without using DateTime.ToString formatting.
    /// </summary>
    private string ReplaceQuarterInText(string pattern, int quarter, DateTime date, CultureInfo culture)
    {
        var result = pattern.Replace("q", quarter.ToString());

        // Handle common patterns manually
        if (result.Contains("yyyy"))
        {
            result = result.Replace("yyyy", date.Year.ToString());
        }
        if (result.Contains("MM"))
        {
            result = result.Replace("MM", date.Month.ToString("D2"));
        }
        if (result.Contains("dd"))
        {
            result = result.Replace("dd", date.Day.ToString("D2"));
        }

        // Remove single quotes used for literal text in patterns
        result = result.Replace("'", "");

        return result;
    }    /// <summary>
         /// Handle custom day format %d by extracting just the day number.
         /// </summary>
         /// <param name="date">The date to format</param>
         /// <param name="pattern">Format pattern containing '%d'</param>
         /// <returns>Formatted date string with day number</returns>
    private string HandleCustomDayFormat(DateTime date, string pattern)
    {
        // For %d pattern, we want just the day number without leading zero
        var dayNumber = date.Day.ToString();

        // Replace %d with the day number in the pattern
        var result = pattern.Replace("%d", dayNumber);

        // If there are other format elements, process them normally
        if (result != dayNumber && result != pattern)
        {
            var culture = GetCultureInfo(_i18n.CurrentCulture);
            try
            {
                return date.ToString(result, culture);
            }
            catch (FormatException)
            {
                // If format fails, return just the result with substitutions
                return result;
            }
        }

        return result;
    }

    /// <summary>
    /// Get CultureInfo from I18N culture string.
    /// </summary>
    /// <param name="cultureString">Culture string from I18N service (e.g., "en-US", "zh-CN")</param>
    /// <returns>Corresponding CultureInfo object</returns>
    private CultureInfo GetCultureInfo(string cultureString)
    {
        return cultureString switch
        {
            "zh-CN" => new CultureInfo("zh-CN"),
            "en-US" => new CultureInfo("en-US"),
            _ => CultureInfo.InvariantCulture
        };
    }
}
