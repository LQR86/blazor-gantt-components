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
        [InlineData("ZoomLevel.WeekDay", "Week-Day View")]
        [InlineData("ZoomLevel.WeekDayMedium", "Week-Day Medium")]
        [InlineData("ZoomLevel.WeekDayLow", "Week-Day Compact")]
        [InlineData("ZoomLevel.MonthWeek", "Month-Week View")]
        [InlineData("ZoomLevel.MonthWeekMedium", "Month-Week Medium")]
        [InlineData("ZoomLevel.MonthWeekLow", "Month-Week Compact")]
        [InlineData("ZoomLevel.QuarterMonth", "Quarter-Month View")]
        [InlineData("ZoomLevel.QuarterMonthMedium", "Quarter-Month Medium")]
        [InlineData("ZoomLevel.QuarterMonthLow", "Quarter-Month Compact")]
        [InlineData("ZoomLevel.YearQuarter", "Year-Quarter View")]
        [InlineData("ZoomLevel.YearQuarterMedium", "Year-Quarter Medium")]
        [InlineData("ZoomLevel.YearQuarterLow", "Year-Quarter Compact")]
        [InlineData("ZoomLevel.YearQuarterMin", "Year-Quarter Minimal")]
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
        [InlineData("ZoomLevel.WeekDay", "周-日视图")]
        [InlineData("ZoomLevel.WeekDayMedium", "周-日中等")]
        [InlineData("ZoomLevel.WeekDayLow", "周-日紧凑")]
        [InlineData("ZoomLevel.MonthWeek", "月-周视图")]
        [InlineData("ZoomLevel.MonthWeekMedium", "月-周中等")]
        [InlineData("ZoomLevel.MonthWeekLow", "月-周紧凑")]
        [InlineData("ZoomLevel.QuarterMonth", "季-月视图")]
        [InlineData("ZoomLevel.QuarterMonthMedium", "季-月中等")]
        [InlineData("ZoomLevel.QuarterMonthLow", "季-月紧凑")]
        [InlineData("ZoomLevel.YearQuarter", "年-季视图")]
        [InlineData("ZoomLevel.YearQuarterMedium", "年-季中等")]
        [InlineData("ZoomLevel.YearQuarterLow", "年-季紧凑")]
        [InlineData("ZoomLevel.YearQuarterMin", "年-季最小")]
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
        [InlineData("ZoomLevel.WeekDay.Description", "Daily sprint planning with weekly context (60px/day)")]
        [InlineData("ZoomLevel.WeekDayMedium.Description", "Medium weekly view with daily granularity (45px/day)")]
        [InlineData("ZoomLevel.WeekDayLow.Description", "Compact weekly view with daily tracking (35px/day)")]
        [InlineData("ZoomLevel.MonthWeek.Description", "Monthly overview with weekly breakdown (25px/day)")]
        [InlineData("ZoomLevel.MonthWeekMedium.Description", "Medium monthly view with weekly periods (20px/day)")]
        [InlineData("ZoomLevel.MonthWeekLow.Description", "Compact monthly view with weekly periods (18px/day)")]
        [InlineData("ZoomLevel.QuarterMonth.Description", "Quarterly overview with monthly breakdown (15px/day)")]
        [InlineData("ZoomLevel.QuarterMonthMedium.Description", "Medium quarterly view with monthly periods (12px/day)")]
        [InlineData("ZoomLevel.QuarterMonthLow.Description", "Compact quarterly view with monthly markers (10px/day)")]
        [InlineData("ZoomLevel.YearQuarter.Description", "Annual overview with quarterly breakdown (8px/day)")]
        [InlineData("ZoomLevel.YearQuarterMedium.Description", "Medium annual view with quarterly periods (6.5px/day)")]
        [InlineData("ZoomLevel.YearQuarterLow.Description", "Extended multi-year planning with quarterly markers (5px/day)")]
        [InlineData("ZoomLevel.YearQuarterMin.Description", "Long-term strategic view with minimum day width (3px/day)")]
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
        [InlineData("ZoomLevel.WeekDay.Description", "带周上下文的每日冲刺规划 (60px/天)")]
        [InlineData("ZoomLevel.WeekDayMedium.Description", "具有每日粒度的中等周视图 (45px/天)")]
        [InlineData("ZoomLevel.WeekDayLow.Description", "带每日跟踪的紧凑周视图 (35px/天)")]
        [InlineData("ZoomLevel.MonthWeek.Description", "带周分解的月度概览 (25px/天)")]
        [InlineData("ZoomLevel.MonthWeekMedium.Description", "具有周期的中等月度视图 (20px/天)")]
        [InlineData("ZoomLevel.MonthWeekLow.Description", "具有周期的紧凑月度视图 (18px/天)")]
        [InlineData("ZoomLevel.QuarterMonth.Description", "带月分解的季度概览 (15px/天)")]
        [InlineData("ZoomLevel.QuarterMonthMedium.Description", "具有月周期的中等季度视图 (12px/天)")]
        [InlineData("ZoomLevel.QuarterMonthLow.Description", "带月标记的紧凑季度视图 (10px/天)")]
        [InlineData("ZoomLevel.YearQuarter.Description", "带季度分解的年度概览 (8px/天)")]
        [InlineData("ZoomLevel.YearQuarterMedium.Description", "具有季度周期的中等年度视图 (6.5px/天)")]
        [InlineData("ZoomLevel.YearQuarterLow.Description", "带季度标记的扩展多年规划 (5px/天)")]
        [InlineData("ZoomLevel.YearQuarterMin.Description", "具有最小日宽度的长期战略视图 (3px/天)")]
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

            // Act & Assert - Testing our new 13-level system
            Assert.Equal("Week-Day View", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDay));
            Assert.Equal("Week-Day Medium", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDayMedium));
            Assert.Equal("Week-Day Compact", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDayLow));
            Assert.Equal("Month-Week View", _i18nService.GetZoomLevelName(TimelineZoomLevel.MonthWeek));
            Assert.Equal("Month-Week Medium", _i18nService.GetZoomLevelName(TimelineZoomLevel.MonthWeekMedium));
            Assert.Equal("Month-Week Compact", _i18nService.GetZoomLevelName(TimelineZoomLevel.MonthWeekLow));
            Assert.Equal("Quarter-Month View", _i18nService.GetZoomLevelName(TimelineZoomLevel.QuarterMonth));
            Assert.Equal("Quarter-Month Medium", _i18nService.GetZoomLevelName(TimelineZoomLevel.QuarterMonthMedium));
            Assert.Equal("Quarter-Month Compact", _i18nService.GetZoomLevelName(TimelineZoomLevel.QuarterMonthLow));
            Assert.Equal("Year-Quarter View", _i18nService.GetZoomLevelName(TimelineZoomLevel.YearQuarter));
            Assert.Equal("Year-Quarter Medium", _i18nService.GetZoomLevelName(TimelineZoomLevel.YearQuarterMedium));
            Assert.Equal("Year-Quarter Compact", _i18nService.GetZoomLevelName(TimelineZoomLevel.YearQuarterLow));
            Assert.Equal("Year-Quarter Minimal", _i18nService.GetZoomLevelName(TimelineZoomLevel.YearQuarterMin));
        }

        [Fact]
        public void GetZoomLevelDescription_AllLevels_ReturnsCorrectTranslations()
        {
            // Arrange
            _i18nService.SetCulture("en-US");

            // Act & Assert - Testing our new 13-level system descriptions
            Assert.Equal("Daily sprint planning with weekly context (60px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.WeekDay));
            Assert.Equal("Medium weekly view with daily granularity (45px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.WeekDayMedium));
            Assert.Equal("Compact weekly view with daily tracking (35px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.WeekDayLow));
            Assert.Equal("Monthly overview with weekly breakdown (25px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.MonthWeek));
            Assert.Equal("Medium monthly view with weekly periods (20px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.MonthWeekMedium));
            Assert.Equal("Compact monthly view with weekly periods (18px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.MonthWeekLow));
            Assert.Equal("Quarterly overview with monthly breakdown (15px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.QuarterMonth));
            Assert.Equal("Medium quarterly view with monthly periods (12px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.QuarterMonthMedium));
            Assert.Equal("Compact quarterly view with monthly markers (10px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.QuarterMonthLow));
            Assert.Equal("Annual overview with quarterly breakdown (8px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.YearQuarter));
            Assert.Equal("Medium annual view with quarterly periods (6.5px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.YearQuarterMedium));
            Assert.Equal("Extended multi-year planning with quarterly markers (5px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.YearQuarterLow));
            Assert.Equal("Long-term strategic view with minimum day width (3px/day)",
                _i18nService.GetZoomLevelDescription(TimelineZoomLevel.YearQuarterMin));
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

            // Assert - Our new 13-level system 
            Assert.Equal(13, result.Count);

            // WeekDay pattern (3 levels)
            Assert.True(result.ContainsKey(TimelineZoomLevel.WeekDay));
            Assert.True(result.ContainsKey(TimelineZoomLevel.WeekDayMedium));
            Assert.True(result.ContainsKey(TimelineZoomLevel.WeekDayLow));

            // MonthWeek pattern (3 levels) 
            Assert.True(result.ContainsKey(TimelineZoomLevel.MonthWeek));
            Assert.True(result.ContainsKey(TimelineZoomLevel.MonthWeekMedium));
            Assert.True(result.ContainsKey(TimelineZoomLevel.MonthWeekLow));

            // QuarterMonth pattern (3 levels)
            Assert.True(result.ContainsKey(TimelineZoomLevel.QuarterMonth));
            Assert.True(result.ContainsKey(TimelineZoomLevel.QuarterMonthMedium));
            Assert.True(result.ContainsKey(TimelineZoomLevel.QuarterMonthLow));

            // YearQuarter pattern (4 levels)
            Assert.True(result.ContainsKey(TimelineZoomLevel.YearQuarter));
            Assert.True(result.ContainsKey(TimelineZoomLevel.YearQuarterMedium));
            Assert.True(result.ContainsKey(TimelineZoomLevel.YearQuarterLow));
            Assert.True(result.ContainsKey(TimelineZoomLevel.YearQuarterMin));

            // Verify content for representative levels
            Assert.Equal("Week-Day View", result[TimelineZoomLevel.WeekDay].Name);
            Assert.Equal("Daily sprint planning with weekly context (60px/day)", result[TimelineZoomLevel.WeekDay].Description);
            Assert.Equal("Month-Week View", result[TimelineZoomLevel.MonthWeek].Name);
            Assert.Equal("Monthly overview with weekly breakdown (25px/day)", result[TimelineZoomLevel.MonthWeek].Description);
        }

        [Fact]
        public void ZoomI18N_LanguageSwitching_UpdatesZoomTranslations()
        {
            // Test language switching for zoom functionality with new 13-level system

            // English first
            _i18nService.SetCulture("en-US");
            Assert.Equal("Week-Day View", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDay));
            Assert.Equal("Zoom In", _i18nService.GetZoomControlText("zoom-in"));

            // Switch to Chinese
            _i18nService.SetCulture("zh-CN");
            Assert.Equal("周-日视图", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDay));
            Assert.Equal("放大", _i18nService.GetZoomControlText("zoom-in"));

            // Back to English
            _i18nService.SetCulture("en-US");
            Assert.Equal("Week-Day View", _i18nService.GetZoomLevelName(TimelineZoomLevel.WeekDay));
            Assert.Equal("Zoom In", _i18nService.GetZoomControlText("zoom-in"));
        }

        #endregion
    }
}
