using GanttComponents.Models;

namespace GanttComponents.Services;

/// <summary>
/// Service interface for simple task generation
/// </summary>
public interface ISimpleTaskGeneratorService
{
    /// <summary>
    /// Generates a list of hierarchical tasks based on the provided configuration
    /// </summary>
    /// <param name="config">Generation configuration parameters</param>
    /// <returns>List of generated GanttTask objects</returns>
    Task<List<GanttTask>> GenerateTasksAsync(SimpleTaskGenerationConfig config);

    /// <summary>
    /// Generates tasks and immediately seeds them into the database
    /// (clears existing data first)
    /// </summary>
    /// <param name="config">Generation configuration parameters</param>
    /// <returns>List of generated and seeded tasks</returns>
    Task<List<GanttTask>> GenerateAndSeedTasksAsync(SimpleTaskGenerationConfig config);

    /// <summary>
    /// Validates the configuration and returns any validation errors
    /// </summary>
    /// <param name="config">Configuration to validate</param>
    /// <returns>List of validation error messages (empty if valid)</returns>
    List<string> ValidateConfiguration(SimpleTaskGenerationConfig config);

    /// <summary>
    /// Generates preview information showing what would be created without actually creating tasks
    /// </summary>
    /// <param name="config">Generation configuration parameters</param>
    /// <returns>Preview information including validation results and sample tasks</returns>
    Task<TaskGenerationPreview> GeneratePreviewAsync(SimpleTaskGenerationConfig config);
}
