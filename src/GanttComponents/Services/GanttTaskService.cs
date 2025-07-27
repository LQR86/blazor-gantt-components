using Microsoft.EntityFrameworkCore;
using GanttComponents.Data;
using GanttComponents.Models;

namespace GanttComponents.Services;

public class GanttTaskService : IGanttTaskService
{
    private readonly GanttDbContext _context;
    private readonly ILogger<GanttTaskService> _logger;
    private readonly IUniversalLogger _universalLogger;
    private readonly IWbsCodeGenerationService _wbsService;

    public GanttTaskService(
        GanttDbContext context,
        ILogger<GanttTaskService> logger,
        IUniversalLogger universalLogger,
        IWbsCodeGenerationService wbsService)
    {
        _context = context;
        _logger = logger;
        _universalLogger = universalLogger;
        _wbsService = wbsService;
    }

    public async Task<List<GanttTask>> GetAllTasksAsync()
    {
        try
        {
            _universalLogger.LogDatabaseOperation("GetAllTasks", new { Operation = "Retrieving all tasks from database" });

            var tasks = await _context.Tasks
                .OrderBy(t => t.Id)
                .ToListAsync();

            _universalLogger.LogDatabaseOperation("GetAllTasks", new { TaskCount = tasks.Count, Success = true });
            return tasks;
        }
        catch (Exception ex)
        {
            _universalLogger.LogError("Error retrieving all tasks from database", ex);
            _logger.LogError(ex, "Error retrieving all tasks");
            throw;
        }
    }

    public async Task<GanttTask?> GetTaskByIdAsync(int id)
    {
        try
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task with ID {TaskId}", id);
            throw;
        }
    }

    public async Task<GanttTask> CreateTaskAsync(GanttTask task)
    {
        try
        {
            // Validate WBS code uniqueness
            if (!await ValidateWbsCodeUniquenessAsync(task.WbsCode))
            {
                throw new InvalidOperationException($"WBS code '{task.WbsCode}' already exists");
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new task with ID {TaskId} and WBS code {WbsCode}", task.Id, task.WbsCode);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task with WBS code {WbsCode}", task.WbsCode);
            throw;
        }
    }

    public async Task<GanttTask> UpdateTaskAsync(GanttTask task)
    {
        try
        {
            // Validate WBS code uniqueness (excluding current task)
            if (!await ValidateWbsCodeUniquenessAsync(task.WbsCode, task.Id))
            {
                throw new InvalidOperationException($"WBS code '{task.WbsCode}' already exists");
            }

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated task with ID {TaskId} and WBS code {WbsCode}", task.Id, task.WbsCode);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task with ID {TaskId}", task.Id);
            throw;
        }
    }

    public async Task DeleteTaskAsync(int id)
    {
        try
        {
            var task = await GetTaskByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Attempted to delete non-existent task with ID {TaskId}", id);
                return; // Gracefully handle non-existent task
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted task with ID {TaskId} and WBS code {WbsCode}", id, task.WbsCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task with ID {TaskId}", id);
            throw;
        }
    }

    public async Task<List<GanttTask>> GetTasksByParentIdAsync(int? parentId)
    {
        try
        {
            return await _context.Tasks
                .Where(t => t.ParentId == parentId)
                .OrderBy(t => t.WbsCode)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for parent ID {ParentId}", parentId);
            throw;
        }
    }

    public async Task<bool> ValidateWbsCodeUniquenessAsync(string wbsCode, int? excludeTaskId = null)
    {
        try
        {
            var query = _context.Tasks.Where(t => t.WbsCode == wbsCode);

            if (excludeTaskId.HasValue)
            {
                query = query.Where(t => t.Id != excludeTaskId.Value);
            }

            return !await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating WBS code uniqueness for {WbsCode}", wbsCode);
            throw;
        }
    }

    /// <summary>
    /// Regenerates WBS codes for all tasks and saves to database
    /// </summary>
    public async Task<List<GanttTask>> RegenerateAllWbsCodesAsync()
    {
        try
        {
            _universalLogger.LogWbsOperation("Regenerating all WBS codes", new { Operation = "Starting regeneration" });

            var allTasks = await GetAllTasksAsync();
            var updatedTasks = await _wbsService.GenerateWbsCodesAsync(allTasks);

            // Save updated tasks to database
            foreach (var task in updatedTasks)
            {
                _context.Tasks.Update(task);
            }

            await _context.SaveChangesAsync();

            _universalLogger.LogWbsOperation("Regenerated all WBS codes", new
            {
                TaskCount = updatedTasks.Count,
                Success = true
            });

            return updatedTasks;
        }
        catch (Exception ex)
        {
            _universalLogger.LogError("Error regenerating WBS codes", ex);
            throw;
        }
    }

    /// <summary>
    /// Validates WBS hierarchy and fixes any issues found
    /// </summary>
    public async Task<List<GanttTask>> ValidateAndFixWbsHierarchyAsync()
    {
        try
        {
            _universalLogger.LogWbsOperation("Validating WBS hierarchy", new { Operation = "Starting validation" });

            var allTasks = await GetAllTasksAsync();
            var validationErrors = _wbsService.ValidateWbsHierarchy(allTasks);

            if (validationErrors.Any())
            {
                _universalLogger.LogWarning("WBS hierarchy validation found errors", new
                {
                    ErrorCount = validationErrors.Count,
                    Errors = validationErrors
                });

                // Fix by regenerating all WBS codes
                return await RegenerateAllWbsCodesAsync();
            }

            _universalLogger.LogWbsOperation("WBS hierarchy validation passed", new
            {
                TaskCount = allTasks.Count,
                Success = true
            });

            return allTasks;
        }
        catch (Exception ex)
        {
            _universalLogger.LogError("Error validating WBS hierarchy", ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the next available WBS code for a new task
    /// </summary>
    public async Task<string> GetNextAvailableWbsCodeAsync(string? parentWbsCode)
    {
        try
        {
            var allTasks = await GetAllTasksAsync();
            var nextWbsCode = _wbsService.GetNextAvailableWbsCode(allTasks, parentWbsCode);

            _universalLogger.LogWbsOperation("Generated next available WBS code", new
            {
                ParentWbsCode = parentWbsCode ?? "ROOT",
                NextWbsCode = nextWbsCode
            });

            return nextWbsCode;
        }
        catch (Exception ex)
        {
            _universalLogger.LogError($"Error getting next available WBS code for parent '{parentWbsCode}'", ex);
            throw;
        }
    }
}
