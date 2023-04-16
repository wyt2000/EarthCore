using TMPro;
using UnityEngine;

namespace GUIs.Common {
// tooltip视图
public class TooltipView : MonoBehaviour {
#region prefab绑定

    // tooltip文本
    [SerializeField]
    private TextMeshProUGUI text;

    // Todo 被card遮挡的问题

#endregion

    public void FreshUI(string str) {
        text.text = str;
    }
}
}
