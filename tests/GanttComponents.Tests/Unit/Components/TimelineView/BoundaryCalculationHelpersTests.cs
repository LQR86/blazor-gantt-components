using System;
using Xunit;
using GanttComponents.Components.TimelineView.Renderers;

namespace GanttComponents.Tests.Unit.Components.TimelineView
{
    /// <summary>
    /// Unit tests for BoundaryCalculationHelpers to ensure accurate period boundary calculations.
    /// Tests cover edge cases including leap years, month lengths, and boundary transitions.
    /// </summary>
    public class BoundaryCalculationHelpersTests
    {
        // === WEEK BOUNDARY TESTS ===

        [Theory]
        [InlineData("2025-08-15", "2025-08-11", "2025-08-17")] // Friday → Monday to Sunday
        [InlineData("2025-08-11", "2025-08-11", "2025-08-17")] // Monday → Same Monday to Sunday
        [InlineData("2025-08-17", "2025-08-11", "2025-08-17")] // Sunday → Monday to same Sunday
        [InlineData("2025-08-18", "2025-08-18", "2025-08-24")] // Monday → Monday to next Sunday
        public void GetWeekBoundaries_SingleDate_ReturnsCorrectWeekSpan(string inputDate, string expectedStart, string expectedEnd)
        {
            // Arrange
            var date = DateTime.Parse(inputDate);
            var expectedStartDate = DateTime.Parse(expectedStart);
            var expectedEndDate = DateTime.Parse(expectedEnd);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetWeekBoundaries(date, date);

            // Assert
            Assert.Equal(expectedStartDate, start);
            Assert.Equal(expectedEndDate, end);
        }

        [Fact]
        public void GetWeekBoundaries_CrossesMultipleWeeks_ReturnsExpandedSpan()
        {
            // Arrange: Aug 15 (Fri) to Sep 30 (Tue)
            var startDate = new DateTime(2025, 8, 15);
            var endDate = new DateTime(2025, 9, 30);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetWeekBoundaries(startDate, endDate);

            // Assert
            Assert.Equal(new DateTime(2025, 8, 11), start);  // Monday of week containing Aug 15
            Assert.Equal(new DateTime(2025, 10, 5), end);    // Sunday of week containing Sep 30
        }

        [Theory]
        [InlineData("2025-08-15", DayOfWeek.Monday, "2025-08-11")] // Friday → Previous Monday
        [InlineData("2025-08-11", DayOfWeek.Monday, "2025-08-11")] // Monday → Same Monday
        [InlineData("2025-08-17", DayOfWeek.Monday, "2025-08-11")] // Sunday → Previous Monday
        public void GetWeekStart_VariousDays_ReturnsCorrectMonday(string inputDate, DayOfWeek expectedDayOfWeek, string expectedMonday)
        {
            // Arrange
            var date = DateTime.Parse(inputDate);
            var expected = DateTime.Parse(expectedMonday);

            // Act
            var result = BoundaryCalculationHelpers.GetWeekStart(date);

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(expectedDayOfWeek, result.DayOfWeek);
        }

        // === MONTH BOUNDARY TESTS ===

        [Theory]
        [InlineData("2025-08-15", "2025-08-01", "2025-08-31")] // Mid-month August
        [InlineData("2025-02-15", "2025-02-01", "2025-02-28")] // February non-leap year
        [InlineData("2024-02-15", "2024-02-01", "2024-02-29")] // February leap year
        [InlineData("2025-04-30", "2025-04-01", "2025-04-30")] // Last day of April
        public void GetMonthBoundaries_SingleDate_ReturnsCorrectMonthSpan(string inputDate, string expectedStart, string expectedEnd)
        {
            // Arrange
            var date = DateTime.Parse(inputDate);
            var expectedStartDate = DateTime.Parse(expectedStart);
            var expectedEndDate = DateTime.Parse(expectedEnd);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetMonthBoundaries(date, date);

            // Assert
            Assert.Equal(expectedStartDate, start);
            Assert.Equal(expectedEndDate, end);
        }

        [Fact]
        public void GetMonthBoundaries_CrossesMultipleMonths_ReturnsExpandedSpan()
        {
            // Arrange: Aug 15 to Sep 30
            var startDate = new DateTime(2025, 8, 15);
            var endDate = new DateTime(2025, 9, 30);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetMonthBoundaries(startDate, endDate);

            // Assert
            Assert.Equal(new DateTime(2025, 8, 1), start);   // First day of August
            Assert.Equal(new DateTime(2025, 9, 30), end);    // Last day of September
        }

        // === QUARTER BOUNDARY TESTS ===

