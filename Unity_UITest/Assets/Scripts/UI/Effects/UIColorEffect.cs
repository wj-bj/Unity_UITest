using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class UIColorEffect : MonoBehaviour, IBackgroundEffect
{
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] private float fadeDuration = 0.3f;
    
    private Image backgroundImage;
    
    // IBackgroundEffect implementation
    public BackgroundEffectType EffectType => BackgroundEffectType.Gradient;
    
    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        
        // Initially hide
        gameObject.SetActive(false);
    }
    
    // IBackgroundEffect implementation
    public void ApplyEffect()
    {
        backgroundImage.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0);
        gameObject.SetActive(true);
        
        // Fade in the color
        backgroundImage.DOFade(backgroundColor.a, fadeDuration);
    }
    
    // IBackgroundEffect implementation
    public void RemoveEffect()
    {
        // Fade out the color
        DOTween.To(() => backgroundImage.color.a, x => backgroundImage.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, x), 0, fadeDuration)
            .OnComplete(() => {
                gameObject.SetActive(false);
            });
    }
} 