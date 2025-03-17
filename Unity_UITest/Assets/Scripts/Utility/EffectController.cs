

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EffectController : MonoBehaviour
{
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    private List<Animator> animators = new List<Animator>();

    void Start()
    {
        // 查找该 GameObject 下的所有特效
        particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());
        animators.AddRange(GetComponentsInChildren<Animator>());
    }

    // 播放所有特效
    public void PlayAllEffects()
    {
        Debug.Log("✨ 播放所有特效！");

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
}

// ✅ 创建 Inspector 上的按钮
#if UNITY_EDITOR
[CustomEditor(typeof(EffectController))]
public class EffectControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EffectController effectController = (EffectController)target;

        if (GUILayout.Button("Play All Effects"))
        {
            effectController.PlayAllEffects();
        }
    }
}
#endif