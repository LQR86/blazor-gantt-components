namespace GanttComponents.Services;

/// <summary>
/// Service interface for WBS (Work Breakdown Structure) code generation and validation
/// Implements REQUIREMENT 12: WBS codes as primary user-facing task identifiers
/// </summary>
public interface IWbsCodeGenerationService
{
    /// <summary>
    /// Generates WBS codes for all tasks based on their hierarchical structure
    /// </summary>
    /// <param name="tasks">List of tasks to generate WBS codes for</param>
    /// <returns>Tasks with updated WBS codes</returns>
    Task<List<Models.GanttTask>> GenerateWbsCodesAsync(List<Models.GanttTask> tasks);

    /// <summary>
    /// Generates a new WBS code for a task based on its parent and position
    /// </summary>
    /// <param name="parentWbsCode">Parent task's WBS code (null for root level)</param>
    /// <param name="position">Position among siblings (1-based)</param>
    /// <returns>Generated WBS code</returns>
    string GenerateWbsCode(string? parentWbsCode, int position);

    /// <summary>
    /// Validates that a WBS code follows the correct hierarchical format
    /// </summary>
    /// <param name="wbsCode">WBS code to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValidWbsCode(string wbsCode);

    /// <summary>
    /// Checks if a WBS code is unique within the given task list
    /// </summary>
    /// <param name="wbsCode">WBS code to check</param>
    /// <param name="tasks">List of tasks to check against</param>
    /// <param name="excludeTaskId">Task ID to exclude from uniqueness check (for updates)</param>
    /// <returns>True if unique, false otherwise</returns>
    bool IsWbsCodeUnique(string wbsCode, List<Models.GanttTask> tasks, int? excludeTaskId = null);

    /// <summary>
    /// Renumbers WBS codes when hierarchy changes occur
    /// </summary>
    /// <param name="tasks">All tasks in the project</param>
    /// <param name="changedTaskId">ID of the task that was moved/modified</param>
    /// <returns>Tasks with updated WBS codes</returns>
    Task<List<Models.GanttTask>> RenumberWbsCodesAsync(List<Models.GanttTask> tasks, int changedTaskId);

    /// <summary>
    /// Gets the next available WBS code for a new task at the specified level
    /// </summary>
    /// <param name="tasks">Existing tasks</param>
    /// <param name="parentWbsCode">Parent WBS code (null for root level)</param>
    /// <returns>Next available WBS code</returns>
    string GetNextAvailableWbsCode(List<Models.GanttTask> tasks, string? parentWbsCode);

    /// <summary>
    /// Validates WBS code hierarchy consistency across all tasks
    /// </summary>
    /// <param name="tasks">Tasks to validate</param>
    /// <returns>List of validation errors (empty if all valid)</returns>
    List<string> ValidateWbsHierarchy(List<Models.GanttTask> tasks);
}
