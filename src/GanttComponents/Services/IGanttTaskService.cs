using GanttComponents.Models;

namespace GanttComponents.Services;

public interface IGanttTaskService
{
    Task<List<GanttTask>> GetAllTasksAsync();
    Task<GanttTask?> GetTaskByIdAsync(int id);
    Task<GanttTask> CreateTaskAsync(GanttTask task);
    Task<GanttTask> UpdateTaskAsync(GanttTask task);
    Task DeleteTaskAsync(int id);
    Task<List<GanttTask>> GetTasksByParentIdAsync(int? parentId);
    Task<bool> ValidateWbsCodeUniquenessAsync(string wbsCode, int? excludeTaskId = null);
}