        [Theory]
        [InlineData("2025-02-15", "2025-01-01", "2025-03-31")] // Q1: Jan-Mar
        [InlineData("2025-05-15", "2025-04-01", "2025-06-30")] // Q2: Apr-Jun  
        [InlineData("2025-08-15", "2025-07-01", "2025-09-30")] // Q3: Jul-Sep
        [InlineData("2025-11-15", "2025-10-01", "2025-12-31")] // Q4: Oct-Dec
        public void GetQuarterBoundaries_SingleDate_ReturnsCorrectQuarterSpan(string inputDate, string expectedStart, string expectedEnd)
        {
            // Arrange
            var date = DateTime.Parse(inputDate);
            var expectedStartDate = DateTime.Parse(expectedStart);
            var expectedEndDate = DateTime.Parse(expectedEnd);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetQuarterBoundaries(date, date);

            // Assert
            Assert.Equal(expectedStartDate, start);
            Assert.Equal(expectedEndDate, end);
        }

        [Fact]
        public void GetQuarterBoundaries_CrossesQuarters_ReturnsExpandedSpan()
        {
            // Arrange: Feb 15 (Q1) to May 15 (Q2)
            var startDate = new DateTime(2025, 2, 15);
            var endDate = new DateTime(2025, 5, 15);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetQuarterBoundaries(startDate, endDate);

            // Assert
            Assert.Equal(new DateTime(2025, 1, 1), start);   // Q1 start
            Assert.Equal(new DateTime(2025, 6, 30), end);    // Q2 end
        }

        [Theory]
        [InlineData("2025-01-15", 1)] // January → Q1
        [InlineData("2025-03-31", 1)] // March → Q1
        [InlineData("2025-04-01", 2)] // April → Q2
        [InlineData("2025-06-30", 2)] // June → Q2
        [InlineData("2025-07-01", 3)] // July → Q3
        [InlineData("2025-09-30", 3)] // September → Q3
        [InlineData("2025-10-01", 4)] // October → Q4
        [InlineData("2025-12-31", 4)] // December → Q4
        public void GetQuarterNumber_VariousDates_ReturnsCorrectQuarter(string inputDate, int expectedQuarter)
        {
            // Arrange
            var date = DateTime.Parse(inputDate);

            // Act
            var result = BoundaryCalculationHelpers.GetQuarterNumber(date);

            // Assert
            Assert.Equal(expectedQuarter, result);
        }

        // === YEAR BOUNDARY TESTS ===

        [Theory]
        [InlineData("2025-08-15", "2025-01-01", "2025-12-31")] // Mid-year
        [InlineData("2024-02-29", "2024-01-01", "2024-12-31")] // Leap year
        [InlineData("2025-01-01", "2025-01-01", "2025-12-31")] // New Year's Day
        [InlineData("2025-12-31", "2025-01-01", "2025-12-31")] // New Year's Eve
        public void GetYearBoundaries_SingleDate_ReturnsCorrectYearSpan(string inputDate, string expectedStart, string expectedEnd)
        {
            // Arrange
            var date = DateTime.Parse(inputDate);
            var expectedStartDate = DateTime.Parse(expectedStart);
            var expectedEndDate = DateTime.Parse(expectedEnd);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetYearBoundaries(date, date);

            // Assert
            Assert.Equal(expectedStartDate, start);
            Assert.Equal(expectedEndDate, end);
        }

        [Fact]
        public void GetYearBoundaries_CrossesYears_ReturnsExpandedSpan()
        {
            // Arrange: Nov 15, 2024 to Feb 15, 2025
            var startDate = new DateTime(2024, 11, 15);
            var endDate = new DateTime(2025, 2, 15);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetYearBoundaries(startDate, endDate);

            // Assert
            Assert.Equal(new DateTime(2024, 1, 1), start);   // 2024 start
            Assert.Equal(new DateTime(2025, 12, 31), end);   // 2025 end
        }

        // === DAY BOUNDARY TESTS ===

        [Fact]
        public void GetDayBoundaries_AnyRange_ReturnsUnchangedRange()
        {
            // Arrange
            var startDate = new DateTime(2025, 8, 15);
            var endDate = new DateTime(2025, 8, 17);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetDayBoundaries(startDate, endDate);

            // Assert
            Assert.Equal(startDate, start);
            Assert.Equal(endDate, end);
        }

        // === EDGE CASE TESTS ===

        [Fact]
        public void GetQuarterDescription_VariousDates_ReturnsCorrectDescription()
        {
            // Arrange & Act & Assert
            Assert.Equal("Q1 2025", BoundaryCalculationHelpers.GetQuarterDescription(new DateTime(2025, 2, 15)));
            Assert.Equal("Q2 2025", BoundaryCalculationHelpers.GetQuarterDescription(new DateTime(2025, 5, 15)));
            Assert.Equal("Q3 2025", BoundaryCalculationHelpers.GetQuarterDescription(new DateTime(2025, 8, 15)));
            Assert.Equal("Q4 2025", BoundaryCalculationHelpers.GetQuarterDescription(new DateTime(2025, 11, 15)));
        }

        [Fact]
        public void ValidateDateRange_ValidRange_DoesNotThrow()
        {
            // Arrange
            var startDate = new DateTime(2025, 8, 15);
            var endDate = new DateTime(2025, 8, 17);

            // Act & Assert (should not throw)
            BoundaryCalculationHelpers.ValidateDateRange(startDate, endDate);
        }

