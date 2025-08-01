using Xunit;
using GanttComponents.Services;
using GanttComponents.Models;

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
        [InlineData("date.day-number", "%d")]
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
        [InlineData("date.day-number", "%d日")]
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

        #region Zoom I18N Tests

        [Theory]
        [InlineData("zoom.level.week-day", "Week-Day")]
        [InlineData("zoom.level.month-day", "Month-Day")]
        [InlineData("zoom.level.month-week", "Month-Week")]
        [InlineData("zoom.level.quarter-week", "Quarter-Week")]
        [InlineData("zoom.level.quarter-month", "Quarter-Month")]
        [InlineData("zoom.level.year-quarter", "Year-Quarter")]
        public void T_ZoomLevelNames_English_ReturnsCorrectTranslation(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("zoom.level.week-day", "周-日")]
        [InlineData("zoom.level.month-day", "月-日")]
        [InlineData("zoom.level.month-week", "月-周")]
        [InlineData("zoom.level.quarter-week", "季-周")]
        [InlineData("zoom.level.quarter-month", "季-月")]
        [InlineData("zoom.level.year-quarter", "年-季")]
        public void T_ZoomLevelNames_Chinese_ReturnsCorrectTranslation(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("zh-CN");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("zoom.description.week-day", "Sprint planning, daily management (2-8 weeks)")]
        [InlineData("zoom.description.month-day", "Project milestones, phase tracking (3-12 months)")]
        [InlineData("zoom.description.month-week", "Quarterly planning, resource scheduling (6-18 months)")]
        [InlineData("zoom.description.quarter-week", "Annual planning, strategic roadmaps (6-24 months)")]
        [InlineData("zoom.description.quarter-month", "Multi-year programs, portfolio planning (1-5 years)")]
        [InlineData("zoom.description.year-quarter", "Enterprise portfolio, strategic planning (2-10 years)")]
        public void T_ZoomDescriptions_English_ReturnsCorrectTranslation(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("zoom.description.week-day", "冲刺规划，日常管理 (2-8周)")]
        [InlineData("zoom.description.month-day", "项目里程碑，阶段跟踪 (3-12个月)")]
        [InlineData("zoom.description.month-week", "季度规划，资源调度 (6-18个月)")]
        [InlineData("zoom.description.quarter-week", "年度规划，战略路线图 (6-24个月)")]
        [InlineData("zoom.description.quarter-month", "多年项目，组合规划 (1-5年)")]
        [InlineData("zoom.description.year-quarter", "企业组合，战略规划 (2-10年)")]
        public void T_ZoomDescriptions_Chinese_ReturnsCorrectTranslation(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("zh-CN");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("zoom.controls.zoom-in", "Zoom In")]
        [InlineData("zoom.controls.zoom-out", "Zoom Out")]
        [InlineData("zoom.controls.fit-viewport", "Fit Viewport")]
        [InlineData("zoom.controls.fit-tasks", "Fit Tasks")]
        [InlineData("zoom.controls.zoom-level", "Zoom Level")]
        [InlineData("zoom.controls.zoom-factor", "Zoom Factor")]
        [InlineData("zoom.overflow.hidden-tasks", "Hidden Tasks")]
        public void T_ZoomControls_English_ReturnsCorrectTranslation(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("zoom.controls.zoom-in", "放大")]
        [InlineData("zoom.controls.zoom-out", "缩小")]
        [InlineData("zoom.controls.fit-viewport", "适应视口")]
        [InlineData("zoom.controls.fit-tasks", "适应任务")]
        [InlineData("zoom.controls.zoom-level", "缩放级别")]
        [InlineData("zoom.controls.zoom-factor", "缩放系数")]
        [InlineData("zoom.overflow.hidden-tasks", "隐藏任务")]
        public void T_ZoomControls_Chinese_ReturnsCorrectTranslation(string key, string expected)
        {
            // Arrange
            _i18nService.SetCulture("zh-CN");

            // Act
            var result = _i18nService.T(key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetZoomLevelName_AllLevels_ReturnsCorrectTranslations()
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act & Assert
            Assert.Equal("Week-Day", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDay));
            Assert.Equal("Month-Day", _i18nService.GetZoomLevelName(TimelineZoomLevel.MonthDay));
            Assert.Equal("Month-Week", _i18nService.GetZoomLevelName(TimelineZoomLevel.MonthWeek));
            Assert.Equal("Quarter-Week", _i18nService.GetZoomLevelName(TimelineZoomLevel.QuarterWeek));
            Assert.Equal("Quarter-Month", _i18nService.GetZoomLevelName(TimelineZoomLevel.QuarterMonth));
            Assert.Equal("Year-Quarter", _i18nService.GetZoomLevelName(TimelineZoomLevel.YearQuarter));
        }

        [Fact]
        public void GetZoomLevelDescription_AllLevels_ReturnsCorrectTranslations()
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act & Assert
            Assert.Equal("Sprint planning, daily management (2-8 weeks)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.WeekDay));
            Assert.Equal("Project milestones, phase tracking (3-12 months)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.MonthDay));
            Assert.Equal("Quarterly planning, resource scheduling (6-18 months)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.MonthWeek));
            Assert.Equal("Annual planning, strategic roadmaps (6-24 months)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.QuarterWeek));
            Assert.Equal("Multi-year programs, portfolio planning (1-5 years)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.QuarterMonth));
            Assert.Equal("Enterprise portfolio, strategic planning (2-10 years)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.YearQuarter));
        }

        [Theory]
        [InlineData("zoom-in", "Zoom In")]
        [InlineData("zoom-out", "Zoom Out")]
        [InlineData("fit-viewport", "Fit Viewport")]
        [InlineData("fit-tasks", "Fit Tasks")]
        [InlineData("zoom-level", "Zoom Level")]
        [InlineData("zoom-factor", "Zoom Factor")]
        [InlineData("hidden-tasks", "Hidden Tasks")]
        public void GetZoomControlText_VariousControls_ReturnsCorrectTranslations(string controlKey, string expected)
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.GetZoomControlText(controlKey);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAllZoomLevelInfo_ReturnsCompleteMapping()
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act
            var result = _i18nService.GetAllZoomLevelInfo();

            // Assert - Preset-only system now supports 13 fine-grained levels
            Assert.Equal(13, result.Count);
            
            // Original 6 levels still exist
            Assert.True(result.ContainsKey(TimelineZoomLevel.WeekDay));
            Assert.True(result.ContainsKey(TimelineZoomLevel.MonthDay));
            Assert.True(result.ContainsKey(TimelineZoomLevel.MonthWeek));
            Assert.True(result.ContainsKey(TimelineZoomLevel.QuarterWeek));
            Assert.True(result.ContainsKey(TimelineZoomLevel.QuarterMonth));
            Assert.True(result.ContainsKey(TimelineZoomLevel.YearQuarter));
            
            // New intermediate levels added for finer granularity
            Assert.True(result.ContainsKey(TimelineZoomLevel.WeekDayMedium));
            Assert.True(result.ContainsKey(TimelineZoomLevel.WeekDayLow));
            Assert.True(result.ContainsKey(TimelineZoomLevel.MonthDayMedium));
            Assert.True(result.ContainsKey(TimelineZoomLevel.MonthWeekMedium));
            Assert.True(result.ContainsKey(TimelineZoomLevel.MonthWeekLow));
            Assert.True(result.ContainsKey(TimelineZoomLevel.QuarterWeekMedium));
            Assert.True(result.ContainsKey(TimelineZoomLevel.QuarterMonthMedium));
            Assert.True(result.ContainsKey(TimelineZoomLevel.YearQuarter));

            // Verify content for a few levels
            Assert.Equal("Week-Day", result[TimelineZoomLevel.WeekDay].Name);
            Assert.Equal("Sprint planning, daily management (2-8 weeks)", result[TimelineZoomLevel.WeekDay].Description);
            Assert.Equal("Month-Day", result[TimelineZoomLevel.MonthDay].Name);
            Assert.Equal("Project milestones, phase tracking (3-12 months)", result[TimelineZoomLevel.MonthDay].Description);
        }

        [Fact]
        public void ZoomI18N_LanguageSwitching_UpdatesZoomTranslations()
        {
            // Test language switching for zoom functionality

            // English first
            _i18nService.SetCulture("en-US");
            Assert.Equal("Week-Day", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDay));
            Assert.Equal("Zoom In", _i18nService.GetZoomControlText("zoom-in"));

            // Switch to Chinese
            _i18nService.SetCulture("zh-CN");
            Assert.Equal("周-日", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDay));
            Assert.Equal("放大", _i18nService.GetZoomControlText("zoom-in"));

            // Back to English
            _i18nService.SetCulture("en-US");
            Assert.Equal("Week-Day", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDay));
            Assert.Equal("Zoom In", _i18nService.GetZoomControlText("zoom-in"));
        }

        #endregion
    }
}
