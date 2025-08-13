using Xunit;
using Microsoft.EntityFrameworkCore;
using GanttComponents.Data;
using GanttComponents.Data.Converters;
using GanttComponents.Models;
using GanttComponents.Models.ValueObjects;
using GanttComponents.Services;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;

namespace GanttComponents.Tests.Integration.Data;

/// <summary>
/// Integration tests demonstrating the database choke point design.
/// These tests prove that the database layer enforces UTC date-only constraints
/// regardless of what data the application tries to store.
/// 
/// DESIGN VALIDATION: These tests demonstrate that the database acts as the
/// ultimate choke point - no invalid date data can be persisted, and all
/// retrieved data is guaranteed to be UTC date-only.
/// </summary>
public class DatabaseChokePointTests : IDisposable
{
    private readonly GanttDbContext _context;
    private readonly DbContextOptions<GanttDbContext> _options;
    private readonly IUniversalLogger _logger;

    public DatabaseChokePointTests()
    {
        // Setup logging for detailed debugging
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
            .AddScoped<IUniversalLogger, UniversalLogger>()
            .BuildServiceProvider();
        
        _logger = serviceProvider.GetRequiredService<IUniversalLogger>();

        // Use temporary file database instead of in-memory to match production behavior
        var tempDbPath = Path.GetTempFileName().Replace(".tmp", ".db");
        var connectionString = $"Data Source={tempDbPath}";
        _options = new DbContextOptionsBuilder<GanttDbContext>()
            .UseSqlite(connectionString)
            .LogTo(message => _logger.LogDebugInfo($"EF CORE: {message}"))
            .EnableSensitiveDataLogging()
            .Options;

        _context = new GanttDbContext(_options);
        _context.Database.EnsureCreated();
        
        _logger.LogInfo("DatabaseChokePointTests initialized with temporary SQLite file database");
    }

    #region Database Schema Enforcement Tests

    [Fact]
    public void Database_SaveDateTimeWithTime_StripsTimeComponent()
    {
        // Arrange: GanttTask with DateTime that has time components
        var taskWithTime = new GanttTask
        {
            Name = "Test Task",
            WbsCode = "1.0",
            // CHOKE POINT INPUT: DateTime with significant time components
            StartDate = new DateTime(2025, 8, 13, 14, 30, 45, 123, DateTimeKind.Local),
            EndDate = new DateTime(2025, 8, 15, 23, 59, 59, 999, DateTimeKind.Unspecified),
            Duration = "3d"
        };

        // Act: CHOKE POINT - Save through database layer
        _context.Tasks.Add(taskWithTime);
        _context.SaveChanges();

        // Clear context to force database read
        _context.Entry(taskWithTime).State = EntityState.Detached;

        // Retrieve from database
        var retrievedTask = _context.Tasks.First(t => t.WbsCode == "1.0");

        // Assert: Database enforced UTC date-only constraints
        Assert.Equal(DateTimeKind.Utc, retrievedTask.StartDate.Kind);
        Assert.Equal(DateTimeKind.Utc, retrievedTask.EndDate.Kind);
        Assert.Equal(TimeSpan.Zero, retrievedTask.StartDate.TimeOfDay);
        Assert.Equal(TimeSpan.Zero, retrievedTask.EndDate.TimeOfDay);

        // Dates should be preserved (adjusted for UTC)
        Assert.Equal(2025, retrievedTask.StartDate.Year);
        Assert.Equal(8, retrievedTask.StartDate.Month);
        Assert.Equal(13, retrievedTask.StartDate.Day);
    }

