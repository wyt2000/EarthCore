using System.Collections;
using DG.Tweening;
using GUIs.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUIs.Globals {
public class HelpView : MonoBehaviour, IPointerClickHandler {
#region prefab配置

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private float saySpeed = 40f;

#endregion

    private Tween m_tween;

    public IEnumerator Show(string msg) {
        gameObject.SetActive(true);
        text.text = "";
        yield return (m_tween = text.DOText(msg, msg.Length / saySpeed)).Wait();
        while (!Input.GetMouseButtonDown(0)) yield return null;
        Hide();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) {
        m_tween?.Complete();
    }
}
}
