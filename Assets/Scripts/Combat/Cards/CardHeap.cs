using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Requests.Details;
using Designs;
using Utils;

namespace Combat.Cards {
public class CardHeap {
    public readonly List<Card> AllCards = new();

    private readonly List<Card> m_disCards = new();

    public int DiscardCount => m_disCards.Count;

    public CardHeap() {
        var raw = new[] {
            CardDetails.Awakening(),
            CardDetails.Probing(),
            CardDetails.Drain(),
            CardDetails.Thirst(),
            CardDetails.FireSuppression(),
            CardDetails.FireSummon(),
            CardDetails.MagicGuard(),
            CardDetails.ManaSurge(),
        };

        for (var i = 0; i < 10; ++i) {
            AllCards.AddRange(raw.Select(v => v.Clone()));
        }

        Shuffle(AllCards);
    }

    // Todo 放到utils中
    private static void Shuffle<T>(IList<T> list) {
        var n = list.Count;
        while (n > 1) {
            n--;
            var k = GRandom.Range(0, n);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    private void ReuseCard() {
        if (m_disCards.Count == 0) return;
        // 洗牌
        Shuffle(m_disCards);
        foreach (var card in m_disCards) {
            AllCards.Add(card.Clone());
        }
        m_disCards.Clear();
    }

    // 回收卡牌
    public void RecycleCard(IEnumerable<Card> cards) {
        m_disCards.AddRange(cards);
    }

    // Todo 优化摸牌算法到O(n)
    public List<Card> GetCards(RequestGetCard request) {
        // 牌不够时尝试重用卡牌
        if (AllCards.Count == 0) ReuseCard();

        var owner = request.Causer;
        var count = request.Count;
        var filter = request.Filter;
        var selector = request.SelectIndex;
        var valid = AllCards.Where(filter).ToList();
        var cards = new List<Card>();
        while (count-- > 0) {
            var index = selector(valid.Count);
            index = Math.Clamp(0, index, valid.Count);
            if (index == valid.Count) continue;
            var card = valid[index];
            card.Owner = owner;
            cards.Add(card);
            valid.RemoveAt(index);
            AllCards.Remove(card);
        }
        return cards;
    }
}
}