    [Fact]
    public void Database_SaveAndRetrieveMultipleTasks_AllDatesUtcOnly()
    {
        // Arrange: Multiple tasks with various DateTime issues
        var tasksWithProblematicDates = new[]
        {
            new GanttTask
            {
                Name = "Local Time Task",
                WbsCode = "1.1",
                StartDate = new DateTime(2025, 1, 1, 9, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2025, 1, 5, 17, 30, 0, DateTimeKind.Local),
                Duration = "5d"
            },
            new GanttTask
            {
                Name = "Unspecified Time Task",
                WbsCode = "1.2",
                StartDate = new DateTime(2025, 2, 1, 12, 15, 30, DateTimeKind.Unspecified),
                EndDate = new DateTime(2025, 2, 10, 14, 45, 15, DateTimeKind.Unspecified),
                Duration = "10d"
            },
            new GanttTask
            {
                Name = "Already UTC Task",
                WbsCode = "1.3",
                StartDate = new DateTime(2025, 3, 1, 8, 20, 10, DateTimeKind.Utc),
                EndDate = new DateTime(2025, 3, 7, 16, 40, 50, DateTimeKind.Utc),
                Duration = "7d"
            }
        };

        // Act: CHOKE POINT - Save all tasks through database
        _context.Tasks.AddRange(tasksWithProblematicDates);
        _context.SaveChanges();

        // Clear tracking to force fresh database reads
        _context.ChangeTracker.Clear();

        // Retrieve all tasks from database
        var retrievedTasks = _context.Tasks.OrderBy(t => t.WbsCode).ToList();

        // Assert: ALL dates are UTC date-only regardless of input
        foreach (var task in retrievedTasks)
        {
            Assert.Equal(DateTimeKind.Utc, task.StartDate.Kind);
            Assert.Equal(DateTimeKind.Utc, task.EndDate.Kind);
            Assert.Equal(TimeSpan.Zero, task.StartDate.TimeOfDay);
            Assert.Equal(TimeSpan.Zero, task.EndDate.TimeOfDay);
        }

        // Verify specific date preservation
        Assert.Equal(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), retrievedTasks[0].StartDate);
        Assert.Equal(new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc), retrievedTasks[1].StartDate);
        Assert.Equal(new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc), retrievedTasks[2].StartDate);
    }

    #endregion

    #region Database Converter Choke Point Tests

    [Fact]
    public void Database_ConverterHandlesEdgeCases_Correctly()
    {
        // Arrange: Tasks with edge case dates
        var edgeCaseTasks = new[]
        {
            new GanttTask
            {
                Name = "Min Date Task",
                WbsCode = "2.1",
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MinValue.AddDays(1),
                Duration = "1d"
            },
            new GanttTask
            {
                Name = "Max Date Task",
                WbsCode = "2.2",
                StartDate = DateTime.MaxValue.AddDays(-1),
                EndDate = DateTime.MaxValue,
                Duration = "1d"
            },
            new GanttTask
            {
                Name = "Midnight UTC Task",
                WbsCode = "2.3",
                StartDate = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2025, 6, 20, 0, 0, 0, DateTimeKind.Utc),
                Duration = "5d"
            }
        };

        // Act: CHOKE POINT - Database handles edge cases
        _context.Tasks.AddRange(edgeCaseTasks);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();

        var retrievedTasks = _context.Tasks.Where(t => t.WbsCode.StartsWith("2.")).OrderBy(t => t.WbsCode).ToList();

        // Assert: Edge cases handled correctly
        foreach (var task in retrievedTasks)
        {
            Assert.Equal(DateTimeKind.Utc, task.StartDate.Kind);
            Assert.Equal(DateTimeKind.Utc, task.EndDate.Kind);
            Assert.Equal(TimeSpan.Zero, task.StartDate.TimeOfDay);
            Assert.Equal(TimeSpan.Zero, task.EndDate.TimeOfDay);
        }

        // Verify midnight times are preserved correctly
        var midnightTask = retrievedTasks.First(t => t.WbsCode == "2.3");
        Assert.Equal(new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc), midnightTask.StartDate);
        Assert.Equal(new DateTime(2025, 6, 20, 0, 0, 0, DateTimeKind.Utc), midnightTask.EndDate);
    }

    #endregion

    #region Database Transaction Choke Point Tests

    [Fact]
    public void Database_ConverterEnforcesUtcConstraints_WhenStoringAndRetrievingDates()
    {
        // Arrange: Task with problematic date
        var problematicTask = new GanttTask
        {
            Name = "Transaction Test Task",
            WbsCode = "3.1",
            StartDate = new DateTime(2025, 8, 13, 15, 30, 45, DateTimeKind.Local),
            EndDate = new DateTime(2025, 8, 18, 9, 15, 30, DateTimeKind.Local),
            Duration = "5d"
        };

        _logger.LogDatabaseOperation("Starting transaction rollback test", new {
            InputStartDate = problematicTask.StartDate,
            InputStartDateKind = problematicTask.StartDate.Kind,
            InputEndDate = problematicTask.EndDate,
            InputEndDateKind = problematicTask.EndDate.Kind
        });

        // Act: CHOKE POINT - Transaction handling
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _logger.LogDatabaseOperation("Adding task to context");
            _context.Tasks.Add(problematicTask);
            
            _logger.LogDatabaseOperation("Saving changes");
            _context.SaveChanges();

            // Test converter within the same transaction first (using change tracker)
            var taskFromTracker = _context.Tasks.First(t => t.WbsCode == "3.1");
            _logger.LogDatabaseOperation("Task from change tracker", new {
                TrackerStartDate = taskFromTracker.StartDate,
                TrackerStartDateKind = taskFromTracker.StartDate.Kind,
                TrackerTimeOfDay = taskFromTracker.StartDate.TimeOfDay
            });

            // Commit the transaction so we can test database conversion with fresh context
            _logger.LogDatabaseOperation("Committing transaction to test database converter");
            transaction.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception during transaction test", ex);
            transaction.Rollback();
            throw;
        }

        // Now test with fresh context - this will go through database converter
        using var freshContext = new GanttDbContext(_options);
        var taskFromDatabase = freshContext.Tasks.First(t => t.WbsCode == "3.1");
        
        _logger.LogDatabaseOperation("Retrieved task from database via converter", new {
            DatabaseStartDate = taskFromDatabase.StartDate,
            DatabaseStartDateKind = taskFromDatabase.StartDate.Kind,
            DatabaseTimeOfDay = taskFromDatabase.StartDate.TimeOfDay,
            DatabaseEndDate = taskFromDatabase.EndDate,
            DatabaseEndDateKind = taskFromDatabase.EndDate.Kind
        });

        // Verify data is converted correctly by the database converter
        Assert.Equal(DateTimeKind.Utc, taskFromDatabase.StartDate.Kind);
        Assert.Equal(TimeSpan.Zero, taskFromDatabase.StartDate.TimeOfDay);

        // Assert: Data was properly persisted and converted
        Assert.True(_context.Tasks.Any(t => t.WbsCode == "3.1"), "Task should be persisted after transaction commit");
        
        // Clean up the test data
        _context.Tasks.RemoveRange(_context.Tasks.Where(t => t.WbsCode == "3.1"));
        _context.SaveChanges();
        _logger.LogDatabaseOperation("Transaction rollback test completed successfully");
    }

    #endregion

    #region Bulk Operations Choke Point Tests

    [Fact]
    public void Database_BulkInsert_EnforcesConstraintsOnAllRecords()
    {
        // Arrange: Large batch of tasks with various date issues
        var bulkTasks = Enumerable.Range(1, 50).Select(i => new GanttTask
        {
            Name = $"Bulk Task {i}",
            WbsCode = $"BULK.{i}",
            // Vary the DateTimeKind and time components to test choke point
            StartDate = new DateTime(2025, 1, i % 28 + 1, i % 24, i % 60, i % 60,
                i % 3 == 0 ? DateTimeKind.Utc :
                i % 3 == 1 ? DateTimeKind.Local : DateTimeKind.Unspecified),
            EndDate = new DateTime(2025, 2, (i % 28) + 1, (i + 12) % 24, (i + 30) % 60, (i + 45) % 60,
                (i + 1) % 3 == 0 ? DateTimeKind.Utc :
                (i + 1) % 3 == 1 ? DateTimeKind.Local : DateTimeKind.Unspecified),
            Duration = $"{i % 10 + 1}d"
        }).ToArray();

        // Act: CHOKE POINT - Bulk operations through database
        _context.Tasks.AddRange(bulkTasks);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();

        // Retrieve all bulk tasks
        var retrievedBulkTasks = _context.Tasks.Where(t => t.WbsCode.StartsWith("BULK.")).ToList();

        // Assert: ALL 50 tasks have properly enforced constraints
        Assert.Equal(50, retrievedBulkTasks.Count);

        foreach (var task in retrievedBulkTasks)
        {
            Assert.Equal(DateTimeKind.Utc, task.StartDate.Kind);
            Assert.Equal(DateTimeKind.Utc, task.EndDate.Kind);
            Assert.Equal(TimeSpan.Zero, task.StartDate.TimeOfDay);
            Assert.Equal(TimeSpan.Zero, task.EndDate.TimeOfDay);
        }
    }

    #endregion

    #region Database Query Choke Point Tests

    [Fact]
    public void Database_DateQueries_WorkWithUtcDatesOnly()
    {
        // Arrange: Tasks with known dates
        var queryTestTasks = new[]
        {
            new GanttTask
            {
                Name = "Early Task",
                WbsCode = "Q.1",
                StartDate = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Local),
                EndDate = new DateTime(2025, 1, 20, 16, 45, 0, DateTimeKind.Local),
                Duration = "5d"
            },
            new GanttTask
            {
                Name = "Middle Task",
                WbsCode = "Q.2",
                StartDate = new DateTime(2025, 6, 15, 14, 15, 30, DateTimeKind.Unspecified),
                EndDate = new DateTime(2025, 6, 25, 9, 45, 15, DateTimeKind.Unspecified),
                Duration = "10d"
            },
            new GanttTask
            {
                Name = "Late Task",
                WbsCode = "Q.3",
                StartDate = new DateTime(2025, 12, 1, 8, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2025, 12, 10, 17, 30, 0, DateTimeKind.Utc),
                Duration = "10d"
            }
        };

        _context.Tasks.AddRange(queryTestTasks);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();

        // Act: CHOKE POINT - Date-based queries
        var midYearDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var tasksAfterMidYear = _context.Tasks
            .Where(t => t.WbsCode.StartsWith("Q.") && t.StartDate >= midYearDate)
            .OrderBy(t => t.StartDate)
            .ToList();

        // Assert: Queries work correctly with UTC dates
        Assert.Equal(2, tasksAfterMidYear.Count);
        Assert.Equal("Q.2", tasksAfterMidYear[0].WbsCode);
        Assert.Equal("Q.3", tasksAfterMidYear[1].WbsCode);

        // Verify all returned dates are UTC date-only
        foreach (var task in tasksAfterMidYear)
        {
            Assert.Equal(DateTimeKind.Utc, task.StartDate.Kind);
            Assert.Equal(TimeSpan.Zero, task.StartDate.TimeOfDay);
        }
    }

    #endregion

    #region Future GanttDate Integration Tests

    [Fact]
    public void Database_ReadyForGanttDateMigration_ConvertersExist()
    {
        // This test verifies that our database is ready for future GanttDate migration

        // Arrange: Test the converters directly
        var ganttDateConverter = new GanttDateConverter();
        var nullableConverter = new NullableGanttDateConverter();

        var testDate = new GanttDate(2025, 8, 13);
        GanttDate? nullableDate = new GanttDate(2025, 12, 25);
        GanttDate? nullDate = null;

        // Act: Test converter functionality
        var convertedDate = ganttDateConverter.ConvertToProvider(testDate);
        var convertedBack = ganttDateConverter.ConvertFromProvider(convertedDate);

        var convertedNullable = nullableConverter.ConvertToProvider(nullableDate);
        var convertedNullableBack = nullableConverter.ConvertFromProvider(convertedNullable);

        var convertedNull = nullableConverter.ConvertToProvider(nullDate);
        var convertedNullBack = nullableConverter.ConvertFromProvider(convertedNull);

        // Assert: Converters work correctly for future migration
        Assert.Equal(testDate, convertedBack);
        Assert.Equal(nullableDate, convertedNullableBack);
        Assert.Null(convertedNullBack);

        // Verify underlying DateOnly conversion
        Assert.Equal(new DateOnly(2025, 8, 13), convertedDate);
        Assert.Equal(new DateOnly(2025, 12, 25), convertedNullable);
        Assert.Null(convertedNull);
    }

    #endregion

    public void Dispose()
    {
        var connectionString = _context.Database.GetConnectionString();
        _context.Dispose();
        
        // Clean up temporary database file
        if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("Data Source=") && !connectionString.Contains(":memory:"))
        {
            var dbPath = connectionString.Replace("Data Source=", "").Split(';')[0];
            if (File.Exists(dbPath))
            {
                try
                {
                    File.Delete(dbPath);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
}
