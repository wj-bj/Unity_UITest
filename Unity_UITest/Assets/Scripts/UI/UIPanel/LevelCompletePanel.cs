using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;  

public class LevelCompletePanel : UIWindowBase
{
    [Header("UI References")]
    [SerializeField] private Button closeButton;
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    private List<Animator> animators = new List<Animator>();
    
    protected override void Awake()
    {
        base.Awake();
        
        // Setup button listeners
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        
       UIManager.Instance.OnUIShow += OnUIShow;

    }

   


    private void OnCloseButtonClicked()
    {
        UIManager.Instance.Close(WindowName);
    }

    
    public void PlayAllEffects()
    {
        Debug.Log("play all effects");

        foreach (var ps in particleSystems)
        {
            ps.Stop();
            ps.Play();
        }

        foreach (var anim in animators)
        {
            anim.SetTrigger("Play"); // 确保 Animator 里有 "Play" Trigger
        }
    }

    private void OnUIShow(string windowName)
    {
        if (windowName == WindowName)
        {
            PlayAllEffects();
        }
    }

} 