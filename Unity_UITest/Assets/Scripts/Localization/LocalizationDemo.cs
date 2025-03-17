using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Localization
{
    /// <summary>
    /// 多语言演示脚本：用于演示多语言功能
    /// </summary>
    public class LocalizationDemo : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button switchLanguageButton;
        [SerializeField] private TextMeshProUGUI currentLanguageText;

        public Sprite[] languageSprites;
        public Image languageImage;
        [SerializeField] int currentLanguageIndex = 0;

        private void Start()
        {
            // 确保LanguageManager已初始化
            LanguageManager languageManager = LanguageManager.Instance;

            // 添加按钮事件
            if (switchLanguageButton != null)
            {
                switchLanguageButton.onClick.AddListener(() => SwitchLanguage());
            }


            // 注册语言切换事件
            languageManager.OnLanguageChanged += UpdateCurrentLanguageText;

            // 初始化当前语言显示
            UpdateCurrentLanguageText();
        }

        private void OnDestroy()
        {
            // 移除事件监听
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged -= UpdateCurrentLanguageText;
            }

            if (switchLanguageButton != null)
            {
                switchLanguageButton.onClick.RemoveAllListeners();
            }

  
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language">目标语言</param>
        private void SwitchLanguage(LanguageType language)
        {
            Debug.Log("SwitchLanguage: " + language);
            
            LanguageManager.Instance.SwitchLanguage(language);
            languageImage.sprite = languageSprites[(int)language];
        }

        private void SwitchLanguage()
        {
            currentLanguageIndex = (currentLanguageIndex + 1) % languageSprites.Length;
            SwitchLanguage((LanguageType)currentLanguageIndex);
        }

        /// <summary>
        /// 更新当前语言显示
        /// </summary>
        private void UpdateCurrentLanguageText()
        {
            if (currentLanguageText != null)
            {
                string languageKey = LanguageManager.Instance.CurrentLanguage == LanguageType.English
                    ? "LANGUAGE_ENGLISH"
                    : "LANGUAGE_CHINESE";

                currentLanguageText.text = $"Current Language: {LanguageManager.Instance.GetText(languageKey)}";
            }
        }
    }
} 