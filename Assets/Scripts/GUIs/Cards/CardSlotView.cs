using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Combat.Cards;
using GUIs.Common;
using UnityEngine;
using Utils;

namespace GUIs.Cards {
public class CardSlotView : MonoBehaviour {
#region prefab配置

    public CombatantComponent combatant;

    public bool isOtherPlayer;

    // 卡牌最大间距
    public float inner = 10;

    [SerializeField]
    // 卡牌预制体 
    private CardView cardPrefab;

#endregion

    // 卡牌列表
    private readonly List<CardView> m_cards = new();

    // 卡槽宽度
    private float m_slotWidth;

    // 卡牌宽度
    private float m_cardWidth;

    private void Start() {
        m_slotWidth = GetComponent<RectTransform>().rect.width;
        m_cardWidth = cardPrefab.GetComponent<RectTransform>().rect.width;
    }

    private float Offset(float width, int cnt, float maxInner) {
        if (cnt <= 1) return 0;
        return Mathf.Min((width - m_cardWidth) / (cnt - 1), m_cardWidth + maxInner);
    }

    public float RealOffset(int index) {
        var isSelect = m_cards[index].Data.IsSelected;
        var selectCnt = m_cards.Count(item => item.Data.IsSelected);
        var unselectCnt = m_cards.Count - selectCnt;
        var selectWidth = Mathf.Min(m_cardWidth * selectCnt, m_slotWidth - 3 * m_cardWidth);
        var unselectWidth = m_slotWidth - selectWidth;
        var selectOffset = Offset(selectWidth,     selectCnt,   0);
        var unselectOffset = Offset(unselectWidth, unselectCnt, inner);
        if (isSelect) return selectOffset * index;
        return selectWidth + unselectOffset * (index - selectCnt);
    }

    // Todo 改成AddCards接口
    public IEnumerator AddCard(Card data) {
        var card = Instantiate(cardPrefab, transform);

        card.Container = this;
        card.Data      = data;
        card.Style     = isOtherPlayer ? CardStyle.Other : CardStyle.Valid;

        m_cards.Add(card);

        card.transform.position = combatant.cardHeap.transform.position;

        return FreshUI();
    }

    private void FreshOrder() {
        if (!isOtherPlayer) {
            m_cards.ForEach(c =>
            {
                if (c.Style != CardStyle.Played && !c.Data.IsSelected) {
                    c.Style = combatant.CanSelectCardToPlay(c.Data) ? CardStyle.Valid : CardStyle.Ban;
                }
                c.FreshUI();
            });
            m_cards.Sort(GTools.ExtractorToComparer<CardView>(c =>
            {
                var index = c.Data.LgElement.HasValue ? (int)c.Data.LgElement.Value : int.MaxValue;
                return (!c.Data.IsSelected, !c.IsSelectable, index, c.Data.UiName);
            }));
        }
        m_cards.ForEach(
            (c, i) => c.GetComponent<Canvas>().SetOrder(SortOrder.Card, c.Index = i)
        );
    }

    public IEnumerator FreshUI() {
        FreshOrder();
        return GCoroutine.Parallel(m_cards.Select(c => c.MoveToTarget()));
    }

    public IEnumerator Discards(IEnumerable<Card> cards) {
        var set = cards.ToHashSet();
        var remove = m_cards.Extract(card => set.Contains(card.Data));
        var sub = (remove.Count - 1) / 2.0f;
        combatant.Heap.RecycleCard(set);
        return GCoroutine.Parallel(remove.Select((card, i) => card.MoveToHeap(i - sub, 0.5f)).Append(FreshUI()));
    }
}
}
