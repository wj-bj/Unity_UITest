using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : UIWindowBase
{
    [Header("UI References")]
    [SerializeField] private Button closeButton;

    
    protected override void Awake()
    {
        base.Awake();
        
        // Setup button listeners
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        

    }
    
    private void OnCloseButtonClicked()
    {
        UIManager.Instance.Close(WindowName);
    }

} 