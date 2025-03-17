
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class DynamicToggleBar : MonoBehaviour
{
    public List<Toggle> toggles;
    public float normalWidth = 100f;
    public float expandedWidth = 200f;
    public float transitionDuration = 0.3f;
    private Toggle selectedToggle;
    public HorizontalLayoutGroup layoutGroup;

    private bool layoutUpdateScheduled = false; // update layout only once

    void Start()
    {
        
        toggles = new List<Toggle>(GetComponentsInChildren<Toggle>());

        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener((isSelected) => OnToggleChanged(toggle, isSelected));


        }
    }

    void OnToggleChanged(Toggle toggle, bool isSelected)
    {
        // if (isSelected && toggle != selectedToggle)
        // {


            // 设置当前选中的 Toggle
            selectedToggle = toggle;

            ScheduleLayoutUpdate();
            Debug.Log(toggle.gameObject.name);
        // }
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
        // Canvas.ForceUpdateCanvases();
        layoutUpdateScheduled = false; // allow next update
    }
}