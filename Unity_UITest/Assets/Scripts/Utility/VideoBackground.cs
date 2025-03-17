using UnityEngine;
using UnityEngine.Video;
public class VideoBackground : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Camera mainCamera;

    void Start()
    {
        // 让视频渲染到 Camera
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = mainCamera;

        // 播放视频
        videoPlayer.Play();
    }
}
