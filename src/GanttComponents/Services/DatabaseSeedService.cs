using GanttComponents.Models;
using GanttComponents.Models.ValueObjects;
using GanttComponents.Data;

namespace GanttComponents.Services;

public interface IDatabaseSeedService
{
    Task SeedSampleTasksAsync();
    Task SeedSampleTasksAsync(GanttDbContext context);
}

public class DatabaseSeedService : IDatabaseSeedService
{
    private readonly ILogger<DatabaseSeedService> _logger;
    private readonly IWebHostEnvironment _environment;

    public DatabaseSeedService(ILogger<DatabaseSeedService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public Task SeedSampleTasksAsync()
    {
        // This overload would require a DbContext from DI, but since we're using it during startup,
        // we'll use the other overload that takes context as parameter
        throw new NotImplementedException("Use the overload that takes GanttDbContext as parameter");
    }

    public async Task SeedSampleTasksAsync(GanttDbContext context)
    {
        try
        {
            // Check if tasks already exist
            if (context.Tasks.Any())
            {
                _logger.LogInformation("Sample tasks already exist, skipping seed");
                return;
            }

            var sampleTasks = CreateSampleTasks();

            // Add new tasks
            context.Tasks.AddRange(sampleTasks);
            await context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {Count} sample tasks", sampleTasks.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding sample tasks");
            throw;
        }
    }

