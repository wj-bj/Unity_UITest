using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UILayer
{
    BOTTOM,
    STACK,
    FLOAT,
    TOP
}

public enum BackgroundEffectType
{
    None,
    Blur,
    Gradient
}

public enum ForegroundEffectType
{
    None,
    Default,
    Loading
}

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("UI References")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private RectTransform backgroundPanel;
    [SerializeField] private RectTransform foregroundPanel;
    
    [Header("Background Effects")]
    [SerializeField] private GameObject[] backgroundEffects;
    [SerializeField] private BackgroundEffectType defaultBackgroundEffect = BackgroundEffectType.None;
    
    [Header("Foreground Effects")]
    [SerializeField] private GameObject[] foregroundEffects;
    [SerializeField] private ForegroundEffectType defaultForegroundEffect = ForegroundEffectType.None;
    
    // UI Events
    public event Action<string> OnUIShow;
    public event Action<string> OnUIAnimationStart;
    public event Action<string> OnUIAnimationEnd;
    public event Action<string> OnUIClose;
    // public event Action<RenderTexture> OnUIScreenCaptured;
    
    // UI Management
    private Dictionary<string, UIWindowBase> registeredWindows = new Dictionary<string, UIWindowBase>();
    private Stack<UIWindowBase> uiStack = new Stack<UIWindowBase>();
    private Dictionary<UILayer, List<UIWindowBase>> layerWindows = new Dictionary<UILayer, List<UIWindowBase>>();
    private int currentSortingOrder = 0;
    
    // Current active effects
    private GameObject currentBackgroundEffect;
    private GameObject currentForegroundEffect;
    
    // Dictionary to map effect types to GameObjects
    private Dictionary<BackgroundEffectType, GameObject> backgroundEffectMap = new Dictionary<BackgroundEffectType, GameObject>();
    private Dictionary<ForegroundEffectType, GameObject> foregroundEffectMap = new Dictionary<ForegroundEffectType, GameObject>();
    
    private void Awake()
    {
        _instance = this;
        
        InitializeLayers();
        InitializeEffects();
        RegisterAllWindows();
    }
    
    private void OnDestroy()
    {
        // Clear static reference when this instance is destroyed
        if (_instance == this)
        {
            _instance = null;
        }
    }
    
    private void OnApplicationQuit()
    {
        // Ensure instance is cleared on application exit
        _instance = null;
    }
    
    private void InitializeLayers()
    {
        layerWindows[UILayer.BOTTOM] = new List<UIWindowBase>();
        layerWindows[UILayer.STACK] = new List<UIWindowBase>();
        layerWindows[UILayer.FLOAT] = new List<UIWindowBase>();
        layerWindows[UILayer.TOP] = new List<UIWindowBase>();
    }
    
    private void InitializeEffects()
    {
        // Initialize background effects
        if (backgroundEffects != null)
        {
            foreach (var effect in backgroundEffects)
            {
                if (effect != null)
                {
                    // Get the effect type from the component
                    IBackgroundEffect bgEffect = effect.GetComponent<IBackgroundEffect>();
                    if (bgEffect != null)
                    {
                        backgroundEffectMap[bgEffect.EffectType] = effect;
                        effect.SetActive(false);
                    }
                    else
                    {
                        Debug.LogWarning($"Background effect {effect.name} does not implement IBackgroundEffect interface");
                    }
                }
            }
        }
        
        // Initialize foreground effects
        if (foregroundEffects != null)
        {
            foreach (var effect in foregroundEffects)
            {
                if (effect != null)
                {
                    // Get the effect type from the component
                    IForegroundEffect fgEffect = effect.GetComponent<IForegroundEffect>();
                    if (fgEffect != null)
                    {
                        foregroundEffectMap[fgEffect.EffectType] = effect;
                        effect.SetActive(false);
                    }
                    else
                    {
                        Debug.LogWarning($"Foreground effect {effect.name} does not implement IForegroundEffect interface");
                    }
                }
            }
        }
    }
    
    private void RegisterAllWindows()
    {
        UIWindowBase[] windows = mainCanvas.GetComponentsInChildren<UIWindowBase>(true);
        foreach (var window in windows)
        {
            RegisterWindow(window);
            window.gameObject.SetActive(false);
        }
    }
    
    public void RegisterWindow(UIWindowBase window)
    {
        if (!registeredWindows.ContainsKey(window.WindowName))
        {
            registeredWindows.Add(window.WindowName, window);
            layerWindows[window.Layer].Add(window);
            window.Initialize(this);
        }
    }
    
    public void Show(string windowName)
    {
        if (!registeredWindows.ContainsKey(windowName))
        {
            Debug.LogError($"UI Window {windowName} not registered!");
            return;
        }
        
        UIWindowBase window = registeredWindows[windowName];
        
        // Trigger UI Show event
        OnUIShow?.Invoke(windowName);
        
        // Handle stack management
        if (window.Layer == UILayer.STACK)
        {
            if (uiStack.Count > 0)
            {
                UIWindowBase topWindow = uiStack.Peek();
                topWindow.gameObject.SetActive(false);
            }
            uiStack.Push(window);
        }
        
        // Apply background effect if needed
        if (window.BackgroundEffectType != BackgroundEffectType.None)
        {
            ApplyBackgroundEffect(window.BackgroundEffectType);
        }
        
        // Apply foreground effect if needed
        if (window.ForegroundEffectType != ForegroundEffectType.None)
        {
            ApplyForegroundEffect(window.ForegroundEffectType);
        }
        
        // Set sorting order
        SetWindowSortingOrder(window);
        
        // Show window
        window.gameObject.SetActive(true);
        window.Show();
    }
    
    public void Close(string windowName)
    {
        if (!registeredWindows.ContainsKey(windowName))
        {
            Debug.LogError($"UI Window {windowName} not registered!");
            return;
        }
        
        UIWindowBase window = registeredWindows[windowName];
        
        // Trigger UI Close event
        OnUIClose?.Invoke(windowName);
        
        // Handle stack management
        if (window.Layer == UILayer.STACK && uiStack.Count > 0 && uiStack.Peek() == window)
        {
            uiStack.Pop();
            if (uiStack.Count > 0)
            {
                UIWindowBase previousWindow = uiStack.Peek();
                previousWindow.gameObject.SetActive(true);
                SetWindowSortingOrder(previousWindow);
                
                // Apply background effect for the previous window if needed
                if (previousWindow.BackgroundEffectType != BackgroundEffectType.None)
                {
                    ApplyBackgroundEffect(previousWindow.BackgroundEffectType);
                }
                else
                {
                    DisableBackgroundEffect();
                }
                
                // Apply foreground effect for the previous window if needed
                if (previousWindow.ForegroundEffectType != ForegroundEffectType.None)
                {
                    ApplyForegroundEffect(previousWindow.ForegroundEffectType);
                }
            }
            else
            {
                DisableBackgroundEffect();
                DisableForegroundEffect();
            }
        }
        else
        {
            // If not in stack, just disable effects
            if (window.BackgroundEffectType != BackgroundEffectType.None)
            {
                DisableBackgroundEffect();
            }
            
            if (window.ForegroundEffectType != ForegroundEffectType.None)
            {
                DisableForegroundEffect();
            }
        }
        
        // Close window
        window.Close();
    }
    
    private void SetWindowSortingOrder(UIWindowBase window)
    {
        Canvas windowCanvas = window.GetComponent<Canvas>();
        if (windowCanvas != null)
        {
            int baseOrder = GetBaseOrderForLayer(window.Layer);
            windowCanvas.sortingOrder = baseOrder + (++currentSortingOrder);
        }
    }
    
    private int GetBaseOrderForLayer(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.BOTTOM: return 0;
            case UILayer.STACK: return 100;
            case UILayer.FLOAT: return 200;
            case UILayer.TOP: return 300;
            default: return 0;
        }
    }
    
    private void ApplyBackgroundEffect(BackgroundEffectType effectType)
    {
        // Disable current background effect
        DisableBackgroundEffect();
        
        // Find and apply the requested effect
        if (backgroundEffectMap.TryGetValue(effectType, out GameObject effectObject))
        {
            IBackgroundEffect effect = effectObject.GetComponent<IBackgroundEffect>();
            if (effect != null)
            {
                effect.ApplyEffect(); // All logic is handled inside this method
                effectObject.SetActive(true);
                currentBackgroundEffect = effectObject;
            }
        }
        else if (effectType != BackgroundEffectType.None && defaultBackgroundEffect != BackgroundEffectType.None)
        {
            // Try to use default effect if the requested one is not available
            if (backgroundEffectMap.TryGetValue(defaultBackgroundEffect, out GameObject defaultObject))
            {
                IBackgroundEffect effect = defaultObject.GetComponent<IBackgroundEffect>();
                if (effect != null)
                {
                    effect.ApplyEffect(); // All logic is handled inside this method
                    defaultObject.SetActive(true);
                    currentBackgroundEffect = defaultObject;
                }
            }
        }
    }
    
    private void DisableBackgroundEffect()
    {
        if (currentBackgroundEffect != null)
        {
            IBackgroundEffect effect = currentBackgroundEffect.GetComponent<IBackgroundEffect>();
            if (effect != null)
            {
                effect.RemoveEffect();
            }
            currentBackgroundEffect.SetActive(false);
            currentBackgroundEffect = null;
        }
    }
    
    private void ApplyForegroundEffect(ForegroundEffectType effectType)
    {
        // Disable current foreground effect
        DisableForegroundEffect();
        
        // Find and apply the requested effect
        if (foregroundEffectMap.TryGetValue(effectType, out GameObject effectObject))
        {
            IForegroundEffect effect = effectObject.GetComponent<IForegroundEffect>();
            if (effect != null)
            {
                effect.PlayEffect();
                currentForegroundEffect = effectObject;
            }
        }
        else if (effectType != ForegroundEffectType.None && defaultForegroundEffect != ForegroundEffectType.None)
        {
            // Try to use default effect if the requested one is not available
            if (foregroundEffectMap.TryGetValue(defaultForegroundEffect, out GameObject defaultObject))
            {
                IForegroundEffect effect = defaultObject.GetComponent<IForegroundEffect>();
                if (effect != null)
                {
                    effect.PlayEffect();
                    currentForegroundEffect = defaultObject;
                }
            }
        }
    }
    
    private void DisableForegroundEffect()
    {
        if (currentForegroundEffect != null)
        {
            IForegroundEffect effect = currentForegroundEffect.GetComponent<IForegroundEffect>();
            if (effect != null)
            {
                effect.StopEffect();
            }
            currentForegroundEffect = null;
        }
    }
    
    public void NotifyAnimationStarted(string windowName)
    {
        OnUIAnimationStart?.Invoke(windowName);
    }
    
    public void NotifyAnimationCompleted(string windowName)
    {
        OnUIAnimationEnd?.Invoke(windowName);
    }


} 
