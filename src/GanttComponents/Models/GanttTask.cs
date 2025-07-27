using System.ComponentModel.DataAnnotations;

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
    /// Task start date (day precision)
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Task end date (day precision)
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

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
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Duration { get; set; } = "1d";
    public int Progress { get; set; } = 0;
}
