using System.ComponentModel.DataAnnotations;

namespace GanttComponents.Models;

/// <summary>
/// Assignment relationship between tasks and resources
/// </summary>
public class GanttAssignment
{
    [Required]
    public int TaskId { get; set; }

    [Required]
    public int ResourceId { get; set; }

    /// <summary>
    /// Resource units assigned to this task (e.g., 0.5 = 50% allocation)
    /// </summary>
    [Range(0.0, 10.0)]
    public double Units { get; set; } = 1.0;

    /// <summary>
    /// Work amount in hours for this assignment
    /// </summary>
    [Range(0.0, double.MaxValue)]
    public double? Work { get; set; }

    /// <summary>
    /// Cost for this specific assignment
    /// </summary>
    [Range(0.0, double.MaxValue)]
    public decimal? Cost { get; set; }

    /// <summary>
    /// Assignment start date (can be different from task start)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Assignment end date (can be different from task end)
    /// </summary>
    public DateTime? EndDate { get; set; }

    // Navigation properties
    public virtual GanttTask Task { get; set; } = null!;
    public virtual GanttResource Resource { get; set; } = null!;
}
