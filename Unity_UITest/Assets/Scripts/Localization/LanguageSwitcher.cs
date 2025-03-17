using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    /// <summary>
    /// LanguageSwitcher
    /// </summary>
    public class LanguageSwitcher : MonoBehaviour
    {
        [Tooltip("LanguageSwitcher")]
        [SerializeField] private LanguageType targetLanguage;

        private Button button;

        // private void Awake()
        // {
        //     button = GetComponent<Button>();
        //     if (button == null)
        //     {
        //         Debug.LogError("LanguageSwitcher requires a Button component", this);
        //         enabled = false;
        //         return;
        //     }

        //     // Add click event
        //     button.onClick.AddListener(SwitchLanguage);
        // }
        private void Start(){
            button = GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("LanguageSwitcher requires a Button component", this);
                enabled = false;
                return;
            }

            // Add click event
            button.onClick.AddListener(SwitchLanguage);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(SwitchLanguage);
            }
        }


        public void SwitchLanguage()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.SwitchLanguage(targetLanguage);
            }
        }


        public void SwitchLanguageByIndex(int languageIndex)
        {
            if (System.Enum.IsDefined(typeof(LanguageType), languageIndex))
            {
                LanguageType language = (LanguageType)languageIndex;
                if (LanguageManager.Instance != null)
                {
                    LanguageManager.Instance.SwitchLanguage(language);
                }
            }
            else
            {
                Debug.LogError($"Invalid language index: {languageIndex}", this);
            }
        }
    }
} 