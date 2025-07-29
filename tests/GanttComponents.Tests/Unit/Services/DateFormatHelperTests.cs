using Xunit;
using GanttComponents.Services;
using System;
using System.Globalization;
using Moq;

namespace GanttComponents.Tests.Unit.Services
{
    public class DateFormatHelperTests
    {
        private readonly Mock<IGanttI18N> _mockI18N;
        private readonly DateFormatHelper _dateFormatHelper;

        public DateFormatHelperTests()
        {
            _mockI18N = new Mock<IGanttI18N>();
            _dateFormatHelper = new DateFormatHelper(_mockI18N.Object);
        }

        [Fact]
        public void FormatDate_WithDefaultFormatKey_ReturnsFormattedDate()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var expectedPattern = "MM/dd/yyyy";
            var expectedResult = "07/29/2025";

            _mockI18N.Setup(x => x.T("date.short-format")).Returns(expectedPattern);

            // Act
            var result = _dateFormatHelper.FormatDate(testDate);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockI18N.Verify(x => x.T("date.short-format"), Times.Once);
        }

        [Fact]
        public void FormatDate_WithCustomFormatKey_ReturnsFormattedDate()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var customKey = "date.month-year";
            var expectedPattern = "MMM yyyy";

            _mockI18N.Setup(x => x.T(customKey)).Returns(expectedPattern);

            // Act - Use invariant culture to ensure consistent test results
            var result = _dateFormatHelper.FormatDate(testDate, CultureInfo.InvariantCulture, customKey);

