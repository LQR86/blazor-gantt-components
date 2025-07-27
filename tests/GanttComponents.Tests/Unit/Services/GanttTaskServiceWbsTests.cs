using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using GanttComponents.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GanttComponents.Tests.Unit.Services;

public class GanttTaskServiceWbsTests : IDisposable
{
    private readonly GanttDbContext _context;
    private readonly GanttTaskService _taskService;
    private readonly Mock<ILogger<GanttTaskService>> _mockLogger;

    public GanttTaskServiceWbsTests()
    {
        var options = new DbContextOptionsBuilder<GanttDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        _context = new GanttDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _mockLogger = new Mock<ILogger<GanttTaskService>>();
        _taskService = new GanttTaskService(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllTasksAsync_ReturnsTasksWithWbsCodes()
    {
        // Arrange
        var tasks = new List<GanttTask>
        {
            new GanttTask
            {
                Id = 1,
                Name = "Task 1",
                WbsCode = "1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Duration = "1d",
                TaskType = TaskType.FixedDuration
            },
            new GanttTask
            {
                Id = 2,
                Name = "Task 1.1",
                WbsCode = "1.1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Duration = "1d",
                TaskType = TaskType.FixedDuration,
                ParentId = 1
            }
        };

        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.GetAllTasksAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("1", result[0].WbsCode);
        Assert.Equal("1.1", result[1].WbsCode);
    }

    [Fact]
    public async Task ValidateWbsCodeUniquenessAsync_ReturnsTrueForUniqueCode()
    {
        // Arrange
        var existingTask = new GanttTask
        {
            Id = 1,
            Name = "Existing Task",
            WbsCode = "1",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
        };

        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.ValidateWbsCodeUniquenessAsync("2");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateWbsCodeUniquenessAsync_ReturnsFalseForDuplicateCode()
    {
        // Arrange
        var existingTask = new GanttTask
        {
            Id = 1,
            Name = "Existing Task",
            WbsCode = "1",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
        };

        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.ValidateWbsCodeUniquenessAsync("1");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateWbsCodeUniquenessAsync_IgnoresExcludedTask()
    {
        // Arrange
        var existingTask = new GanttTask
        {
            Id = 1,
            Name = "Existing Task",
            WbsCode = "1",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
        };

        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();

        // Act - Check if WBS code "1" is unique, excluding task with ID 1
        var result = await _taskService.ValidateWbsCodeUniquenessAsync("1", 1);

        // Assert
        Assert.True(result); // Should be true because we're excluding the task that has this WBS code
    }

    [Fact]
    public async Task CreateTaskAsync_ThrowsExceptionForDuplicateWbsCode()
    {
        // Arrange
        var existingTask = new GanttTask
        {
            Id = 1,
            Name = "Existing Task",
            WbsCode = "1",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
        };

        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();

        var newTask = new GanttTask
        {
            Name = "New Task",
            WbsCode = "1", // Duplicate WBS code
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _taskService.CreateTaskAsync(newTask));

        Assert.Contains("WBS code '1' already exists", exception.Message);
    }

    [Fact]
    public async Task UpdateTaskAsync_ThrowsExceptionForDuplicateWbsCode()
    {
        // Arrange
        var task1 = new GanttTask
        {
            Id = 1,
            Name = "Task 1",
            WbsCode = "1",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
        };

        var task2 = new GanttTask
        {
            Id = 2,
            Name = "Task 2",
            WbsCode = "2",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
        };

        _context.Tasks.AddRange(task1, task2);
        await _context.SaveChangesAsync();

        // Try to update task2 to have the same WBS code as task1
        task2.WbsCode = "1";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _taskService.UpdateTaskAsync(task2));

        Assert.Contains("WBS code '1' already exists", exception.Message);
    }

    public void Dispose()
    {
        _context?.Database.CloseConnection();
        _context?.Dispose();
    }
}
