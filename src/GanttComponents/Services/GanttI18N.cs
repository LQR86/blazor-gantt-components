using System.Collections.Generic;

namespace GanttComponents.Services
{
    /// <summary>
    /// Core internationalization service for Gantt Components.
    /// Provides simple translation functionality with English/Chinese support.
    /// </summary>
    public static class GanttI18N
    {
        private static string _currentCulture = "en-US";
        
        /// <summary>
        /// Gets or sets the current culture for translations.
        /// </summary>
        public static string CurrentCulture
        {
            get => _currentCulture;
            set => _currentCulture = value ?? "en-US";
        }

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
                
                // Demo Page Elements
                ["demo.load-sample-data"] = "Load Sample Data",
                ["demo.clear-selection"] = "Clear Selection"
            }
        };

        /// <summary>
        /// Sets the current culture for translations.
        /// </summary>
        /// <param name="culture">Culture code (e.g., "en-US", "zh-CN")</param>
        public static void SetCulture(string culture)
        {
            CurrentCulture = culture;
        }

        /// <summary>
        /// Translates a key to the current culture.
        /// Implements fallback chain: Current Culture → English → Key
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <returns>Translated text or key if not found</returns>
        public static string T(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key ?? string.Empty;

            // Try current culture
            if (Translations.TryGetValue(CurrentCulture, out var currentDict) &&
                currentDict.TryGetValue(key, out var currentTranslation))
            {
                return currentTranslation;
            }

            // Fallback to English
            if (CurrentCulture != "en-US" &&
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
        public static IEnumerable<string> GetAvailableCultures()
        {
            return Translations.Keys;
        }

        /// <summary>
        /// Checks if a translation key exists in any culture.
        /// </summary>
        /// <param name="key">Translation key to check</param>
        /// <returns>True if key exists</returns>
        public static bool HasTranslation(string key)
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
