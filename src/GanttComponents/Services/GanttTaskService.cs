using Microsoft.EntityFrameworkCore;
using GanttComponents.Data;
using GanttComponents.Models;

namespace GanttComponents.Services;

public class GanttTaskService : IGanttTaskService
{
    private readonly GanttDbContext _context;
    private readonly ILogger<GanttTaskService> _logger;

    public GanttTaskService(GanttDbContext context, ILogger<GanttTaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<GanttTask>> GetAllTasksAsync()
    {
        try
        {
            return await _context.Tasks
                .OrderBy(t => t.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
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
}
