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

                // Legacy levels (backward compatibility) - Suppressing obsolete warnings for compatibility
#pragma warning disable CS0618
                TimelineZoomLevel.WeekDay97px => i18n.T("ZoomLevel.WeekDay97px"),
                TimelineZoomLevel.WeekDay68px => i18n.T("ZoomLevel.WeekDay68px"),
                TimelineZoomLevel.MonthDay48px => i18n.T("ZoomLevel.MonthDay48px"),
                TimelineZoomLevel.MonthDay34px => i18n.T("ZoomLevel.MonthDay34px"),
                TimelineZoomLevel.QuarterMonth24px => i18n.T("ZoomLevel.QuarterMonth24px"),
                TimelineZoomLevel.QuarterMonth17px => i18n.T("ZoomLevel.QuarterMonth17px"),
                TimelineZoomLevel.Month12px => i18n.T("ZoomLevel.Month12px"),
                TimelineZoomLevel.Month8px => i18n.T("ZoomLevel.Month8px"),
                TimelineZoomLevel.YearQuarter6px => i18n.T("ZoomLevel.YearQuarter6px"),
                TimelineZoomLevel.YearQuarter4px => i18n.T("ZoomLevel.YearQuarter4px"),
                TimelineZoomLevel.YearQuarter3px => i18n.T("ZoomLevel.YearQuarter3px"),
#pragma warning restore CS0618

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

                // Legacy levels (backward compatibility) - Suppressing obsolete warnings for compatibility
#pragma warning disable CS0618
                TimelineZoomLevel.WeekDay97px => i18n.T("ZoomLevel.WeekDay97px.Description"),
                TimelineZoomLevel.WeekDay68px => i18n.T("ZoomLevel.WeekDay68px.Description"),
                TimelineZoomLevel.MonthDay48px => i18n.T("ZoomLevel.MonthDay48px.Description"),
                TimelineZoomLevel.MonthDay34px => i18n.T("ZoomLevel.MonthDay34px.Description"),
                TimelineZoomLevel.QuarterMonth24px => i18n.T("ZoomLevel.QuarterMonth24px.Description"),
                TimelineZoomLevel.QuarterMonth17px => i18n.T("ZoomLevel.QuarterMonth17px.Description"),
                TimelineZoomLevel.Month12px => i18n.T("ZoomLevel.Month12px.Description"),
                TimelineZoomLevel.Month8px => i18n.T("ZoomLevel.Month8px.Description"),
                TimelineZoomLevel.YearQuarter6px => i18n.T("ZoomLevel.YearQuarter6px.Description"),
                TimelineZoomLevel.YearQuarter4px => i18n.T("ZoomLevel.YearQuarter4px.Description"),
                TimelineZoomLevel.YearQuarter3px => i18n.T("ZoomLevel.YearQuarter3px.Description"),
#pragma warning restore CS0618

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
