using GanttComponents.Models;
using GanttComponents.Services;
using Microsoft.AspNetCore.Components;

namespace GanttComponents.Components.TemplateSelector;

/// <summary>
/// Template selector component for choosing timeline view templates.
/// Provides manual template selection with descriptions and zoom range information.
/// </summary>
public partial class TemplateSelector
{
    /// <summary>
    /// Currently selected template.
    /// </summary>
    [Parameter] public TimelineZoomLevel SelectedTemplate { get; set; } = TimelineZoomLevel.MonthWeekOptimal50px;

    /// <summary>
    /// Event fired when template selection changes.
    /// </summary>
    [Parameter] public EventCallback<TimelineZoomLevel> OnTemplateChanged { get; set; }

    /// <summary>
    /// Available templates for selection.
    /// </summary>
    private static readonly TimelineZoomLevel[] AvailableTemplates = new[]
    {
        TimelineZoomLevel.YearQuarterOptimal70px,
        TimelineZoomLevel.QuarterMonthOptimal60px,
        TimelineZoomLevel.MonthWeekOptimal50px,
        TimelineZoomLevel.WeekDayOptimal50px
    };

    /// <summary>
    /// Handle template selection change.
    /// </summary>
    private async Task HandleTemplateSelectionChange(ChangeEventArgs e)
    {
        if (Enum.TryParse<TimelineZoomLevel>(e.Value?.ToString(), out var newTemplate))
        {
            SelectedTemplate = newTemplate;
            await OnTemplateChanged.InvokeAsync(newTemplate);
        }
    }

    /// <summary>
    /// Get user-friendly display name for a template.
    /// </summary>
    private string GetTemplateDisplayName(TimelineZoomLevel template)
    {
        return template switch
        {
            TimelineZoomLevel.YearQuarterOptimal70px => "Year-Quarter",
            TimelineZoomLevel.QuarterMonthOptimal60px => "Quarter-Month",
            TimelineZoomLevel.MonthWeekOptimal50px => "Month-Week",
            TimelineZoomLevel.WeekDayOptimal50px => "Week-Day",
            _ => template.ToString()
        };
    }

    /// <summary>
    /// Get description of optimal use case for a template.
    /// </summary>
    private string GetTemplateDescription(TimelineZoomLevel template)
    {
        return template switch
        {
            TimelineZoomLevel.YearQuarterOptimal70px => "Strategic planning",
            TimelineZoomLevel.QuarterMonthOptimal60px => "Quarterly planning",
            TimelineZoomLevel.MonthWeekOptimal50px => "Monthly planning",
            TimelineZoomLevel.WeekDayOptimal50px => "Weekly planning",
            _ => ""
        };
    }

    /// <summary>
    /// Get maximum zoom factor for the selected template.
    /// </summary>
    private double GetMaxZoom()
    {
        var config = TimelineZoomService.GetConfiguration(SelectedTemplate);
        return config.MaxZoomFactor;
    }
}
