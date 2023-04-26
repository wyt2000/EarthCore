using System.Collections;
using DG.Tweening;
using GUIs.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace GUIs.Menus {
// Todo! 完善ui,加全屏遮罩
public class DialogView : MonoBehaviour, IPointerClickHandler {
#region prefab配置

    [SerializeField]
    private TextMeshProUGUI title;

    [SerializeField]
    private TextMeshProUGUI content;

    [SerializeField]
    private Image leftImage;

    [SerializeField]
    private Image rightImage;

    [SerializeField]
    private float showTime = 0.5f;

#endregion

    private readonly CoroutineLocker m_locker = new(ResolvePolicy.Delay);

    private Tween m_tween;

    public void Hide(string side) {
        switch (side) {
            case "left":
                leftImage.DOFade(0, showTime);
                break;
            case "right":
                rightImage.DOFade(0, showTime);
                break;
            default:
                gameObject.SetActive(false);
                break;
        }
    }

    public IEnumerator Say(string character, string text) {
        gameObject.SetActive(true);
        title.text   = character;
        content.text = "";
        return (m_tween = content.DOText(text, showTime)).Wait().Lock(m_locker);
    }

    public IEnumerator Show(string texture, string side) {
        var image = side == "left" ? leftImage : rightImage;
        yield return image.DOColor(new Color(1, 1, 1, 0), showTime / 2).Wait().Lock(m_locker);
        var path = "Textures/Characters/" + texture;
        image.sprite = Resources.Load<Sprite>(path);
        yield return image.DOColor(new Color(1, 1, 1, 1), showTime / 2).Wait().Lock(m_locker);
    }

    public void OnPointerClick(PointerEventData eventData) {
        m_tween?.Complete();
    }
}
}