    private List<GanttTask> CreateSampleTasks()
    {
        return new List<GanttTask>
        {
            // Project Planning Phase
            new GanttTask { Id = 1, Name = "Project Planning Phase", StartDate = GanttDate.Parse("2025-01-01"), EndDate = GanttDate.Parse("2025-01-10"), Duration = "10d", WbsCode = "1", ParentId = null, Predecessors = "", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 2, Name = "Requirements Analysis", StartDate = GanttDate.Parse("2025-01-01"), EndDate = GanttDate.Parse("2025-01-05"), Duration = "5d", WbsCode = "1.1", ParentId = 1, Predecessors = "", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 3, Name = "Stakeholder Interviews", StartDate = GanttDate.Parse("2025-01-01"), EndDate = GanttDate.Parse("2025-01-03"), Duration = "3d", WbsCode = "1.1.1", ParentId = 2, Predecessors = "", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 4, Name = "Requirements Documentation", StartDate = GanttDate.Parse("2025-01-04"), EndDate = GanttDate.Parse("2025-01-05"), Duration = "2d", WbsCode = "1.1.2", ParentId = 2, Predecessors = "3FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 5, Name = "Technical Specification", StartDate = GanttDate.Parse("2025-01-06"), EndDate = GanttDate.Parse("2025-01-10"), Duration = "5d", WbsCode = "1.2", ParentId = 1, Predecessors = "2FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 6, Name = "Architecture Design", StartDate = GanttDate.Parse("2025-01-06"), EndDate = GanttDate.Parse("2025-01-08"), Duration = "3d", WbsCode = "1.2.1", ParentId = 5, Predecessors = "2FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 7, Name = "Database Design", StartDate = GanttDate.Parse("2025-01-09"), EndDate = GanttDate.Parse("2025-01-10"), Duration = "2d", WbsCode = "1.2.2", ParentId = 5, Predecessors = "6FS", TaskType = TaskType.FixedDuration },

            // Development Phase
            new GanttTask { Id = 8, Name = "Development Phase", StartDate = GanttDate.Parse("2025-01-13"), EndDate = GanttDate.Parse("2025-02-11"), Duration = "30d", WbsCode = "2", ParentId = null, Predecessors = "1FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 9, Name = "Frontend Development", StartDate = GanttDate.Parse("2025-01-13"), EndDate = GanttDate.Parse("2025-02-01"), Duration = "20d", WbsCode = "2.1", ParentId = 8, Predecessors = "5FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 10, Name = "UI Components", StartDate = GanttDate.Parse("2025-01-13"), EndDate = GanttDate.Parse("2025-01-22"), Duration = "10d", WbsCode = "2.1.1", ParentId = 9, Predecessors = "5FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 11, Name = "TaskGrid Component", StartDate = GanttDate.Parse("2025-01-13"), EndDate = GanttDate.Parse("2025-01-17"), Duration = "5d", WbsCode = "2.1.1.1", ParentId = 10, Predecessors = "5FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 12, Name = "TimelineView Component", StartDate = GanttDate.Parse("2025-01-18"), EndDate = GanttDate.Parse("2025-01-22"), Duration = "5d", WbsCode = "2.1.1.2", ParentId = 10, Predecessors = "11FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 13, Name = "GanttComposer Component", StartDate = GanttDate.Parse("2025-01-23"), EndDate = GanttDate.Parse("2025-01-27"), Duration = "5d", WbsCode = "2.1.1.3", ParentId = 10, Predecessors = "12FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 14, Name = "State Management", StartDate = GanttDate.Parse("2025-01-23"), EndDate = GanttDate.Parse("2025-02-01"), Duration = "10d", WbsCode = "2.1.2", ParentId = 9, Predecessors = "10FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 15, Name = "Task Operations", StartDate = GanttDate.Parse("2025-01-23"), EndDate = GanttDate.Parse("2025-01-27"), Duration = "5d", WbsCode = "2.1.2.1", ParentId = 14, Predecessors = "10FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 16, Name = "Dependency Management", StartDate = GanttDate.Parse("2025-01-28"), EndDate = GanttDate.Parse("2025-02-01"), Duration = "5d", WbsCode = "2.1.2.2", ParentId = 14, Predecessors = "15FS", TaskType = TaskType.FixedDuration },

            // Backend Development
            new GanttTask { Id = 17, Name = "Backend Development", StartDate = GanttDate.Parse("2025-01-13"), EndDate = GanttDate.Parse("2025-02-06"), Duration = "25d", WbsCode = "2.2", ParentId = 8, Predecessors = "7FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 18, Name = "API Development", StartDate = GanttDate.Parse("2025-01-13"), EndDate = GanttDate.Parse("2025-01-27"), Duration = "15d", WbsCode = "2.2.1", ParentId = 17, Predecessors = "7FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 19, Name = "Task API Endpoints", StartDate = GanttDate.Parse("2025-01-13"), EndDate = GanttDate.Parse("2025-01-17"), Duration = "5d", WbsCode = "2.2.1.1", ParentId = 18, Predecessors = "7FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 20, Name = "Dependency API Endpoints", StartDate = GanttDate.Parse("2025-01-18"), EndDate = GanttDate.Parse("2025-01-22"), Duration = "5d", WbsCode = "2.2.1.2", ParentId = 18, Predecessors = "19FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 21, Name = "Resource API Endpoints", StartDate = GanttDate.Parse("2025-01-23"), EndDate = GanttDate.Parse("2025-01-27"), Duration = "5d", WbsCode = "2.2.1.3", ParentId = 18, Predecessors = "20FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 22, Name = "Database Implementation", StartDate = GanttDate.Parse("2025-01-28"), EndDate = GanttDate.Parse("2025-02-06"), Duration = "10d", WbsCode = "2.2.2", ParentId = 17, Predecessors = "18FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 23, Name = "Entity Framework Setup", StartDate = GanttDate.Parse("2025-01-28"), EndDate = GanttDate.Parse("2025-01-30"), Duration = "3d", WbsCode = "2.2.2.1", ParentId = 22, Predecessors = "18FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 24, Name = "Data Migrations", StartDate = GanttDate.Parse("2025-01-31"), EndDate = GanttDate.Parse("2025-02-01"), Duration = "2d", WbsCode = "2.2.2.2", ParentId = 22, Predecessors = "23FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 25, Name = "Repository Pattern", StartDate = GanttDate.Parse("2025-02-02"), EndDate = GanttDate.Parse("2025-02-06"), Duration = "5d", WbsCode = "2.2.2.3", ParentId = 22, Predecessors = "24FS", TaskType = TaskType.FixedDuration },

            // Testing Phase
            new GanttTask { Id = 26, Name = "Testing Phase", StartDate = GanttDate.Parse("2025-02-07"), EndDate = GanttDate.Parse("2025-02-21"), Duration = "15d", WbsCode = "3", ParentId = null, Predecessors = "8FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 27, Name = "Unit Testing", StartDate = GanttDate.Parse("2025-02-07"), EndDate = GanttDate.Parse("2025-02-14"), Duration = "8d", WbsCode = "3.1", ParentId = 26, Predecessors = "8FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 28, Name = "Component Tests", StartDate = GanttDate.Parse("2025-02-07"), EndDate = GanttDate.Parse("2025-02-10"), Duration = "4d", WbsCode = "3.1.1", ParentId = 27, Predecessors = "13FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 29, Name = "Service Tests", StartDate = GanttDate.Parse("2025-02-11"), EndDate = GanttDate.Parse("2025-02-14"), Duration = "4d", WbsCode = "3.1.2", ParentId = 27, Predecessors = "28FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 30, Name = "Integration Testing", StartDate = GanttDate.Parse("2025-02-15"), EndDate = GanttDate.Parse("2025-02-21"), Duration = "7d", WbsCode = "3.2", ParentId = 26, Predecessors = "27FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 31, Name = "API Integration Tests", StartDate = GanttDate.Parse("2025-02-15"), EndDate = GanttDate.Parse("2025-02-17"), Duration = "3d", WbsCode = "3.2.1", ParentId = 30, Predecessors = "25FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 32, Name = "End-to-End Tests", StartDate = GanttDate.Parse("2025-02-18"), EndDate = GanttDate.Parse("2025-02-21"), Duration = "4d", WbsCode = "3.2.2", ParentId = 30, Predecessors = "31FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 33, Name = "Performance Testing", StartDate = GanttDate.Parse("2025-02-22"), EndDate = GanttDate.Parse("2025-02-26"), Duration = "5d", WbsCode = "3.3", ParentId = 26, Predecessors = "30FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 34, Name = "Load Testing", StartDate = GanttDate.Parse("2025-02-22"), EndDate = GanttDate.Parse("2025-02-24"), Duration = "3d", WbsCode = "3.3.1", ParentId = 33, Predecessors = "30FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 35, Name = "Stress Testing", StartDate = GanttDate.Parse("2025-02-25"), EndDate = GanttDate.Parse("2025-02-26"), Duration = "2d", WbsCode = "3.3.2", ParentId = 33, Predecessors = "34FS", TaskType = TaskType.FixedDuration },

            // Documentation Phase
            new GanttTask { Id = 36, Name = "Documentation Phase", StartDate = GanttDate.Parse("2025-02-27"), EndDate = GanttDate.Parse("2025-03-08"), Duration = "10d", WbsCode = "4", ParentId = null, Predecessors = "26FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 37, Name = "API Documentation", StartDate = GanttDate.Parse("2025-02-27"), EndDate = GanttDate.Parse("2025-03-03"), Duration = "5d", WbsCode = "4.1", ParentId = 36, Predecessors = "21FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 38, Name = "User Manual", StartDate = GanttDate.Parse("2025-03-04"), EndDate = GanttDate.Parse("2025-03-08"), Duration = "5d", WbsCode = "4.2", ParentId = 36, Predecessors = "37FS", TaskType = TaskType.FixedDuration },

            // Deployment Phase
            new GanttTask { Id = 39, Name = "Deployment Phase", StartDate = GanttDate.Parse("2025-03-09"), EndDate = GanttDate.Parse("2025-03-16"), Duration = "8d", WbsCode = "5", ParentId = null, Predecessors = "36FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 40, Name = "Environment Setup", StartDate = GanttDate.Parse("2025-03-09"), EndDate = GanttDate.Parse("2025-03-11"), Duration = "3d", WbsCode = "5.1", ParentId = 39, Predecessors = "33FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 41, Name = "Production Deployment", StartDate = GanttDate.Parse("2025-03-12"), EndDate = GanttDate.Parse("2025-03-13"), Duration = "2d", WbsCode = "5.2", ParentId = 39, Predecessors = "40FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 42, Name = "Go-Live Activities", StartDate = GanttDate.Parse("2025-03-14"), EndDate = GanttDate.Parse("2025-03-16"), Duration = "3d", WbsCode = "5.3", ParentId = 39, Predecessors = "41FS", TaskType = TaskType.FixedDuration },

            // Quality Assurance
            new GanttTask { Id = 43, Name = "Quality Assurance", StartDate = GanttDate.Parse("2025-02-01"), EndDate = GanttDate.Parse("2025-02-20"), Duration = "20d", WbsCode = "6", ParentId = null, Predecessors = "", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 44, Name = "Code Review Process", StartDate = GanttDate.Parse("2025-02-01"), EndDate = GanttDate.Parse("2025-02-15"), Duration = "15d", WbsCode = "6.1", ParentId = 43, Predecessors = "16FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 45, Name = "Security Audit", StartDate = GanttDate.Parse("2025-02-16"), EndDate = GanttDate.Parse("2025-02-20"), Duration = "5d", WbsCode = "6.2", ParentId = 43, Predecessors = "44FS", TaskType = TaskType.FixedDuration },

            // Risk Management
            new GanttTask { Id = 46, Name = "Risk Management", StartDate = GanttDate.Parse("2025-01-15"), EndDate = GanttDate.Parse("2025-02-28"), Duration = "45d", WbsCode = "7", ParentId = null, Predecessors = "", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 47, Name = "Risk Assessment", StartDate = GanttDate.Parse("2025-01-15"), EndDate = GanttDate.Parse("2025-01-19"), Duration = "5d", WbsCode = "7.1", ParentId = 46, Predecessors = "9FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 48, Name = "Mitigation Strategies", StartDate = GanttDate.Parse("2025-01-20"), EndDate = GanttDate.Parse("2025-01-29"), Duration = "10d", WbsCode = "7.2", ParentId = 46, Predecessors = "47FS", TaskType = TaskType.FixedDuration },
            new GanttTask { Id = 49, Name = "Monitoring & Control", StartDate = GanttDate.Parse("2025-01-30"), EndDate = GanttDate.Parse("2025-02-28"), Duration = "30d", WbsCode = "7.3", ParentId = 46, Predecessors = "48FS", TaskType = TaskType.FixedDuration },

            // Project Closure
            new GanttTask { Id = 50, Name = "Project Closure", StartDate = GanttDate.Parse("2025-03-17"), EndDate = GanttDate.Parse("2025-03-19"), Duration = "3d", WbsCode = "8", ParentId = null, Predecessors = "42FS+39FS", TaskType = TaskType.FixedDuration }
        };
    }
}
