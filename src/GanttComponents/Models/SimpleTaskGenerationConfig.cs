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

/// <summary>
/// Preview information for task generation showing what would be created
/// </summary>
public class TaskGenerationPreview
{
    /// <summary>
    /// Whether the configuration is valid for generation
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation error messages (empty if valid)
    /// </summary>
    public List<string> ValidationErrors { get; set; } = new();

    /// <summary>
    /// Estimated number of tasks that will be generated
    /// </summary>
    public int EstimatedTaskCount { get; set; }

    /// <summary>
    /// Project duration in months
    /// </summary>
    public double ProjectDurationMonths { get; set; }

    /// <summary>
    /// Sample of first few tasks that would be generated (for preview)
    /// </summary>
    public List<GanttTask> SampleTasks { get; set; } = new();

    /// <summary>
    /// Statistics about the generation
    /// </summary>
    public GenerationStatistics Statistics { get; set; } = new();

    /// <summary>
    /// Current tasks in database (before seeding)
    /// </summary>
    public List<GanttTask> CurrentDatabaseTasks { get; set; } = new();

    /// <summary>
    /// Count of current tasks in database
    /// </summary>
    public int CurrentTaskCount { get; set; }

    /// <summary>
    /// Detailed validation results
    /// </summary>
    public List<ValidationResult> ValidationResults { get; set; } = new();
}

/// <summary>
/// Detailed validation result with category and status
/// </summary>
public class ValidationResult
{
    public string Category { get; set; } = string.Empty;
    public string Check { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Icon => IsValid ? "✅" : "❌";
    public string CssClass => IsValid ? "text-success" : "text-danger";
}

/// <summary>
/// Statistics about task generation
/// </summary>
public class GenerationStatistics
{
    public int TotalTasks { get; set; }
    public int RootTasks { get; set; }
    public int MaxDepth { get; set; }
    public DateTime EarliestStart { get; set; }
    public DateTime LatestEnd { get; set; }
    public int AverageTaskDuration { get; set; }
}
