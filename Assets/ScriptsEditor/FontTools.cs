using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace ScriptsEditor {
public static class FontTools {
    // 替换所有text字体
    public static void ReplaceDefaultFont(TMP_FontAsset current, TMP_FontAsset replace) {
        var prefabs = AssetDatabase.FindAssets("t:prefab").Select(AssetDatabase.GUIDToAssetPath).ToArray();
        var cnt = 0;
        foreach (var prefab in prefabs) {
            var go = PrefabUtility.LoadPrefabContents(prefab);
            // Todo 只改prefab的源文件对象,不用改内部复用的对象
            var texts = go.GetComponentsInChildren<TextMeshProUGUI>().Where(t => t.font == current).ToArray();
            if (texts.Length == 0) continue;
            cnt += texts.Length;
            foreach (var text in texts) text.font = replace;
            PrefabUtility.SaveAsPrefabAsset(go, prefab);
        }
        Debug.Log($"更新了{cnt}个Text组件");
    }
}

// 选择新字体的gui
internal class FontSelector : EditorWindow {
    [MenuItem("Tools/TMPro字体替换")]
    public static void ShowWindow() {
        GetWindow<FontSelector>("TMPro字体替换");
    }

    private TMP_FontAsset m_current, m_replace;

    private void OnClick() {
        if (m_current == null || m_replace == null) return;
        FontTools.ReplaceDefaultFont(m_current, m_replace);
    }

    // 整两个选择器就行
    private void OnGUI() {
        m_current = (TMP_FontAsset)EditorGUILayout.ObjectField("当前字体", m_current, typeof(TMP_FontAsset), false);
        m_replace = (TMP_FontAsset)EditorGUILayout.ObjectField("替换字体", m_replace, typeof(TMP_FontAsset), false);
        if (GUILayout.Button("替换")) {
            OnClick();
        }
    }
}
}
