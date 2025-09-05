using Microsoft.AspNetCore.Components;
using GanttComponents.Models.Filtering;
using GanttComponents.Models.ValueObjects;

namespace GanttComponents.Components.TaskFilter;

/// <summary>
/// Task filtering component for controlling task visibility and rendering options
/// </summary>
public partial class TaskFilter : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Current filter criteria
    /// </summary>
    [Parameter]
    public TaskFilterCriteria FilterCriteria { get; set; } = new();

    /// <summary>
    /// Event callback when filter criteria changes
    /// </summary>
    [Parameter]
    public EventCallback<TaskFilterCriteria> FilterCriteriaChanged { get; set; }

    /// <summary>
    /// Event callback when filters are applied
    /// </summary>
    [Parameter]
    public EventCallback<TaskFilterCriteria> OnFiltersApplied { get; set; }

    /// <summary>
    /// Whether the filter panel is initially expanded
    /// </summary>
    [Parameter]
    public bool InitiallyExpanded { get; set; } = false;

    #endregion

    #region Private Fields

    private bool IsExpanded { get; set; }

    // Input helper for nullable boolean handling
    private bool HasPredecessorsInput
    {
        get => FilterCriteria.HasPredecessors ?? false;
        set
        {
            FilterCriteria.HasPredecessors = value ? true : null;
            _ = NotifyFilterChange();
        }
    }

    // Helper properties for date display
    private string? StartDateInput => FilterCriteria.StartDateFilter?.ToString("yyyy-MM-dd");
    private string? EndDateInput => FilterCriteria.EndDateFilter?.ToString("yyyy-MM-dd");

    #endregion

    #region Lifecycle

    protected override void OnInitialized()
    {
        IsExpanded = InitiallyExpanded;
        base.OnInitialized();
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle start date input change
    /// </summary>
    private async Task OnStartDateChanged(ChangeEventArgs e)
    {
        var value = e.Value?.ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            FilterCriteria.StartDateFilter = null;
        }
        else if (DateTime.TryParse(value, out var date))
        {
            FilterCriteria.StartDateFilter = GanttDate.FromDateTime(date);
        }
        await NotifyFilterChange();
    }

    /// <summary>
    /// Handle end date input change
    /// </summary>
    private async Task OnEndDateChanged(ChangeEventArgs e)
    {
        var value = e.Value?.ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            FilterCriteria.EndDateFilter = null;
        }
        else if (DateTime.TryParse(value, out var date))
        {
            FilterCriteria.EndDateFilter = GanttDate.FromDateTime(date);
        }
        await NotifyFilterChange();
    }

    /// <summary>
    /// Toggle the expanded state of the filter panel
    /// </summary>
    private void ToggleExpanded()
    {
        IsExpanded = !IsExpanded;
    }

    /// <summary>
    /// Apply the current filter criteria
    /// </summary>
    private async Task ApplyFilters()
    {
        await NotifyFilterChange();
        await OnFiltersApplied.InvokeAsync(FilterCriteria);
    }

    /// <summary>
    /// Clear all filter criteria
    /// </summary>
    private async Task ClearFilters()
    {
        FilterCriteria = new TaskFilterCriteria();
        await NotifyFilterChange();
    }

    /// <summary>
    /// Reset filter criteria to default values
    /// </summary>
    private async Task ResetToDefaults()
    {
        FilterCriteria = new TaskFilterCriteria
        {
            ShowTinyTasks = true,
            ShowOnlyRootTasks = false,
            ShowOnlyCriticalPath = false
        };
        await NotifyFilterChange();
    }

    /// <summary>
    /// Notify parent component of filter changes
    /// </summary>
    private async Task NotifyFilterChange()
    {
        await FilterCriteriaChanged.InvokeAsync(FilterCriteria);
        StateHasChanged();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Get a summary of active filters for display
    /// </summary>
    public string GetActiveFiltersSummary()
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(FilterCriteria.NameFilter))
            filters.Add($"Name: {FilterCriteria.NameFilter}");

        if (!string.IsNullOrWhiteSpace(FilterCriteria.WbsFilter))
            filters.Add($"WBS: {FilterCriteria.WbsFilter}");

        if (FilterCriteria.StartDateFilter.HasValue)
            filters.Add($"Start ≥ {FilterCriteria.StartDateFilter.Value}");

        if (FilterCriteria.EndDateFilter.HasValue)
            filters.Add($"End ≤ {FilterCriteria.EndDateFilter.Value}");

        if (FilterCriteria.MinProgress.HasValue)
            filters.Add($"Progress ≥ {FilterCriteria.MinProgress}%");

        if (FilterCriteria.MaxProgress.HasValue)
            filters.Add($"Progress ≤ {FilterCriteria.MaxProgress}%");

        if (FilterCriteria.ShowOnlyRootTasks)
            filters.Add("Root tasks only");

        if (FilterCriteria.HasPredecessors.HasValue)
            filters.Add(FilterCriteria.HasPredecessors.Value ? "Has dependencies" : "No dependencies");

        if (!FilterCriteria.ShowTinyTasks)
            filters.Add("Hide tiny tasks");

        if (FilterCriteria.ShowOnlyCriticalPath)
            filters.Add("Critical path only");

        return filters.Any() ? string.Join(", ", filters) : "No active filters";
    }

    /// <summary>
    /// Check if any filters are currently active
    /// </summary>
    public bool HasActiveFilters()
    {
        return !string.IsNullOrWhiteSpace(FilterCriteria.NameFilter) ||
               !string.IsNullOrWhiteSpace(FilterCriteria.WbsFilter) ||
               FilterCriteria.StartDateFilter.HasValue ||
               FilterCriteria.EndDateFilter.HasValue ||
               FilterCriteria.MinProgress.HasValue ||
               FilterCriteria.MaxProgress.HasValue ||
               FilterCriteria.HasPredecessors.HasValue ||
               FilterCriteria.ShowOnlyRootTasks ||
               !FilterCriteria.ShowTinyTasks ||
               FilterCriteria.ShowOnlyCriticalPath;
    }

    #endregion
}
