using System.ComponentModel.DataAnnotations;

namespace GanttComponents.Models;

/// <summary>
/// Configuration model for simple task generation.
/// Contains parameters for generating hierarchical task data for development/testing purposes.
/// </summary>
public class SimpleTaskGenerationConfig
{
    /// <summary>
    /// Project start date. Must be at least 6 months before end date.
    /// </summary>
    [Required]
    public DateTime ProjectStartDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Project end date. Must be at least 6 months after start date.
    /// </summary>
    [Required]
    public DateTime ProjectEndDate { get; set; } = DateTime.Today.AddMonths(12);

    /// <summary>
    /// Total number of tasks to generate across all hierarchy levels.
    /// Suggested: ~10 tasks per month based on project duration.
    /// </summary>
    [Range(5, 1000)]
    public int TotalTaskCount { get; set; } = 50;

    /// <summary>
    /// Maximum hierarchy depth for task tree structure.
    /// Range: 2-5 levels (e.g., 1, 1.1, 1.1.1)
    /// </summary>
    [Range(2, 5)]
    public int HierarchyDepth { get; set; } = 3;

    /// <summary>
    /// Minimum number of child tasks per parent task.
    /// </summary>
    [Range(2, 10)]
    public int MinTasksPerParent { get; set; } = 3;

    /// <summary>
    /// Maximum number of child tasks per parent task.
    /// </summary>
    [Range(2, 15)]
    public int MaxTasksPerParent { get; set; } = 7;

    /// <summary>
    /// Minimum task duration in days.
    /// </summary>
    [Range(1, 365)]
    public int MinTaskDurationDays { get; set; } = 1;

    /// <summary>
    /// Maximum task duration in days.
    /// </summary>
    [Range(1, 365)]
    public int MaxTaskDurationDays { get; set; } = 60;

    /// <summary>
    /// Random seed for reproducible task generation.
    /// If null, uses current timestamp for randomization.
    /// </summary>
    public int? RandomSeed { get; set; }

    /// <summary>
    /// Validates that the configuration has logical values.
    /// </summary>
    public bool IsValid()
    {
        // Project duration must be at least 6 months
        var duration = ProjectEndDate - ProjectStartDate;
        if (duration.TotalDays < 180) return false;

        // End date must be after start date
        if (ProjectEndDate <= ProjectStartDate) return false;

        // Min values must be less than max values
        if (MinTasksPerParent > MaxTasksPerParent) return false;
        if (MinTaskDurationDays > MaxTaskDurationDays) return false;

        return true;
    }

    /// <summary>
    /// Gets the suggested number of tasks based on project duration.
    /// Calculation: ~10 tasks per month.
    /// </summary>
    public int SuggestedTaskCount()
    {
        var duration = ProjectEndDate - ProjectStartDate;
        var months = (int)(duration.TotalDays / 30);
        return Math.Max(10, months * 10);
    }

    /// <summary>
    /// Gets the project duration in months for display purposes.
    /// </summary>
    public double ProjectDurationMonths()
    {
        var duration = ProjectEndDate - ProjectStartDate;
        return duration.TotalDays / 30.0;
    }
}
