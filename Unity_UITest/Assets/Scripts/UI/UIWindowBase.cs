using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public enum AnimationType
{
    None,
    Fade,
    Scale,
    Slide,
    Custom
}

[RequireComponent(typeof(CanvasGroup))]
public class UIWindowBase : MonoBehaviour
{
    [Header("Window Settings")]
    [SerializeField] private string windowName;
    [SerializeField] private UILayer layer = UILayer.STACK;
    
    [Header("Effect Settings")]
    [SerializeField] private BackgroundEffectType backgroundEffectType = BackgroundEffectType.None;
    [SerializeField] private ForegroundEffectType foregroundEffectType = ForegroundEffectType.None;
    
    [Header("Animation Settings")]
    [SerializeField] private AnimationType openAnimationType = AnimationType.Fade;
    [SerializeField] private AnimationType closeAnimationType = AnimationType.Fade;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease animationEase = Ease.OutQuad;
    [SerializeField] private Animator customAnimator;
    [SerializeField] private string openAnimationTrigger = "Open";
    [SerializeField] private string closeAnimationTrigger = "Close";
    
    // References
    protected CanvasGroup canvasGroup;
    protected RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    
    // Properties
    public string WindowName => windowName;
    public UILayer Layer => layer;
    public BackgroundEffectType BackgroundEffectType => backgroundEffectType;
    public ForegroundEffectType ForegroundEffectType => foregroundEffectType;
    
    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }
    
    public void Initialize(UIManager manager)
    {
        // This method is kept for backward compatibility
    }
    
    public void Show()
    {
        // Reset state
        gameObject.SetActive(true);
        
        // Start animation
        UIManager.Instance.NotifyAnimationStarted(windowName);
        
        switch (openAnimationType)
        {
            case AnimationType.None:
                canvasGroup.alpha = 1;
                OnAnimationComplete();
                break;
                
            case AnimationType.Fade:
                PlayFadeInAnimation();
                break;
                
            case AnimationType.Scale:
                PlayScaleInAnimation();
                break;
                
            case AnimationType.Slide:
                PlaySlideInAnimation();
                break;
                
            case AnimationType.Custom:
                PlayCustomOpenAnimation();
                break;
        }
    }
    
    public void Close()
    {
        switch (closeAnimationType)
        {
            case AnimationType.None:
                OnCloseAnimationComplete();
                break;
                
            case AnimationType.Fade:
                PlayFadeOutAnimation();
                break;
                
            case AnimationType.Scale:
                PlayScaleOutAnimation();
                break;
                
            case AnimationType.Slide:
                PlaySlideOutAnimation();
                break;
                
            case AnimationType.Custom:
                PlayCustomCloseAnimation();
                break;
        }
    }
    
    private void PlayFadeInAnimation()
    {
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, animationDuration)
            .SetEase(animationEase)
            .OnComplete(OnAnimationComplete);
    }
    
    private void PlayFadeOutAnimation()
    {
        canvasGroup.DOFade(0, animationDuration)
            .SetEase(animationEase)
            .OnComplete(OnCloseAnimationComplete);
    }
    
    private void PlayScaleInAnimation()
    {
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(originalScale, animationDuration)
            .SetEase(animationEase)
            .OnComplete(OnAnimationComplete);
    }
    
    private void PlayScaleOutAnimation()
    {
        rectTransform.DOScale(Vector3.zero, animationDuration)
            .SetEase(animationEase)
            .OnComplete(OnCloseAnimationComplete);
    }
    
    private void PlaySlideInAnimation()
    {
        Vector2 startPosition = new Vector2(originalPosition.x, originalPosition.y - Screen.height);
        rectTransform.anchoredPosition = startPosition;
        rectTransform.DOAnchorPos(originalPosition, animationDuration)
            .SetEase(animationEase)
            .OnComplete(OnAnimationComplete);
    }
    
    private void PlaySlideOutAnimation()
    {
        Vector2 endPosition = new Vector2(originalPosition.x, originalPosition.y - Screen.height);
        rectTransform.DOAnchorPos(endPosition, animationDuration)
            .SetEase(animationEase)
            .OnComplete(OnCloseAnimationComplete);
    }
    
    private void PlayCustomOpenAnimation()
    {
        if (customAnimator != null)
        {
            customAnimator.SetTrigger(openAnimationTrigger);
            StartCoroutine(WaitForAnimatorState(openAnimationTrigger, false));
        }
        else
        {
            OnAnimationComplete();
        }
    }
    
    private void PlayCustomCloseAnimation()
    {
        if (customAnimator != null)
        {
            customAnimator.SetTrigger(closeAnimationTrigger);
            StartCoroutine(WaitForAnimatorState(closeAnimationTrigger, true));
        }
        else
        {
            OnCloseAnimationComplete();
        }
    }
    
    private IEnumerator WaitForAnimatorState(string stateName, bool isClosing)
    {
        // Wait for animation to start
        yield return null;
        
        // Wait until animation is done
        while (customAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
               customAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        
        if (isClosing)
        {
            OnCloseAnimationComplete();
        }
        else
        {
            OnAnimationComplete();
        }
    }
    
    private void OnAnimationComplete()
    {
        UIManager.Instance.NotifyAnimationCompleted(windowName);
    }
    
    private void OnCloseAnimationComplete()
    {
        UIManager.Instance.NotifyAnimationCompleted(windowName);
        gameObject.SetActive(false);
        
        // Reset to original state for next time
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        canvasGroup.alpha = 1;
    }
} 