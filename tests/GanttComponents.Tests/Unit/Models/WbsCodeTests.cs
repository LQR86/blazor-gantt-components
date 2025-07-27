using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using GanttComponents.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Text.Json;

namespace GanttComponents.Tests.Unit.Models;

public class WbsCodeTests
{
    [Fact]
    public void GanttTask_WbsCode_CanBeSetAndRetrieved()
    {
        // Arrange
        var task = new GanttTask();
        var expectedWbsCode = "1.2.3";

        // Act
        task.WbsCode = expectedWbsCode;

        // Assert
        Assert.Equal(expectedWbsCode, task.WbsCode);
    }

    [Fact]
    public void GanttTask_WbsCode_RequiredValidation()
    {
        // Arrange
        var task = new GanttTask
        {
            Name = "Test Task",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
            // WbsCode intentionally not set
        };

        // Act & Assert
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(task);
        bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(task, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("WbsCode"));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1.1")]
    [InlineData("1.1.1")]
    [InlineData("2.3.4.5")]
    [InlineData("A.B.C")]
    public void GanttTask_WbsCode_AcceptsValidFormats(string wbsCode)
    {
        // Arrange
        var task = new GanttTask
        {
            Name = "Test Task",
            WbsCode = wbsCode,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
        };

        // Act & Assert
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(task);
        bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(task, validationContext, validationResults, true);

        Assert.True(isValid);
        Assert.Equal(wbsCode, task.WbsCode);
    }

    [Fact]
    public void GanttTask_WbsCode_RejectsExcessivelyLongStrings()
    {
        // Arrange
        var longWbsCode = new string('1', 51) + "." + new string('2', 51); // Over 50 characters
        var task = new GanttTask
        {
            Name = "Test Task",
            WbsCode = longWbsCode,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Duration = "1d",
            TaskType = TaskType.FixedDuration
        };

        // Act & Assert
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(task);
        bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(task, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("WbsCode"));
    }
}
