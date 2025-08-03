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
                // Week→Day pattern levels (97px, 68px)
                TimelineZoomLevel.WeekDay97px => i18n.T("ZoomLevel.WeekDay97px"),
                TimelineZoomLevel.WeekDay68px => i18n.T("ZoomLevel.WeekDay68px"),

                // Month→Day pattern levels (48px, 34px)
                TimelineZoomLevel.MonthDay48px => i18n.T("ZoomLevel.MonthDay48px"),
                TimelineZoomLevel.MonthDay34px => i18n.T("ZoomLevel.MonthDay34px"),

                // Quarter→Month pattern levels (24px, 17px)
                TimelineZoomLevel.QuarterMonth24px => i18n.T("ZoomLevel.QuarterMonth24px"),
                TimelineZoomLevel.QuarterMonth17px => i18n.T("ZoomLevel.QuarterMonth17px"),

                // Month-only pattern levels (12px, 8px)
                TimelineZoomLevel.Month12px => i18n.T("ZoomLevel.Month12px"),
                TimelineZoomLevel.Month8px => i18n.T("ZoomLevel.Month8px"),

                // Year→Quarter pattern levels (6px, 4px, 3px)
                TimelineZoomLevel.YearQuarter6px => i18n.T("ZoomLevel.YearQuarter6px"),
                TimelineZoomLevel.YearQuarter4px => i18n.T("ZoomLevel.YearQuarter4px"),
                TimelineZoomLevel.YearQuarter3px => i18n.T("ZoomLevel.YearQuarter3px"),

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
                // Week→Day pattern levels (97px, 68px)
                TimelineZoomLevel.WeekDay97px => i18n.T("ZoomLevel.WeekDay97px.Description"),
                TimelineZoomLevel.WeekDay68px => i18n.T("ZoomLevel.WeekDay68px.Description"),

                // Month→Day pattern levels (48px, 34px)
                TimelineZoomLevel.MonthDay48px => i18n.T("ZoomLevel.MonthDay48px.Description"),
                TimelineZoomLevel.MonthDay34px => i18n.T("ZoomLevel.MonthDay34px.Description"),

                // Quarter→Month pattern levels (24px, 17px)
                TimelineZoomLevel.QuarterMonth24px => i18n.T("ZoomLevel.QuarterMonth24px.Description"),
                TimelineZoomLevel.QuarterMonth17px => i18n.T("ZoomLevel.QuarterMonth17px.Description"),

                // Month-only pattern levels (12px, 8px)
                TimelineZoomLevel.Month12px => i18n.T("ZoomLevel.Month12px.Description"),
                TimelineZoomLevel.Month8px => i18n.T("ZoomLevel.Month8px.Description"),

                // Year→Quarter pattern levels (6px, 4px, 3px)
                TimelineZoomLevel.YearQuarter6px => i18n.T("ZoomLevel.YearQuarter6px.Description"),
                TimelineZoomLevel.YearQuarter4px => i18n.T("ZoomLevel.YearQuarter4px.Description"),
                TimelineZoomLevel.YearQuarter3px => i18n.T("ZoomLevel.YearQuarter3px.Description"),

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
