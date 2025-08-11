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
                // ABC Composition Architecture - Implemented Levels Only
                TimelineZoomLevel.YearQuarterOptimal70px => i18n.T("ZoomLevel.YearQuarterOptimal70px"),
                TimelineZoomLevel.QuarterMonthOptimal60px => i18n.T("ZoomLevel.QuarterMonthOptimal60px"),
                TimelineZoomLevel.MonthWeekOptimal50px => i18n.T("ZoomLevel.MonthWeekOptimal50px"),
                TimelineZoomLevel.WeekDayOptimal50px => i18n.T("ZoomLevel.WeekDayOptimal50px"),

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
                // ABC Composition Architecture - Implemented Levels Only
                TimelineZoomLevel.YearQuarterOptimal70px => i18n.T("ZoomLevel.YearQuarterOptimal70px.Description"),
                TimelineZoomLevel.QuarterMonthOptimal60px => i18n.T("ZoomLevel.QuarterMonthOptimal60px.Description"),
                TimelineZoomLevel.MonthWeekOptimal50px => i18n.T("ZoomLevel.MonthWeekOptimal50px.Description"),
                TimelineZoomLevel.WeekDayOptimal50px => i18n.T("ZoomLevel.WeekDayOptimal50px.Description"),

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
