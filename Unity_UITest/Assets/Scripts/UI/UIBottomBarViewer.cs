using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;


public class UIBottomBarViewer : MonoBehaviour
{
    [HideInInspector]public List<Toggle> toggles;

    private Toggle selectedToggle;
    public HorizontalLayoutGroup layoutGroup;
    public RectTransform bgmMove; // 添加bgm_move引用
    public float transitionDuration = 0.3f; // 移动动画时长
    public float popupDuration = 0.4f; // 弹出动画时长
    public float offScreenOffset = 100f; // 屏幕下方偏移量
    public Vector2 positionOffset = Vector2.zero; // 最终位置的偏移量
    
    // 拉伸效果相关参数
    public float stretchFactor = 1.2f; // 拉伸系数，控制拉伸程度
    public float stretchDuration = 0.15f; // 拉伸动画时长
    public float returnDuration = 0.3f; // 弹回动画时长
    
    private bool isFirstActivation = true; // 是否第一次激活
    private bool layoutUpdateScheduled = false; // update layout only once
    private Vector3 lastPosition; // 记录上一次位置，用于判断移动方向

    void Start()
    {
        // 初始状态下隐藏bgm_move
        if (bgmMove != null)
        {
            bgmMove.gameObject.SetActive(false);
        }
        
        toggles = new List<Toggle>(GetComponentsInChildren<Toggle>());

        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener((isSelected) => OnToggleChanged(toggle, isSelected));
        }
    }

    void OnToggleChanged(Toggle toggle, bool isSelected)
    {
        if (!isSelected && selectedToggle == toggle)
        {
            selectedToggle = null;
            bool alloff = toggles.All(toggle => !toggle.isOn);
            if(alloff)
            {
                Debug.Log("Closed");
                // 所有toggle都关闭时，播放向下移出屏幕动画然后隐藏bgm_move
                if (bgmMove != null && bgmMove.gameObject.activeSelf)
                {
                    // 计算向下移动的目标位置
                    Vector3 targetPosition = bgmMove.position;
                    targetPosition.y -= offScreenOffset;
                    
                    // 向下移动并隐藏
                    bgmMove.DOMove(targetPosition, popupDuration * 0.5f).SetEase(Ease.InBack).OnComplete(() => {
                        bgmMove.gameObject.SetActive(false);
                    });
                }
            }
        }
        if(isSelected)
        {
            selectedToggle = toggle;
            Debug.Log("ContainActivated: " + toggle.gameObject.name);
            
            // 不在这里立即移动bgm_move，而是等布局重建后再移动
            
            // 第一次激活时特殊处理
            if (isFirstActivation && bgmMove != null)
            {
                isFirstActivation = false;
                // 获取toggle位置作为目标位置
                RectTransform toggleRect = toggle.GetComponent<RectTransform>();
                if (toggleRect != null)
                {
                    // 计算最终目标位置（应用偏移量）
                    Vector3 targetPosition = toggleRect.position;
                    targetPosition.x += positionOffset.x;
                    targetPosition.y += positionOffset.y;
                    
                    // 设置起始位置（屏幕下方）
                    Vector3 startPosition = targetPosition;
                    startPosition.y -= offScreenOffset;
                    
                    bgmMove.position = startPosition;
                    bgmMove.gameObject.SetActive(true);
                    lastPosition = bgmMove.position; // 记录初始位置
                    
                    // 播放从下方弹出的动画
                    bgmMove.DOMove(targetPosition, popupDuration).SetEase(Ease.OutBack);
                }
            }
        }
        ScheduleLayoutUpdate();
    }

    // 移动bgm_move到指定Toggle的位置
    void MoveBgmToToggle(Toggle toggle)
    {
        if (bgmMove != null && toggle != null)
        {
            // 找到toggleRect引用
            RectTransform toggleRect = toggle.GetComponent<RectTransform>();
            if (toggleRect != null)
            {
                // 计算最终目标位置（应用偏移量）
                Vector3 targetPosition = toggleRect.position;
                targetPosition.x += positionOffset.x;
                targetPosition.y += positionOffset.y;
                
                // 确保bgm_move是激活的
                if (!bgmMove.gameObject.activeSelf)
                {
                    // 设置起始位置（屏幕下方）
                    Vector3 startPosition = targetPosition;
                    startPosition.y -= offScreenOffset;
                    
                    bgmMove.position = startPosition;
                    bgmMove.gameObject.SetActive(true);
                    lastPosition = bgmMove.position; // 记录初始位置
                    
                    // 播放从下方弹出的动画
                    bgmMove.DOMove(targetPosition, popupDuration).SetEase(Ease.OutBack);
                }
                else
                {
                    // 记录当前位置，用于判断移动方向
                    Vector3 startPosition = bgmMove.position;
                    
                    // 计算移动方向（这个会在移动完成后使用）
                    bool movingRight = targetPosition.x > startPosition.x;
                    
                    // 移动时只执行水平移动动画，不改变垂直位置
                    Vector3 horizontalTarget = targetPosition;
                    horizontalTarget.y = bgmMove.position.y; // 保持相同的y坐标
                    
                    // 首先移动到目标位置
                    bgmMove.DOMove(horizontalTarget, transitionDuration)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            // 移动完成后，执行拉伸动画
                            ApplyStretchEffectWithPivot(movingRight);
                            
                            // 然后平滑调整y坐标到目标位置（如果需要）
                            if (!Mathf.Approximately(bgmMove.position.y, targetPosition.y))
                            {
                                bgmMove.DOMoveY(targetPosition.y, transitionDuration * 0.5f).SetEase(Ease.OutQuad);
                            }
                        });
                }
            }
        }
    }
    

    public void SetPivotWithoutMoving(RectTransform rectTransform, Vector2 newPivot)
    {
        // 当前的锚点和尺寸
        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = newPivot - rectTransform.pivot;

        // 计算位置偏移
        Vector2 offset = new Vector2(deltaPivot.x * size.x, deltaPivot.y * size.y);

        // 更新 anchoredPosition（抵消位置变化）
        rectTransform.anchoredPosition += offset;

        // 最后设置新的 pivot
        rectTransform.pivot = newPivot;
    }
    // 使用pivot方式实现拉伸效果
    void ApplyStretchEffectWithPivot(bool movingRight)
    {
        if (bgmMove == null) return;
        
        RectTransform rectTransform = bgmMove;
        
        // 保存原始状态
        Vector2 originalSize = rectTransform.sizeDelta;
        Vector2 originalPivot = rectTransform.pivot;
        Vector3 originalPosition = rectTransform.position;
        
        // 创建序列
        Sequence stretchSequence = DOTween.Sequence();
        
        // 添加设置pivot的动作
        stretchSequence.AppendCallback(() => {
            // 根据移动方向设置不同的pivot
            if (movingRight)
            {
                // 向右移动时，固定右侧边缘，拉伸左侧
                // rectTransform.pivot = new Vector2(1f, 0.5f);
                SetPivotWithoutMoving(rectTransform, new Vector2(1f, 0.5f));
            }
            else
            {
                // 向左移动时，固定左侧边缘，拉伸右侧
                // rectTransform.pivot = new Vector2(0f, 0.5f);
                SetPivotWithoutMoving(rectTransform, new Vector2(0f, 0.5f));
            }
            
            // 修改pivot会导致位置变化，所以需要恢复位置
            // rectTransform.position = originalPosition;
        });
        
        // 计算拉伸后的宽度
        float stretchedWidth = originalSize.x * stretchFactor;
        
        // 添加拉伸动画
        stretchSequence.Append(
            rectTransform.DOSizeDelta(new Vector2(stretchedWidth, originalSize.y), stretchDuration)
                .SetEase(Ease.OutQuad)
        );
        
        // 添加弹回动画
        stretchSequence.Append(
            rectTransform.DOSizeDelta(originalSize, returnDuration)
                .SetEase(Ease.OutElastic, 0.5f, 0.3f)
        );
        
        // 完成后恢复pivot
        stretchSequence.OnComplete(() => {
            Vector3 positionBeforeReset = rectTransform.position;
            // rectTransform.pivot = originalPivot;
            // rectTransform.position = positionBeforeReset;
            SetPivotWithoutMoving(rectTransform, originalPivot);
        });
        
        // 播放序列
        stretchSequence.Play();
    }

    bool ChecktogglesState()
    {
        
       bool alloff = toggles.All(toggle => !toggle.isOn);
       return alloff;
    }

    void ScheduleLayoutUpdate()
    {
        if (!layoutUpdateScheduled)
        {
            layoutUpdateScheduled = true;
            StartCoroutine(DelayedUpdateLayout());
        }
    }

    IEnumerator DelayedUpdateLayout()
    {
        yield return null; // wait for one frame
        LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.GetComponent<RectTransform>());
        
        // 等待布局重建完成
        yield return null;
        
        // 在布局重建后移动bgm_move到选中的Toggle
        if (selectedToggle != null)
        {
            MoveBgmToToggle(selectedToggle);
        }
      
        layoutUpdateScheduled = false; // allow next update
    }
}