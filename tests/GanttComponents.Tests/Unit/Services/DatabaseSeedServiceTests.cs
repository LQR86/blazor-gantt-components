using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using GanttComponents.Data;
using GanttComponents.Models;
using GanttComponents.Models.ValueObjects;
using GanttComponents.Services;
using Xunit;

namespace GanttComponents.Tests.Unit.Services;

public class DatabaseSeedServiceTests : IDisposable
{
    private readonly GanttDbContext _context;
    private readonly DatabaseSeedService _service;
    private readonly Mock<ILogger<DatabaseSeedService>> _loggerMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;

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

        _service = new DatabaseSeedService(_loggerMock.Object, _environmentMock.Object);
    }

    [Fact]
    public async Task SeedSampleTasksAsync_ShouldAddTasks_WhenDatabaseIsEmpty()
    {
        // Act
        await _service.SeedSampleTasksAsync(_context);

        // Assert
        var tasks = await _context.Tasks.ToListAsync();
        Assert.True(tasks.Count >= 5); // Should have at least the sample tasks
        Assert.Contains(tasks, t => t.Name == "Project Planning Phase");
        Assert.Contains(tasks, t => t.Name == "Requirements Analysis");
        Assert.Contains(tasks, t => t.Name == "Development Phase");
    }

    [Fact]
    public async Task SeedSampleTasksAsync_ShouldSkipSeeding_WhenTasksAlreadyExist()
    {
        // Arrange - Add an existing task
        var existingTask = new GanttTask
        {
            Name = "Existing Task",
            Duration = "1d",
            StartDate = GanttDate.Parse("2025-01-01"),
            EndDate = GanttDate.Parse("2025-01-02")
        };
        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();

        // Act
        await _service.SeedSampleTasksAsync(_context);

        // Assert
        var tasks = await _context.Tasks.ToListAsync();
        Assert.Single(tasks);
        Assert.Equal("Existing Task", tasks.First().Name);
    }

    [Fact]
    public async Task SeedSampleTasksAsync_ShouldLogSuccess_WhenSeedingCompletes()
    {
        // Act
        await _service.SeedSampleTasksAsync(_context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully seeded")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SeedSampleTasksAsync_ShouldLogSkip_WhenTasksAlreadyExist()
    {
        // Arrange - Add an existing task
        var existingTask = new GanttTask
        {
            Name = "Existing Task",
            Duration = "1d",
            StartDate = GanttDate.Parse("2025-01-01"),
            EndDate = GanttDate.Parse("2025-01-02")
        };
        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();

        // Act
        await _service.SeedSampleTasksAsync(_context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("already exist, skipping seed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }
}
