using System.Collections.Generic;
using UnityEngine;
using System;

namespace Localization
{
    /// <summary>
    /// LanguageManager, Load language file, manage text data and language switching
    /// </summary>
    public class LanguageManager : MonoBehaviour
    {
        private static LanguageManager _instance;
        public static LanguageManager Instance
        {
            get
            {
              
                return _instance;
            }
        }

        // Current language
        private LanguageType _currentLanguage = LanguageType.English;
        public LanguageType CurrentLanguage
        {
            get { return _currentLanguage; }
            private set { _currentLanguage = value; }
        }

        // Text data dictionary
        private Dictionary<string, string> _textData = new Dictionary<string, string>();

        // Language switching event
        public event Action OnLanguageChanged;

        private void Awake()
        {
            _instance = this;
   

            // Initialize loading default language
            LoadLanguage(_currentLanguage);
        }

        private void OnDestroy()
        {
            // 当对象被销毁时，如果它是当前实例，则清除静态引用
            if (_instance == this)
            {
                _instance = null;
            }
        }

        // 添加这个方法来处理应用退出
        private void OnApplicationQuit()
        {
            // 确保在应用退出时清理实例
            _instance = null;
        }

        /// <summary>
        /// Switch language
        /// </summary>
        /// <param name="language">Target language</param>
        public void SwitchLanguage(LanguageType language)
        {
            if (language == _currentLanguage)
                return;

            _currentLanguage = language;
            LoadLanguage(language);
            
            // Trigger language switching event
            OnLanguageChanged?.Invoke();
        }

        /// <summary>
        /// Load text data for specified language
        /// </summary>
        /// <param name="language">Target language</param>
        private void LoadLanguage(LanguageType language)
        {
            string fileName = "Localization/Localization_" + GetLanguageCode(language);
            TextAsset textAsset = Resources.Load<TextAsset>(fileName);

            if (textAsset == null)
            {
                Debug.LogError($"Failed to load language file: {fileName}");
                return;
            }

            // Clear old data
            _textData.Clear();

            // Parse JSON data
            try
            {
                // Unity's JsonUtility does not directly support Dictionary, so we need to custom parse
                string jsonText = textAsset.text;
                LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>("{\"items\":" + jsonText + "}");
                
                if (localizationData != null && localizationData.items != null)
                {
                    foreach (var item in localizationData.items)
                    {
                        _textData[item.key] = item.value;
                    }
                    Debug.Log($"Loaded {_textData.Count} text entries for language: {language}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing language file: {e.Message}");
            }
        }

        /// <summary>
        /// Get localized text for specified key
        /// </summary>
        /// <param name="key">Text key</param>
        /// <returns>Localized text, if not found return key name</returns>
        public string GetText(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (_textData.TryGetValue(key, out string value))
                return value;

            Debug.LogWarning($"Localization key not found: {key}");
            return key; // Return key name as default value
        }

        /// <summary>
        /// Get language code
        /// </summary>
        private string GetLanguageCode(LanguageType language)
        {
            switch (language)
            {
                case LanguageType.English:
                    return "en";
                case LanguageType.Chinese:
                    return "zh";
                default:
                    return "en";
            }
        }

        /// <summary>
        /// Localization data class, used for JSON deserialization
        /// </summary>
        [Serializable]
        private class LocalizationData
        {
            public List<LocalizationItem> items;
        }

        /// <summary>
        /// Localization item
        /// </summary>
        [Serializable]
        private class LocalizationItem
        {
            public string key;
            public string value;
        }
    }
} 