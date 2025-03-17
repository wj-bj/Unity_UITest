
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainUIToggleItem : MonoBehaviour
{
    public Toggle toggle;
    public Image background; // 选中的背景
    public RectTransform iconTransform; // 图标的 RectTransform
    public Text label; // 文字
    public RectTransform toggleRect; // Toggle 本身的 RectTransform（需要手动拖入）

    public float iconMoveDistance = 20f; // 图标向上移动的距离
    public float transitionDuration = 0.3f; // 动画时长
    public float bgmTransitionDuration = 0.1f; // 背景淡入淡出动画时长  

    public float scaleDuration = 0.15f; //变大时长
    public float scaleSize = 1.5f; //变大倍数
    public float normalWidth = 100f; // 默认 Toggle 宽度
    public float expandedWidth = 200f; // 选中后 Toggle 宽度

    private Vector2 normalIconPosition;
    private Vector2 selectedIconPosition;

    void Start()
    {
        // 记录初始位置
        normalIconPosition = iconTransform.anchoredPosition;
        selectedIconPosition = normalIconPosition + new Vector2(0, iconMoveDistance); // 图标向上移动

        // 监听 Toggle 变化
        toggle.onValueChanged.AddListener(OnToggleChanged);

        // 初始化状态
        UpdateToggleState(toggle.isOn, true);
    }

    void OnToggleChanged(bool isOn)
    {
        UpdateToggleState(isOn, false);

    }

    void UpdateToggleState(bool isOn, bool instant)
    {
        float duration = instant ? 0f : transitionDuration;


        if (isOn)
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                 iconTransform.DOScale(scaleSize, scaleDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                 {
                        iconTransform.DOScale(1f, scaleDuration).SetEase(Ease.OutQuad);
                });
                iconTransform.DOAnchorPos(isOn ? selectedIconPosition : normalIconPosition, duration)
                .SetEase(Ease.OutQuad);
                background.DOFade(isOn ? 1f : 0f, bgmTransitionDuration).OnComplete(() =>
                {
                    background.enabled = isOn;
                });
                label.gameObject.SetActive(true);
                label.DOFade(1f, transitionDuration);
            });
        }
        else
        {
            iconTransform.DOAnchorPos(isOn ? selectedIconPosition : normalIconPosition, duration)
            .SetEase(Ease.OutQuad);
            background.DOFade(isOn ? 1f : 0f, bgmTransitionDuration).OnComplete(() =>
            {
                background.enabled = isOn;
            });
             label.DOFade(0f, transitionDuration).OnComplete(() =>
            {
                label.gameObject.SetActive(false);
            });
        }


        if (toggleRect != null)
        {
            // toggleRect.DOSizeDelta(new Vector2(isOn ? expandedWidth : normalWidth, toggleRect.sizeDelta.y), duration)
            //     .SetEase(Ease.OutQuad);
            toggleRect.sizeDelta = new Vector2(isOn ? expandedWidth : normalWidth, toggleRect.sizeDelta.y);

        }
    }
}