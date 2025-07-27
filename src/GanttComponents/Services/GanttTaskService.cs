using Microsoft.EntityFrameworkCore;
using GanttComponents.Data;
using GanttComponents.Models;

namespace GanttComponents.Services;

public class GanttTaskService : IGanttTaskService
{
    private readonly GanttDbContext _context;

    public GanttTaskService(GanttDbContext context)
    {
        _context = context;
    }

    public async Task<List<GanttTask>> GetAllTasksAsync()
    {
        return await _context.Tasks
            .OrderBy(t => t.Id)
            .ToListAsync();
    }

    public async Task<GanttTask?> GetTaskByIdAsync(int id)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<GanttTask> CreateTaskAsync(GanttTask task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<GanttTask> UpdateTaskAsync(GanttTask task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
