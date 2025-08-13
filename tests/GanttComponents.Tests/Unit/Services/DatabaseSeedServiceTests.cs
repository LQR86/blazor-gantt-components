using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using GanttComponents.Data;
using GanttComponents.Models;
using GanttComponents.Models.ValueObjects;
using GanttComponents.Services;
using Xunit;
using System.Text.Json;

namespace GanttComponents.Tests.Unit.Services;

public class DatabaseSeedServiceTests : IDisposable
{
    private readonly GanttDbContext _context;
    private readonly DatabaseSeedService _service;
    private readonly Mock<ILogger<DatabaseSeedService>> _loggerMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly string _tempDirectory;

    public DatabaseSeedServiceTests()
    {
        var options = new DbContextOptionsBuilder<GanttDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        _context = new GanttDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _loggerMock = new Mock<ILogger<DatabaseSeedService>>();
        _environmentMock = new Mock<IWebHostEnvironment>();

        // Create temporary directory for test files
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
        _environmentMock.Setup(x => x.ContentRootPath).Returns(_tempDirectory);

        _service = new DatabaseSeedService(_loggerMock.Object, _environmentMock.Object);
    }

    [Fact]
    public async Task SeedTasksFromJsonAsync_ShouldAddTasks_WhenValidJsonFile()
    {
        // Arrange
        var sampleTasks = new List<GanttTask>
        {
            new GanttTask
            {
                Id = 1,
                Name = "Test Task 1",
                Duration = "2d",
                StartDate = GanttDate.FromDateTime(DateTime.UtcNow.Date),
                EndDate = GanttDate.FromDateTime(DateTime.UtcNow.Date.AddDays(2)),
                TaskType = TaskType.FixedDuration
            },
            new GanttTask
            {
                Id = 2,
                Name = "Test Task 2",
                Duration = "3d",
                StartDate = GanttDate.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
                EndDate = GanttDate.FromDateTime(DateTime.UtcNow.Date.AddDays(6)),
                TaskType = TaskType.FixedWork
            }
        };

        var jsonContent = JsonSerializer.Serialize(sampleTasks, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new TaskTypeJsonConverter() }
        });

        var testFilePath = "test-tasks.json";
        var fullPath = Path.Combine(_tempDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, jsonContent);

        // Act
        await _service.SeedTasksFromJsonAsync(testFilePath, _context);

        // Assert
        var tasks = await _context.Tasks.ToListAsync();
        Assert.Equal(2, tasks.Count);
        Assert.Equal("Test Task 1", tasks.First(t => t.Id == 1).Name);
        Assert.Equal("Test Task 2", tasks.First(t => t.Id == 2).Name);
    }

    [Fact]
    public async Task SeedTasksFromJsonAsync_ShouldLogWarning_WhenFileNotFound()
    {
        // Arrange
        var nonExistentFile = "non-existent.json";

        // Act
        await _service.SeedTasksFromJsonAsync(nonExistentFile, _context);

        // Assert
        var tasks = await _context.Tasks.ToListAsync();
        Assert.Empty(tasks);

        // Verify warning was logged
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Seed file not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SeedTasksFromJsonAsync_ShouldClearExistingTasks_BeforeSeeding()
    {
        // Arrange
        // Add an existing task
        var existingTask = new GanttTask
        {
            Name = "Existing Task",
            Duration = "1d",
            StartDate = GanttDate.FromDateTime(DateTime.UtcNow.Date),
            EndDate = GanttDate.FromDateTime(DateTime.UtcNow.Date.AddDays(1))
        };
        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();

        // Create JSON file with new tasks
        var newTasks = new List<GanttTask>
        {
            new GanttTask
            {
                Id = 1,
                Name = "New Task",
                Duration = "2d",
                StartDate = GanttDate.FromDateTime(DateTime.UtcNow.Date),
                EndDate = GanttDate.FromDateTime(DateTime.UtcNow.Date.AddDays(2)),
                TaskType = TaskType.FixedDuration
            }
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new TaskTypeJsonConverter() }
        };
        var jsonContent = JsonSerializer.Serialize(newTasks, jsonOptions);
        var testFilePath = "new-tasks.json";
        var fullPath = Path.Combine(_tempDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, jsonContent);

        // Act
        await _service.SeedTasksFromJsonAsync(testFilePath, _context);

        // Assert
        var tasks = await _context.Tasks.ToListAsync();
        Assert.Single(tasks);
        Assert.Equal("New Task", tasks.First().Name);
        Assert.DoesNotContain(tasks, t => t.Name == "Existing Task");
    }

    [Fact]
    public async Task SeedTasksFromJsonAsync_ShouldLogWarning_WhenEmptyJsonArray()
    {
        // Arrange
        var jsonContent = "[]";
        var testFilePath = "empty-tasks.json";
        var fullPath = Path.Combine(_tempDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, jsonContent);

        // Act
        await _service.SeedTasksFromJsonAsync(testFilePath, _context);

        // Assert
        var tasks = await _context.Tasks.ToListAsync();
        Assert.Empty(tasks);

        // Verify warning was logged
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No tasks found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}
