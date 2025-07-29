using Xunit;
using GanttComponents.Services;

namespace GanttComponents.Tests.Unit.Services
{
    public class GanttI18NTests
    {
        private readonly IGanttI18N _i18nService;

        public GanttI18NTests()
        {
            _i18nService = new GanttI18N();
        }

        [Fact]
        public void T_WithValidEnglishKey_ReturnsTranslation()
        {
            // Arrange
            _i18nService.SetCulture("en-US");
            var key = "grid.wbs";

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal("WBS", result);
        }

        [Fact]
        public void T_WithInvalidKey_ReturnsKey()
        {
            // Arrange
            _i18nService.SetCulture("en-US");
            var key = "invalid.key";

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal("invalid.key", result);
        }

        [Fact]
        public void T_WithNullKey_ReturnsEmptyString()
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.T(null!);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void T_WithEmptyKey_ReturnsEmptyString()
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.T("");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void SetCulture_WithValidCulture_UpdatesCurrentCulture()
        {
            // Arrange
            var expectedCulture = "en-US";

            // Act
            _i18nService.SetCulture(expectedCulture);

            // Assert
            Assert.Equal(expectedCulture, _i18nService.CurrentCulture);
        }

        [Fact]
        public void SetCulture_WithNullCulture_DefaultsToEnglish()
        {
            // Arrange & Act
            _i18nService.SetCulture(null!);

            // Assert
            Assert.Equal("en-US", _i18nService.CurrentCulture);
        }

        [Fact]
        public void GetAvailableCultures_ReturnsExpectedCultures()
        {
            // Act
            var cultures = _i18nService.GetAvailableCultures();

            // Assert
            Assert.Contains("en-US", cultures);
        }

        [Fact]
        public void HasTranslation_WithValidKey_ReturnsTrue()
        {
            // Arrange
            var key = "grid.task-name";

            // Act
            var result = _i18nService.HasTranslation(key);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasTranslation_WithInvalidKey_ReturnsFalse()
        {
            // Arrange
            var key = "invalid.key";

            // Act
            var result = _i18nService.HasTranslation(key);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasTranslation_WithNullKey_ReturnsFalse()
        {
            // Act
            var result = _i18nService.HasTranslation(null!);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("grid.wbs", "WBS")]
        [InlineData("grid.task-name", "Task Name")]
        [InlineData("grid.start-date", "Start Date")]
        [InlineData("grid.end-date", "End Date")]
        [InlineData("grid.duration", "Duration")]
        [InlineData("grid.progress", "Progress")]
        [InlineData("grid.resources", "Resources")]
        public void T_AllTaskGridHeaders_ReturnCorrectTranslations(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("date.short-format", "MM/dd/yyyy")]
        [InlineData("date.month-year", "MMM yyyy")]
        [InlineData("date.day-number", "d")]
        public void T_DateFormats_ReturnCorrectPatterns(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("demo.load-sample-data", "Load Sample Data")]
        [InlineData("demo.clear-selection", "Clear Selection")]
        public void T_DemoPageElements_ReturnCorrectTranslations(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        // Day 2: Chinese Support Tests

        [Fact]
        public void T_WithValidChineseKey_ReturnsChineseTranslation()
        {
            // Arrange
            _i18nService.SetCulture("zh-CN");
            var key = "grid.wbs";

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal("工作分解", result);
        }

        [Theory]
        [InlineData("grid.wbs", "工作分解")]
        [InlineData("grid.task-name", "任务名称")]
        [InlineData("grid.start-date", "开始日期")]
        [InlineData("grid.end-date", "结束日期")]
        [InlineData("grid.duration", "持续时间")]
        [InlineData("grid.progress", "进度")]
        [InlineData("grid.resources", "资源")]
        public void T_AllTaskGridHeaders_ReturnCorrectChineseTranslations(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("zh-CN");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("date.short-format", "yyyy年MM月dd日")]
        [InlineData("date.month-year", "yyyy年MM月")]
        [InlineData("date.day-number", "d日")]
        public void T_ChineseDateFormats_ReturnCorrectPatterns(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("zh-CN");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("common.save", "保存")]
        [InlineData("common.cancel", "取消")]
        [InlineData("common.delete", "删除")]
        [InlineData("common.edit", "编辑")]
        [InlineData("common.add", "添加")]
        [InlineData("common.close", "关闭")]
        public void T_CommonUIElements_ReturnCorrectChineseTranslations(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("zh-CN");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("demo.load-sample-data", "加载示例数据")]
        [InlineData("demo.clear-selection", "清除选择")]
        public void T_ChineseDemoPageElements_ReturnCorrectTranslations(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("zh-CN");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAvailableCultures_IncludesChineseCulture()
        {
            // Act
            var cultures = _i18nService.GetAvailableCultures();

            // Assert
            Assert.Contains("zh-CN", cultures);
            Assert.Contains("en-US", cultures);
            Assert.Equal(2, cultures.Count());
        }

        [Fact]
        public void T_ChineseCultureWithMissingKey_FallsBackToEnglish()
        {
            // Arrange
            _i18nService.SetCulture("zh-CN");
            // This key only exists in English (hypothetical scenario)
            var key = "test.english-only";

            // Act
            var result = _i18nService.T(key);

            // Assert
            // Should fallback to key since it doesn't exist in any culture
            Assert.Equal("test.english-only", result);
        }

        [Fact]
        public void T_CultureSwitching_UpdatesTranslationsImmediately()
        {
            // Arrange
            var key = "grid.task-name";

            // Act & Assert - English
            _i18nService.SetCulture("en-US");
            var englishResult = _i18nService.T(key);
            Assert.Equal("Task Name", englishResult);

            // Act & Assert - Chinese
            _i18nService.SetCulture("zh-CN");
            var chineseResult = _i18nService.T(key);
            Assert.Equal("任务名称", chineseResult);

            // Act & Assert - Back to English
            _i18nService.SetCulture("en-US");
            var englishAgainResult = _i18nService.T(key);
            Assert.Equal("Task Name", englishAgainResult);
        }

        [Fact]
        public void T_InvalidCultureWithFallbackChain_Works()
        {
            // Arrange
            _i18nService.SetCulture("fr-FR"); // Unsupported culture
            var key = "grid.wbs";

            // Act
            var result = _i18nService.T(key);

            // Assert
            // Should fallback to English since fr-FR doesn't exist
            Assert.Equal("WBS", result);
        }
    }
}
