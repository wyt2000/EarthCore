using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptsEditor {
public static class ImageTools {
    // 设置sprite模式
    private static void LoadSprite(string folderPath) {
        var files = AssetDatabase.FindAssets("t:texture2D", new[] { folderPath }).Select(AssetDatabase.GUIDToAssetPath).ToArray();
        var cnt = 0;
        AssetDatabase.StartAssetEditing();
        foreach (var file in files) {
            var importer = AssetImporter.GetAtPath(file) as TextureImporter;
            if (
                importer == null ||
                importer.textureType == TextureImporterType.Sprite ||
                importer.spriteImportMode == SpriteImportMode.Single
            ) continue;
            importer.textureType      = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
            cnt++;
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
        Debug.Log($"设置{cnt}个贴图的importer");
    }

    // 处理Resources/Textures下所有文件
    [MenuItem("Tools/自动导入贴图")]
    public static void LoadTextures() {
        LoadSprite("Assets/Resources/Textures");
    }
}
}
