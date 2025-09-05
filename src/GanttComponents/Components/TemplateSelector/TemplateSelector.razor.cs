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
    [Parameter] public TimelineZoomLevel SelectedTemplate { get; set; } = TimelineZoomLevel.MonthWeek;

    /// <summary>
    /// Event fired when template selection changes.
    /// </summary>
    [Parameter] public EventCallback<TimelineZoomLevel> OnTemplateChanged { get; set; }

    /// <summary>
    /// Available templates for selection.
    /// </summary>
    private static readonly TimelineZoomLevel[] AvailableTemplates = new[]
    {
        TimelineZoomLevel.YearQuarter,
        TimelineZoomLevel.QuarterMonth,
        TimelineZoomLevel.MonthWeek,
        TimelineZoomLevel.WeekDay
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
            TimelineZoomLevel.YearQuarter => "Year-Quarter",
            TimelineZoomLevel.QuarterMonth => "Quarter-Month",
            TimelineZoomLevel.MonthWeek => "Month-Week",
            TimelineZoomLevel.WeekDay => "Week-Day",
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
            TimelineZoomLevel.YearQuarter => "Strategic planning",
            TimelineZoomLevel.QuarterMonth => "Quarterly planning",
            TimelineZoomLevel.MonthWeek => "Monthly planning",
            TimelineZoomLevel.WeekDay => "Weekly planning",
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
