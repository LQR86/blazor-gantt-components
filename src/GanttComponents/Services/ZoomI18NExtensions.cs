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
                // Year→Quarter pattern levels (optimal cell density)
                TimelineZoomLevel.YearQuarterOptimal30px => i18n.T("ZoomLevel.YearQuarterOptimal30px"),
                TimelineZoomLevel.YearQuarterOptimal40px => i18n.T("ZoomLevel.YearQuarterOptimal40px"),
                TimelineZoomLevel.YearQuarterOptimal50px => i18n.T("ZoomLevel.YearQuarterOptimal50px"),
                TimelineZoomLevel.YearQuarterOptimal70px => i18n.T("ZoomLevel.YearQuarterOptimal70px"),

                // Quarter→Month pattern levels (optimal cell density)
                TimelineZoomLevel.QuarterMonthOptimal30px => i18n.T("ZoomLevel.QuarterMonthOptimal30px"),
                TimelineZoomLevel.QuarterMonthOptimal40px => i18n.T("ZoomLevel.QuarterMonthOptimal40px"),
                TimelineZoomLevel.QuarterMonthOptimal50px => i18n.T("ZoomLevel.QuarterMonthOptimal50px"),
                TimelineZoomLevel.QuarterMonthOptimal60px => i18n.T("ZoomLevel.QuarterMonthOptimal60px"),

                // Month→Week pattern levels (optimal cell density)
                TimelineZoomLevel.MonthWeekOptimal30px => i18n.T("ZoomLevel.MonthWeekOptimal30px"),
                TimelineZoomLevel.MonthWeekOptimal40px => i18n.T("ZoomLevel.MonthWeekOptimal40px"),
                TimelineZoomLevel.MonthWeekOptimal50px => i18n.T("ZoomLevel.MonthWeekOptimal50px"),
                TimelineZoomLevel.MonthWeekOptimal60px => i18n.T("ZoomLevel.MonthWeekOptimal60px"),

                // Week→Day pattern levels (optimal cell density)
                TimelineZoomLevel.WeekDayOptimal30px => i18n.T("ZoomLevel.WeekDayOptimal30px"),
                TimelineZoomLevel.WeekDayOptimal40px => i18n.T("ZoomLevel.WeekDayOptimal40px"),
                TimelineZoomLevel.WeekDayOptimal50px => i18n.T("ZoomLevel.WeekDayOptimal50px"),
                TimelineZoomLevel.WeekDayOptimal60px => i18n.T("ZoomLevel.WeekDayOptimal60px"),

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
                // Year→Quarter pattern levels (optimal cell density)
                TimelineZoomLevel.YearQuarterOptimal30px => i18n.T("ZoomLevel.YearQuarterOptimal30px.Description"),
                TimelineZoomLevel.YearQuarterOptimal40px => i18n.T("ZoomLevel.YearQuarterOptimal40px.Description"),
                TimelineZoomLevel.YearQuarterOptimal50px => i18n.T("ZoomLevel.YearQuarterOptimal50px.Description"),
                TimelineZoomLevel.YearQuarterOptimal70px => i18n.T("ZoomLevel.YearQuarterOptimal70px.Description"),

                // Quarter→Month pattern levels (optimal cell density)
                TimelineZoomLevel.QuarterMonthOptimal30px => i18n.T("ZoomLevel.QuarterMonthOptimal30px.Description"),
                TimelineZoomLevel.QuarterMonthOptimal40px => i18n.T("ZoomLevel.QuarterMonthOptimal40px.Description"),
                TimelineZoomLevel.QuarterMonthOptimal50px => i18n.T("ZoomLevel.QuarterMonthOptimal50px.Description"),
                TimelineZoomLevel.QuarterMonthOptimal60px => i18n.T("ZoomLevel.QuarterMonthOptimal60px.Description"),

                // Month→Week pattern levels (optimal cell density)
                TimelineZoomLevel.MonthWeekOptimal30px => i18n.T("ZoomLevel.MonthWeekOptimal30px.Description"),
                TimelineZoomLevel.MonthWeekOptimal40px => i18n.T("ZoomLevel.MonthWeekOptimal40px.Description"),
                TimelineZoomLevel.MonthWeekOptimal50px => i18n.T("ZoomLevel.MonthWeekOptimal50px.Description"),
                TimelineZoomLevel.MonthWeekOptimal60px => i18n.T("ZoomLevel.MonthWeekOptimal60px.Description"),

                // Week→Day pattern levels (optimal cell density)
                TimelineZoomLevel.WeekDayOptimal30px => i18n.T("ZoomLevel.WeekDayOptimal30px.Description"),
                TimelineZoomLevel.WeekDayOptimal40px => i18n.T("ZoomLevel.WeekDayOptimal40px.Description"),
                TimelineZoomLevel.WeekDayOptimal50px => i18n.T("ZoomLevel.WeekDayOptimal50px.Description"),
                TimelineZoomLevel.WeekDayOptimal60px => i18n.T("ZoomLevel.WeekDayOptimal60px.Description"),

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
