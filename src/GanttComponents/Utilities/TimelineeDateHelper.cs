using NodaTime;
using NodaTime.Extensions;

namespace GanttComponents.Utilities;

/// <summary>
/// Robust date calculation utilities for timeline components using NodaTime.
/// Handles all complex date boundary calculations with proper week/month logic.
/// </summary>
public static class TimelineDateHelper
{
    /// <summary>
    /// Gets the Monday of the week containing the given date.
    /// Uses ISO 8601 week definition (Monday = start of week).
    /// </summary>
    /// <param name="date">Date within the week</param>
    /// <returns>Monday of that week</returns>
    public static DateTime GetWeekStart(DateTime date)
    {
        var localDate = LocalDate.FromDateTime(date);
        var monday = localDate.Previous(IsoDayOfWeek.Monday);

        // If the date is already Monday, Previous() goes to previous week
        if (localDate.DayOfWeek == IsoDayOfWeek.Monday)
        {
            monday = localDate;
        }

        return monday.ToDateTimeUnspecified();
    }

    /// <summary>
    /// Gets the Sunday of the week containing the given date.
    /// </summary>
    /// <param name="date">Date within the week</param>
    /// <returns>Sunday of that week</returns>
    public static DateTime GetWeekEnd(DateTime date)
    {
        var monday = GetWeekStart(date);
        return monday.AddDays(6); // Sunday
    }

    /// <summary>
    /// Calculates week boundaries that fully contain the given date range.
    /// Returns Monday of the first week and Sunday of the last week.
    /// </summary>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    /// <returns>Week boundaries (Monday start, Sunday end)</returns>
    public static (DateTime start, DateTime end) GetWeekBoundaries(DateTime startDate, DateTime endDate)
    {
        var weekStart = GetWeekStart(startDate);
        var weekEnd = GetWeekEnd(endDate);
        return (weekStart, weekEnd);
    }

    /// <summary>
    /// Gets the first day of the month containing the given date.
    /// </summary>
    /// <param name="date">Date within the month</param>
    /// <returns>First day of the month</returns>
    public static DateTime GetMonthStart(DateTime date)
    {
        var localDate = LocalDate.FromDateTime(date);
        var monthStart = new LocalDate(localDate.Year, localDate.Month, 1);
        return monthStart.ToDateTimeUnspecified();
    }

    /// <summary>
    /// Gets the last day of the month containing the given date.
    /// </summary>
    /// <param name="date">Date within the month</param>
    /// <returns>Last day of the month</returns>
    public static DateTime GetMonthEnd(DateTime date)
    {
        var localDate = LocalDate.FromDateTime(date);
        var monthEnd = new LocalDate(localDate.Year, localDate.Month, localDate.Calendar.GetDaysInMonth(localDate.Year, localDate.Month));
        return monthEnd.ToDateTimeUnspecified();
    }

    /// <summary>
    /// Calculates month boundaries that fully contain the given date range.
    /// Returns first day of the first month and last day of the last month.
    /// </summary>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    /// <returns>Month boundaries (first day, last day)</returns>
    public static (DateTime start, DateTime end) GetMonthBoundaries(DateTime startDate, DateTime endDate)
    {
        var monthStart = GetMonthStart(startDate);
        var monthEnd = GetMonthEnd(endDate);
        return (monthStart, monthEnd);
    }

    /// <summary>
    /// Gets all Monday dates in the given date range.
    /// Useful for week header rendering.
    /// </summary>
    /// <param name="startDate">Range start (should be a Monday from GetWeekStart)</param>
    /// <param name="endDate">Range end</param>
    /// <returns>Enumerable of Monday dates</returns>
    public static IEnumerable<DateTime> GetMondaysInRange(DateTime startDate, DateTime endDate)
    {
        var current = GetWeekStart(startDate); // Ensure we start on a Monday

        while (current <= endDate)
        {
            yield return current;
            current = current.AddDays(7); // Next Monday
        }
    }

    /// <summary>
    /// Gets all month start dates in the given date range.
    /// Useful for month header rendering.
    /// </summary>
    /// <param name="startDate">Range start</param>
    /// <param name="endDate">Range end</param>
    /// <returns>Enumerable of month start dates</returns>
    public static IEnumerable<DateTime> GetMonthStartsInRange(DateTime startDate, DateTime endDate)
    {
        var current = GetMonthStart(startDate); // Start at first day of first month

        while (current <= endDate)
        {
            yield return current;
            // Move to first day of next month
            var localDate = LocalDate.FromDateTime(current);
            current = localDate.PlusMonths(1).ToDateTimeUnspecified();
        }
    }
}
