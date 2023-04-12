using System;
using System.Collections;
using Combat.Cards;
using Combat.Enums;
using GUIs.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace GUIs.Cards {
public class CardView : MonoBehaviour, IPointerDownHandler {
#region prefab配置

    // 自身rect
    [SerializeField]
    private RectTransform rect;

    // 卡牌图片
    [SerializeField]
    private Image cardImage;

    // 卡牌名称
    [SerializeField]
    private TextMeshProUGUI cardName;

    // 卡牌描述
    [SerializeField]
    private TextMeshProUGUI cardDescription;

    // 卡牌元素类型
    [SerializeField]
    private TextMeshProUGUI cardElementType;

    // 卡牌法力消耗
    [SerializeField]
    private TextMeshProUGUI cardCost;

    // 上升幅度
    [SerializeField]
    private float upRate = 0.2f;

    // 上升动画时间
    [SerializeField]
    private float upDuration = 0.1f;

#endregion

    // 父组件
    [NonSerialized]
    public CardSlotView Container;

    // 绑定的数据
    public Card Data;

    // 数据索引
    [NonSerialized]
    public int Index;

    private void Start() {
        if (Data == null) return;
        var imageUrl = Data.UiImagePath;
        cardImage.sprite = Resources.Load<Sprite>(imageUrl);
        if (cardImage.sprite == null) {
            cardImage.color = Data.LgElement switch {
                ElementType.Huo  => Color.red,
                ElementType.Shui => Color.blue,
                ElementType.Mu  => Color.green,
                ElementType.Jin => Color.yellow,
                ElementType.Tu => new Color(1f, 0.47f, 0f),

                _ => Color.gray,
            };
        }

        cardName.text        = Data.UiName;
        cardDescription.text = Data.UiDescription;
        cardElementType.text = Data.LgElement?.ToDescription() ?? "";
        cardCost.text        = Data.LgManaCostFunc == null ? $"{Data.LgManaCost}" : "???";
    }

    // 目标位置
    private Vector3 TargetPosition() {
        var target = Vector3.right * (Index * Container.RealOffset());
        if (Data.IsSelected) {
            target += Vector3.up * (rect.rect.height * upRate);
        }

        return Container.transform.TransformPoint(target);
    }

    public IEnumerator MoveToTarget(float duration) {
        yield return this.MoveTo(TargetPosition(), duration);
    }

    public IEnumerator MoveToHeap(float duration) {
        yield return this.MoveTo(Container.combatant.cardHeap.transform.position, duration);
        Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Data.IsSelected ^= true;
        Container.FreshOrder();
        if (Data.IsSelected) transform.SetAsLastSibling();
        StartCoroutine(MoveToTarget(upDuration));
    }
}
}
