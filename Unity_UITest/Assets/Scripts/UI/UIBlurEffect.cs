using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;   
using UnityEngine.Rendering;
using System.Collections.Generic;

[RequireComponent(typeof(RawImage))]
public class UIBlurEffect : MonoBehaviour, IBackgroundEffect
{
    [Header("Blur Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float captureScale = 0.5f; // Lower value = better performance but lower quality
    [SerializeField] private Material blurMaterial;
    [SerializeField] [Range(0, 10)] private int blurIterations = 3;
    [SerializeField] [Range(0, 5)] private float blurSpread = 0.6f;
    
    private RawImage rawImage;
    private RenderTexture captureTexture;
    private RenderTexture blurTexture1;
    private RenderTexture blurTexture2;
    private RenderTexture srcRT = null;
    private bool isCapturing = false;
    private bool shouldCapture = false;
    private bool blurProcessed = false; // 标记是否已经处理过模糊
    
    // IBackgroundEffect implementation
    public BackgroundEffectType EffectType => BackgroundEffectType.Blur;
    
    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (blurMaterial == null)
        {
            Debug.LogError("Blur material is not assigned to UIBlurEffect!");
        }
        
        // Initially hide
        // gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        // 重置模糊处理标记
        blurProcessed = false;
        
        // 注册渲染回调
        RenderPipelineManager.beginContextRendering += BeginFrameRendering;
        RenderPipelineManager.endContextRendering += EndFrameRendering;
   
        shouldCapture = true;
    }
    
    private void OnDisable()
    {
        // 取消注册渲染回调
        RenderPipelineManager.beginContextRendering -= BeginFrameRendering;
        RenderPipelineManager.endContextRendering -= EndFrameRendering;
        
        shouldCapture = false;
        blurProcessed = false;
        
        // 清理资源
        if (srcRT != null)
        {
            RenderTexture.ReleaseTemporary(srcRT);
            srcRT = null;
        }
    }
    
    private void OnDestroy()
    {
        CleanupTextures();
    }
    
    // IBackgroundEffect implementation
    public void ApplyEffect()
    {
        rawImage = GetComponent<RawImage>();
        
        // 预先设置一个透明纹理，避免显示上一次的模糊结果
        if (rawImage.texture == null)
        {
            // 创建一个1x1的透明纹理
            Texture2D transparentTexture = new Texture2D(1, 1);
            transparentTexture.SetPixel(0, 0, Color.clear);
            transparentTexture.Apply();
            rawImage.texture = transparentTexture;
        }
        
        // 重置模糊处理标记
        blurProcessed = false;
        
        // 激活对象，OnEnable会自动注册渲染回调
        gameObject.SetActive(true);
    }
    
    // IBackgroundEffect implementation
    public void RemoveEffect()
    {
        // 禁用对象，OnDisable会自动取消注册渲染回调
        gameObject.SetActive(false);
        CleanupTextures();
    }
    
    private void BeginFrameRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        // 如果已经处理过模糊，则不再捕获
        if (!shouldCapture || isCapturing || blurProcessed)
            return;
            
        foreach (var cam in cameras)
        {
            if (cam.cameraType == CameraType.SceneView)
                return;
        }
        
        foreach (var cam in cameras)
        {
            CameraCaptureBridge.AddCaptureAction(cam, CameraCaptureAction);
        }
        
        if (srcRT == null)
        {
            int width = Mathf.RoundToInt(Screen.width * captureScale);
            int height = Mathf.RoundToInt(Screen.height * captureScale);
            srcRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default);
            srcRT.name = "BlurEffect_srcRT";
        }
        
        isCapturing = true;
    }
    
    private void EndFrameRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        // 如果已经处理过模糊，则不再处理
        if (!isCapturing || srcRT == null || blurProcessed)
            return;
            
        foreach (var cam in cameras)
        {
            if (cam.cameraType == CameraType.SceneView)
                return;
        }
        
        foreach (var cam in cameras)
        {
            CameraCaptureBridge.RemoveCaptureAction(cam, CameraCaptureAction);
        }

        if (srcRT != null)
        {
            RenderTexture currentActiveRT = RenderTexture.active;
            
            // 处理模糊效果
            ProcessBlurEffect(srcRT);
            
            // 标记已经处理过模糊
            blurProcessed = true;
            
            // 处理完成后，取消注册渲染回调，避免继续捕获
            RenderPipelineManager.beginContextRendering -= BeginFrameRendering;
            RenderPipelineManager.endContextRendering -= EndFrameRendering;
            
            // 不要在这里释放srcRT，因为我们需要保留模糊结果
            // 只有在OnDisable时才释放
            
            RenderTexture.active = currentActiveRT;
        }

        isCapturing = false;
    }
    
    private void CameraCaptureAction(RenderTargetIdentifier source, CommandBuffer cmd)
    {
        cmd.Blit(source, srcRT);
    }
    
    private void ProcessBlurEffect(RenderTexture source)
    {
        if (source == null || blurMaterial == null)
        {
            return;
        }
        
        // 创建或调整模糊纹理的大小
        int width = source.width;
        int height = source.height;
        
        if (blurTexture1 == null || blurTexture1.width != width || blurTexture1.height != height)
        {
            CleanupBlurTextures();
            
            blurTexture1 = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            blurTexture2 = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            
            blurTexture1.filterMode = FilterMode.Bilinear;
            blurTexture2.filterMode = FilterMode.Bilinear;
        }
        
        // 应用模糊效果
        for (int i = 0; i < blurIterations; i++)
        {
            // 水平模糊
            blurMaterial.SetVector("_BlurDirection", new Vector4(blurSpread, 0, 0, 0));
            Graphics.Blit(i == 0 ? source : blurTexture2, blurTexture1, blurMaterial);
            
            // 垂直模糊
            blurMaterial.SetVector("_BlurDirection", new Vector4(0, blurSpread, 0, 0));
            Graphics.Blit(blurTexture1, blurTexture2, blurMaterial);
        }
        
        // 设置模糊纹理到RawImage
        rawImage.texture = blurTexture2;
    }
    
    private void CleanupBlurTextures()
    {
        if (blurTexture1 != null)
        {
            blurTexture1.Release();
            Destroy(blurTexture1);
            blurTexture1 = null;
        }
        
        if (blurTexture2 != null)
        {
            blurTexture2.Release();
            Destroy(blurTexture2);
            blurTexture2 = null;
        }
    }
    
    private void CleanupTextures()
    {
        CleanupBlurTextures();
        
        if (captureTexture != null)
        {
            captureTexture.Release();
            Destroy(captureTexture);
            captureTexture = null;
        }
        
        if (srcRT != null)
        {
            RenderTexture.ReleaseTemporary(srcRT);
            srcRT = null;
        }
    }
} 