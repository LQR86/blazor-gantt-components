using System.ComponentModel.DataAnnotations;
using GanttComponents.Models.ValueObjects;

namespace GanttComponents.Models.Filtering;

/// <summary>
/// Comprehensive filter criteria for task visibility and rendering
/// </summary>
public class TaskFilterCriteria
{
    /// <summary>
    /// Filter by task name (case-insensitive partial match)
    /// </summary>
    public string? NameFilter { get; set; }

    /// <summary>
    /// Filter by WBS code (case-insensitive partial match)
    /// </summary>
    public string? WbsFilter { get; set; }

    /// <summary>
    /// Show tasks within date range
    /// </summary>
    public GanttDate? StartDateFilter { get; set; }
    public GanttDate? EndDateFilter { get; set; }

    /// <summary>
    /// Filter by progress percentage range
    /// </summary>
    [Range(0, 100)]
    public int? MinProgress { get; set; }

    [Range(0, 100)]
    public int? MaxProgress { get; set; }

    /// <summary>
    /// Show only tasks with predecessors/dependencies
    /// </summary>
    public bool? HasPredecessors { get; set; }

    /// <summary>
    /// Filter by parent task (null for root tasks)
    /// </summary>
    public int? ParentTaskId { get; set; }

    /// <summary>
    /// Show only root-level tasks (ParentId is null)
    /// </summary>
    public bool ShowOnlyRootTasks { get; set; } = false;

    /// <summary>
    /// Show tiny tasks that would render smaller than pixel threshold
    /// </summary>
    public bool ShowTinyTasks { get; set; } = true;

    /// <summary>
    /// Pixel width threshold for considering a task "tiny" - hardcoded to 3px
    /// Tasks below this width show red asterisk marker instead of full task bar
    /// </summary>
    public double TinyTaskPixelThreshold => 3.0;

    /// <summary>
    /// Filter by minimum task duration (in days)
    /// Tasks with duration below this threshold will be hidden unless ShowTinyTasks is true
    /// </summary>
    public int? MinTaskDurationDays { get; set; }

    /// <summary>
    /// Show tasks on critical path only
    /// </summary>
    public bool ShowOnlyCriticalPath { get; set; } = false;

    /// <summary>
    /// Custom field filters (field name -> expected value)
    /// </summary>
    public Dictionary<string, object>? CustomFieldFilters { get; set; }

    /// <summary>
    /// Determines if a task should be rendered as a tiny marker instead of full task bar
    /// </summary>
    /// <param name="task">The task to evaluate</param>
    /// <param name="pixelWidth">The calculated pixel width for the task</param>
    /// <returns>True if task should show red asterisk marker, false for normal task bar</returns>
    public bool ShouldRenderAsTinyMarker(GanttTask task, double pixelWidth)
    {
        // If tiny tasks are disabled, never render as marker
        if (!ShowTinyTasks)
            return false;

        // Render as marker if pixel width is below threshold
        return pixelWidth < TinyTaskPixelThreshold;
    }

    /// <summary>
    /// Evaluates if a task passes all filter criteria
    /// </summary>
    /// <param name="task">The task to evaluate</param>
    /// <returns>True if task should be visible, false to hide</returns>
    public bool PassesFilter(GanttTask task)
    {
        // Name filter
        if (!string.IsNullOrWhiteSpace(NameFilter) &&
            !task.Name.Contains(NameFilter, StringComparison.OrdinalIgnoreCase))
            return false;

        // WBS filter
        if (!string.IsNullOrWhiteSpace(WbsFilter) &&
            !task.WbsCode.Contains(WbsFilter, StringComparison.OrdinalIgnoreCase))
            return false;

        // Date range filter
        if (StartDateFilter.HasValue && task.EndDate.CompareTo(StartDateFilter.Value) < 0)
            return false;

        if (EndDateFilter.HasValue && task.StartDate.CompareTo(EndDateFilter.Value) > 0)
            return false;

        // Progress filter
        if (MinProgress.HasValue && task.Progress < MinProgress.Value)
            return false;

        if (MaxProgress.HasValue && task.Progress > MaxProgress.Value)
            return false;

        // Predecessors filter
        if (HasPredecessors.HasValue)
        {
            bool hasPreds = !string.IsNullOrWhiteSpace(task.Predecessors);
            if (HasPredecessors.Value != hasPreds)
                return false;
        }

        // Parent filter
        if (ParentTaskId.HasValue && task.ParentId != ParentTaskId.Value)
            return false;

        // Root tasks only filter
        if (ShowOnlyRootTasks && task.ParentId.HasValue)
            return false;

        // Task duration filter
        if (MinTaskDurationDays.HasValue)
        {
            var taskDurationDays = (task.EndDate.ToUtcDateTime() - task.StartDate.ToUtcDateTime()).TotalDays + 1;
            if (taskDurationDays < MinTaskDurationDays.Value)
                return false;
        }

        // Custom fields filter
        if (CustomFieldFilters?.Any() == true && task.CustomFields != null)
        {
            foreach (var filter in CustomFieldFilters)
            {
                if (!task.CustomFields.TryGetValue(filter.Key, out var value) ||
                    !Equals(value, filter.Value))
                    return false;
            }
        }

        return true;
    }
}