        [Fact]
        public void ValidateDateRange_EqualDates_DoesNotThrow()
        {
            // Arrange
            var date = new DateTime(2025, 8, 15);

            // Act & Assert (should not throw)
            BoundaryCalculationHelpers.ValidateDateRange(date, date);
        }

        [Fact]
        public void ValidateDateRange_InvalidRange_ThrowsArgumentException()
        {
            // Arrange
            var startDate = new DateTime(2025, 8, 17);
            var endDate = new DateTime(2025, 8, 15);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                BoundaryCalculationHelpers.ValidateDateRange(startDate, endDate));

            Assert.Contains("Start date", exception.Message);
            Assert.Contains("cannot be after end date", exception.Message);
        }

        // === LEAP YEAR EDGE CASES ===

        [Theory]
        [InlineData(2024, true)]  // Leap year
        [InlineData(2025, false)] // Non-leap year
        [InlineData(2000, true)]  // Century leap year
        [InlineData(1900, false)] // Century non-leap year
        public void GetMonthBoundaries_FebruaryInVariousYears_HandlesLeapYearsCorrectly(int year, bool isLeapYear)
        {
            // Arrange
            var febDate = new DateTime(year, 2, 15);
            var expectedEnd = isLeapYear ? new DateTime(year, 2, 29) : new DateTime(year, 2, 28);

            // Act
            var (start, end) = BoundaryCalculationHelpers.GetMonthBoundaries(febDate, febDate);

            // Assert
            Assert.Equal(new DateTime(year, 2, 1), start);
            Assert.Equal(expectedEnd, end);
        }

        // === BOUNDARY COMBINATION TESTS (for dual expansion validation) ===

        [Fact]
        public void CombinedBoundaryExpansion_MonthWeekPattern_DemonstratesUnionCalculation()
        {
            // Arrange: Aug 15 - Sep 30, 2025 (MonthWeek pattern test case)
            var startDate = new DateTime(2025, 8, 15);
            var endDate = new DateTime(2025, 9, 30);

            // Act
            var monthBounds = BoundaryCalculationHelpers.GetMonthBoundaries(startDate, endDate);
            var weekBounds = BoundaryCalculationHelpers.GetWeekBoundaries(startDate, endDate);

            // Expected union calculation (widest span)
            var expectedUnionStart = monthBounds.start < weekBounds.start ? monthBounds.start : weekBounds.start;
            var expectedUnionEnd = monthBounds.end > weekBounds.end ? monthBounds.end : weekBounds.end;

            // Assert individual boundaries
            Assert.Equal(new DateTime(2025, 8, 1), monthBounds.start);   // Month: Aug 1
            Assert.Equal(new DateTime(2025, 9, 30), monthBounds.end);    // Month: Sep 30
            Assert.Equal(new DateTime(2025, 8, 11), weekBounds.start);   // Week: Aug 11 (Monday)
            Assert.Equal(new DateTime(2025, 10, 5), weekBounds.end);     // Week: Oct 5 (Sunday)

            // Assert union calculation - take earliest start and latest end
            Assert.Equal(new DateTime(2025, 8, 1), expectedUnionStart);  // Month start wins (earlier)
            Assert.Equal(new DateTime(2025, 10, 5), expectedUnionEnd);   // Week end wins (later)
        }

        [Fact]
        public void CombinedBoundaryExpansion_QuarterMonthPattern_DemonstratesUnionCalculation()
        {
            // Arrange: Feb 15 - May 15, 2025 (QuarterMonth pattern test case)
            var startDate = new DateTime(2025, 2, 15);
            var endDate = new DateTime(2025, 5, 15);

            // Act
            var quarterBounds = BoundaryCalculationHelpers.GetQuarterBoundaries(startDate, endDate);
            var monthBounds = BoundaryCalculationHelpers.GetMonthBoundaries(startDate, endDate);

            // Expected union calculation (widest span)
            var expectedUnionStart = quarterBounds.start < monthBounds.start ? quarterBounds.start : monthBounds.start;
            var expectedUnionEnd = quarterBounds.end > monthBounds.end ? quarterBounds.end : monthBounds.end;

            // Assert individual boundaries
            Assert.Equal(new DateTime(2025, 1, 1), quarterBounds.start);  // Quarter: Jan 1 (Q1 start)
            Assert.Equal(new DateTime(2025, 6, 30), quarterBounds.end);   // Quarter: Jun 30 (Q2 end)
            Assert.Equal(new DateTime(2025, 2, 1), monthBounds.start);    // Month: Feb 1
            Assert.Equal(new DateTime(2025, 5, 31), monthBounds.end);     // Month: May 31

            // Assert union would be quarter boundaries (widest span)
            Assert.Equal(new DateTime(2025, 1, 1), expectedUnionStart);  // Quarter boundaries win
            Assert.Equal(new DateTime(2025, 6, 30), expectedUnionEnd);   // Quarter boundaries win
        }
    }
}