            // Assert
            Assert.Equal("Jul 2025", result);
            _mockI18N.Verify(x => x.T(customKey), Times.Once);
        }

        [Fact]
        public void FormatDate_WithChinesePattern_ReturnsChineseFormattedDate()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var chinesePattern = "yyyy年MM月dd日";
            var expectedResult = "2025年07月29日";

            _mockI18N.Setup(x => x.T("date.short-format")).Returns(chinesePattern);

            // Act
            var result = _dateFormatHelper.FormatDate(testDate);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void FormatDate_WithExplicitCulture_ReturnsFormattedDateInSpecifiedCulture()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var culture = new CultureInfo("en-US");
            var formatKey = "date.short-format";
            var expectedPattern = "MM/dd/yyyy";
            var expectedResult = "07/29/2025";

            _mockI18N.Setup(x => x.T(formatKey)).Returns(expectedPattern);

            // Act
            var result = _dateFormatHelper.FormatDate(testDate, culture, formatKey);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockI18N.Verify(x => x.T(formatKey), Times.Once);
        }

        [Fact]
        public void GetDateFormatPattern_WithDefaultKey_ReturnsPattern()
        {
            // Arrange
            var expectedPattern = "MM/dd/yyyy";
            _mockI18N.Setup(x => x.T("date.short-format")).Returns(expectedPattern);

            // Act
            var result = _dateFormatHelper.GetDateFormatPattern();

            // Assert
            Assert.Equal(expectedPattern, result);
            _mockI18N.Verify(x => x.T("date.short-format"), Times.Once);
        }

        [Fact]
        public void GetDateFormatPattern_WithCustomKey_ReturnsPattern()
        {
            // Arrange
            var customKey = "date.custom-format";
            var expectedPattern = "yyyy-MM-dd";
            _mockI18N.Setup(x => x.T(customKey)).Returns(expectedPattern);

            // Act
            var result = _dateFormatHelper.GetDateFormatPattern(customKey);

            // Assert
            Assert.Equal(expectedPattern, result);
            _mockI18N.Verify(x => x.T(customKey), Times.Once);
        }

        [Fact]
        public void FormatTimelineMonth_ReturnsFormattedMonthYear()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var expectedPattern = "MMM yyyy";

            _mockI18N.Setup(x => x.T("date.month-year")).Returns(expectedPattern);

            // Act - This should call FormatTimelineMonth which internally calls FormatDate with "date.month-year"
            var result = _dateFormatHelper.FormatTimelineMonth(testDate);

            // Assert
            // The actual result depends on the current culture, but since the test system is Chinese, 
            // we should expect "7月 2025" for Chinese culture
            Assert.Contains("2025", result); // Verify year is present
            Assert.True(result.Contains("7月") || result.Contains("Jul")); // Accept either Chinese or English month
            _mockI18N.Verify(x => x.T("date.month-year"), Times.Once);
        }

        [Fact]
        public void FormatTimelineMonth_WithChinesePattern_ReturnsChineseMonthYear()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var chinesePattern = "yyyy年MM月";
            var expectedResult = "2025年07月";

            _mockI18N.Setup(x => x.T("date.month-year")).Returns(chinesePattern);

            // Act
            var result = _dateFormatHelper.FormatTimelineMonth(testDate);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void FormatTimelineDay_ReturnsFormattedDay()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var expectedPattern = "%d"; // Use %d to force day-only interpretation and avoid short date pattern

            _mockI18N.Setup(x => x.T("date.day-number")).Returns(expectedPattern);

            // Act
            var result = _dateFormatHelper.FormatTimelineDay(testDate);

            // Assert
            Assert.Equal("29", result);
            _mockI18N.Verify(x => x.T("date.day-number"), Times.Once);
        }

        [Fact]
        public void FormatTimelineDay_WithChinesePattern_ReturnsChineseDay()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var chinesePattern = "d日";
            var expectedResult = "29日";

            _mockI18N.Setup(x => x.T("date.day-number")).Returns(chinesePattern);

            // Act
            var result = _dateFormatHelper.FormatTimelineDay(testDate);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void FormatDate_WithLeapYearDate_HandlesCorrectly()
        {
            // Arrange
            var leapYearDate = new DateTime(2024, 2, 29); // Leap year
            var expectedPattern = "MM/dd/yyyy";
            var expectedResult = "02/29/2024";

            _mockI18N.Setup(x => x.T("date.short-format")).Returns(expectedPattern);

            // Act
            var result = _dateFormatHelper.FormatDate(leapYearDate);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void FormatDate_WithYearEndDate_HandlesCorrectly()
        {
            // Arrange
            var yearEndDate = new DateTime(2025, 12, 31);
            var expectedPattern = "MM/dd/yyyy";
            var expectedResult = "12/31/2025";

            _mockI18N.Setup(x => x.T("date.short-format")).Returns(expectedPattern);

            // Act
            var result = _dateFormatHelper.FormatDate(yearEndDate);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Constructor_WithNullI18N_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DateFormatHelper(null!));
        }

        [Fact]
        public void FormatDate_CallsI18NServiceCorrectly_ForMultipleInvocations()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var formatKey = "date.short-format";
            var expectedPattern = "MM/dd/yyyy";

            _mockI18N.Setup(x => x.T(formatKey)).Returns(expectedPattern);

            // Act
            var result1 = _dateFormatHelper.FormatDate(testDate, formatKey);
            var result2 = _dateFormatHelper.FormatDate(testDate, formatKey);

            // Assert
            Assert.Equal("07/29/2025", result1);
            Assert.Equal("07/29/2025", result2);
            _mockI18N.Verify(x => x.T(formatKey), Times.Exactly(2)); // Called once for each invocation
        }

        [Fact]
        public void FormatDate_WithDifferentCultures_ReturnsCorrectFormat()
        {
            // Arrange
            var testDate = new DateTime(2025, 7, 29);
            var formatKey = "date.short-format";
            var pattern = "MM/dd/yyyy";

            var usCulture = new CultureInfo("en-US");
            var chineseCulture = new CultureInfo("zh-CN");

            _mockI18N.Setup(x => x.T(formatKey)).Returns(pattern);

            // Act
            var usResult = _dateFormatHelper.FormatDate(testDate, usCulture, formatKey);
            var chineseResult = _dateFormatHelper.FormatDate(testDate, chineseCulture, formatKey);

            // Assert
            Assert.Equal("07/29/2025", usResult);
            Assert.Equal("07/29/2025", chineseResult); // Same pattern, different culture context
            _mockI18N.Verify(x => x.T(formatKey), Times.Exactly(2));
        }
    }
}
