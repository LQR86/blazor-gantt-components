using GanttComponents.Models;
using GanttComponents.Data;
using Microsoft.EntityFrameworkCore;

namespace GanttComponents.Services;

/// <summary>
/// Simple task generator service implementation
/// </summary>
public class SimpleTaskGeneratorService : ISimpleTaskGeneratorService
{
    private readonly GanttDbContext _context;
    private readonly IUniversalLogger _logger;
    private static readonly string[] TaskPrefixes = { "Task", "Project", "Work", "Phase", "Item", "Module", "Component", "System" };
    private static readonly string[] TaskSuffixes = { "Alpha", "Beta", "Gamma", "Delta", "Prime", "Core", "Main", "Sub", "Advanced", "Basic" };

    public SimpleTaskGeneratorService(GanttDbContext context, IUniversalLogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task<List<GanttTask>> GenerateTasksAsync(SimpleTaskGenerationConfig config)
    {
        var validationErrors = ValidateConfiguration(config);
        if (validationErrors.Any())
        {
            throw new ArgumentException($"Configuration validation failed: {string.Join(", ", validationErrors)}");
        }

        _logger.LogOperation("TaskGeneration", "Starting task generation", new { config.TotalTaskCount, config.HierarchyDepth });

        var random = config.RandomSeed.HasValue ? new Random(config.RandomSeed.Value) : new Random();
        var tasks = new List<GanttTask>();
        var taskIdCounter = 1;

        // Generate hierarchy structure
        var rootTaskCount = CalculateRootTaskCount(config.TotalTaskCount, config.HierarchyDepth, config.MinTasksPerParent, config.MaxTasksPerParent);
        var tasksPerLevel = DistributeTasksAcrossLevels(config.TotalTaskCount, config.HierarchyDepth, rootTaskCount);

        // Generate root level tasks
        var timelineSegments = DivideTimelineIntoSegments(config.ProjectStartDate, config.ProjectEndDate, rootTaskCount);
        
        for (int i = 0; i < rootTaskCount; i++)
        {
            var rootTask = CreateTask(
                taskIdCounter++,
                GenerateTaskName(random),
                (i + 1).ToString(), // WBS: "1", "2", "3", etc.
                null, // No parent
                timelineSegments[i].Start,
                timelineSegments[i].End,
                random.Next(0, 101), // Random progress 0-100%
                random
            );

            tasks.Add(rootTask);

            // Generate child tasks recursively
            GenerateChildTasks(rootTask, tasks, ref taskIdCounter, 1, config, random, tasksPerLevel);
        }

        _logger.LogOperation("TaskGeneration", "Task generation completed", new { GeneratedCount = tasks.Count });
        return Task.FromResult(tasks);
    }

    public async Task<List<GanttTask>> GenerateAndSeedTasksAsync(SimpleTaskGenerationConfig config)
    {
        var tasks = await GenerateTasksAsync(config);

        _logger.LogDatabaseOperation("ClearTasks", new { Message = "Clearing existing tasks before seeding" });
        
        // Clear existing tasks
        _context.Tasks.RemoveRange(_context.Tasks);
        await _context.SaveChangesAsync();

        // Add generated tasks
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        _logger.LogDatabaseOperation("SeedTasks", new { TaskCount = tasks.Count, Message = "Successfully seeded generated tasks" });
        return tasks;
    }

    public List<string> ValidateConfiguration(SimpleTaskGenerationConfig config)
    {
        var errors = new List<string>();

        if (config.ProjectEndDate <= config.ProjectStartDate)
            errors.Add("Project end date must be after start date");

        var duration = config.ProjectEndDate - config.ProjectStartDate;
        if (duration.TotalDays < 180) // 6 months minimum
            errors.Add("Project duration must be at least 6 months");

        if (config.TotalTaskCount <= 0)
            errors.Add("Total task count must be positive");

        if (config.HierarchyDepth < 2 || config.HierarchyDepth > 5)
            errors.Add("Hierarchy depth must be between 2 and 5");

        if (config.MinTasksPerParent <= 0 || config.MaxTasksPerParent <= 0)
            errors.Add("Tasks per parent range must be positive");

        if (config.MinTasksPerParent > config.MaxTasksPerParent)
            errors.Add("Min tasks per parent cannot exceed max tasks per parent");

        if (config.MinTaskDurationDays <= 0 || config.MaxTaskDurationDays <= 0)
            errors.Add("Task duration range must be positive");

        if (config.MinTaskDurationDays > config.MaxTaskDurationDays)
            errors.Add("Min task duration cannot exceed max task duration");

        return errors;
    }

    private void GenerateChildTasks(GanttTask parentTask, List<GanttTask> allTasks, ref int taskIdCounter, 
        int currentLevel, SimpleTaskGenerationConfig config, Random random, Dictionary<int, int> tasksPerLevel)
    {
        if (currentLevel >= config.HierarchyDepth)
            return;

        var childCount = random.Next(config.MinTasksPerParent, config.MaxTasksPerParent + 1);
        var childTimelineSegments = DivideTimelineIntoSegments(parentTask.StartDate, parentTask.EndDate, childCount);

        for (int i = 0; i < childCount; i++)
        {
            var childWbsCode = $"{parentTask.WbsCode}.{i + 1}";
            var childTask = CreateTask(
                taskIdCounter++,
                GenerateTaskName(random),
                childWbsCode,
                parentTask.Id,
                childTimelineSegments[i].Start,
                childTimelineSegments[i].End,
                random.Next(0, 101),
                random
            );

            allTasks.Add(childTask);

            // Recursively generate children
            GenerateChildTasks(childTask, allTasks, ref taskIdCounter, currentLevel + 1, config, random, tasksPerLevel);
        }
    }

    private GanttTask CreateTask(int id, string name, string wbsCode, int? parentId, 
        DateTime startDate, DateTime endDate, int progress, Random random)
    {
        var duration = (endDate - startDate).Days;
        return new GanttTask
        {
            Id = id,
            Name = name,
            WbsCode = wbsCode,
            ParentId = parentId,
            StartDate = startDate,
            EndDate = endDate,
            Duration = $"{Math.Max(1, duration)}d",
            Progress = progress,
            Predecessors = "" // No dependencies for simplicity
        };
    }

    private string GenerateTaskName(Random random)
    {
        var prefix = TaskPrefixes[random.Next(TaskPrefixes.Length)];
        var suffix = TaskSuffixes[random.Next(TaskSuffixes.Length)];
        return $"{prefix} {suffix}";
    }

    private int CalculateRootTaskCount(int totalTasks, int depth, int minPerParent, int maxPerParent)
    {
        // Simple calculation: aim for balanced tree
        var avgPerParent = (minPerParent + maxPerParent) / 2.0;
        var estimatedRootCount = (int)Math.Ceiling(totalTasks / Math.Pow(avgPerParent, depth - 1));
        return Math.Max(2, Math.Min(10, estimatedRootCount)); // Keep it reasonable
    }

    private Dictionary<int, int> DistributeTasksAcrossLevels(int totalTasks, int depth, int rootCount)
    {
        // Simple distribution - not used in current implementation but kept for future enhancement
        return new Dictionary<int, int> { { 1, rootCount } };
    }

    private List<(DateTime Start, DateTime End)> DivideTimelineIntoSegments(DateTime start, DateTime end, int segmentCount)
    {
        var segments = new List<(DateTime Start, DateTime End)>();
        var totalDays = (end - start).Days;
        var daysPerSegment = Math.Max(1, totalDays / segmentCount);

        for (int i = 0; i < segmentCount; i++)
        {
            var segmentStart = start.AddDays(i * daysPerSegment);
            var segmentEnd = i == segmentCount - 1 ? end : start.AddDays((i + 1) * daysPerSegment);
            segments.Add((segmentStart, segmentEnd));
        }

        return segments;
    }
}
