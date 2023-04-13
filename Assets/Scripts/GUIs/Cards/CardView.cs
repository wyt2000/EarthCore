using System;
using System.Collections;
using Combat.Cards;
using Combat.Enums;
using GUIs.Animations;
using TMPro;
using Unity.VisualScripting;
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

    // 卡牌边框
    [SerializeField]
    private Image cardBorder;

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

    private readonly AnimLocker m_locker = new(AnimConflictPolicy.Delay);

    public bool isFlipped;

    public void Show()
    {
        var imageUrl = Data.UiImagePath;
        cardImage.sprite = Resources.Load<Sprite>(imageUrl) ?? cardImage.sprite;
        cardBorder.color = Data.LgElement switch {
            ElementType.Huo  => new Color(0.78f, 0.24f, 0.1f),
            ElementType.Shui => new Color(0.41f, 0.68f, 0.74f),
            ElementType.Mu   => new Color(0.34f, 0.76f, 0.53f),
            ElementType.Jin  => new Color(0.65f, 0.51f, 0.29f),
            ElementType.Tu   => new Color(0.43f, 0.27f, 0.18f),

            _ => new Color(0.38f, 0.39f, 0.42f),
        };

        cardName.text        = Data.UiName;
        cardDescription.text = Data.UiDescription;
        cardElementType.text = Data.LgElement?.ToDescription() ?? "";
        cardCost.text        = Data.LgManaCostFunc == null ? $"{Data.LgManaCost}" : "???";
        // Todo 实现preview法力消耗
        // Todo 加伤害text
    }
    
    private void Start() {
        if (Data == null) return;
        if (isFlipped)
        {
            Flip();
        }
        else
        {
            Show();
        }
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
        yield return this.MoveTo(m_locker, TargetPosition(), duration);
    }

    public IEnumerator MoveToHeap(int index, float duration) {
        rect.SetPivotWithoutChangingPosition(new Vector2(0.5f, 0.5f));
        var target = Container.combatant.cardHeap.transform.position;
        var first = target + Vector3.right * ((index + 1) * rect.rect.width * 1.25f);
        yield return this.MoveTo(null, first, duration);
        yield return GAnimation.Wait(1.0f);
        yield return this.MoveTo(null, target, 0.2f);
        // Todo 加缩放动画
        Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (Data.IsSelectable)
        {
            Data.IsSelected ^= true;
            Container.FreshOrder();
            if (Data.IsSelected) transform.SetAsLastSibling();
            StartCoroutine(MoveToTarget(upDuration));
        }
    }

    // 将卡牌翻面
    public void Flip()
    {
        cardImage.sprite     = null;
        cardImage.color      = Color.grey;
        cardName.text        = "";
        cardDescription.text = "";
        cardElementType.text = "";
        cardCost.text        = "???";
    }

}
}
