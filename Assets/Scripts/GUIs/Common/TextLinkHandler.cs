using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUIs.Common {
[RequireComponent(typeof(TextMeshProUGUI))]
// url 文本处理 Todo 处理IPointerClickHandler穿透点击
public class TextLinkHandler : MonoBehaviour /*,IPointerClickHandler*/ {
    private TextMeshProUGUI m_text;

    // text, url
    public event Action<string, string> OnClickLink;

    private void Start() {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(m_text, eventData.position, Camera.main);
        if (linkIndex == -1) return;
        var linkInfo = m_text.textInfo.linkInfo[linkIndex];
        OnClickLink?.Invoke(linkInfo.GetLinkText(), linkInfo.GetLinkID());
    }
}
}
