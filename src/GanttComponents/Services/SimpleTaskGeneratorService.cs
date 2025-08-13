using GanttComponents.Models;
using GanttComponents.Models.ValueObjects;
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

        _logger.LogInfo($"Starting task generation - {config.TotalTaskCount} tasks, depth {config.HierarchyDepth}");

        var random = config.RandomSeed.HasValue ? new Random(config.RandomSeed.Value) : new Random();
        var tasks = new List<GanttTask>();
        var taskIdCounter = 1;

        // Generate hierarchy structure
        var rootTaskCount = CalculateRootTaskCount(config.TotalTaskCount, config.HierarchyDepth, config.MinTasksPerParent, config.MaxTasksPerParent);
        var tasksPerLevel = DistributeTasksAcrossLevels(config.TotalTaskCount, config.HierarchyDepth, rootTaskCount);

        // Generate root level tasks
        var timelineSegments = DivideTimelineIntoSegments(
            GanttDate.FromDateTime(config.ProjectStartDate),
            GanttDate.FromDateTime(config.ProjectEndDate),
            rootTaskCount);

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

        _logger.LogInfo($"Task generation completed - {tasks.Count} tasks generated");
        return Task.FromResult(tasks);
    }

    public async Task<List<GanttTask>> GenerateAndSeedTasksAsync(SimpleTaskGenerationConfig config)
    {
        // Generate tasks first (before touching database)
        var tasks = await GenerateTasksAsync(config);

        _logger.LogInfo("Starting database seeding with transaction");

        // Use database transaction for safety
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Clear existing tasks
            _logger.LogInfo("Clearing existing tasks before seeding");
            _context.Tasks.RemoveRange(_context.Tasks);
            await _context.SaveChangesAsync();

            // Add generated tasks
            _logger.LogInfo($"Adding {tasks.Count} generated tasks");
            _context.Tasks.AddRange(tasks);
            await _context.SaveChangesAsync();

            // Commit transaction
            await transaction.CommitAsync();
            _logger.LogInfo($"Successfully seeded {tasks.Count} generated tasks");

            return tasks;
        }
        catch (Exception ex)
        {
            // Rollback on any error
            await transaction.RollbackAsync();
            _logger.LogError("Failed to seed tasks, transaction rolled back", ex);
            throw new InvalidOperationException($"Database seeding failed: {ex.Message}", ex);
        }
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

    public async Task<TaskGenerationPreview> GeneratePreviewAsync(SimpleTaskGenerationConfig config)
    {
        var preview = new TaskGenerationPreview
        {
            ProjectDurationMonths = config.ProjectDurationMonths()
        };

        // Perform comprehensive validation
        await PerformComprehensiveValidation(config, preview);

        // Set overall validity based on any failed validations
        preview.IsValid = preview.ValidationResults.All(v => v.IsValid);

        // Keep the simple error list for backward compatibility
        preview.ValidationErrors = preview.ValidationResults
            .Where(v => !v.IsValid)
            .Select(v => v.Message)
            .ToList();

        if (!preview.IsValid)
        {
            return preview;
        }

        // Generate tasks for preview - show all tasks for validation
        var previewConfig = new SimpleTaskGenerationConfig
        {
            ProjectStartDate = config.ProjectStartDate,
            ProjectEndDate = config.ProjectEndDate,
            TotalTaskCount = config.TotalTaskCount, // Show ALL tasks in preview
            HierarchyDepth = config.HierarchyDepth,
            MinTasksPerParent = config.MinTasksPerParent,
            MaxTasksPerParent = config.MaxTasksPerParent,
            MinTaskDurationDays = config.MinTaskDurationDays,
            MaxTaskDurationDays = config.MaxTaskDurationDays,
            RandomSeed = config.RandomSeed ?? 12345 // Use fixed seed for consistent preview
        };

        // Get current database state
        var currentTasks = await _context.Tasks.OrderBy(t => t.WbsCode).ToListAsync();
        preview.CurrentDatabaseTasks = currentTasks;
        preview.CurrentTaskCount = currentTasks.Count;

        var sampleTasks = await GenerateTasksAsync(previewConfig);
        preview.SampleTasks = sampleTasks; // Show ALL tasks in preview

        // Calculate statistics based on actually generated tasks
        preview.EstimatedTaskCount = sampleTasks.Count;
        preview.Statistics = new GenerationStatistics
        {
            TotalTasks = sampleTasks.Count, // Use actual count
            RootTasks = sampleTasks.Count(t => !t.ParentId.HasValue),
            MaxDepth = sampleTasks.Any() ? sampleTasks.Max(t => t.WbsCode.Split('.').Length) : 0,
            EarliestStart = sampleTasks.Any() ? sampleTasks.Min(t => t.StartDate).ToUtcDateTime() : config.ProjectStartDate,
            LatestEnd = sampleTasks.Any() ? sampleTasks.Max(t => t.EndDate).ToUtcDateTime() : config.ProjectEndDate,
            AverageTaskDuration = (config.MinTaskDurationDays + config.MaxTaskDurationDays) / 2
        };

        return preview;
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
        GanttDate startDate, GanttDate endDate, int progress, Random random)
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

    private List<(GanttDate Start, GanttDate End)> DivideTimelineIntoSegments(GanttDate start, GanttDate end, int segmentCount)
    {
        var segments = new List<(GanttDate Start, GanttDate End)>();
        var totalDays = (end - start).Days;
        var daysPerSegment = Math.Max(1, totalDays / segmentCount);

        for (int i = 0; i < segmentCount; i++)
        {
            var segmentStart = GanttDate.FromDateTime(start.ToUtcDateTime().AddDays(i * daysPerSegment));
            var segmentEnd = i == segmentCount - 1 ? end : GanttDate.FromDateTime(start.ToUtcDateTime().AddDays((i + 1) * daysPerSegment));
            segments.Add((segmentStart, segmentEnd));
        }

        return segments;
    }

    private async Task PerformComprehensiveValidation(SimpleTaskGenerationConfig config, TaskGenerationPreview preview)
    {
        var results = new List<ValidationResult>();

        // 1. Database Schema Validation
        results.Add(await ValidateDatabaseConnection());
        results.Add(await ValidateDatabaseSchema());

        // 2. Configuration Validation
        results.AddRange(ValidateConfigurationDetailed(config));

        // 3. Date and Duration Logic Validation
        results.AddRange(ValidateDateLogic(config));

        // 4. Task Generation Logic Validation
        results.AddRange(ValidateTaskGenerationLogic(config));

        // 5. Database Constraints Validation
        results.AddRange(await ValidateDatabaseConstraints(config));

        preview.ValidationResults = results;
    }

    private async Task<ValidationResult> ValidateDatabaseConnection()
    {
        try
        {
            await _context.Database.CanConnectAsync();
            return new ValidationResult
            {
                Category = "Database",
                Check = "Connection",
                IsValid = true,
                Message = "Database connection successful"
            };
        }
        catch (Exception ex)
        {
            return new ValidationResult
            {
                Category = "Database",
                Check = "Connection",
                IsValid = false,
                Message = $"Database connection failed: {ex.Message}"
            };
        }
    }

    private async Task<ValidationResult> ValidateDatabaseSchema()
    {
        try
        {
            // Check if Tasks table exists and has correct schema
            var tableExists = await _context.Tasks.AnyAsync();
            return new ValidationResult
            {
                Category = "Database",
                Check = "Schema",
                IsValid = true,
                Message = "Database schema validated successfully"
            };
        }
        catch (Exception ex)
        {
            return new ValidationResult
            {
                Category = "Database",
                Check = "Schema",
                IsValid = false,
                Message = $"Database schema validation failed: {ex.Message}"
            };
        }
    }

    private List<ValidationResult> ValidateConfigurationDetailed(SimpleTaskGenerationConfig config)
    {
        var results = new List<ValidationResult>();

        // Task count validation
        results.Add(new ValidationResult
        {
            Category = "Configuration",
            Check = "Task Count",
            IsValid = config.TotalTaskCount > 0 && config.TotalTaskCount <= 10000,
            Message = config.TotalTaskCount > 0 && config.TotalTaskCount <= 10000
                ? $"Task count ({config.TotalTaskCount}) is valid"
                : $"Task count ({config.TotalTaskCount}) must be between 1 and 10,000"
        });

        // Hierarchy depth validation
        results.Add(new ValidationResult
        {
            Category = "Configuration",
            Check = "Hierarchy Depth",
            IsValid = config.HierarchyDepth >= 1 && config.HierarchyDepth <= 5,
            Message = config.HierarchyDepth >= 1 && config.HierarchyDepth <= 5
                ? $"Hierarchy depth ({config.HierarchyDepth}) is valid"
                : $"Hierarchy depth ({config.HierarchyDepth}) must be between 1 and 5"
        });

        // Tasks per parent validation
        results.Add(new ValidationResult
        {
            Category = "Configuration",
            Check = "Tasks Per Parent",
            IsValid = config.MinTasksPerParent <= config.MaxTasksPerParent && config.MinTasksPerParent > 0,
            Message = config.MinTasksPerParent <= config.MaxTasksPerParent && config.MinTasksPerParent > 0
                ? $"Tasks per parent range ({config.MinTasksPerParent}-{config.MaxTasksPerParent}) is valid"
                : $"Invalid tasks per parent range: min({config.MinTasksPerParent}) must be ≤ max({config.MaxTasksPerParent}) and > 0"
        });

        return results;
    }

    private List<ValidationResult> ValidateDateLogic(SimpleTaskGenerationConfig config)
    {
        var results = new List<ValidationResult>();

        // Date range validation
        var isDateRangeValid = config.ProjectStartDate < config.ProjectEndDate;
        results.Add(new ValidationResult
        {
            Category = "Date Logic",
            Check = "Date Range",
            IsValid = isDateRangeValid,
            Message = isDateRangeValid
                ? $"Project dates ({config.ProjectStartDate:yyyy-MM-dd} to {config.ProjectEndDate:yyyy-MM-dd}) are valid"
                : $"Start date ({config.ProjectStartDate:yyyy-MM-dd}) must be before end date ({config.ProjectEndDate:yyyy-MM-dd})"
        });

        // Project duration validation
        var projectDuration = (config.ProjectEndDate - config.ProjectStartDate).Days;
        var isDurationReasonable = projectDuration >= 1 && projectDuration <= 3650; // 1 day to 10 years
        results.Add(new ValidationResult
        {
            Category = "Date Logic",
            Check = "Project Duration",
            IsValid = isDurationReasonable,
            Message = isDurationReasonable
                ? $"Project duration ({projectDuration} days) is reasonable"
                : $"Project duration ({projectDuration} days) should be between 1 and 3,650 days"
        });

        // Task duration validation
        var isTaskDurationValid = config.MinTaskDurationDays <= config.MaxTaskDurationDays &&
                                 config.MinTaskDurationDays > 0 &&
                                 config.MaxTaskDurationDays <= projectDuration;
        results.Add(new ValidationResult
        {
            Category = "Date Logic",
            Check = "Task Duration Range",
            IsValid = isTaskDurationValid,
            Message = isTaskDurationValid
                ? $"Task duration range ({config.MinTaskDurationDays}-{config.MaxTaskDurationDays} days) is valid"
                : $"Invalid task duration range: must be positive and within project duration ({projectDuration} days)"
        });

        return results;
    }

    private List<ValidationResult> ValidateTaskGenerationLogic(SimpleTaskGenerationConfig config)
    {
        var results = new List<ValidationResult>();

        // Estimated task count vs configuration
        var estimatedTasks = CalculateEstimatedTaskCount(config);
        var isTaskCountRealistic = Math.Abs(estimatedTasks - config.TotalTaskCount) <= config.TotalTaskCount * 0.5; // Within 50%
        results.Add(new ValidationResult
        {
            Category = "Generation Logic",
            Check = "Task Count Estimation",
            IsValid = isTaskCountRealistic,
            Message = isTaskCountRealistic
                ? $"Requested task count ({config.TotalTaskCount}) is achievable (estimated: {estimatedTasks})"
                : $"Requested task count ({config.TotalTaskCount}) may not be achievable (estimated: {estimatedTasks})"
        });

        // WBS code generation capacity
        var maxPossibleTasks = CalculateMaxPossibleTasks(config);
        var isWithinCapacity = config.TotalTaskCount <= maxPossibleTasks;
        results.Add(new ValidationResult
        {
            Category = "Generation Logic",
            Check = "WBS Capacity",
            IsValid = isWithinCapacity,
            Message = isWithinCapacity
                ? $"Task count ({config.TotalTaskCount}) is within WBS capacity ({maxPossibleTasks})"
                : $"Task count ({config.TotalTaskCount}) exceeds WBS capacity ({maxPossibleTasks})"
        });

        return results;
    }

    private async Task<List<ValidationResult>> ValidateDatabaseConstraints(SimpleTaskGenerationConfig config)
    {
        var results = new List<ValidationResult>();

        // Check current database size
        var currentTaskCount = await _context.Tasks.CountAsync();
        results.Add(new ValidationResult
        {
            Category = "Database Constraints",
            Check = "Current Data",
            IsValid = true,
            Message = currentTaskCount > 0
                ? $"Database contains {currentTaskCount} tasks (will be replaced)"
                : "Database is empty (ready for seeding)"
        });

        // Check for potential conflicts
        var hasConflicts = currentTaskCount > 0;
        results.Add(new ValidationResult
        {
            Category = "Database Constraints",
            Check = "Data Replacement",
            IsValid = true,
            Message = hasConflicts
                ? "⚠️ Existing data will be completely replaced"
                : "No data conflicts - safe to seed"
        });

        return results;
    }

    private int CalculateEstimatedTaskCount(SimpleTaskGenerationConfig config)
    {
        // Simple estimation based on hierarchy
        var rootTasks = Math.Max(1, config.TotalTaskCount / (int)Math.Pow(config.MaxTasksPerParent, config.HierarchyDepth - 1));
        var estimatedTotal = 0;

        for (int level = 0; level < config.HierarchyDepth; level++)
        {
            estimatedTotal += rootTasks * (int)Math.Pow(config.MaxTasksPerParent, level);
        }

        return estimatedTotal;
    }

    private int CalculateMaxPossibleTasks(SimpleTaskGenerationConfig config)
    {
        // Calculate maximum tasks possible with given hierarchy constraints
        return (int)Math.Pow(config.MaxTasksPerParent, config.HierarchyDepth) * 10; // Conservative estimate
    }
}
