using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUIs.Common {
// 自动设置大小的文本
public class AutoText : MonoBehaviour, ISerializationCallbackReceiver {
#region prefab配置

    public float maxWidth = 200;

    [SerializeField]
    private RectTransform rect;

    [SerializeField]
    private ContentSizeFitter fitter;

    [SerializeField]
    private TextMeshProUGUI text;

#endregion

    private void FreshUI() {
        if (text.preferredWidth >= maxWidth) {
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            rect.sizeDelta       = new Vector2(maxWidth, text.preferredHeight);
        }
        else {
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    public void OnBeforeSerialize() { }

    // 保证在预览模式下也能跑这个逻辑
    public void OnAfterDeserialize() {
        // 监听text的渲染事件
        text.OnPreRenderText += _ => FreshUI();
    }
}
}
