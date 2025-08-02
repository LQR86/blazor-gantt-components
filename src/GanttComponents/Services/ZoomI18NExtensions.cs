using GanttComponents.Models;

namespace GanttComponents.Services
{
    /// <summary>
    /// Type-safe extension methods for timeline zoom I18N resource keys.
    /// Provides convenient access to zoom level names, descriptions, and UI controls.
    /// </summary>
    public static class ZoomI18NExtensions
    {
        /// <summary>
        /// Gets the localized name for a zoom level.
        /// </summary>
        /// <param name="i18n">The I18N service instance</param>
        /// <param name="zoomLevel">The zoom level</param>
        /// <returns>Localized zoom level name</returns>
        public static string GetZoomLevelName(this IGanttI18N i18n, TimelineZoomLevel zoomLevel)
        {
            return zoomLevel switch
            {
                // WeekDay pattern levels
                TimelineZoomLevel.WeekDay => i18n.T("ZoomLevel.WeekDay"),
                TimelineZoomLevel.WeekDayMedium => i18n.T("ZoomLevel.WeekDayMedium"),
                TimelineZoomLevel.WeekDayLow => i18n.T("ZoomLevel.WeekDayLow"),

                // MonthWeek pattern levels
                TimelineZoomLevel.MonthWeek => i18n.T("ZoomLevel.MonthWeek"),
                TimelineZoomLevel.MonthWeekMedium => i18n.T("ZoomLevel.MonthWeekMedium"),
                TimelineZoomLevel.MonthWeekLow => i18n.T("ZoomLevel.MonthWeekLow"),

                // QuarterMonth pattern levels
                TimelineZoomLevel.QuarterMonth => i18n.T("ZoomLevel.QuarterMonth"),
                TimelineZoomLevel.QuarterMonthMedium => i18n.T("ZoomLevel.QuarterMonthMedium"),
                TimelineZoomLevel.QuarterMonthLow => i18n.T("ZoomLevel.QuarterMonthLow"),

                // YearQuarter pattern levels
                TimelineZoomLevel.YearQuarter => i18n.T("ZoomLevel.YearQuarter"),
                TimelineZoomLevel.YearQuarterMedium => i18n.T("ZoomLevel.YearQuarterMedium"),
                TimelineZoomLevel.YearQuarterLow => i18n.T("ZoomLevel.YearQuarterLow"),
                TimelineZoomLevel.YearQuarterMin => i18n.T("ZoomLevel.YearQuarterMin"),

                _ => zoomLevel.ToString()
            };
        }

        /// <summary>
        /// Gets the localized description for a zoom level.
        /// </summary>
        /// <param name="i18n">The I18N service instance</param>
        /// <param name="zoomLevel">The zoom level</param>
        /// <returns>Localized zoom level description</returns>
        public static string GetZoomLevelDescription(this IGanttI18N i18n, TimelineZoomLevel zoomLevel)
        {
            return zoomLevel switch
            {
                // WeekDay pattern levels
                TimelineZoomLevel.WeekDay => i18n.T("ZoomLevel.WeekDay.Description"),
                TimelineZoomLevel.WeekDayMedium => i18n.T("ZoomLevel.WeekDayMedium.Description"),
                TimelineZoomLevel.WeekDayLow => i18n.T("ZoomLevel.WeekDayLow.Description"),

                // MonthWeek pattern levels
                TimelineZoomLevel.MonthWeek => i18n.T("ZoomLevel.MonthWeek.Description"),
                TimelineZoomLevel.MonthWeekMedium => i18n.T("ZoomLevel.MonthWeekMedium.Description"),
                TimelineZoomLevel.MonthWeekLow => i18n.T("ZoomLevel.MonthWeekLow.Description"),

                // QuarterMonth pattern levels
                TimelineZoomLevel.QuarterMonth => i18n.T("ZoomLevel.QuarterMonth.Description"),
                TimelineZoomLevel.QuarterMonthMedium => i18n.T("ZoomLevel.QuarterMonthMedium.Description"),
                TimelineZoomLevel.QuarterMonthLow => i18n.T("ZoomLevel.QuarterMonthLow.Description"),

                // YearQuarter pattern levels
                TimelineZoomLevel.YearQuarter => i18n.T("ZoomLevel.YearQuarter.Description"),
                TimelineZoomLevel.YearQuarterMedium => i18n.T("ZoomLevel.YearQuarterMedium.Description"),
                TimelineZoomLevel.YearQuarterLow => i18n.T("ZoomLevel.YearQuarterLow.Description"),
                TimelineZoomLevel.YearQuarterMin => i18n.T("ZoomLevel.YearQuarterMin.Description"),

                _ => string.Empty
            };
        }

        /// <summary>
        /// Gets localized text for zoom UI controls.
        /// </summary>
        /// <param name="i18n">The I18N service instance</param>
        /// <param name="controlKey">The control key</param>
        /// <returns>Localized control text</returns>
        public static string GetZoomControlText(this IGanttI18N i18n, string controlKey)
        {
            return controlKey.ToLower() switch
            {
                "zoom-in" => i18n.T("zoom.controls.zoom-in"),
                "zoom-out" => i18n.T("zoom.controls.zoom-out"),
                "fit-viewport" => i18n.T("zoom.controls.fit-viewport"),
                "fit-tasks" => i18n.T("zoom.controls.fit-tasks"),
                "zoom-level" => i18n.T("zoom.controls.zoom-level"),
                "zoom-factor" => i18n.T("zoom.controls.zoom-factor"),
                "hidden-tasks" => i18n.T("zoom.overflow.hidden-tasks"),
                _ => controlKey
            };
        }

        /// <summary>
        /// Gets all available zoom level names with their descriptions.
        /// Useful for building UI dropdowns or tooltips.
        /// </summary>
        /// <param name="i18n">The I18N service instance</param>
        /// <returns>Dictionary of zoom level to (name, description) pairs</returns>
        public static Dictionary<TimelineZoomLevel, (string Name, string Description)> GetAllZoomLevelInfo(this IGanttI18N i18n)
        {
            var result = new Dictionary<TimelineZoomLevel, (string, string)>();

            foreach (TimelineZoomLevel level in Enum.GetValues<TimelineZoomLevel>())
            {
                result[level] = (
                    i18n.GetZoomLevelName(level),
                    i18n.GetZoomLevelDescription(level)
                );
            }

            return result;
        }
    }
}
