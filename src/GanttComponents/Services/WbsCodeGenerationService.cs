using System.Text.RegularExpressions;
using GanttComponents.Models;

namespace GanttComponents.Services;

/// <summary>
/// Service for WBS (Work Breakdown Structure) code generation and validation
/// Implements REQUIREMENT 12: WBS codes as primary user-facing task identifiers
/// </summary>
public class WbsCodeGenerationService : IWbsCodeGenerationService
{
    private readonly IUniversalLogger _logger;
    private static readonly Regex WbsCodePattern = new(@"^(\d+(?:\.\d+)*)$", RegexOptions.Compiled);

    public WbsCodeGenerationService(IUniversalLogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates WBS codes for all tasks based on their hierarchical structure
    /// </summary>
    public Task<List<GanttTask>> GenerateWbsCodesAsync(List<GanttTask> tasks)
    {
        try
        {
            _logger.LogInfo($"Starting WBS code generation for {tasks.Count} tasks");

            // Create a lookup for parent-child relationships
            var taskById = tasks.ToDictionary(t => t.Id);
            var childrenByParent = tasks
                .Where(t => t.ParentId.HasValue)
                .GroupBy(t => t.ParentId!.Value)
                .ToDictionary(g => g.Key, g => g.OrderBy(t => t.Id).ToList());

            // Get root tasks (no parent) and order them
            var rootTasks = tasks
                .Where(t => !t.ParentId.HasValue)
                .OrderBy(t => t.Id)
                .ToList();

            // Generate WBS codes starting from root level
            int rootPosition = 1;
            foreach (var rootTask in rootTasks)
            {
                GenerateWbsCodeRecursive(rootTask, rootPosition.ToString(), childrenByParent, taskById);
                rootPosition++;
            }

            _logger.LogInfo($"Completed WBS code generation for {tasks.Count} tasks");
            return Task.FromResult(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error generating WBS codes", ex);
            throw;
        }
    }

    /// <summary>
    /// Recursive helper to generate WBS codes for a task and its children
    /// </summary>
    private void GenerateWbsCodeRecursive(
        GanttTask task,
        string wbsCode,
        Dictionary<int, List<GanttTask>> childrenByParent,
        Dictionary<int, GanttTask> taskById)
    {
        task.WbsCode = wbsCode;

        if (childrenByParent.TryGetValue(task.Id, out var children))
        {
            int childPosition = 1;
            foreach (var child in children)
            {
                string childWbsCode = $"{wbsCode}.{childPosition}";
                GenerateWbsCodeRecursive(child, childWbsCode, childrenByParent, taskById);
                childPosition++;
            }
        }
    }

    /// <summary>
    /// Generates a new WBS code for a task based on its parent and position
    /// </summary>
    public string GenerateWbsCode(string? parentWbsCode, int position)
    {
        if (string.IsNullOrEmpty(parentWbsCode))
        {
            return position.ToString();
        }

        return $"{parentWbsCode}.{position}";
    }

    /// <summary>
    /// Validates that a WBS code follows the correct hierarchical format
    /// </summary>
    public bool IsValidWbsCode(string wbsCode)
    {
        if (string.IsNullOrWhiteSpace(wbsCode))
        {
            return false;
        }

        // Check format: digits separated by dots (e.g., "1", "1.2", "1.2.3")
        if (!WbsCodePattern.IsMatch(wbsCode))
        {
            return false;
        }

        // Check that each level is a positive integer
        var parts = wbsCode.Split('.');
        foreach (var part in parts)
        {
            if (!int.TryParse(part, out int number) || number <= 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if a WBS code is unique within the given task list
    /// </summary>
    public bool IsWbsCodeUnique(string wbsCode, List<GanttTask> tasks, int? excludeTaskId = null)
    {
        return !tasks.Any(t =>
            t.WbsCode.Equals(wbsCode, StringComparison.OrdinalIgnoreCase) &&
            t.Id != excludeTaskId);
    }

    /// <summary>
    /// Renumbers WBS codes when hierarchy changes occur
    /// </summary>
    public async Task<List<GanttTask>> RenumberWbsCodesAsync(List<GanttTask> tasks, int changedTaskId)
    {
        try
        {
            _logger.LogInfo($"Starting WBS code renumbering for {tasks.Count} tasks (changed task: {changedTaskId})");

            // For simplicity, regenerate all WBS codes when hierarchy changes
            // This ensures consistency and is straightforward to implement
            var updatedTasks = await GenerateWbsCodesAsync(tasks);

            _logger.LogInfo($"Completed WBS code renumbering for {tasks.Count} tasks");

            return updatedTasks;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error renumbering WBS codes for changed task {changedTaskId}", ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the next available WBS code for a new task at the specified level
    /// </summary>
    public string GetNextAvailableWbsCode(List<GanttTask> tasks, string? parentWbsCode)
    {
        try
        {
            // Find all existing sibling WBS codes
            var siblingCodes = new List<string>();

            if (string.IsNullOrEmpty(parentWbsCode))
            {
                // Root level - find all root WBS codes
                siblingCodes = tasks
                    .Where(t => !string.IsNullOrEmpty(t.WbsCode) && !t.WbsCode.Contains('.'))
                    .Select(t => t.WbsCode)
                    .ToList();
            }
            else
            {
                // Child level - find all codes that start with parent code
                string prefix = $"{parentWbsCode}.";
                siblingCodes = tasks
                    .Where(t => !string.IsNullOrEmpty(t.WbsCode) &&
                               t.WbsCode.StartsWith(prefix) &&
                               t.WbsCode.Count(c => c == '.') == parentWbsCode.Count(c => c == '.') + 1)
                    .Select(t => t.WbsCode)
                    .ToList();
            }

            // Find the next available number
            int nextNumber = 1;
            var existingNumbers = new HashSet<int>();

            foreach (var code in siblingCodes)
            {
                string lastPart = code.Split('.').Last();
                if (int.TryParse(lastPart, out int number))
                {
                    existingNumbers.Add(number);
                }
            }

            while (existingNumbers.Contains(nextNumber))
            {
                nextNumber++;
            }

            return GenerateWbsCode(parentWbsCode, nextNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error getting next available WBS code", ex);
            throw;
        }
    }

    /// <summary>
    /// Validates WBS code hierarchy consistency across all tasks
    /// </summary>
    public List<string> ValidateWbsHierarchy(List<GanttTask> tasks)
    {
        var errors = new List<string>();

        try
        {
            _logger.LogInfo($"Starting WBS hierarchy validation for {tasks.Count} tasks");

            var tasksByWbsCode = new Dictionary<string, GanttTask>();

            // Check for duplicate WBS codes and invalid formats
            foreach (var task in tasks)
            {
                if (string.IsNullOrEmpty(task.WbsCode))
                {
                    errors.Add($"Task '{task.Name}' (ID: {task.Id}) has empty WBS code");
                    continue;
                }

                if (!IsValidWbsCode(task.WbsCode))
                {
                    errors.Add($"Task '{task.Name}' (ID: {task.Id}) has invalid WBS code format: '{task.WbsCode}'");
                    continue;
                }

                if (tasksByWbsCode.ContainsKey(task.WbsCode))
                {
                    errors.Add($"Duplicate WBS code '{task.WbsCode}' found in tasks: '{task.Name}' (ID: {task.Id}) and '{tasksByWbsCode[task.WbsCode].Name}' (ID: {tasksByWbsCode[task.WbsCode].Id})");
                }
                else
                {
                    tasksByWbsCode[task.WbsCode] = task;
                }
            }

            // Check hierarchy consistency
            foreach (var task in tasks)
            {
                if (string.IsNullOrEmpty(task.WbsCode) || !IsValidWbsCode(task.WbsCode))
                {
                    continue; // Already reported above
                }

                var wbsParts = task.WbsCode.Split('.');
                if (wbsParts.Length > 1)
                {
                    // This is a child task - check if parent exists
                    var parentWbsCode = string.Join(".", wbsParts.Take(wbsParts.Length - 1));

                    if (!tasksByWbsCode.ContainsKey(parentWbsCode))
                    {
                        errors.Add($"Task '{task.Name}' (WBS: {task.WbsCode}) references non-existent parent WBS code: '{parentWbsCode}'");
                    }
                    else
                    {
                        // Check if the database ParentId relationship matches WBS hierarchy
                        var parentTask = tasksByWbsCode[parentWbsCode];
                        if (task.ParentId != parentTask.Id)
                        {
                            errors.Add($"WBS hierarchy mismatch: Task '{task.Name}' (WBS: {task.WbsCode}) should have ParentId {parentTask.Id} but has {task.ParentId}");
                        }
                    }
                }
                else if (task.ParentId.HasValue)
                {
                    // Root level WBS code but has a parent ID
                    errors.Add($"Task '{task.Name}' (WBS: {task.WbsCode}) is root level but has ParentId: {task.ParentId}");
                }
            }

            _logger.LogInfo($"Completed WBS hierarchy validation for {tasks.Count} tasks ({errors.Count} errors found)");

            return errors;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error validating WBS hierarchy", ex);
            errors.Add($"Validation error: {ex.Message}");
            return errors;
        }
    }
}
