using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Combat.Cards;
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

    public float RealOffset(int index) {
        // 前面的卡使用了的宽度
        var offset = 0.0f;
        var i = 0;
        for (; i < index && m_cards[i].Data.IsSelected; ++i) offset += m_cardWidth;
        var cnt = m_cards.Count - i;
        // (cnt-1) * real + m_cardWidth = m_slotWidth - offset
        if (cnt > 1) {
            var real = (m_slotWidth - offset - m_cardWidth) / (cnt - 1);
            real   =  Mathf.Min(real, m_cardWidth + inner);
            offset += real * (index - i);
        }

        return offset;
    }

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
                if (c.Style == CardStyle.Played || c.Data.IsSelected) return;
                c.Style = combatant.CanSelectCardToPlay(c.Data) ? CardStyle.Valid : CardStyle.Ban;
                c.FreshUI();
            });
            m_cards.Sort(GTools.ExtractorToComparer<CardView>(c =>
            {
                var index = c.Data.LgElement.HasValue ? (int)c.Data.LgElement.Value : int.MaxValue;
                return (!c.Data.IsSelected, !c.IsSelectable, index, c.Data.UiName);
            }));
        }
        m_cards.ForEach(
            (c, i) => c.transform.SetSiblingIndex(c.Index = i)
        );
    }

    public IEnumerator FreshUI() {
        FreshOrder();
        return GCoroutine.Parallel(m_cards.Select(c => c.MoveToTarget()));
    }

    public IEnumerator Discards(IEnumerable<Card> cards) {
        var set = cards.ToHashSet();
        var remove = m_cards.Extract(card => set.Contains(card.Data));
        combatant.Heap.RecycleCard(set);
        return GCoroutine.Parallel(remove.Select((card, i) => card.MoveToHeap(i, 0.5f)).Append(FreshUI()));
    }
}
}
