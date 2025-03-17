using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : UIWindowBase
{
    [Header("UI References")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button levelCompleteButton;
 
    
    protected override void Awake()
    {
        base.Awake();
        
        // Setup button listeners
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }
        
        if (levelCompleteButton != null)
        {
            levelCompleteButton.onClick.AddListener(OnlevelCompleteButtonButtonClicked);
        }
        

    }
    
    private void OnSettingsButtonClicked()
    {
        UIManager.Instance.Show("SettingsPanel");
  
    }
    
    private void OnlevelCompleteButtonButtonClicked()
    {
        UIManager.Instance.Show("LevelComplete");
    }
    
} 