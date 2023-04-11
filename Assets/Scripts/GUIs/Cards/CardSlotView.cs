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

    [SerializeField]
    // 卡牌预制体 
    private CardView cardPrefab;

    // 卡牌间距
    public float inner = 10;

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

    public float RealOffset() {
        var cnt = m_cards.Count;
        if (cnt <= 1) return 0;
        // real * (cnt - 1) + cardWidth = slotWidth
        var realOffset = (m_slotWidth - m_cardWidth) / (cnt - 1);
        realOffset = Mathf.Min(realOffset, m_cardWidth + inner);
        return realOffset;
    }

    public IEnumerator AddCard(Card data) {
        var card = Instantiate(cardPrefab, transform);
        card.Slot = this;
        card.Data = data;
        m_cards.Add(card);

        card.transform.position = combatant.cardHeap.transform.position;

        return FreshUI();
    }

    private IEnumerator FreshUI() {
        return ToolsCoroutine.Combine(m_cards.Select((card, i) =>
        {
            card.Index = i;
            return card.MoveToTarget(0.3f);
        }));
    }

    public IEnumerator PlayCards(IEnumerable<Card> cards) {
        var set = cards.ToHashSet();
        var remove = m_cards.Extract(card => set.Contains(card.Data));
        combatant.Heap.RecycleCard(set);
        return ToolsCoroutine.Combine(remove.Select(card => card.MoveToHeap(0.1f)).Append(FreshUI()));
    }
}
}
