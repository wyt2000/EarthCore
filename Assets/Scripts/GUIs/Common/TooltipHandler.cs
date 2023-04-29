using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUIs.Common {
// 附着在要显示tooltip的物体上,控制tooltip的显示,隐藏,内容
public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
#region prefab绑定

    public TooltipView tooltipView;

#endregion

    public Func<string> OnShow;

    public void OnPointerEnter(PointerEventData eventData) {
        tooltipView.gameObject.SetActive(true);
        if (OnShow != null) tooltipView.FreshUI(OnShow.Invoke());
    }

    public void OnPointerExit(PointerEventData eventData) {
        tooltipView.gameObject.SetActive(false);
    }
}
}
