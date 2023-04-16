using System;
using GUIs.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUIs.States {
public class StateFieldView : MonoBehaviour {
#region prefab绑定

    [SerializeField]
    public TooltipHandler tooltip;

    [SerializeField]
    public Image icon;

    [SerializeField]
    private TextMeshProUGUI text;

#endregion

    public Func<string> OnShow;

    public void FreshUI() {
        var str = OnShow?.Invoke() ?? "";
        text.text = str;
    }
}
}
