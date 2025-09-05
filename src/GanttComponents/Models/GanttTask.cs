using System.ComponentModel.DataAnnotations;
using GanttComponents.Models.ValueObjects;

namespace GanttComponents.Models;

/// <summary>
/// Core Gantt task entity with day-level precision
/// </summary>
public class GanttTask
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Work Breakdown Structure (WBS) code for task identification
    /// Format examples: "1", "1.1", "1.1.1", "2", "2.1"
    /// </summary>
    [Required]
    [StringLength(50)]
    public string WbsCode { get; set; } = string.Empty;

    /// <summary>
    /// Task start date (day precision, UTC enforced).
    /// INCLUSIVE: The task begins at the start of this date.
    /// For a task starting January 1st, work begins on January 1st.
    /// </summary>
    [Required]
    public GanttDate StartDate { get; set; }

    /// <summary>
    /// Task end date (day precision, UTC enforced).
    /// EXCLUSIVE: The task ends at the start of this date (this date is not included in work).
    /// For a task ending January 3rd, work stops at the end of January 2nd.
    /// Duration = EndDate - StartDate (in days).
    /// Example: StartDate=Jan 1, EndDate=Jan 3 means 2 days of work (Jan 1 and Jan 2).
    /// </summary>
    [Required]
    public GanttDate EndDate { get; set; }

    /// <summary>
    /// Duration in format "5d" (days) or "8h" (hours)
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Duration { get; set; } = "1d";

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    [Range(0, 100)]
    public int Progress { get; set; } = 0;

    /// <summary>
    /// Parent task ID for hierarchical structure
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Dependency string format: "2FS+3d,5SS-1d" (day precision only)
    /// </summary>
    [StringLength(500)]
    public string? Predecessors { get; set; }

    /// <summary>
    /// Task type for scheduling calculations
    /// </summary>
    public TaskType TaskType { get; set; } = TaskType.FixedDuration;

    /// <summary>
    /// Work amount in hours
    /// </summary>
    public double? Work { get; set; }

    /// <summary>
    /// Baseline information for planned vs actual comparison
    /// </summary>
    public GanttBaseline? Baseline { get; set; }

    /// <summary>
    /// Custom properties for additional data
    /// </summary>
    public Dictionary<string, object>? CustomFields { get; set; }

    // Note: Following Syncfusion's self-referential/flat data binding pattern
    // No navigation properties needed - hierarchy is managed via ParentId
    // This simplifies EF Core mapping and works perfectly with Syncfusion Gantt
}

/// <summary>
/// Task type for scheduling behavior
/// </summary>
public enum TaskType
{
    FixedDuration,
    FixedWork,
    FixedUnits
}

/// <summary>
/// Baseline information for planned vs actual tracking
/// </summary>
public class GanttBaseline
{
    public GanttDate StartDate { get; set; }
    public GanttDate EndDate { get; set; }
    public string Duration { get; set; } = "1d";
    public int Progress { get; set; } = 0;
}
