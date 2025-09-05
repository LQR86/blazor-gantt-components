using System;
using GanttComponents.Utilities;

namespace GanttComponents.Components.TimelineView.Renderers
{
    /// <summary>
    /// Centralized boundary calculation utilities for timeline header rendering.
    /// Provides consistent period boundary calculations across all timeline patterns.
    /// Used by template-pure renderers for header alignment and boundary calculations.
    /// </summary>
    public static class BoundaryCalculationHelpers
    {
        // === WEEK BOUNDARIES ===

        /// <summary>
        /// Calculates week boundaries for the given date range.
        /// Expands to include complete weeks (Monday to Sunday) that intersect with the range.
        /// </summary>
        /// <param name="startDate">Range start date</param>
        /// <param name="endDate">Range end date</param>
        /// <returns>Tuple of expanded start (Monday) and end (Sunday) dates</returns>
        public static (DateTime start, DateTime end) GetWeekBoundaries(DateTime startDate, DateTime endDate)
        {
            var expandedStart = GetWeekStart(startDate);
            var expandedEnd = GetWeekEnd(endDate);
            return (expandedStart, expandedEnd);
        }

        /// <summary>
        /// Gets the Monday of the week containing the given date.
        /// </summary>
        /// <param name="date">Date within the week</param>
        /// <returns>Monday of the week</returns>
        public static DateTime GetWeekStart(DateTime date)
        {
            return TimelineDateHelper.GetWeekStart(date);
        }

        /// <summary>
        /// Gets the Sunday of the week containing the given date.
        /// </summary>
        /// <param name="date">Date within the week</param>
        /// <returns>Sunday of the week</returns>
        public static DateTime GetWeekEnd(DateTime date)
        {
            var monday = GetWeekStart(date);
            return monday.AddDays(6); // Sunday
        }

        // === MONTH BOUNDARIES ===

        /// <summary>
        /// Calculates month boundaries for the given date range.
        /// Expands to include complete months (1st to last day) that intersect with the range.
        /// </summary>
        /// <param name="startDate">Range start date</param>
        /// <param name="endDate">Range end date</param>
        /// <returns>Tuple of expanded start (1st of month) and end (last day of month) dates</returns>
        public static (DateTime start, DateTime end) GetMonthBoundaries(DateTime startDate, DateTime endDate)
        {
            var expandedStart = GetMonthStart(startDate);
            var expandedEnd = GetMonthEnd(endDate);
            return (expandedStart, expandedEnd);
        }

        /// <summary>
        /// Gets the first day of the month containing the given date.
        /// </summary>
        /// <param name="date">Date within the month</param>
        /// <returns>First day of the month</returns>
        public static DateTime GetMonthStart(DateTime date)
        {
            return TimelineDateHelper.GetMonthStart(date);
        }

        /// <summary>
        /// Gets the last day of the month containing the given date.
        /// </summary>
        /// <param name="date">Date within the month</param>
        /// <returns>Last day of the month</returns>
        public static DateTime GetMonthEnd(DateTime date)
        {
            return TimelineDateHelper.GetMonthEnd(date);
        }

        // === QUARTER BOUNDARIES ===

        /// <summary>
        /// Calculates quarter boundaries for the given date range.
        /// Expands to include complete quarters (Q1: Jan-Mar, Q2: Apr-Jun, Q3: Jul-Sep, Q4: Oct-Dec) that intersect with the range.
        /// </summary>
        /// <param name="startDate">Range start date</param>
        /// <param name="endDate">Range end date</param>
        /// <returns>Tuple of expanded start (first day of quarter) and end (last day of quarter) dates</returns>
        public static (DateTime start, DateTime end) GetQuarterBoundaries(DateTime startDate, DateTime endDate)
        {
            var expandedStart = GetQuarterStart(startDate);
            var expandedEnd = GetQuarterEnd(endDate);
            return (expandedStart, expandedEnd);
        }

