using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UITransitionEffect : MonoBehaviour, IForegroundEffect
{
    public enum ImageTransitionEffectType
    {
        None,
        Dissolve,
        DissolveGrid
    }
    [SerializeField] private ForegroundEffectType effectType = ForegroundEffectType.Default;
  
    [SerializeField] private float transitionDuration = 0.5f;
    [SerializeField] private float transitionValueMax = 1f;
    [SerializeField] private Ease transitionEase = Ease.InOutQuad;
    
    [Header("Dissolve Settings")]
    [SerializeField] private Image dissolveImage;
    [SerializeField] private string dissolvePropertyName = "_DissolveAmount";
    
    private CanvasGroup canvasGroup;
    private Sequence currentSequence;
    
    // IForegroundEffect implementation
    public ForegroundEffectType EffectType => effectType;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        // 如果没有指定溶解图像，则使用第一个找到的Image
        if (dissolveImage == null)
        {
            dissolveImage = GetComponentInChildren<Image>();
        }
        
        
    }
    
    // IForegroundEffect implementation
    public void PlayEffect()
    {
        // Reset state
        gameObject.SetActive(true);
        
        // Stop any running transition
        if (currentSequence != null)
        {
            currentSequence.Kill();
        }

        if(effectType == ForegroundEffectType.None)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            PlayDissolveTransition();
        }
    }
    
    // IForegroundEffect implementation
    public void StopEffect()
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
            currentSequence = null;
        }
        
        gameObject.SetActive(false);
    }
    
    private void PlayDissolveTransition()
    {
        // 检查是否有可用的Image和Material
        if (dissolveImage == null || dissolveImage.material == null)
        {
            Debug.LogWarning("Dissolve effect requires an Image with a material that has a dissolve property!");
            gameObject.SetActive(false);
            return;
        }
        
        // 确保Material是实例化的，避免修改共享材质
        if (!dissolveImage.material.name.Contains("(Instance)"))
        {
            dissolveImage.material = new Material(dissolveImage.material);
        }
        
        currentSequence = DOTween.Sequence();
        
        // 设置初始溶解值为1（完全显示）
        dissolveImage.material.SetFloat(dissolvePropertyName, transitionValueMax);
        
        // 创建溶解动画序列
        // 第一阶段：显示（已经设置为1）
        
        // 第二阶段：溶解消失（从1到0）
        currentSequence.Append(
            DOTween.To(
                () => dissolveImage.material.GetFloat(dissolvePropertyName),
                x => dissolveImage.material.SetFloat(dissolvePropertyName, x),
                0f,
                transitionDuration
            ).SetEase(transitionEase)
        );
        
        // 完成后隐藏对象
        currentSequence.OnComplete(() => {
            gameObject.SetActive(false);
        });
    }
    
    private void OnDestroy()
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
        }
    }

 
} 