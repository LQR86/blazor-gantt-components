using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using GanttComponents.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace GanttComponents.Tests.Unit.Services;

public class SampleDataValidationTests
{
    [Fact]
    public async Task SampleData_HasValidWbsCodes()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = CreateSeedService();

        // Act
        await service.SeedSampleTasksAsync(context);
        var tasks = await context.Tasks.ToListAsync();

        // Assert
        Assert.True(tasks.Count >= 5);
        Assert.All(tasks, task => Assert.False(string.IsNullOrEmpty(task.WbsCode)));

        // Verify specific WBS structure
        var planningPhase = tasks.FirstOrDefault(t => t.Name == "Project Planning Phase");
        Assert.NotNull(planningPhase);
        Assert.Equal("1", planningPhase.WbsCode);

        var requirements = tasks.FirstOrDefault(t => t.Name == "Requirements Analysis");
        Assert.NotNull(requirements);
        Assert.Equal("1.1", requirements.WbsCode);
        Assert.Equal(planningPhase.Id, requirements.ParentId);

        context.Dispose();
    }

    [Fact]
    public async Task SampleData_HasValidHierarchy()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = CreateSeedService();

        // Act
        await service.SeedSampleTasksAsync(context);
        var tasks = await context.Tasks.ToListAsync();

        // Assert
        var parentTasks = tasks.Where(t => t.ParentId == null).ToList();
        var childTasks = tasks.Where(t => t.ParentId != null).ToList();

        Assert.True(parentTasks.Count >= 2);
        Assert.True(childTasks.Count >= 3);

        // Verify all child tasks have valid parent references
        foreach (var child in childTasks)
        {
            var parent = tasks.FirstOrDefault(t => t.Id == child.ParentId);
            Assert.NotNull(parent);
        }

        context.Dispose();
    }

    [Fact]
    public async Task SampleData_HasValidDates()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = CreateSeedService();

        // Act
        await service.SeedSampleTasksAsync(context);
        var tasks = await context.Tasks.ToListAsync();

        // Assert
        Assert.All(tasks, task =>
        {
            Assert.True(task.StartDate.ToUtcDateTime() <= task.EndDate.ToUtcDateTime());
            Assert.False(string.IsNullOrEmpty(task.Duration));
        });

        context.Dispose();
    }

    private GanttDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GanttDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        var context = new GanttDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

    private DatabaseSeedService CreateSeedService()
    {
        var mockLogger = new Mock<ILogger<DatabaseSeedService>>();
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        return new DatabaseSeedService(mockLogger.Object, mockEnvironment.Object);
    }
}
