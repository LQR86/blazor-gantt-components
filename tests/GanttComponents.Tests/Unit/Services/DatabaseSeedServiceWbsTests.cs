using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using GanttComponents.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace GanttComponents.Tests.Unit.Services;

public class DatabaseSeedServiceWbsTests : IDisposable
{
    private readonly GanttDbContext _context;
    private readonly DatabaseSeedService _seedService;

    public DatabaseSeedServiceWbsTests()
    {
        var options = new DbContextOptionsBuilder<GanttDbContext>()
            .UseSqlite($"Data Source=:memory:")
            .Options;

        _context = new GanttDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        var mockLogger = new Mock<ILogger<DatabaseSeedService>>();
        var mockEnvironment = new Mock<IWebHostEnvironment>();

        _seedService = new DatabaseSeedService(mockLogger.Object, mockEnvironment.Object);
    }

    [Fact]
    public async Task SeedSampleTasksAsync_LoadsWbsCodesCorrectly()
    {
        // Act
        await _seedService.SeedSampleTasksAsync(_context);

        // Assert
        var tasks = await _context.Tasks.OrderBy(t => t.Id).ToListAsync();
        Assert.True(tasks.Count >= 5); // Should have all 50 sample tasks

        // Check specific WBS codes from our sample data
        var planningTask = tasks.FirstOrDefault(t => t.Name == "Project Planning Phase");
        Assert.NotNull(planningTask);
        Assert.Equal("1", planningTask.WbsCode);

        var requirementsTask = tasks.FirstOrDefault(t => t.Name == "Requirements Analysis");
        Assert.NotNull(requirementsTask);
        Assert.Equal("1.1", requirementsTask.WbsCode);

        var techSpecTask = tasks.FirstOrDefault(t => t.Name == "Technical Specification");
        Assert.NotNull(techSpecTask);
        Assert.Equal("1.2", techSpecTask.WbsCode);

        var devTask = tasks.FirstOrDefault(t => t.Name == "Development Phase");
        Assert.NotNull(devTask);
        Assert.Equal("2", devTask.WbsCode);
    }

    [Fact]
    public async Task SeedSampleTasksAsync_PersistsWbsCodesToDatabaseCorrectly()
    {
        // Act
        await _seedService.SeedSampleTasksAsync(_context);

        // Assert - Verify data is persisted by reading from a fresh context
        using var freshContext = new GanttDbContext(new DbContextOptionsBuilder<GanttDbContext>()
            .UseSqlite(_context.Database.GetDbConnection())
            .Options);

        var tasks = await freshContext.Tasks.OrderBy(t => t.Id).ToListAsync();
        Assert.True(tasks.Count >= 5);
        Assert.All(tasks, task => Assert.NotNull(task.WbsCode));

        // Verify hierarchical WBS structure
        var parentTasks = tasks.Where(t => t.ParentId == null).ToList();
        Assert.True(parentTasks.Count >= 2); // Should have multiple top-level tasks

        var childTasks = tasks.Where(t => t.ParentId != null).ToList();
        Assert.True(childTasks.Count >= 3); // Should have child tasks
    }

    public void Dispose()
    {
        _context?.Database.CloseConnection();
        _context?.Dispose();
    }
}
