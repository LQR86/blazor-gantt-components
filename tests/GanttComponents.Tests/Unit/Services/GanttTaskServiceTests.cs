using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GanttComponents.Data;
using GanttComponents.Models;
using GanttComponents.Services;
using Moq;
using Xunit;

namespace GanttComponents.Tests.Unit.Services;

public class GanttTaskServiceTests : IDisposable
{
    private readonly GanttDbContext _context;
    private readonly GanttTaskService _service;
    private readonly Mock<ILogger<GanttTaskService>> _mockLogger;
    private readonly Mock<IUniversalLogger> _mockUniversalLogger;
    private readonly Mock<IWbsCodeGenerationService> _mockWbsService;

    public GanttTaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<GanttDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        _context = new GanttDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _mockLogger = new Mock<ILogger<GanttTaskService>>();
        _mockUniversalLogger = new Mock<IUniversalLogger>();
        _mockWbsService = new Mock<IWbsCodeGenerationService>();
        _service = new GanttTaskService(_context, _mockLogger.Object, _mockUniversalLogger.Object, _mockWbsService.Object);
    }

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnEmptyList_WhenNoTasks()
    {
        // Act
        var result = await _service.GetAllTasksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldAddTask_AndReturnTaskWithId()
    {
        // Arrange
        var task = new GanttTask
        {
            Name = "Test Task",
            Duration = "3d",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(3)
        };

        // Act
        var result = await _service.CreateTaskAsync(task);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Name);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var task = new GanttTask
        {
            Name = "Test Task",
            Duration = "2d",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(2)
        };
        var createdTask = await _service.CreateTaskAsync(task);

        // Act
        var result = await _service.GetTaskByIdAsync(createdTask.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdTask.Id, result.Id);
        Assert.Equal("Test Task", result.Name);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Act
        var result = await _service.GetTaskByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTask_WhenTaskExists()
    {
        // Arrange
        var task = new GanttTask
        {
            Name = "Original Task",
            Duration = "1d",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(1)
        };
        var createdTask = await _service.CreateTaskAsync(task);

        // Act
        createdTask.Name = "Updated Task";
        createdTask.Duration = "2d";
        var result = await _service.UpdateTaskAsync(createdTask);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Task", result.Name);
        Assert.Equal("2d", result.Duration);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldRemoveTask_WhenTaskExists()
    {
        // Arrange
        var task = new GanttTask
        {
            Name = "Task to Delete",
            Duration = "1d",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(1)
        };
        var createdTask = await _service.CreateTaskAsync(task);

        // Act
        await _service.DeleteTaskAsync(createdTask.Id);

        // Assert
        var deletedTask = await _service.GetTaskByIdAsync(createdTask.Id);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldNotThrow_WhenTaskDoesNotExist()
    {
        // Act & Assert - Should not throw
        await _service.DeleteTaskAsync(999);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }
}
