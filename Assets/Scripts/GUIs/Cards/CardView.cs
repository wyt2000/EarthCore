using System;
using System.Collections;
using Combat.Cards;
using Combat.Enums;
using Combat.Requests.Details;
using GUIs.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace GUIs.Cards {
public enum CardStyle {
    // 自己可选的卡牌
    Valid,

    // 自己不可选的卡牌
    Ban,

    // 对方的卡牌
    Other,

    // 双方打出的牌
    Played,
}

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

    // 卡背贴图
    [SerializeField]
    private Image cardBack;

    [SerializeField]
    // 不可选中的时候加个灰度滤镜
    private Image cardBanFilter;

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

#endregion

    // 父组件
    [NonSerialized]
    public CardSlotView Container;

    // 绑定的数据
    public Card Data;

    // 数据索引
    [NonSerialized]
    public int Index;

    [NonSerialized]
    public CardStyle Style = CardStyle.Other;

    public bool IsSelectable => Style == CardStyle.Valid;

    private readonly AnimLocker m_locker = new(AnimConflictPolicy.Overwrite);

    private void Start() {
        if (Data == null) return;
        FreshUI();
    }

    // 目标位置
    private Vector3 TargetPosition() {
        var target = Vector3.right * Container.RealOffset(Index);
        if (Data.IsSelected) {
            target += Vector3.up * (rect.rect.height * upRate);
        }

        return Container.transform.TransformPoint(target);
    }

    public void FreshUI() {
        if (Style == CardStyle.Other) {
            cardBack.gameObject.SetActive(true);
            return;
        }
        cardBack.gameObject.SetActive(false);
        cardBorder.color = Data.LgElement switch {
            ElementType.Huo  => new Color(0.78f, 0.24f, 0.1f),
            ElementType.Shui => new Color(0.41f, 0.68f, 0.74f),
            ElementType.Mu   => new Color(0.34f, 0.76f, 0.53f),
            ElementType.Jin  => new Color(0.65f, 0.51f, 0.29f),
            ElementType.Tu   => new Color(0.43f, 0.27f, 0.18f),

            _ => new Color(0.38f, 0.39f, 0.42f),
        };
        var imageUrl = Data.UiImagePath;
        cardImage.sprite = Resources.Load<Sprite>(imageUrl) ?? cardImage.sprite;

        cardName.text        = Data.UiName;
        cardDescription.text = Data.UiDescription;
        cardElementType.text = Data.LgElement?.ToDescription() ?? "";
        cardCost.text        = $"{Data.ManaCost:F0}";

        // Todo 加伤害预览text

        cardBanFilter.gameObject.SetActive(
            Style == CardStyle.Ban ||
            Style != CardStyle.Played && Data.IsSelected && !Data.Owner.PreviewBatch.CanEnqueue()
        );
    }

    public IEnumerator MoveToTarget() {
        yield return this.MoveTo(m_locker, TargetPosition(), 0.3f);
    }

    public IEnumerator MoveToHeap(float index, float duration) {
        Style = CardStyle.Played;
        FreshUI();
        var heap = Container.combatant.cardHeap;
        var trans = GetComponent<RectTransform>();
        var mid = new Vector2(0.5f, 0.5f);
        var first = trans.GetPosition(mid, heap.center.position + Vector3.right * (trans.lossyScale.x * index * rect.rect.width));
        var final = trans.GetPosition(mid, heap.transform);
        yield return this.MoveTo(null, first, duration);
        yield return GAnimation.Wait(1.0f);
        yield return this.MoveTo(null, final, 0.2f);
        // Todo 加缩放动画
        Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!IsSelectable) return;
        Data.IsSelected ^= true;
        if (Data.IsSelected) {
            // Todo 游戏侧边加个详细信息面板 
        }
        else if (Data.Owner.PreviewBatch.EvaluateState() == BatchCardState.CannotSelect) {
            Data.Owner.Cards.ForEach(card => card.IsSelected = false);
        }
        StartCoroutine(Container.FreshUI());
    }
}
}
