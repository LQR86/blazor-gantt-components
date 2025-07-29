using System.Collections.Generic;
using System.Globalization;
using System.Resources;

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
    /// Resource-based internationalization service for Gantt Components.
    /// Provides scalable translation functionality using .resx files.
    /// Supports English/Chinese with easy extensibility for additional languages.
    /// </summary>
    public class GanttI18N : IGanttI18N
    {
        private string _currentCulture = "en-US";
        private readonly ResourceManager _resourceManager;
        private readonly string[] _supportedCultures = { "en-US", "zh-CN" };

        /// <summary>
        /// Event fired when language changes - allows components to react independently
        /// </summary>
        public event Action? LanguageChanged;

        /// <summary>
        /// Gets the current culture for translations.
        /// </summary>
        public string CurrentCulture => _currentCulture;

        public GanttI18N()
        {
            // Initialize ResourceManager pointing to the default resource file
            _resourceManager = new ResourceManager("GanttComponents.Resources.GanttResources",
                typeof(GanttI18N).Assembly);
        }

        /// <summary>
        /// Sets the current culture for translations and notifies subscribers.
        /// </summary>
        /// <param name="culture">Culture code (e.g., "en-US", "zh-CN")</param>
        public void SetCulture(string culture)
        {
            // Handle null culture by defaulting to English
            culture = culture ?? "en-US";

            if (_supportedCultures.Contains(culture) && _currentCulture != culture)
            {
                _currentCulture = culture;

                // Notify all components that language changed
                LanguageChanged?.Invoke();
            }
        }

        /// <summary>
        /// Translates a key to the current culture using resource files.
        /// Implements fallback chain: Current Culture → English → Key
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <returns>Translated text or key if not found</returns>
        public string T(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key ?? string.Empty;

            try
            {
                // Create CultureInfo for the current culture
                var cultureInfo = new CultureInfo(_currentCulture);

                // Try to get the resource for the current culture
                var translation = _resourceManager.GetString(key, cultureInfo);

                if (!string.IsNullOrEmpty(translation))
                    return translation;

                // Fallback to English if current culture is not English
                if (_currentCulture != "en-US")
                {
                    var englishTranslation = _resourceManager.GetString(key, new CultureInfo("en-US"));
                    if (!string.IsNullOrEmpty(englishTranslation))
                        return englishTranslation;
                }

                // Fallback to key itself if no translation found
                return key;
            }
            catch (Exception)
            {
                // If any error occurs, return the key as fallback
                return key;
            }
        }

        /// <summary>
        /// Gets all available cultures.
        /// </summary>
        /// <returns>List of culture codes</returns>
        public IEnumerable<string> GetAvailableCultures()
        {
            return _supportedCultures;
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

            try
            {
                // Check if the key exists in the default (English) resource file
                var translation = _resourceManager.GetString(key, new CultureInfo("en-US"));
                return !string.IsNullOrEmpty(translation);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
