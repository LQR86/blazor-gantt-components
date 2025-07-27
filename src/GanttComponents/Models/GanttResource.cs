using System.ComponentModel.DataAnnotations;

namespace GanttComponents.Models;

/// <summary>
/// Resource entity for project resource management
/// </summary>
public class GanttResource
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Maximum units/capacity per day (e.g., 1.0 = 100% availability)
    /// </summary>
    [Range(0.0, 10.0)]
    public double MaxUnits { get; set; } = 1.0;

    /// <summary>
    /// Cost per hour for resource
    /// </summary>
    [Range(0.0, double.MaxValue)]
    public decimal? Cost { get; set; }

    /// <summary>
    /// Resource type for categorization
    /// </summary>
    [StringLength(50)]
    public string? Type { get; set; }

    /// <summary>
    /// Additional resource information
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Resource availability calendar (day-level)
    /// </summary>
    public List<GanttResourceAvailability> Availability { get; set; } = new();

    // Navigation properties
    public virtual List<GanttAssignment> Assignments { get; set; } = new();
}

/// <summary>
/// Resource availability for specific date ranges
/// </summary>
public class GanttResourceAvailability
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double AvailableUnits { get; set; } = 1.0;
    public string? Reason { get; set; } // e.g., "Vacation", "Training"
}
