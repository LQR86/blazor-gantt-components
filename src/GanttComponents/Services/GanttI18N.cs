using System.Collections.Generic;

namespace GanttComponents.Services
{
    /// <summary>
    /// Internationalization service
    /// Enables dependency injection and removes cascading parameter coupling
    /// </summary>
    public interface IGanttI18N
    {
        string CurrentCulture { get; }
        string T(string key);
        void SetCulture(string culture);
        IEnumerable<string> GetAvailableCultures();
        bool HasTranslation(string key);
        event Action? LanguageChanged;
    }

    /// <summary>
    /// Core internationalization service for Gantt Components.
    /// Provides simple translation functionality with English/Chinese support.
    /// Uses singleton pattern with event notification to eliminate cascading parameter coupling.
    /// </summary>
    public class GanttI18N : IGanttI18N
    {
        private string _currentCulture = "en-US";

        /// <summary>
        /// Event fired when language changes - allows components to react independently
        /// </summary>
        public event Action? LanguageChanged;

        /// <summary>
        /// Gets the current culture for translations.
        /// </summary>
        public string CurrentCulture => _currentCulture;

        /// <summary>
        /// Translation dictionaries for supported cultures.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
        {
            ["en-US"] = new()
            {
                // TaskGrid Headers
                ["grid.wbs"] = "WBS",
                ["grid.task-name"] = "Task Name",
                ["grid.start-date"] = "Start Date",
                ["grid.end-date"] = "End Date",
                ["grid.duration"] = "Duration",
                ["grid.progress"] = "Progress",
                ["grid.resources"] = "Resources",

                // Date Formats
                ["date.short-format"] = "MM/dd/yyyy",
                ["date.month-year"] = "MMM yyyy",
                ["date.day-number"] = "d",

                // Common UI Elements
                ["common.save"] = "Save",
                ["common.cancel"] = "Cancel",
                ["common.delete"] = "Delete",
                ["common.edit"] = "Edit",
                ["common.add"] = "Add",
                ["common.close"] = "Close",

                // Language Selector
                ["language.selector-label"] = "Language",

                // Demo Page Elements
                ["demo.load-sample-data"] = "Load Sample Data",
                ["demo.clear-selection"] = "Clear Selection"
            },

            ["zh-CN"] = new()
            {
                // TaskGrid Headers
                ["grid.wbs"] = "工作分解",
                ["grid.task-name"] = "任务名称",
                ["grid.start-date"] = "开始日期",
                ["grid.end-date"] = "结束日期",
                ["grid.duration"] = "持续时间",
                ["grid.progress"] = "进度",
                ["grid.resources"] = "资源",

                // Date Formats
                ["date.short-format"] = "yyyy年MM月dd日",
                ["date.month-year"] = "yyyy年MM月",
                ["date.day-number"] = "d日",

                // Common UI Elements
                ["common.save"] = "保存",
                ["common.cancel"] = "取消",
                ["common.delete"] = "删除",
                ["common.edit"] = "编辑",
                ["common.add"] = "添加",
                ["common.close"] = "关闭",

                // Language Selector
                ["language.selector-label"] = "语言",

                // Demo Page Elements
                ["demo.load-sample-data"] = "加载示例数据",
                ["demo.clear-selection"] = "清除选择"
            }
        };

        /// <summary>
        /// Sets the current culture for translations and notifies subscribers.
        /// </summary>
        /// <param name="culture">Culture code (e.g., "en-US", "zh-CN")</param>
        public void SetCulture(string culture)
        {
            // Handle null culture by defaulting to English
            culture = culture ?? "en-US";

            if (Translations.ContainsKey(culture) && _currentCulture != culture)
            {
                _currentCulture = culture;

                // Notify all components that language changed
                LanguageChanged?.Invoke();
            }
        }

        /// <summary>
        /// Translates a key to the current culture.
        /// Implements fallback chain: Current Culture → English → Key
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <returns>Translated text or key if not found</returns>
        public string T(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key ?? string.Empty;

            // Try current culture
            if (Translations.TryGetValue(_currentCulture, out var currentDict) &&
                currentDict.TryGetValue(key, out var currentTranslation))
            {
                return currentTranslation;
            }

            // Fallback to English
            if (_currentCulture != "en-US" &&
                Translations.TryGetValue("en-US", out var englishDict) &&
                englishDict.TryGetValue(key, out var englishTranslation))
            {
                return englishTranslation;
            }

            // Fallback to key itself
            return key;
        }

        /// <summary>
        /// Gets all available cultures.
        /// </summary>
        /// <returns>List of culture codes</returns>
        public IEnumerable<string> GetAvailableCultures()
        {
            return Translations.Keys;
        }

        /// <summary>
        /// Checks if a translation key exists in any culture.
        /// </summary>
        /// <param name="key">Translation key to check</param>
        /// <returns>True if key exists</returns>
        public bool HasTranslation(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            foreach (var culture in Translations.Values)
            {
                if (culture.ContainsKey(key))
                    return true;
            }

            return false;
        }
    }
}
