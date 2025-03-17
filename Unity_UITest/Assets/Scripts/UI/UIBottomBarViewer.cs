using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class UIBottomBarViewer : MonoBehaviour
{
    [HideInInspector]public List<Toggle> toggles;

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
        if (!isSelected && selectedToggle == toggle)
        {
            selectedToggle = null;
            bool alloff = toggles.All(toggle => !toggle.isOn);
            if(alloff)
            {
                Debug.Log("Closed");
            }
        }
        if(isSelected)
        {
            selectedToggle = toggle;
            Debug.Log("ContainActivated: " + toggle.gameObject.name);
        }
        ScheduleLayoutUpdate();
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
      
        layoutUpdateScheduled = false; // allow next update
    }
}