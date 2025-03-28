#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D; // SpriteAtlas 所在的命名空间

public class SpriteAtlasPreviewExporter
{
    [MenuItem("Tools/导出 SpriteAtlas 预览图")]
    public static void ExportAtlasPreview()
    {
        Object atlasObj = Selection.activeObject;
        if (atlasObj is SpriteAtlas atlas)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(atlas);
            if (preview != null)
            {
                byte[] pngData = preview.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.dataPath + "/AtlasPreview.png", pngData);
                Debug.Log("已保存到: " + Application.dataPath + "/AtlasPreview.png");
            }
            else
            {
                Debug.LogWarning("无法生成图集预览图，请确保图集已被打包或使用中的 SpriteAtlas 设置正确。");
            }
        }
        else
        {
            Debug.LogWarning("请选中一个 SpriteAtlas 资源！");
        }
    }
}
#endif