        /// <summary>
        /// Gets the first day of the quarter containing the given date.
        /// Q1: January 1, Q2: April 1, Q3: July 1, Q4: October 1
        /// </summary>
        /// <param name="date">Date within the quarter</param>
        /// <returns>First day of the quarter</returns>
        public static DateTime GetQuarterStart(DateTime date)
        {
            var quarter = (date.Month - 1) / 3 + 1;
            var quarterStartMonth = (quarter - 1) * 3 + 1;
            return new DateTime(date.Year, quarterStartMonth, 1);
        }

        /// <summary>
        /// Gets the last day of the quarter containing the given date.
        /// Q1: March 31, Q2: June 30, Q3: September 30, Q4: December 31
        /// </summary>
        /// <param name="date">Date within the quarter</param>
        /// <returns>Last day of the quarter</returns>
        public static DateTime GetQuarterEnd(DateTime date)
        {
            var quarter = (date.Month - 1) / 3 + 1;
            var quarterStartMonth = (quarter - 1) * 3 + 1;
            var quarterEndMonth = quarterStartMonth + 2;
            return new DateTime(date.Year, quarterEndMonth, DateTime.DaysInMonth(date.Year, quarterEndMonth));
        }

        // === YEAR BOUNDARIES ===

        /// <summary>
        /// Calculates year boundaries for the given date range.
        /// Expands to include complete years (January 1 to December 31) that intersect with the range.
        /// </summary>
        /// <param name="startDate">Range start date</param>
        /// <param name="endDate">Range end date</param>
        /// <returns>Tuple of expanded start (January 1) and end (December 31) dates</returns>
        public static (DateTime start, DateTime end) GetYearBoundaries(DateTime startDate, DateTime endDate)
        {
            var expandedStart = GetYearStart(startDate);
            var expandedEnd = GetYearEnd(endDate);
            return (expandedStart, expandedEnd);
        }

        /// <summary>
        /// Gets the first day of the year containing the given date.
        /// </summary>
        /// <param name="date">Date within the year</param>
        /// <returns>January 1 of the year</returns>
        public static DateTime GetYearStart(DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        /// <summary>
        /// Gets the last day of the year containing the given date.
        /// </summary>
        /// <param name="date">Date within the year</param>
        /// <returns>December 31 of the year</returns>
        public static DateTime GetYearEnd(DateTime date)
        {
            return new DateTime(date.Year, 12, 31);
        }

        // === DAY BOUNDARIES ===

        /// <summary>
        /// Calculates day boundaries for the given date range.
        /// Since day is the minimum granularity, this simply returns the input range.
        /// Provided for completeness and future extensibility.
        /// </summary>
        /// <param name="startDate">Range start date</param>
        /// <param name="endDate">Range end date</param>
        /// <returns>Tuple of input dates (no expansion needed for day boundaries)</returns>
        public static (DateTime start, DateTime end) GetDayBoundaries(DateTime startDate, DateTime endDate)
        {
            return (startDate, endDate);
        }

        // === UTILITY METHODS ===

        /// <summary>
        /// Gets the quarter number (1-4) for the given date.
        /// </summary>
        /// <param name="date">Date to get quarter for</param>
        /// <returns>Quarter number (1, 2, 3, or 4)</returns>
        public static int GetQuarterNumber(DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }

        /// <summary>
        /// Gets a human-readable quarter description for the given date.
        /// </summary>
        /// <param name="date">Date to get quarter description for</param>
        /// <returns>Quarter description (e.g., "Q1 2025", "Q3 2024")</returns>
        public static string GetQuarterDescription(DateTime date)
        {
            var quarter = GetQuarterNumber(date);
            return $"Q{quarter} {date.Year}";
        }

        /// <summary>
        /// Validates that start date is not after end date.
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <exception cref="ArgumentException">Thrown when start date is after end date</exception>
        public static void ValidateDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException($"Start date ({startDate:yyyy-MM-dd}) cannot be after end date ({endDate:yyyy-MM-dd})");
            }
        }
    }
}
