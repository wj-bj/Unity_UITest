using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Localization
{
    /// <summary>
    /// Localized text component: Attached to UI text component, automatically update text content
    /// </summary>
    public class LocalizedText : MonoBehaviour
    {
        [Tooltip("Localized text key")]
        [SerializeField] private string textKey;

        // Text component reference
        private Text uiText;
        private TextMeshProUGUI tmpText;

        private void Awake()
        {
            // Get text component
            uiText = GetComponent<Text>();
            tmpText = GetComponent<TextMeshProUGUI>();

            if (uiText == null && tmpText == null)
            {
                Debug.LogError("LocalizedText requires a Text or TextMeshProUGUI component", this);
                enabled = false;
                return;
            }
        }

        void Start(){
                        // Register language switching event
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged += UpdateText;
            }
            
            // Initialize text
            UpdateText();
        }

        void OnDestroy(){
            // Unregister language switching event
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged -= UpdateText;
            }
        }   
    

        /// <summary>
        /// Update text content
        /// </summary>
        public void UpdateText()
        {
            if (string.IsNullOrEmpty(textKey))
            {
                Debug.LogWarning("Text key is empty", this);
                return;
            }

            string localizedText = LanguageManager.Instance.GetText(textKey);

            // Update text component
            if (uiText != null)
            {
                uiText.text = localizedText;
            }
            else if (tmpText != null)
            {
                tmpText.text = localizedText;
            }
        }

        /// <summary>
        /// Set text key and update text
        /// </summary>
        /// <param name="key">Text key</param>
        public void SetTextKey(string key)
        {
            if (textKey != key)
            {
                textKey = key;
                UpdateText();
            }
        }
    }
} 