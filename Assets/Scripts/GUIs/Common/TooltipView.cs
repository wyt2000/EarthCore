using TMPro;
using UnityEngine;

namespace GUIs.Common {
// tooltip视图
public class TooltipView : MonoBehaviour {
#region prefab绑定

    // tooltip文本
    [SerializeField]
    private TextMeshProUGUI text;

    // canvas
    [SerializeField]
    private Canvas canvas;

#endregion

    public void FreshUI(string str) {
        text.text = str;
    }

    private void Start() {
        canvas.SetOrder(SortOrder.Tooltip);
    }
}
}
