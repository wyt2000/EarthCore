using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUIs.Common {
// 通用工具栏
public class TooltipView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField]
    private GameObject tipView;

    public event Action OnShow;

    public void OnPointerEnter(PointerEventData eventData) {
        tipView.SetActive(true);
        OnShow?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData) {
        tipView.SetActive(false);
    }
}
}
